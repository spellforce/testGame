using Godot;
using GameLogic.Design;

namespace GameLogic.Cards
{
    /// <summary>
    /// 单张卡的视图（design 09-godot-handoff.md "Card.tscn" + 04-card-system.md）。
    ///
    /// 节点结构对齐设计文档，但用 C# 直接搭：
    /// <code>
    /// Card (Node2D)                 原点 = 卡牌左上角
    ///  |- Shadow (ColorRect 48x56)  ink 45%，不随抬升移动，只扩大偏移
    ///  |- Body (Node2D)             抬升时整体上移；逻辑位置永不改变
    ///      |- Frame (Sprite2D)      家族卡框
    ///      |- Icon (Sprite2D)
    ///      |- Title (Label, 10px)
    ///      |- BadgeLeft / BadgeRight
    ///      |- NewDot (Sprite2D)
    ///      |- Outline (Sprite2D)    cyan/red 状态外框
    /// </code>
    ///
    /// 关键规则：卡牌**永不缩放、永不旋转**（01-visual-direction.md 第 3 条），
    /// 所有状态只用位移、描边与调色板交换表达。
    /// </summary>
    public partial class CardView : Node2D
    {
        public CardData Data { get; private set; }

        private ColorRect m_Shadow;
        private Node2D m_Body;
        private Sprite2D m_Frame;
        private Sprite2D m_Icon;
        private Label m_Title;
        private Sprite2D m_Outline;
        private Sprite2D m_CountPlate;
        private Sprite2D m_BadgeLeft;
        private Sprite2D m_BadgeRight;

        private CardState m_State = CardState.Resting;
        private float m_Lift;

        /// <summary>当前抬升量（art px），由状态推导。</summary>
        public float Lift => m_Lift;

        /// <summary>卡牌矩形（世界坐标，原点在左上角，尺寸恒为 48 x 56）。</summary>
        public Rect2 Bounds => new(GlobalPosition, new Vector2(Metrics.CardW, Metrics.CardH));

        /// <summary>中心点，用于邻近判定。</summary>
        public Vector2 Center => GlobalPosition + new Vector2(Metrics.CardW * 0.5f, Metrics.CardH * 0.5f);

        public CardState State => m_State;

        public override void _Ready()
        {
            BuildNodes();
            Refresh();
        }

        /// <summary>绑定数据。可在 _Ready 之前调用。</summary>
        public void SetData(CardData data)
        {
            Data = data;
            if (m_Frame != null)
            {
                Refresh();
            }
        }

        private void BuildNodes()
        {
            // ---- 投影：不随抬升移动，只扩大偏移（设计文档明确要求） ----
            m_Shadow = new ColorRect
            {
                Name = "Shadow",
                Color = Palette.ShadowCard,
                Size = new Vector2(Metrics.CardW, Metrics.CardH),
                MouseFilter = Control.MouseFilterEnum.Ignore,
            };
            m_Shadow.Position = new Vector2(1, 2);   // shadow.rest = +1, +2
            AddChild(m_Shadow);

            // ---- 主体：抬升时整体上移 ----
            m_Body = new Node2D { Name = "Body" };
            AddChild(m_Body);

            m_Frame = new Sprite2D
            {
                Name = "Frame",
                Centered = false,                    // 原点 = 左上角
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            };
            m_Body.AddChild(m_Frame);

            m_Icon = new Sprite2D
            {
                Name = "Icon",
                Centered = true,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                Visible = false,
            };
            m_Body.AddChild(m_Icon);

            m_Title = new Label
            {
                Name = "Title",
                Position = new Vector2(3, 1),        // 标题左对齐 x3，占据 2..11 行
                Size = new Vector2(Metrics.CardContentW - 4, Metrics.CardHeader),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                ClipText = true,
            };
            m_Title.AddThemeFontSizeOverride("font_size", Metrics.FontSizeCard);
            m_Body.AddChild(m_Title);

            // ---- 状态外框：画在卡牌外侧 1 px（50 x 58） ----
            m_Outline = new Sprite2D
            {
                Name = "Outline",
                Centered = false,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                Position = new Vector2(-1, -1),
                Visible = false,
            };
            m_Body.AddChild(m_Outline);
        }

        /// <summary>把当前 Data + State 渲染出来。</summary>
        public void Refresh()
        {
            if (m_Frame == null)
            {
                return;
            }

            var style = Data?.Style ?? CardFamilies.Cargo;

            m_Frame.Texture = CardFramePainter.Get(style.Family, Data?.Grade ?? CardGrade.Standard);

            // ---- 标题 ----
            m_Title.Text = Data?.Title ?? "";
            m_Title.AddThemeColorOverride("font_color", style.TitleColor);

            // ---- 图标 ----
            if (Data?.Icon != null)
            {
                m_Icon.Texture = Data.Icon;
                m_Icon.Visible = true;
                // 图标居中于美术区（y14..42，中心 y28）
                m_Icon.Position = new Vector2(Metrics.CardW * 0.5f, Metrics.ArtAreaTop + Metrics.RowArtArea * 0.5f);
            }
            else
            {
                m_Icon.Visible = false;
            }

            ApplyState();
            RefreshBadges();
        }

