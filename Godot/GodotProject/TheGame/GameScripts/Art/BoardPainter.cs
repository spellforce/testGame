using Godot;
using GameLogic.Design;

namespace GameLogic.Art
{
    /// <summary>
    /// 棋盘绘制器（design 01-visual-direction.md "Board Look" + 03/16 号文档）。
    ///
    /// 结构自上而下：
    ///   - 围栏：3 px，自外向内 ink / steel.7 / steel.4（替代 Stacklands 的白色圆角外框）
    ///   - 甲板：steel.6 底，16 x 16 菱形防滑纹（steel.5）
    ///   - 板缝：每 64 art px 一条，1 px steel.5 + 下方 1 px steel.7 高光
    ///   - 磨损：少量 rust.1 锈斑、底部浮尘、褪色钢印数字（覆盖面积 &lt; 5%）
    ///   - 槽位带：164 高，两行；分隔线 2 px（ink + steel.7）位于 y 164
    ///
    /// 设计原则："Keep the board calm: wear marks cover less than 5% of its area.
    /// Cards must be the busiest thing on it."
    /// </summary>
    public static class BoardPainter
    {
        private const int W = Metrics.BoardW;   // 1104
        private const int H = Metrics.BoardH;   // 644
        private const int Rail = Metrics.BoardRail;      // 3
        private const int Band = Metrics.BoardSlotBand;  // 164
        private const int SeamStep = 64;

        private static ImageTexture s_Cached;

        /// <summary>
        /// 取棋盘纹理（惰性构建 + 缓存）。
        /// 必须走 <see cref="Build"/> —— 直接 new 一个填充色画布会得到
        /// 一块没有任何细节的纯色矩形。
        /// </summary>
        public static ImageTexture GetTexture()
        {
            return s_Cached ??= Build().ToTexture("BoardPlate");
        }

        /// <summary>完整绘制棋盘甲板 + 围栏 + 槽位带 + 磨损。</summary>
        public static PixelCanvas Build()
        {
            var c = new PixelCanvas(W, H, Palette.Steel6);

            PaintDeck(c);
            PaintWear(c);
            PaintSlotBand(c);
            PaintRail(c);

            return c;
        }

        // ==================== 甲板防滑纹 ====================

        /// <summary>
        /// 16 x 16 菱形防滑纹。绘制很淡——每格只有 4 个像素点用 steel.5，
        /// 保证棋盘"安静"，不与卡牌争夺注意力。
        /// </summary>
        private static void PaintDeck(PixelCanvas c)
        {
            const int tile = 16;
            for (int ty = 0; ty < H; ty += tile)
            {
                for (int tx = 0; tx < W; tx += tile)
                {
                    // 菱形四角：轻微斜向纹理
                    c.Set(tx + 4, ty + 4, Palette.Steel5);
                    c.Set(tx + 11, ty + 4, Palette.Steel5);
                    c.Set(tx + 4, ty + 11, Palette.Steel5);
                    c.Set(tx + 11, ty + 11, Palette.Steel5);
                }
            }

            // 板缝：每 64 px 一条，1 px steel.5 + 下方 1 px steel.7 高光
            for (int x = SeamStep; x < W - Rail; x += SeamStep)
            {
                for (int y = Rail; y < H - Rail; y++)
                {
                    c.Set(x, y, Palette.Steel5);
                    c.Set(x + 1, y, Palette.Steel7);
                }
            }
            for (int y = SeamStep; y < H - Rail; y += SeamStep)
            {
                for (int x = Rail; x < W - Rail; x++)
                {
                    c.Set(x, y, Palette.Steel5);
                    c.Set(x, y + 1, Palette.Steel7);
                }
            }
        }

        // ==================== 磨损（覆盖 < 5%） ====================

        /// <summary>
        /// 固定种子的确定性随机——保证每次运行棋盘外观一致，
        /// 避免读档前后棋盘"换了张脸"。
        /// </summary>
        private static void PaintWear(PixelCanvas c)
        {
            // 固定种子：棋盘磨损每次运行必须一致
            var rng = new System.Random(20471);

            // 锈斑：2..4 px，散布在甲板上
            for (int i = 0; i < 26; i++)
            {
                int x = rng.Next(Rail + 8, W - Rail - 8);
                int y = rng.Next(Band + 8, H - Rail - 8);
                int size = rng.Next(2, 5);
                for (int dy = 0; dy < size; dy++)
                {
                    for (int dx = 0; dx < size; dx++)
                    {
                        // 只画外缘，内部留空 → 更像锈迹而非色块
                        bool edge = dx == 0 || dy == 0 || dx == size - 1 || dy == size - 1;
                        if (edge && rng.Next(2) == 0)
                        {
                            c.Set(x + dx, y + dy, Palette.Rust1);
                        }
                    }
                }
            }

            // 底部浮尘：沿下沿的 sand.3 点状带
            for (int i = 0; i < 220; i++)
            {
                int x = rng.Next(Rail, W - Rail);
                int y = H - Rail - 1 - rng.Next(0, 6);
                if (rng.Next(2) == 0)
                {
                    c.Set(x, y, Palette.Sand3);
                }
            }

            // 褪色钢印：6..10 px 的钢印编号，steel.4
            PaintStencil(c, 96, Band + 62);
            PaintStencil(c, 640, H - 96);
            PaintStencil(c, 300, H - 210);
        }

