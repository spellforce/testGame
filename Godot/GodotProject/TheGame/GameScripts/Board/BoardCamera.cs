using Godot;
using GameLogic.Design;

namespace GameLogic.Board
{
    /// <summary>
    /// 棋盘摄像机（design 03-canvas-layout.md "Camera" + 06 "Camera"）。
    ///
    /// 关键设计：世界**连续缩放**，不渲染到固定低分辨率视口（与 Stacklands 一致）。
    /// 因此 zoom 不是整数倍也是可以的：在 1/2/3/4 倍时每个 art px 恰好等宽，
    /// 介于其间的比例会有 1 屏幕像素的宽度差，肉眼几乎不可见。
    /// 可选的 "Pixel-perfect zoom" 设置（<see cref="PixelPerfect"/>）会吸附到整数倍。
    ///
    /// 缩放基准：z = 0.5 时在 1080p 下 1 art px = 1 屏幕像素。
    /// 公式 screen_px_per_art_px = z x screen_height / 540。
    /// </summary>
    public partial class BoardCamera : Camera2D
    {
        /// <summary>设计缩放等级 0.5 (min) .. 2.0 (max)。默认 1.0。</summary>
        private float m_ZoomLevel = Metrics.ZoomDefault;

        private float m_TargetZoomLevel = Metrics.ZoomDefault;

        /// <summary>吸附到整数屏幕像素/art px（设置项）。</summary>
        public bool PixelPerfect { get; set; }

        /// <summary>以像素为单位的当前视口尺寸。</summary>
        private Vector2 ViewportSize => GetViewportRect().Size;

        /// <summary>当前缩放等级（设计值，非 Camera2D.zoom）。</summary>
        public float ZoomLevel => m_ZoomLevel;

        public override void _Ready()
        {
            Name = "BoardCamera";
            Enabled = true;
            PositionSmoothingEnabled = false;
            AnchorMode = AnchorModeEnum.DragCenter;
            MakeCurrent();
            ApplyZoom();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            // ---- 滚轮缩放：以光标为锚点，每格 x1.15，缓动 120 ms ----
            if (@event is InputEventMouseButton mb && mb.Pressed)
            {
                if (mb.ButtonIndex == MouseButton.WheelUp)
                {
                    ZoomBy(1f / Metrics.ZoomStep);
                    GetViewport().SetInputAsHandled();
                }
                else if (mb.ButtonIndex == MouseButton.WheelDown)
                {
                    ZoomBy(Metrics.ZoomStep);
                    GetViewport().SetInputAsHandled();
                }
            }
        }

        public override void _Process(double delta)
        {
            // 缩放缓动到目标值（120 ms 内收敛）
            if (!Mathf.IsEqualApprox(m_ZoomLevel, m_TargetZoomLevel))
            {
                float rate = (float)delta / Metrics.MotionFast;
                m_ZoomLevel = Mathf.MoveToward(m_ZoomLevel, m_TargetZoomLevel, rate * Mathf.Abs(m_TargetZoomLevel - m_ZoomLevel) + 0.0001f);
                if (Mathf.Abs(m_ZoomLevel - m_TargetZoomLevel) < 0.001f)
                {
                    m_ZoomLevel = m_TargetZoomLevel;
                }
                ApplyZoom();
            }

            HandleKeyPan((float)delta);
        }

        /// <summary>按倍率缩放（保持光标位置大致不动）。</summary>
        public void ZoomBy(float factor)
        {
            var before = GetGlobalMousePosition();
            m_TargetZoomLevel = Mathf.Clamp(m_TargetZoomLevel * factor, Metrics.ZoomMin, Metrics.ZoomMax);
            m_ZoomLevel = m_TargetZoomLevel;
            ApplyZoom();

            // 缩放后把光标位置拉回：与屏幕中心取中点，得到"大致固定"的手感
            var after = GetGlobalMousePosition();
            Position += (before - after) * 0.5f;
            ClampPosition();
        }

        /// <summary>直接设定缩放等级。</summary>
        public void SetZoomLevel(float level)
        {
            m_TargetZoomLevel = Mathf.Clamp(level, Metrics.ZoomMin, Metrics.ZoomMax);
            m_ZoomLevel = m_TargetZoomLevel;
            ApplyZoom();
        }

