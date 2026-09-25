using Godot;
using GameLogic.Design;

namespace GameLogic.Cards
{
	/// <summary>卡牌品级（05-card-families.md）：只改描边，永不改尺寸或机箱色。</summary>
	public enum CardGrade
	{
		/// <summary>ink 描边。</summary>
		Standard = 0,

		/// <summary>ink 描边 + 内侧 1 px amber.3 线（仅顶边）。</summary>
		Rare = 1,

		/// <summary>ink 描边 + 内侧 1 px cyan.3 线（整圈）。</summary>
		Prototype = 2,
	}

	/// <summary>
	/// 卡牌状态（04-card-system.md，共 9 态 + 11 号文档新增 2 态）。
	/// 组合时的优先级：Held &gt; Invalid &gt; Selected &gt; Hit &gt; Busy &gt; Hover &gt; Resting。
	/// </summary>
	[System.Flags]
	public enum CardState
	{
		None = 0,
		Resting = 1 << 0,
		Hover = 1 << 1,
		Held = 1 << 2,
		ValidTarget = 1 << 3,
		InvalidTarget = 1 << 4,
		Selected = 1 << 5,
		Busy = 1 << 6,
		Disabled = 1 << 7,
		Hit = 1 << 8,
		New = 1 << 9,

		/// <summary>11-card-taxonomy.md 新增：固定不动（区域卡、带内内容）。</summary>
		Pinned = 1 << 10,

		/// <summary>11-card-taxonomy.md 新增：超载（车辆）。</summary>
		Overloaded = 1 << 11,
	}

	/// <summary>
	/// 卡牌数据（09-godot-handoff.md 的 CardData + 11/12 号文档的扩展）。
	/// 这是**唯一**的卡牌静态定义；视图只读它，玩法字段后续追加。
	/// </summary>
	[GlobalClass]
	public partial class CardData : Resource
	{
		/// <summary>与图标文件名匹配：card_icon_&lt;family&gt;_&lt;id&gt;.png</summary>
		[Export] public string Id { get; set; } = "";

		[Export] public CardFamily Family { get; set; } = CardFamily.Cargo;

		/// <summary>短名：最多 4 个汉字 / 约 8 个拉丁字母。卡片数据必须提供短名，永不换行省略。</summary>
		[Export] public string Title { get; set; } = "";

		/// <summary>全名，显示在信息面板。</summary>
		[Export] public string FullName { get; set; } = "";

		[Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";

		[Export] public Texture2D Icon { get; set; }

		/// <summary>-1 = 隐藏。左侧徽章（bone 底板、ink 数字）。</summary>
		[Export] public int BadgeLeft { get; set; } = -1;

		/// <summary>-1 = 隐藏。右侧徽章。</summary>
		[Export] public int BadgeRight { get; set; } = -1;

		[Export] public CardGrade Grade { get; set; } = CardGrade.Standard;

		// ---- 11-card-taxonomy.md 的子暗示 ----

		/// <summary>Part 家族的种类标记（炮弹/引擎/C 单元/图纸）。None = 无。</summary>
		[Export] public PartKind PartKind { get; set; } = PartKind.None;

		/// <summary>Cargo 家族的补给带（装甲包、维修包等可直接消耗品）。</summary>
		[Export] public bool IsSupply { get; set; }

		/// <summary>Region 家族的撤离点信标。</summary>
		[Export] public bool IsEvacuationPoint { get; set; }

		// ---- 包（pack）presentation，不是家族 ----

		/// <summary>是卡包：卡背使用内容物家族色 + 6 px bone 包装带。</summary>
		[Export] public bool IsPack { get; set; }

		/// <summary>包内剩余张数（-1 = 非包）。</summary>
		[Export] public int PackRemaining { get; set; } = -1;

		/// <summary>视觉样式（含主角）。</summary>
		public CardFamilyStyle Style => CardFamilies.Get(Family);

		/// <summary>面板与调试用的显示名。</summary>
		public string DisplayName => string.IsNullOrEmpty(FullName) ? Title : FullName;
	}

	/// <summary>Part 家族的种类标记（11-card-taxonomy.md "Part — kind tag"）。</summary>
	public enum PartKind
	{
		None = 0,

		/// <summary>主炮 — 大炮弹。</summary>
		MainCannon = 1,

		/// <summary>副炮 — 小炮弹。</summary>
		SecondaryCannon = 2,

		/// <summary>S-E — 四角星。</summary>
		SpecialEquipment = 3,

		/// <summary>引擎 — 活塞。</summary>
		Engine = 4,

		/// <summary>C 单元 — 芯片。</summary>
		CUnit = 5,

		/// <summary>图纸 — 3x3 网格，cyan.3 绘制。</summary>
		Blueprint = 6,
	}
}
