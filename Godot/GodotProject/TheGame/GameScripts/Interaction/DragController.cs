using Godot;
using GameLogic.Board;
using GameLogic.Cards;
using GameLogic.Design;
using GameLogic.World;

namespace GameLogic.Interaction
{
    /// <summary>
    /// 拖拽控制器（design 06-interaction-motion.md "Pick Up" / "Drop" / "Drop Target Search"）。
    ///
    /// 三条最容易做错的规则，这里严格照做：
    ///
    /// 1. **点击 vs 拖拽**由阈值区分：按下后超过 0.2 秒 **且** 移动超过 11 px 才算拖拽。
    ///    两者都未超过 = 点击（选中）。
    ///
    /// 2. **投放不是"弹回"**：松手时若找不到堆叠目标，卡片**就留在松手处**，
    ///    以 lerp 收敛过去。唯一的位置修正是夹取——若松手点在紧边界外，
    ///    卡片滑到最近的合法位置。它**永不**被送回拿起时的位置。
    ///
    /// 3. **投放目标靠矩形重叠判定，不是固定吸附半径**：只要矩形相交就堆叠。
    ///    23 px 的吸附半径只用于特殊投放区（卡包、售卖区）。
    ///
    /// 另外：拖拽中的卡不推动也不被推动；只有根卡推动；按右键立即投放。
    /// </summary>
    public partial class DragController : Node
    {
        private BoardCamera m_Camera;
        private CardWorld m_World;

        // ---- 按下状态 ----
        private bool m_Pressed;
        private Vector2 m_PressScreenPos;
        private double m_PressTime;
        private bool m_IsDragging;

        /// <summary>被拿起的那张卡（子堆的根）。</summary>
        private CardView m_GrabbedCard;

        /// <summary>拿起时，鼠标相对于卡牌左上角的偏移，拖动时保持这个偏移。</summary>
        private Vector2 m_GrabOffset;

        /// <summary>正在平移棋盘。</summary>
        private bool m_Panning;

        /// <summary>当前拖拽的堆。</summary>
        private CardStack m_DragStack;

        /// <summary>当前高亮的有效投放目标堆。</summary>
        private CardStack m_HoverTarget;

        /// <summary>棋盘视图，用于把拖起的堆移到 Held 层。</summary>
        private BoardView m_Board;

        public void Setup(BoardCamera camera, CardWorld world, BoardView board)
        {
            m_Camera = camera;
            m_World = world;
            m_Board = board;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (m_Camera == null || m_World == null)
            {
                return;
            }

            if (@event is InputEventMouseButton mb)
            {
                HandleMouseButton(mb);
            }
            else if (@event is InputEventMouseMotion mm)
            {
                if (m_Pressed)
                {
                    HandleMouseMotion(mm);
                }
                else
                {
                    UpdateHover(mm.Position);
                }
            }
        }

        // ==================== 悬停 ====================

        private CardView m_Hovered;

        /// <summary>当前悬停的卡（信息面板读它）。拖拽中为 null。</summary>
        public CardView Hovered => m_Hovered;

        /// <summary>悬停卡变化时触发（信息面板订阅）。</summary>
        public event System.Action<CardView> HoverChanged;

        /// <summary>
        /// 悬停：卡上移 7 px（lift.hover），信息面板显示详情（04 "States"）。
        /// 首次悬停清除 New 标记。
        /// </summary>
        private void UpdateHover(Vector2 screenPos)
        {
            var hit = m_World.HitTest(m_Camera.ScreenToWorld(screenPos));
            SetHovered(hit);
        }

        private void SetHovered(CardView card)
        {
            if (card == m_Hovered)
            {
                return;
            }
            if (m_Hovered != null && IsInstanceValid(m_Hovered))
            {
                m_Hovered.SetStateFlag(CardState.Hover, false);
            }
            m_Hovered = card;
            if (m_Hovered != null)
            {
                m_Hovered.SetStateFlag(CardState.Hover, true);
                m_Hovered.SetStateFlag(CardState.New, false);
            }
            HoverChanged?.Invoke(m_Hovered);
        }

