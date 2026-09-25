using Godot;
using GameLogic.Design;

namespace GameLogic.Cards
{
    /// <summary>
    /// 状态外框（design 04-card-system.md "States" + 08-asset-production.md "State outlines"）。
    ///
    /// 规格：50 x 58，画在卡牌 1 px 外侧。卡牌是 48 x 56，所以外框恰好包裹它一圈。
    /// 两种颜色：
    ///  - <c>cyan.3</c> — 有效投放目标 / 选中 / 键盘焦点
    ///  - <c>rust.3</c> — 无效投放目标
    ///
    /// 注意"外框"是**空心**的：只画 1 px 边，不填充内部，否则会盖住卡面。
    /// </summary>
    public static class StateOutline
    {
        public const int W = Metrics.CardW + 2;   // 50
        public const int H = Metrics.CardH + 2;   // 58

        private static readonly System.Collections.Generic.Dictionary<Color, ImageTexture> Cache = new();

        public static ImageTexture Get(Color color)
        {
            if (Cache.TryGetValue(color, out var tex) && tex != null)
            {
                return tex;
            }
            tex = ImageTexture.CreateFromImage(Build(color));
            Cache[color] = tex;
            return tex;
        }

        public static Image Build(Color color)
        {
            var canvas = new Art.PixelCanvas(W, H, new Color(0, 0, 0, 0));

            // 空心 1 px 边
            canvas.FillRect(0, 0, W, 1, color);
            canvas.FillRect(0, H - 1, W, 1, color);
            canvas.FillRect(0, 0, 1, H, color);
            canvas.FillRect(W - 1, 0, 1, H, color);

            // 1 px 圆角，与卡框保持一致
            var clear = new Color(0, 0, 0, 0);
            canvas.Set(0, 0, clear);
            canvas.Set(W - 1, 0, clear);
            canvas.Set(0, H - 1, clear);
            canvas.Set(W - 1, H - 1, clear);

            return canvas.ToImage();
        }
    }

    /// <summary>
    /// 选中态的角括号（design 04-card-system.md）：3x3 cyan.3 括号，
    /// 画在卡牌外侧 2 px，四角各一个。与状态外框叠加使用。
    /// </summary>
    public static class CornerBrackets
    {
        public const int Size = 3;
        public const int Offset = 2;

        private static ImageTexture s_Cached;

        public static ImageTexture Get()
        {
            return s_Cached ??= ImageTexture.CreateFromImage(Build());
        }

        /// <summary>生成 4 个角括号的图集：2x2 布局，每格 3x3。</summary>
        public static Image Build()
        {
            const int cell = Size;
            var canvas = new Art.PixelCanvas(cell * 2, cell * 2, new Color(0, 0, 0, 0));
            var c = Palette.Cyan3;

            // 左上
            canvas.FillRect(0, 0, cell, 1, c);
            canvas.FillRect(0, 0, 1, cell, c);
            // 右上
            canvas.FillRect(cell, 0, cell, 1, c);
            canvas.FillRect(cell * 2 - 1, 0, 1, cell, c);
            // 左下
            canvas.FillRect(0, cell, cell, 1, c);
            canvas.FillRect(0, cell, 1, cell, c);
            // 右下
            canvas.FillRect(cell, cell * 2 - 1, cell, 1, c);
            canvas.FillRect(cell * 2 - 1, cell, 1, cell, c);

            return canvas.ToImage();
        }
    }
}