        /// <summary>
        /// 刷新页脚徽章（design 04-card-system.md "Badges" + 11-card-taxonomy.md 的默认值）。
        ///
        /// 左徽章 x2,y44；右徽章 x33,y44，均为 13x9。数字用 3x5 点阵手绘——
        /// zoom 0.5 下每个数字只有 3x5 art px，缩放文本会糊，
        /// 这是设计文档坚持手绘数字而非 Label 的原因。-1 表示隐藏。
        /// </summary>
        private void RefreshBadges()
        {
            if (Data == null)
            {
                return;
            }

            var style = Data.Style;

            // ---- 左徽章：bone 底板 + ink 数字 ----
            if (Data.BadgeLeft >= 0)
            {
                EnsureBadge(ref m_BadgeLeft, "BadgeLeft",
                    new Vector2(Metrics.BadgeLeftX, Metrics.BadgeY));
                m_BadgeLeft.Texture = NumberPlate.Get(
                    Data.BadgeLeft, style.BadgeLeftPlate, Palette.Ink);
                m_BadgeLeft.Visible = true;
            }
            else if (m_BadgeLeft != null)
            {
                m_BadgeLeft.Visible = false;
            }

            // ---- 右徽章：rust.2 底板 + white 数字（Enemy 家族改用 steel.2 以便可见） ----
            if (Data.BadgeRight >= 0)
            {
                EnsureBadge(ref m_BadgeRight, "BadgeRight",
                    new Vector2(Metrics.BadgeRightX, Metrics.BadgeY));
                m_BadgeRight.Texture = NumberPlate.Get(
                    Data.BadgeRight, style.BadgeRightPlate, style.BadgeRightText);
                m_BadgeRight.Visible = true;
            }
            else if (m_BadgeRight != null)
            {
                m_BadgeRight.Visible = false;
            }
        }

        /// <summary>惰性创建徽章 Sprite（卡牌被池化复用时只建一次）。</summary>
        private void EnsureBadge(ref Sprite2D slot, string name, Vector2 pos)
        {
            if (slot != null)
            {
                return;
            }
            slot = new Sprite2D
            {
                Name = name,
                Centered = false,
                Position = pos,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
            };
            m_Body.AddChild(slot);
        }
        public void SetStateFlag(CardState flag, bool on)
        {
            var next = on ? m_State | flag : m_State & ~flag;
            if (next == m_State)
            {
                return;
            }
            m_State = next;
            ApplyState();
        }

        public void ClearState() => SetStateFlag(m_State, false);

        /// <summary>把状态位翻译成位移与描边。这是唯一读取 m_State 的地方。</summary>
        private void ApplyState()
        {
            bool held = m_State.HasFlag(CardState.Held);
            bool invalid = m_State.HasFlag(CardState.InvalidTarget);
            bool valid = m_State.HasFlag(CardState.ValidTarget);
            bool selected = m_State.HasFlag(CardState.Selected);
            bool hover = m_State.HasFlag(CardState.Hover);

            // ---- 抬升：Held 优先于 Hover ----
            m_Lift = held ? Metrics.LiftDrag : hover ? Metrics.LiftHover : 0f;
            if (m_Body != null)
            {
                m_Body.Position = new Vector2(0, -m_Lift);
            }

            // ---- 投影：静止 +1,+2；持握 +2,+6；且持握时压在机身下方 ----
            if (m_Shadow != null)
            {
                m_Shadow.Position = held ? new Vector2(2, 6) : new Vector2(1, 2);
            }

            // ---- 外框：Invalid > Selected > Valid ----
            if (m_Outline != null)
            {
                if (invalid || selected || valid)
                {
                    var color = invalid ? Palette.Rust3 : Palette.Cyan3;
                    m_Outline.Texture = StateOutline.Get(color);
                    m_Outline.Visible = true;
                }
                else
                {
                    m_Outline.Visible = false;
                }
            }
        }

        /// <summary>瞬间定位（用于生成、读档；不做插值）。</summary>
        public void SnapTo(Vector2 worldPos)
        {
            GlobalPosition = new Vector2(Mathf.Round(worldPos.X), Mathf.Round(worldPos.Y));
        }

        /// <summary>
        /// 堆叠计数牌（04-card-system.md）：10 张以上在根卡表头右端显示。
        /// <paramref name="count"/> 传 -1 隐藏。
        /// 用 3x5 点阵绘制，不依赖字体资源。
        /// </summary>
        public void SetStackCount(int count)
        {
            if (count < 0 || count > 99)
            {
                if (m_CountPlate != null)
                {
                    m_CountPlate.Visible = false;
                }
                return;
            }

            if (m_CountPlate == null)
            {
                m_CountPlate = new Sprite2D
                {
                    Name = "CountPlate",
                    Centered = false,
                    TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                    // 表头右端，13x9 的数字牌
                    Position = new Vector2(Metrics.CardW - 1 - 13 - 2, 2),
                };
                m_Body.AddChild(m_CountPlate);
            }

            m_CountPlate.Texture = NumberPlate.Get(count);
            m_CountPlate.Visible = true;
        }

        /// <summary>是否包含一个世界坐标点（用完整 48x56 矩形，含透明角）。</summary>
        public bool HitTestPoint(Vector2 worldPos)
        {
            var b = Bounds;
            return worldPos.X >= b.Position.X && worldPos.X < b.End.X
                && worldPos.Y >= b.Position.Y && worldPos.Y < b.End.Y;
        }
    }
}