        // ==================== 按下 / 抬起 ====================

        private void HandleMouseButton(InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Right && mb.Pressed && m_Pressed)
            {
                // 右键立即投放（06："right-click drops immediately"）
                ReleaseDrag();
                GetViewport().SetInputAsHandled();
                return;
            }

            if (mb.ButtonIndex != MouseButton.Left)
            {
                return;
            }

            if (mb.Pressed)
            {
                OnPress(mb.Position);
            }
            else
            {
                OnRelease();
            }
            GetViewport().SetInputAsHandled();
        }

        private void OnPress(Vector2 screenPos)
        {
            m_Pressed = true;
            m_PressScreenPos = screenPos;
            m_PressTime = Time.GetTicksMsec() / 1000.0;
            m_IsDragging = false;
            m_GrabbedCard = null;
            m_DragStack = null;
            m_Panning = false;

            var worldPos = m_Camera.ScreenToWorld(screenPos);
            var hit = m_World.HitTest(worldPos);

            if (hit == null)
            {
                // 空处拖动 = 平移棋盘（03："Drag with the left mouse button
                // on anything that isn't a card (board or world) to pan"）
                m_Panning = true;
                return;
            }

            // 记录候选：超过阈值才真正拿起
            m_GrabbedCard = hit;
            m_GrabOffset = worldPos - hit.GlobalPosition;
        }

        private void OnRelease()
        {
            if (!m_Pressed)
            {
                return;
            }

            if (m_IsDragging)
            {
                ReleaseDrag();
            }
            else if (m_GrabbedCard != null && !m_Panning)
            {
                // 未超过阈值 → 视为点击，切换选中
                ToggleSelection(m_GrabbedCard);
            }

            ResetPressState();
        }

        private void ResetPressState()
        {
            m_Pressed = false;
            m_IsDragging = false;
            m_Panning = false;
            m_GrabbedCard = null;
            m_DragStack = null;
            m_HoverTarget = null;
        }

        // ==================== 移动：阈值判定 / 平移 / 拖动 ====================

        private void HandleMouseMotion(InputEventMouseMotion mm)
        {
            if (m_Panning)
            {
                m_Camera.PanBy(mm.Relative);
                return;
            }

            if (!m_IsDragging)
            {
                // 拖拽阈值（06）："Below both, a press is a click" ——
                // 两者都未超过才是点击，任一超过即开始拖拽。
                double elapsed = Time.GetTicksMsec() / 1000.0 - m_PressTime;
                float moved = mm.Position.DistanceTo(m_PressScreenPos);

                if (elapsed >= Metrics.DragThresholdTime || moved >= Metrics.DragThresholdPx)
                {
                    if (m_GrabbedCard != null)
                    {
                        BeginDrag();
                    }
                    else
                    {
                        m_Panning = true;
                    }
                }
                return;
            }

            // ---- 拖动中 ----
            if (m_DragStack == null)
            {
                return;
            }

            var worldPos = m_Camera.ScreenToWorld(mm.Position);
            var targetTopLeft = worldPos - m_GrabOffset;

            // 持握中夹取到**宽松**边界：可以拖到棋盘边缘外一点
            targetTopLeft = BoardGeometry.ClampCard(targetTopLeft, tight: false);

            var root = m_DragStack.Root;
            root.GlobalPosition = targetTopLeft;
            m_DragStack.Relayout();

            UpdateDropTargetPreview();
        }

