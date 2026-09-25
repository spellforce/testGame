using Godot;
using GameLogic.Design;

namespace GameLogic.Cards
{
    /// <summary>
    /// 数字牌与徽章底板（design 04-card-system.md "Badges" + 08-asset-production.md）。
    ///
    /// 徽章规格 13 x 9：底板 + ink 描边 + 3x5 点阵数字。
    /// 设计文档要求数字手绘而非用 Label — 因为在 zoom 0.5 下
    /// 每个数字只有 3x5 个 art px，缩放文本会糊掉。
    ///
    /// 底板配色：
    ///  - 左徽章：bone 底板，ink 数字
    ///  - 右徽章：rust.2 底板，white 数字（Enemy 家族改用 steel.2）
    ///
    /// 数值 100 以上显示 "99"（2 位上限）。
    /// </summary>
    public static class NumberPlate
    {
        public const int W = Metrics.BadgeW;   // 13
        public const int H = Metrics.BadgeH;   // 9

        private static readonly System.Collections.Generic.Dictionary<string, ImageTexture> Cache = new();

        /// <summary>缓存键为 "value_plateHex_textHex" 的纹理。</summary>
        public static ImageTexture Get(int value, Color plate, Color text)
        {
            string key = $"{value}_{plate.ToHtml(false)}_{text.ToHtml(false)}";
            if (Cache.TryGetValue(key, out var tex) && tex != null)
            {
                return tex;
            }
            tex = ImageTexture.CreateFromImage(Build(value, plate, text));
            Cache[key] = tex;
            return tex;
        }

        /// <summary>默认外观（bone 底板 / ink 数字）。</summary>
        public static ImageTexture Get(int value) => Get(value, Palette.Bone, Palette.Ink);

        public static Image Build(int value, Color plate, Color text)
        {
            var c = new Art.PixelCanvas(W, H, new Color(0, 0, 0, 0));

            // 底板 + ink 描边（1 px 圆角）
            c.FillRect(1, 1, W - 2, H - 2, plate);
            c.StrokeRounded(0, 0, W, H, Palette.Ink);

            // 数字：2 位上限，100 以上封顶 "99"
            int shown = Mathf.Clamp(value, 0, 99);
            int d0 = shown / 10;
            int d1 = shown % 10;
            bool twoDigit = shown >= 10;

            // 3x5 数字垂直居中：(9 - 5) / 2 = 2
            const int y = 2;
            if (twoDigit)
            {
                Art.BoardPainter.PaintDigit3x5(c, 2, y, d0, text);
                Art.BoardPainter.PaintDigit3x5(c, 7, y, d1, text);
            }
            else
            {
                // 单个数字居中：x 5
                Art.BoardPainter.PaintDigit3x5(c, 5, y, d1, text);
            }

            return c.ToImage();
        }
    }
}
