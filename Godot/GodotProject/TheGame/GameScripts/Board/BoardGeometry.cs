using Godot;
using GameLogic.Design;

namespace GameLogic.Board
{
    /// <summary>
    /// 棋盘几何（design 03-canvas-layout.md）。
    ///
    /// 世界画布 2208 x 1288，棋盘 1104 x 644 居中于 (552, 322)。
    /// 棋盘自上而下分为槽位带（164）与游戏区（480）。
    ///
    /// 坐标系：世界原点 (0,0) 为世界画布左上角，+y 向下。
    /// 卡牌坐标一律指**卡牌左上角**（与 CardView 的原点约定一致）。
    /// </summary>
    public static class BoardGeometry
    {
        /// <summary>世界画布矩形。</summary>
        public static Rect2 WorldRect => new(0, 0, Metrics.WorldW, Metrics.WorldH);

        /// <summary>棋盘矩形（世界坐标）。</summary>
        public static Rect2 BoardRect => new(
            Metrics.BoardLeft, Metrics.BoardTop, Metrics.BoardW, Metrics.BoardH);

        /// <summary>游戏区矩形（棋盘减去槽位带）。卡牌被限制在这里面。</summary>
        public static Rect2 PlayAreaRect => new(
            Metrics.BoardLeft,
            Metrics.BoardTop + Metrics.BoardSlotBand,
            Metrics.BoardW,
            Metrics.PlayAreaH);

        /// <summary>槽位带矩形。</summary>
        public static Rect2 SlotBandRect => new(
            Metrics.BoardLeft, Metrics.BoardTop, Metrics.BoardW, Metrics.BoardSlotBand);

        /// <summary>
        /// 卡牌左上角的合法范围。卡牌是 48 x 56，所以要减去自身尺寸。
        /// </summary>
        public static Rect2 CardPositionBounds(bool tight)
        {
            var play = PlayAreaRect;
            float margin = tight ? Metrics.ClampMargin : 0f;
            return new Rect2(
                play.Position.X + margin,
                play.Position.Y + margin,
                play.Size.X - Metrics.CardW - margin * 2,
                play.Size.Y - Metrics.CardH - margin * 2);
        }

        /// <summary>把卡牌左上角夹取到合法范围（持握用宽松边界，静止用紧边界）。</summary>
        public static Vector2 ClampCard(Vector2 topLeft, bool tight)
        {
            var b = CardPositionBounds(tight);
            return new Vector2(
                Mathf.Clamp(topLeft.X, b.Position.X, b.End.X),
                Mathf.Clamp(topLeft.Y, b.Position.Y, b.End.Y));
        }

        /// <summary>对齐到网格（E5：86 x 96，刻意大于卡牌）。取最近单元格。</summary>
        public static Vector2 SnapToGrid(Vector2 topLeft)
        {
            var play = PlayAreaRect;
            float relX = topLeft.X - play.Position.X;
            float relY = topLeft.Y - play.Position.Y;
            float col = Mathf.Round(relX / Metrics.GridCellW);
            float row = Mathf.Round(relY / Metrics.GridCellH);
            return ClampCard(new Vector2(
                play.Position.X + col * Metrics.GridCellW,
                play.Position.Y + row * Metrics.GridCellH), tight: true);
        }

        /// <summary>某个世界坐标是否落在棋盘上（拖拽用：是则拖卡，否则平移）。</summary>
        public static bool IsOnBoard(Vector2 worldPos) => BoardRect.HasPoint(worldPos);

        /// <summary>摄像机中心允许的范围：必须留在棋盘矩形内。</summary>
        public static Vector2 ClampCameraCenter(Vector2 center)
        {
            var b = BoardRect;
            return new Vector2(
                Mathf.Clamp(center.X, b.Position.X, b.End.X),
                Mathf.Clamp(center.Y, b.Position.Y, b.End.Y));
        }
    }
}
