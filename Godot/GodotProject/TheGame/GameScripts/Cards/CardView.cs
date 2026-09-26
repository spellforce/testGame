using Godot;
using GameLogic.Design;

namespace GameLogic.Cards
{
    /// <summary>
    /// 鍗曞紶鍗＄殑瑙嗗浘锛坉esign 09-godot-handoff.md "Card.tscn" + 04-card-system.md锛夈€?    ///
    /// 鑺傜偣缁撴瀯瀵归綈璁捐鏂囨。锛屼絾鐢?C# 鐩存帴鎼細
    /// <code>
    /// Card (Node2D)                 鍘熺偣 = 鍗＄墝宸︿笂瑙?    ///  |- Shadow (ColorRect 48x56)  ink 45%锛屼笉闅忔姮鍗囩Щ鍔紝鍙墿澶у亸绉?    ///  |- Body (Node2D)             鎶崌鏃舵暣浣撲笂绉伙紱閫昏緫浣嶇疆姘镐笉鏀瑰彉
    ///      |- Frame (Sprite2D)      瀹舵棌鍗℃
    ///      |- Icon (Sprite2D)
    ///      |- Title (Label, 10px)
    ///      |- BadgeLeft / BadgeRight
    ///      |- NewDot (Sprite2D)
    ///      |- Outline (Sprite2D)    cyan/red 鐘舵€佸妗?    /// </code>
    ///
    /// 鍏抽敭瑙勫垯锛氬崱鐗?*姘镐笉缂╂斁銆佹案涓嶆棆杞?*锛?1-visual-direction.md 绗?3 鏉★級锛?    /// 鎵€鏈夌姸鎬佸彧鐢ㄤ綅绉汇€佹弿杈逛笌璋冭壊鏉夸氦鎹㈣〃杈俱€?    /// </summary>
    [Tool]
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

        /// <summary>褰撳墠鎶崌閲忥紙art px锛夛紝鐢辩姸鎬佹帹瀵笺€?/summary>
        public float Lift => m_Lift;

        /// <summary>鍗＄墝鐭╁舰锛堜笘鐣屽潗鏍囷紝鍘熺偣鍦ㄥ乏涓婅锛屽昂瀵告亽涓?48 x 56锛夈€?/summary>
        public Rect2 Bounds => new(GlobalPosition, new Vector2(Metrics.CardW, Metrics.CardH));

        /// <summary>涓績鐐癸紝鐢ㄤ簬閭昏繎鍒ゅ畾銆?/summary>
        public Vector2 Center => GlobalPosition + new Vector2(Metrics.CardW * 0.5f, Metrics.CardH * 0.5f);

        public CardState State => m_State;

        public override void _Ready()
        {
            m_Shadow = GetNode<ColorRect>("Shadow");
            m_Body = GetNode<Node2D>("Body");
            m_Frame = GetNode<Sprite2D>("Body/Frame");
            m_Icon = GetNode<Sprite2D>("Body/Icon");
            m_Title = GetNode<Label>("Body/Title");
            m_Outline = GetNode<Sprite2D>("Body/Outline");
            if (Engine.IsEditorHint() && Data == null)
            {
                Data = CreateEditorPreviewData();
            }
            Refresh();
        }

        private static CardData CreateEditorPreviewData()
        {
            return new CardData
            {
                Id = "scout_buggy_preview",
                Family = CardFamily.Vehicle,
                Title = "侦察战车",
                FullName = "侦察战车",
                Description = "预制体默认预览数据",
                BadgeLeft = 78,
                BadgeRight = 12,
                Icon = BuildEditorPreviewIcon(),
            };
        }

        private static ImageTexture BuildEditorPreviewIcon()
        {
            var image = Image.CreateEmpty(32, 20, false, Image.Format.Rgba8);
            image.Fill(new Color(0, 0, 0, 0));
            image.FillRect(new Rect2I(4, 7, 24, 8), new Color("#3d878e"));
            image.FillRect(new Rect2I(8, 4, 16, 4), new Color("#6faeb0"));
            image.FillRect(new Rect2I(12, 1, 8, 3), new Color("#9fc4b9"));
            image.FillRect(new Rect2I(1, 15, 8, 3), new Color("#0e1110"));
            image.FillRect(new Rect2I(23, 15, 8, 3), new Color("#0e1110"));
            image.FillRect(new Rect2I(7, 16, 4, 4), new Color("#1b211f"));
            image.FillRect(new Rect2I(21, 16, 4, 4), new Color("#1b211f"));
            return ImageTexture.CreateFromImage(image);
        }

