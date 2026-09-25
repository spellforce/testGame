using Godot;

namespace GameLogic.Design
{
    /// <summary>
    /// 卡牌家族（design 11-card-taxonomy.md）。8 个家族，其中 Region 与 Cargo
    /// 是为覆盖玩法简报中"无家可归"的类型而新增（复用 world 与 UI 的既有色阶）。
    /// 家族 3 由 "Searchable Building" 更名为 Site。
    /// </summary>
    public enum CardFamily
    {
        /// <summary>玩家车队单位。Cyan 斜坡，轮齿切口。</summary>
        Vehicle = 0,

        /// <summary>掠夺者、无人机、变异机械。Rust 斜坡，锯齿表头。</summary>
        Enemy = 1,

        /// <summary>废墟、残骸、补给站、地堡。Olive 斜坡，锁孔标记。</summary>
        Site = 2,

        /// <summary>风暴、无线电信号、伏击、空投。Amber 斜坡，撕裂顶边。</summary>
        Event = 3,

        /// <summary>车库、炮塔、工坊、燃料库。Steel.2 机箱，四角铆钉。</summary>
        Facility = 4,

        /// <summary>枪、装甲板、引擎、扫描仪。Bone 机箱，危险条纹。</summary>
        Part = 5,

        /// <summary>区域卡：任务表头。Sand 斜坡，等高线弧。</summary>
        Region = 6,

        /// <summary>普通货品（材料、贵重品、补给）。Steel.4 机箱，模板带。</summary>
        Cargo = 7,

        /// <summary>主角。不属于任何家族：bone 机箱 + cyan.3 内描边 + 军牌缺角。</summary>
        Protagonist = 8,
    }

    /// <summary>
    /// 一个家族的视觉定义。视图层永不 switch 家族——一切差异都从这里读取，
    /// 对应 design 09-godot-handoff.md 的 "CardFamilyStyle resources ... so the
    /// view never branches on family in code"。
    /// </summary>
    public sealed class CardFamilyStyle
    {
        public CardFamily Family { get; init; }

        /// <summary>家族名，用于资源路径与调试。</summary>
        public string Token { get; init; } = "";

        /// <summary>机箱基色。</summary>
        public Color Body { get; init; }

        /// <summary>机箱暗色（分隔行、下沿）。</summary>
        public Color Shade { get; init; }

        /// <summary>机箱亮色（美术区顶行高光）。</summary>
        public Color Highlight { get; init; }

        /// <summary>标题颜色。</summary>
        public Color TitleColor { get; init; }

        /// <summary>形状暗示：不用颜色也能区分家族。</summary>
        public ShapeCue Cue { get; init; }

        /// <summary>卡背徽记尺寸 24 x 24（07+08 资产表）。</summary>
        public const int EmblemSize = 24;

        /// <summary>左侧徽章底板颜色（11-card-taxonomy.md 的徽章默认值表）。</summary>
        public Color BadgeLeftPlate { get; init; } = Palette.Bone;

        /// <summary>右侧徽章底板颜色。Enemy 家族改用 steel.2 以免在 rust.2 机箱上不可见。</summary>
        public Color BadgeRightPlate { get; init; } = Palette.Rust2;

        /// <summary>右侧徽章数字颜色。</summary>
        public Color BadgeRightText { get; init; } = Palette.White;
    }

    /// <summary>形状暗示：家族的可达性标识（灰度下仍可辨）。</summary>
    public enum ShapeCue
    {
        /// <summary>Vehicle — 底边 2 个 3x2 轮齿切口。</summary>
        WheelNotches,

        /// <summary>Enemy — 表头下沿 3 颗小齿。</summary>
        HeaderTeeth,

        /// <summary>Site — 页脚中央 3x5 锁孔。</summary>
        Keyhole,

        /// <summary>Event — 顶边 1 px 阶梯锯齿。</summary>
        TornTop,

        /// <summary>Facility — 美术区四角 2x2 steel.7 铆钉。</summary>
        CornerRivets,

        /// <summary>Part — 美术区左沿 4 px hazard/ink 斜纹。</summary>
        HazardStripe,

        /// <summary>Region — 美术区左上 3 道 1 px sand.3 等高线弧。</summary>
        Contours,

