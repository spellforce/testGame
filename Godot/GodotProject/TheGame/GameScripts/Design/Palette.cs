using Godot;

namespace GameLogic.Design
{
    /// <summary>
    /// Iron Horizon 32 色调色板（design 02-design-tokens.md）。
    /// 全部美术资源必须锁定到这些颜色；不允许新增色。
    ///
    /// 同时提供：
    /// - 名称 → 索引 / 索引 → 颜色 的双向查表（供 GIMP .gpl 与工具链使用）
    /// - DisabledSwap：按相对亮度把任意调色板颜色映射到 steel 斜坡，
    ///   用于"禁用/耗尽"状态的调色板交换（04-card-system.md）。
    /// </summary>
    public static class Palette
    {
        // ---- 基础 32 色（顺序与 art/palette/iron_horizon_32.gpl 完全一致） ----

        public static readonly Color Ink = Color.FromHtml("0E1110");

        public static readonly Color Steel1 = Color.FromHtml("1B211F");
        public static readonly Color Steel2 = Color.FromHtml("252A28");
        public static readonly Color Steel3 = Color.FromHtml("353A36");
        public static readonly Color Steel4 = Color.FromHtml("4B524D");
        public static readonly Color Steel5 = Color.FromHtml("66706A");
        public static readonly Color Steel6 = Color.FromHtml("8E968D");
        public static readonly Color Steel7 = Color.FromHtml("B8BEB0");

        public static readonly Color Bone = Color.FromHtml("D7D5BF");
        public static readonly Color White = Color.FromHtml("EEF0DF");

        public static readonly Color Sand0 = Color.FromHtml("3B3328");
        public static readonly Color Sand1 = Color.FromHtml("5E5240");
        public static readonly Color Sand2 = Color.FromHtml("857657");
        public static readonly Color Sand3 = Color.FromHtml("B3A485");
        public static readonly Color Sand4 = Color.FromHtml("D8CBA6");

        public static readonly Color Rust0 = Color.FromHtml("3A1D17");
        public static readonly Color Rust1 = Color.FromHtml("6E2F22");
        public static readonly Color Rust2 = Color.FromHtml("A83B32");
        public static readonly Color Rust3 = Color.FromHtml("C94D3C");
        public static readonly Color Rust4 = Color.FromHtml("EC8B78");

        public static readonly Color Amber0 = Color.FromHtml("5C3514");
        public static readonly Color Amber1 = Color.FromHtml("A7641F");
        public static readonly Color Amber2 = Color.FromHtml("E9A93A");
        public static readonly Color Amber3 = Color.FromHtml("F5D27A");

        public static readonly Color Olive0 = Color.FromHtml("2E3522");
        public static readonly Color Olive1 = Color.FromHtml("56613C");
        public static readonly Color Olive2 = Color.FromHtml("98A66B");

        public static readonly Color Cyan0 = Color.FromHtml("1E3B40");
        public static readonly Color Cyan1 = Color.FromHtml("2B5F65");
        public static readonly Color Cyan2 = Color.FromHtml("3D878E");
        public static readonly Color Cyan3 = Color.FromHtml("8ECED1");

        public static readonly Color Hazard = Color.FromHtml("D5C15A");

        // ---- 半透明例外色（仅这三处允许透明度） ----

        /// <summary>卡牌投影：ink 45%。</summary>
        public static readonly Color ShadowCard = new(0x0E / 255f, 0x11 / 255f, 0x10 / 255f, 0.45f);

        /// <summary>暂停遮罩：ink 35%，压暗世界。</summary>
        public static readonly Color OverlayPause = new(0x0E / 255f, 0x11 / 255f, 0x10 / 255f, 0.35f);

        /// <summary>模态遮罩：ink 75%，置于模态菜单之后。</summary>
        public static readonly Color OverlayModal = new(0x0E / 255f, 0x11 / 255f, 0x10 / 255f, 0.75f);

        // ---- 有序表 ----

        /// <summary>32 色，索引即调色板序号。</summary>
        public static readonly Color[] Colors =
        {
            Ink,
            Steel1, Steel2, Steel3, Steel4, Steel5, Steel6, Steel7,
            Bone, White,
            Sand0, Sand1, Sand2, Sand3, Sand4,
            Rust0, Rust1, Rust2, Rust3, Rust4,
            Amber0, Amber1, Amber2, Amber3,
            Olive0, Olive1, Olive2,
            Cyan0, Cyan1, Cyan2, Cyan3,
            Hazard,
        };

        /// <summary>与 <see cref="Colors"/> 一一对应的 token 名称。</summary>
        public static readonly string[] Names =
        {
            "ink",
            "steel.1", "steel.2", "steel.3", "steel.4", "steel.5", "steel.6", "steel.7",
            "bone", "white",
            "sand.0", "sand.1", "sand.2", "sand.3", "sand.4",
            "rust.0", "rust.1", "rust.2", "rust.3", "rust.4",
            "amber.0", "amber.1", "amber.2", "amber.3",
            "olive.0", "olive.1", "olive.2",
            "cyan.0", "cyan.1", "cyan.2", "cyan.3",
            "hazard",
        };

        public const int Count = 32;

        /// <summary>名称为空时返回 true（用于校验美术资源未越出色板）。</summary>
        public static bool TryGetIndex(Color color, out int index)
        {
            for (int i = 0; i < Colors.Length; i++)
            {
                if (Colors[i].IsEqualApprox(color))
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        public static Color ByName(string token)
        {
            int i = System.Array.IndexOf(Names, token);
            return i < 0 ? Ink : Colors[i];
        }

        // ---- 禁用状态：调色板交换到 steel 斜坡 ----

        /// <summary>
        /// 索引 i 的"禁用态"颜色索引。规则：取相对亮度最接近的 steel 色。
        /// steel 斜坡索引为 1..7（steel.1..steel.7），不参与交换的颜色映射到自身。
        /// 预计算一次，供 shader LUT 或程序化贴图使用。
        /// </summary>
        public static readonly int[] DisabledSwap = BuildDisabledSwap();

        private static int[] BuildDisabledSwap()
        {
            int[] map = new int[Count];
            for (int i = 0; i < Count; i++)
            {
                // 已经是 steel 或 ink，保持原样
                if (i >= 1 && i <= 7 || i == 0)
                {
                    map[i] = i;
                    continue;
                }

                float target = Luminance(Colors[i]);
                int best = 1;
                float bestDelta = float.MaxValue;
                for (int s = 1; s <= 7; s++)
                {
                    float delta = Mathf.Abs(Luminance(Colors[s]) - target);
                    if (delta < bestDelta)
                    {
                        bestDelta = delta;
                        best = s;
                    }
                }
                map[i] = best;
            }
            return map;
        }

        /// <summary>Rec. 709 相对亮度，用于交换匹配与对比度校验。</summary>
        public static float Luminance(Color c) => 0.2126f * c.R + 0.7152f * c.G + 0.0722f * c.B;

        /// <summary>WCAG 对比度（1..21）。</summary>
        public static float Contrast(Color a, Color b)
        {
            float la = Luminance(a);
            float lb = Luminance(b);
            float hi = Mathf.Max(la, lb);
            float lo = Mathf.Min(la, lb);
            return (hi + 0.05f) / (lo + 0.05f);
        }
    }
}