        /// <summary>缁戝畾鏁版嵁銆傚彲鍦?_Ready 涔嬪墠璋冪敤銆?/summary>
        public void SetData(CardData data)
        {
            Data = data;
            if (m_Frame != null)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            if (m_Frame == null)
            {
                return;
            }

            var style = Data?.Style ?? CardFamilies.Cargo;

            m_Frame.Texture = CardFramePainter.Get(style.Family, Data?.Grade ?? CardGrade.Standard);

            // ---- 鏍囬 ----
            m_Title.Text = Data?.Title ?? "";
            m_Title.AddThemeColorOverride("font_color", style.TitleColor);

            // ---- 鍥炬爣 ----
            if (Data?.Icon != null)
            {
                m_Icon.Texture = Data.Icon;
                m_Icon.Visible = true;
                // 鍥炬爣灞呬腑浜庣編鏈尯锛坹14..42锛屼腑蹇?y28锛?                m_Icon.Position = new Vector2(Metrics.CardW * 0.5f, Metrics.ArtAreaTop + Metrics.RowArtArea * 0.5f);
            }
            else
            {
                m_Icon.Visible = false;
            }

            ApplyState();
            RefreshBadges();
        }

        /// <summary>
        /// 鍒锋柊椤佃剼寰界珷锛坉esign 04-card-system.md "Badges" + 11-card-taxonomy.md 鐨勯粯璁ゅ€硷級銆?        ///
        /// 宸﹀窘绔?x2,y44锛涘彸寰界珷 x33,y44锛屽潎涓?13x9銆傛暟瀛楃敤 3x5 鐐归樀鎵嬬粯鈥斺€?        /// zoom 0.5 涓嬫瘡涓暟瀛楀彧鏈?3x5 art px锛岀缉鏀炬枃鏈細绯婏紝
        /// 杩欐槸璁捐鏂囨。鍧氭寔鎵嬬粯鏁板瓧鑰岄潪 Label 鐨勫師鍥犮€?1 琛ㄧず闅愯棌銆?        /// </summary>
        private void RefreshBadges()
        {
            if (Data == null)
            {
                return;
            }

            var style = Data.Style;

            // ---- 宸﹀窘绔狅細bone 搴曟澘 + ink 鏁板瓧 ----
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

            // ---- 鍙冲窘绔狅細rust.2 搴曟澘 + white 鏁板瓧锛圗nemy 瀹舵棌鏀圭敤 steel.2 浠ヤ究鍙锛?----
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

        /// <summary>鎯版€у垱寤哄窘绔?Sprite锛堝崱鐗岃姹犲寲澶嶇敤鏃跺彧寤轰竴娆★級銆?/summary>
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

        /// <summary>鎶婄姸鎬佷綅缈昏瘧鎴愪綅绉讳笌鎻忚竟銆傝繖鏄敮涓€璇诲彇 m_State 鐨勫湴鏂广€?/summary>
        private void ApplyState()
        {
            bool held = m_State.HasFlag(CardState.Held);
            bool invalid = m_State.HasFlag(CardState.InvalidTarget);
            bool valid = m_State.HasFlag(CardState.ValidTarget);
            bool selected = m_State.HasFlag(CardState.Selected);
            bool hover = m_State.HasFlag(CardState.Hover);

            // ---- 鎶崌锛欻eld 浼樺厛浜?Hover ----
            m_Lift = held ? Metrics.LiftDrag : hover ? Metrics.LiftHover : 0f;
            if (m_Body != null)
            {
                m_Body.Position = new Vector2(0, -m_Lift);
            }

            // ---- 鎶曞奖锛氶潤姝?+1,+2锛涙寔鎻?+2,+6锛涗笖鎸佹彙鏃跺帇鍦ㄦ満韬笅鏂?----
            if (m_Shadow != null)
            {
                m_Shadow.Position = held ? new Vector2(2, 6) : new Vector2(1, 2);
            }

            // ---- 澶栨锛欼nvalid > Selected > Valid ----
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

        /// <summary>鐬棿瀹氫綅锛堢敤浜庣敓鎴愩€佽妗ｏ紱涓嶅仛鎻掑€硷級銆?/summary>
        public void SnapTo(Vector2 worldPos)
        {
            GlobalPosition = new Vector2(Mathf.Round(worldPos.X), Mathf.Round(worldPos.Y));
        }

        /// <summary>
        /// 鍫嗗彔璁℃暟鐗岋紙04-card-system.md锛夛細10 寮犱互涓婂湪鏍瑰崱琛ㄥご鍙崇鏄剧ず銆?        /// <paramref name="count"/> 浼?-1 闅愯棌銆?        /// 鐢?3x5 鐐归樀缁樺埗锛屼笉渚濊禆瀛椾綋璧勬簮銆?        /// </summary>
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
                    // 琛ㄥご鍙崇锛?3x9 鐨勬暟瀛楃墝
                    Position = new Vector2(Metrics.CardW - 1 - 13 - 2, 2),
                };
                m_Body.AddChild(m_CountPlate);
            }

            m_CountPlate.Texture = NumberPlate.Get(count);
            m_CountPlate.Visible = true;
        }

        /// <summary>鏄惁鍖呭惈涓€涓笘鐣屽潗鏍囩偣锛堢敤瀹屾暣 48x56 鐭╁舰锛屽惈閫忔槑瑙掞級銆?/summary>
        public bool HitTestPoint(Vector2 worldPos)
        {
            var b = Bounds;
            return worldPos.X >= b.Position.X && worldPos.X < b.End.X
                && worldPos.Y >= b.Position.Y && worldPos.Y < b.End.Y;
        }
    }
}