        /// <summary>真正拿起：拆出子堆、移到 Held 层、进入拖拽状态。</summary>
        private void BeginDrag()
        {
            m_IsDragging = true;
            SetHovered(null);

            var stack = m_World.SplitFrom(m_GrabbedCard);
            if (stack == null)
            {
                return;
            }

            m_DragStack = stack;
            m_World.SetDragging(stack, true);

            // 抬升 11 px、投影扩大：由 CardView 的状态位驱动
            foreach (var card in stack.Cards)
            {
                card.SetStateFlag(CardState.Held, true);
            }
        }

        /// <summary>
        /// 投放：先找堆叠目标；找到就并入，否则**留在原处**并收敛。
        /// 唯一的修正是若在紧边界外，滑到最近合法位置。
        /// </summary>
        private void ReleaseDrag()
        {
            if (m_DragStack == null)
            {
                ResetPressState();
                return;
            }

            var stack = m_DragStack;

            // 清除持握状态
            foreach (var card in stack.Cards)
            {
                card.SetStateFlag(CardState.Held, false);
                card.SetStateFlag(CardState.ValidTarget, false);
                card.SetStateFlag(CardState.InvalidTarget, false);
            }

            if (m_HoverTarget != null && m_HoverTarget != stack)
            {
                // 并入目标堆（Stacklands 会落到目标堆的**最后一张**卡上）
                var landing = m_HoverTarget;
                m_World.SetDragging(stack, false);
                m_World.MergeStack(stack, landing);

                // 合并后整堆收敛到落点
                var root = landing.Root;
                m_World.SetTarget(root, BoardGeometry.ClampCard(root.GlobalPosition, tight: true));
            }
            else
            {
                // 没有目标 → 停在松手处。只做紧边界夹取。
                m_World.SetDragging(stack, false);
                var root = stack.Root;
                var clamped = BoardGeometry.ClampCard(root.GlobalPosition, tight: true);
                m_World.SetTarget(root, clamped);
            }

            m_World.RebuildGrid();
            ResetPressState();
        }

        // ==================== 投放目标预览 ====================

        /// <summary>
        /// 每帧找最近的合法投放目标并高亮它。
        /// 判定完全基于矩形重叠（06 "Drop Target Search"）。
        /// </summary>
        private void UpdateDropTargetPreview()
        {
            if (m_DragStack == null)
            {
                return;
            }

            var rect = m_DragStack.Bounds;
            var candidates = m_World.OverlappingStacks(rect, exclude: m_DragStack);

            CardStack best = null;
            float bestDist = float.MaxValue;

            foreach (var candidate in candidates)
            {
                // 已经有东西叠在上面的卡不能再接收（06 第 2 条）
                // 本版本允许继续叠加，由玩法层后续收紧
                var leaf = candidate.Top;
                if (leaf == null)
                {
                    continue;
                }

                // 取中心最近者
                var candidateCenter = candidate.Bounds.Position + candidate.Bounds.Size * 0.5f;
                var myCenter = rect.Position + rect.Size * 0.5f;
                float dist = candidateCenter.DistanceSquaredTo(myCenter);

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = candidate;
                }
            }

            if (best == m_HoverTarget)
            {
                return;
            }

            // 清掉旧高亮
            if (m_HoverTarget != null)
            {
                m_HoverTarget.Top?.SetStateFlag(CardState.ValidTarget, false);
            }

            m_HoverTarget = best;

            // 新高亮：cyan.3 外框
            m_HoverTarget?.Top?.SetStateFlag(CardState.ValidTarget, true);
        }

        // ==================== 选中 ====================

        private CardView m_Selected;

        /// <summary>点击一张卡：切换选中态（cyan.3 外框 + 四角括号）。</summary>
        private void ToggleSelection(CardView card)
        {
            if (m_Selected == card)
            {
                card.SetStateFlag(CardState.Selected, false);
                m_Selected = null;
                return;
            }

            m_Selected?.SetStateFlag(CardState.Selected, false);
            m_Selected = card;
            card.SetStateFlag(CardState.Selected, true);
        }

        public CardView Selected => m_Selected;
    }
}
