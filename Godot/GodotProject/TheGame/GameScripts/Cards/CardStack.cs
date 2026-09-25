using Godot;
using GameLogic.Design;
using System.Collections.Generic;

namespace GameLogic.Cards
{
    /// <summary>
    /// 一个卡牌堆（design 04-card-system.md "Stacking" + 16 号文档的车辆堆叠规则）。
    ///
    /// 术语：
    ///  - <b>root card</b>（根卡）：堆的第一张，屏幕位置最高。
    ///  - <b>top card</b>（顶卡）：最后加入的一张，位置最低、完全可见。
    ///
    /// 布局规则：
    ///  - 每张卡比下一张低 <see cref="Metrics.StackOffset"/> px，使下方每张卡的
    ///    12 行表头保持可见。
    ///  - 堆高 = 56 + 12 x (n - 1)。9 张卡（满配车辆）= 152 px。
    ///  - 10 张以上，根卡表头右端出现计数牌（steel.2 底板 + 数字）。
    ///  - 抓取某张卡时，它**以及它上面的所有卡**一起移动；抓根卡 = 移动整堆。
    /// </summary>
    public partial class CardStack : Node2D
    {
        /// <summary>从根卡到顶卡的顺序表。</summary>
        private readonly List<CardView> m_Cards = new();

        /// <summary>相邻卡片的视觉偏移（art px）。</summary>
        public static int Offset => Metrics.StackOffset;

        public IReadOnlyList<CardView> Cards => m_Cards;

        public int Count => m_Cards.Count;

        /// <summary>根卡；空堆返回 null。</summary>
        public CardView Root => m_Cards.Count > 0 ? m_Cards[0] : null;

        /// <summary>顶卡（最后一张、完全可见）；空堆返回 null。</summary>
        public CardView Top => m_Cards.Count > 0 ? m_Cards[^1] : null;

        /// <summary>堆的世界矩形：根卡左上角 → 顶卡右下角。用于重叠判定与推动。</summary>
        public Rect2 Bounds
        {
            get
            {
                if (m_Cards.Count == 0)
                {
                    return new Rect2();
                }
                var rootPos = m_Cards[0].GlobalPosition;
                return new Rect2(rootPos, new Vector2(
                    Metrics.CardW,
                    Metrics.CardH + Offset * (m_Cards.Count - 1)));
            }
        }

        /// <summary>堆的总质量（06-interaction-motion.md "Mass"）。</summary>
        public float Mass { get; private set; } = Metrics.MassBase;

        /// <summary>堆高（art px）。</summary>
        public int Height => Metrics.CardH + Offset * Mathf.Max(0, m_Cards.Count - 1);

        /// <summary>
        /// 本堆是否正在被拖拽。
        /// 拖拽中的堆不参与收敛，也不参与推动（06-interaction-motion.md 规则 1）。
        /// </summary>
        public bool IsDragging { get; set; }

        public CardStack()
        {
            Name = "CardStack";
        }

        /// <summary>加入一张卡到堆顶（视觉上最下方）。</summary>
        public void Push(CardView card)
        {
            if (card == null || m_Cards.Contains(card))
            {
                return;
            }

            if (card.GetParent() != this)
            {
                card.GetParent()?.RemoveChild(card);
                AddChild(card);
            }

            m_Cards.Add(card);
            RecalculateMass();
            Relayout();
        }

        /// <summary>把一张卡插到指定索引位置（车辆堆叠的固定槽序需要）。</summary>
        public void Insert(int index, CardView card)
        {
            if (card == null || m_Cards.Contains(card))
            {
                return;
            }
            index = Mathf.Clamp(index, 0, m_Cards.Count);

            if (card.GetParent() != this)
            {
                card.GetParent()?.RemoveChild(card);
                AddChild(card);
            }
            m_Cards.Insert(index, card);
            RecalculateMass();
            Relayout();
        }

        /// <summary>移除一张卡。返回是否真的移除了。</summary>
        public bool Remove(CardView card)
        {
            if (!m_Cards.Remove(card))
            {
                return false;
            }
            RecalculateMass();
            Relayout();
            return true;
        }

        /// <summary>
        /// 抓起某张卡：它以及它在堆中的**所有上层卡**一起被摘出，
        /// 组成一个新的堆（design 06 "Pick Up" 第 2 条）。
        /// 返回被摘出的卡列表（自该卡起，顺序不变）。
        /// </summary>
        public List<CardView> DetachFrom(CardView card)
        {
            int index = m_Cards.IndexOf(card);
            if (index < 0)
            {
                return new List<CardView>();
            }

            var taken = m_Cards.GetRange(index, m_Cards.Count - index);
            m_Cards.RemoveRange(index, taken.Count);

            foreach (var c in taken)
            {
                RemoveChild(c);
            }

            RecalculateMass();
            Relayout();
            return taken;
        }

        /// <summary>摘出堆顶的 <paramref name="count"/> 张卡。</summary>
        public List<CardView> DetachTop(int count)
        {
            count = Mathf.Clamp(count, 0, m_Cards.Count);
            if (count == 0)
            {
                return new List<CardView>();
            }
            return DetachFrom(m_Cards[m_Cards.Count - count]);
        }

        /// <summary>是否包含某张卡。</summary>
        public bool Contains(CardView card) => m_Cards.Contains(card);

        /// <summary>
        /// 重新排布：每张卡相对根卡下移 Offset x index。
        /// 根卡位置由外部（推动 / 拖拽 / 收敛）决定，这里只负责跟随。
        /// </summary>
        public void Relayout()
        {
            if (m_Cards.Count == 0)
            {
                return;
            }

            var rootPos = m_Cards[0].Position;

            for (int i = 0; i < m_Cards.Count; i++)
            {
                int depth = i;
                m_Cards[i].Position = rootPos + new Vector2(0, depth * Offset);

                // 深度排序：越靠下越靠前（06 "Depth Sorting"）
                m_Cards[i].ZIndex = i;
            }

            UpdateCountPlate();
        }

        /// <summary>把整堆移到某个世界坐标（根卡左上角）。</summary>
        public void MoveTo(Vector2 worldPos)
        {
            Position = Vector2.Zero;
            if (m_Cards.Count > 0)
            {
                m_Cards[0].GlobalPosition = new Vector2(Mathf.Round(worldPos.X), Mathf.Round(worldPos.Y));
            }
            Relayout();
        }

        private void RecalculateMass()
        {
            float mass = 0;
            foreach (var c in m_Cards)
            {
                mass += CardMass.Of(c.Data);
            }
            Mass = Mathf.Max(Metrics.MassBase, mass);
        }

        /// <summary>10 张以上在根卡表头右端显示计数牌（04-card-system.md）。</summary>
        private void UpdateCountPlate()
        {
            if (m_Cards.Count > 0)
            {
                m_Cards[0].SetStackCount(m_Cards.Count >= 10 ? m_Cards.Count : -1);
            }
        }
    }

    /// <summary>卡牌质量（design 06-interaction-motion.md "Mass"）。</summary>
    public static class CardMass
    {
        /// <summary>单张卡的基准质量，按家族加算。</summary>
        public static float Of(CardData data)
        {
            if (data == null)
            {
                return Metrics.MassBase;
            }

            float mass = Metrics.MassBase;

            // 单位/生物类（本游戏：车辆、主角）按 Mob 计
            if (data.Family is CardFamily.Vehicle or CardFamily.Protagonist)
            {
                mass += Metrics.MassMob;
            }

            // 建筑类
            if (data.Family is CardFamily.Facility)
            {
                mass += Metrics.MassBuilding;
            }

            return mass;
        }
    }
}