        /// <summary>Cargo — 美术区底边 3 px steel.7 模板带，带 3 个缺口。</summary>
        StencilBand,

        /// <summary>Protagonist — 左上角 3x3 军牌缺角 + cyan.3 全内描边。</summary>
        DogTagNotch,
    }

    /// <summary>8 个家族（+ 主角）的唯一定义表。色调锁死在 32 色板内。</summary>
    public static class CardFamilies
    {
        public static readonly CardFamilyStyle Vehicle = new()
        {
            Family = CardFamily.Vehicle,
            Token = "vehicle",
            Body = Palette.Cyan1,
            Shade = Palette.Cyan0,
            Highlight = Palette.Cyan2,
            TitleColor = Palette.White,
            Cue = ShapeCue.WheelNotches,
        };

        public static readonly CardFamilyStyle Enemy = new()
        {
            Family = CardFamily.Enemy,
            Token = "enemy",
            Body = Palette.Rust2,
            Shade = Palette.Rust1,
            Highlight = Palette.Rust3,
            TitleColor = Palette.White,
            Cue = ShapeCue.HeaderTeeth,
            // "On an enemy it switches to a steel.2 plate so it stays visible against the red body."
            BadgeRightPlate = Palette.Steel2,
        };

        public static readonly CardFamilyStyle Site = new()
        {
            Family = CardFamily.Site,
            Token = "site",
            Body = Palette.Olive1,
            Shade = Palette.Olive0,
            Highlight = Palette.Olive2,
            TitleColor = Palette.White,
            Cue = ShapeCue.Keyhole,
        };

        public static readonly CardFamilyStyle Event = new()
        {
            Family = CardFamily.Event,
            Token = "event",
            Body = Palette.Amber2,
            Shade = Palette.Amber1,
            Highlight = Palette.Amber3,
            TitleColor = Palette.Ink,
            Cue = ShapeCue.TornTop,
        };

        public static readonly CardFamilyStyle Facility = new()
        {
            Family = CardFamily.Facility,
            Token = "facility",
            Body = Palette.Steel2,
            Shade = Palette.Steel1,
            Highlight = Palette.Steel3,
            TitleColor = Palette.White,
            Cue = ShapeCue.CornerRivets,
        };

        public static readonly CardFamilyStyle Part = new()
        {
            Family = CardFamily.Part,
            Token = "part",
            Body = Palette.Bone,
            Shade = Palette.Steel7,
            Highlight = Palette.White,
            TitleColor = Palette.Ink,
            Cue = ShapeCue.HazardStripe,
        };

        public static readonly CardFamilyStyle Region = new()
        {
            Family = CardFamily.Region,
            Token = "region",
            Body = Palette.Sand1,
            Shade = Palette.Sand0,
            Highlight = Palette.Sand2,
            TitleColor = Palette.White,
            Cue = ShapeCue.Contours,
        };

        public static readonly CardFamilyStyle Cargo = new()
        {
            Family = CardFamily.Cargo,
            Token = "cargo",
            Body = Palette.Steel4,
            Shade = Palette.Steel3,
            Highlight = Palette.Steel5,
            TitleColor = Palette.White,
            Cue = ShapeCue.StencilBand,
        };

        public static readonly CardFamilyStyle Protagonist = new()
        {
            Family = CardFamily.Protagonist,
            Token = "protagonist",
            Body = Palette.Bone,
            Shade = Palette.Steel7,
            Highlight = Palette.White,
            TitleColor = Palette.Ink,
            Cue = ShapeCue.DogTagNotch,
        };

        /// <summary>8 个真正意义上的"家族"（不含主角，主角是单张卡）。</summary>
        public static readonly CardFamilyStyle[] All =
        {
            Vehicle, Enemy, Site, Event, Facility, Part, Region, Cargo,
        };

        /// <summary>含主角的全部样式，索引 = (int)CardFamily。</summary>
        private static readonly CardFamilyStyle[] ByFamily =
        {
            Vehicle, Enemy, Site, Event, Facility, Part, Region, Cargo, Protagonist,
        };

        public static CardFamilyStyle Get(CardFamily family) => ByFamily[(int)family];

        public static CardFamilyStyle Get(string token)
        {
            foreach (var s in ByFamily)
            {
                if (s.Token == token)
                {
                    return s;
                }
            }
            return Cargo;
        }
    }
}