        /// <summary>
        /// 把设计缩放等级换算成 Godot 的 zoom。
        /// 设计式 screen_px_per_art_px = z x screen_height / 540 是对的，
        /// 而 Godot 的 Camera2D.zoom 恰好就是"屏幕像素 / 世界单位"，
        /// 所以 zoom = screen_px_per_art_px。
        /// </summary>
        private void ApplyZoom()
        {
            float spp = Metrics.ScreenPixelsPerArtPixel(m_ZoomLevel, ViewportSize.Y);
            if (PixelPerfect)
            {
                spp = Metrics.SnapToWholeArtPixel(spp);
            }
            Zoom = new Vector2(spp, spp);
        }

        /// <summary>
        /// 屏幕坐标 → 世界坐标。
        ///
        /// 不能用 <c>Node2D.GetGlobalMousePosition()</c>：那个只反映**真实**鼠标位置，
        /// 而拖拽判定需要用输入事件里携带的位置（可能被模拟事件或手柄驱动）。
        /// 换算：world = cameraPos + (screen - viewportCenter) / zoom。
        /// </summary>
        public Vector2 ScreenToWorld(Vector2 screenPos)
        {
            var viewport = GetViewport();
            if (viewport == null)
            {
                return screenPos;
            }

            // 输入事件的位置已在视口坐标系内（stretch 已由 Godot 处理），
            // 逆画布变换即得世界坐标 —— 与摄像机的 zoom/position/anchor 完全一致。
            return viewport.GetCanvasTransform().AffineInverse() * screenPos;
        }

        /// <summary>世界坐标 → 视口坐标（HUD 与提示用）。</summary>
        public Vector2 WorldToScreen(Vector2 worldPos)
        {
            var viewport = GetViewport();
            return viewport == null ? worldPos : viewport.GetCanvasTransform() * worldPos;
        }

        /// <summary>WASD / 方向键平移：600 art px/s ÷ z。</summary>
        private void HandleKeyPan(float delta)
        {
            var dir = Vector2.Zero;
            if (Input.IsActionPressed("ui_left")) dir.X -= 1;
            if (Input.IsActionPressed("ui_right")) dir.X += 1;
            if (Input.IsActionPressed("ui_up")) dir.Y -= 1;
            if (Input.IsActionPressed("ui_down")) dir.Y += 1;

            if (dir == Vector2.Zero)
            {
                return;
            }

            // 除以 zoom：缩放越大，屏幕上的移动越慢，保持"世界速度"恒定
            Position += dir.Normalized() * (Metrics.PanKeySpeed / Mathf.Max(0.01f, Zoom.X)) * delta;
            ClampPosition();
        }

        /// <summary>拖拽平移（由 DragController 在拖空白处时调用）。</summary>
        public void PanBy(Vector2 screenDelta)
        {
            Position -= screenDelta / Zoom;
            ClampPosition();
        }

        /// <summary>把摄像机位置夹到棋盘上：视图中心必须留在棋盘矩形内。</summary>
        private void ClampPosition()
        {
            Position = BoardGeometry.ClampCameraCenter(Position);
        }

        /// <summary>事件卡落在视野外时的 250 ms 微推。</summary>
        public void NudgeTo(Vector2 worldPos)
        {
            var target = Position;
            float halfW = ViewportSize.X * 0.5f / Zoom.X;
            float halfH = ViewportSize.Y * 0.5f / Zoom.Y;

            // 只在该点超出视野时微推
            if (worldPos.X < Position.X - halfW * 0.8f) target.X = worldPos.X + halfW * 0.6f;
            if (worldPos.X > Position.X + halfW * 0.8f) target.X = worldPos.X - halfW * 0.6f;
            if (worldPos.Y < Position.Y - halfH * 0.8f) target.Y = worldPos.Y + halfH * 0.6f;
            if (worldPos.Y > Position.Y + halfH * 0.8f) target.Y = worldPos.Y - halfH * 0.6f;

            if (!target.IsEqualApprox(Position))
            {
                var tween = CreateTween();
                tween.TweenProperty(this, "position", BoardGeometry.ClampCameraCenter(target), Metrics.CameraNudgeTime)
                    .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
            }
        }

        /// <summary>让摄像机初始看向棋盘中心。</summary>
        public void CenterOnBoard()
        {
            var b = BoardGeometry.BoardRect;
            Position = b.Position + b.Size * 0.5f;
            ClampPosition();
        }
    }
}