        /// <summary>褪色钢印数字（用 3x5 点阵，避免依赖字体资源）。</summary>
        private static void PaintStencil(PixelCanvas c, int x, int y)
        {
            // 5 个数字：3x5 点阵
            int[] digits = { 2, 0, 4, 7, 1 };
            for (int d = 0; d < digits.Length; d++)
            {
                PaintDigit3x5(c, x + d * 5, y, digits[d], Palette.Steel4);
            }
        }

        // ==================== 槽位带 ====================

        /// <summary>
        /// 槽位带（16-garage-and-expedition-boards.md）：
        ///   14 + 81 + 9 + 48 + 12 = 164
        /// 行 1：9 个 60x81 槽位，64 px 间距，居中，顶边 y 14
        /// 行 2：40x48 芯片格，48 px 间距，顶边 y 104
        /// 分隔线：2 px（ink + steel.7）位于 y 164
        /// </summary>
        private static void PaintSlotBand(PixelCanvas c)
        {
            // 带底：比甲板略深，让槽位区读作"另一层"
            c.FillRect(Rail, Rail, W - Rail * 2, Band - Rail, Palette.Steel5);

            // ---- 行 1：9 个槽位，水平居中 ----
            int row1Y = Metrics.SlotRow1Y;
            int total1 = Metrics.SlotPitch * Metrics.SlotCount - (Metrics.SlotPitch - Metrics.SlotW);
            int startX = (W - total1) / 2;

            for (int i = 0; i < Metrics.SlotCount; i++)
            {
                int sx = startX + i * Metrics.SlotPitch;
                PaintSlotPlate(c, sx, row1Y, Metrics.SlotW, Metrics.SlotH);
            }

            // ---- 行 2：芯片格（技能托盘 / 预留），自左起 ----
            int row2Y = Metrics.ChipRow2Y;
            const int chipCount = 12;
            for (int i = 0; i < chipCount; i++)
            {
                int sx = startX + i * Metrics.ChipPitch;
                PaintSlotPlate(c, sx, row2Y, Metrics.ChipW, Metrics.ChipH);
            }

            // ---- 分隔线 2 px：ink + steel.7，位于 y 164 ----
            c.FillRect(Rail, Band, W - Rail * 2, 1, Palette.Ink);
            c.FillRect(Rail, Band + 1, W - Rail * 2, 1, Palette.Steel7);
        }

        /// <summary>单个槽位底板：steel.2 暗板 + ink 描边 + steel.3 内高光。</summary>
        private static void PaintSlotPlate(PixelCanvas c, int x, int y, int w, int h)
        {
            c.FillRect(x, y, w, h, Palette.Steel2);
            // 内高光：顶行 + 左列
            c.FillRect(x + 1, y + 1, w - 2, 1, Palette.Steel3);
            c.FillRect(x + 1, y + 1, 1, h - 2, Palette.Steel3);
            c.StrokeRounded(x, y, w, h, Palette.Ink);
        }

        // ==================== 围栏 ====================

        /// <summary>围栏 3 px：ink → steel.7 → steel.4（自外向内）。</summary>
        private static void PaintRail(PixelCanvas c)
        {
            for (int i = 0; i < Rail; i++)
            {
                var color = i switch
                {
                    0 => Palette.Ink,
                    1 => Palette.Steel7,
                    _ => Palette.Steel4,
                };
                c.FillRect(i, i, W - i * 2, 1, color);
                c.FillRect(i, H - 1 - i, W - i * 2, 1, color);
                c.FillRect(i, i, 1, H - i * 2, color);
                c.FillRect(W - 1 - i, i, 1, H - i * 2, color);
            }
        }

        // ==================== 3x5 点阵数字 ====================

        /// <summary>3x5 点阵数字（对应设计文档的 font.digits.small，1 px 笔画）。</summary>
        private static readonly byte[][] Digits3x5 =
        {
            new byte[] { 0b111, 0b101, 0b101, 0b101, 0b111 }, // 0
            new byte[] { 0b010, 0b110, 0b010, 0b010, 0b111 }, // 1
            new byte[] { 0b111, 0b001, 0b111, 0b100, 0b111 }, // 2
            new byte[] { 0b111, 0b001, 0b111, 0b001, 0b111 }, // 3
            new byte[] { 0b101, 0b101, 0b111, 0b001, 0b001 }, // 4
            new byte[] { 0b111, 0b100, 0b111, 0b001, 0b111 }, // 5
            new byte[] { 0b111, 0b100, 0b111, 0b101, 0b111 }, // 6
            new byte[] { 0b111, 0b001, 0b010, 0b010, 0b010 }, // 7
            new byte[] { 0b111, 0b101, 0b111, 0b101, 0b111 }, // 8
            new byte[] { 0b111, 0b101, 0b111, 0b001, 0b111 }, // 9
        };

        /// <summary>
        /// 画一个 3x5 数字。设计文档要求所有数字等宽（含 1 和 7），
        /// 否则右对齐的列会抖动。
        /// </summary>
        public static void PaintDigit3x5(PixelCanvas c, int x, int y, int digit, Color color)
        {
            if (digit < 0 || digit > 9)
            {
                return;
            }
            var rows = Digits3x5[digit];
            for (int r = 0; r < 5; r++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if ((rows[r] & (1 << (2 - col))) != 0)
                    {
                        c.Set(x + col, y + r, color);
                    }
                }
            }
        }
    }
}
