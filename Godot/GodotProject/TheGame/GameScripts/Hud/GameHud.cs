using Godot;
using GameLogic.Cards;
using GameLogic.Design;

namespace GameLogic.Hud
{
    /// <summary>
    /// 局内 HUD（design 07-menus-and-hud.md + 03 "HUD Placement"）。
    ///
    /// 位于独立 CanvasLayer（layer 10），960 x 540 UI 画布整数倍缩放
    /// （1080p 下 2x，4K 下 4x，09-godot-handoff.md "UI Scaling"）。
    /// 面板锚定到各自的角，宽于 16:9 时左列留左、右上盒子留右。
    ///
    /// 本版本包含：
    ///  - 信息面板（左下 172 x 147）：悬停卡的标题条 + 描述
    ///  - 时间盒（右上 182 x 27）：状态文字 + 暂停/播放按钮
    ///  - PAUSED 标签（屏幕中央，font.ui.heading）
    ///  - 操作提示（左上，替代尚未实现的任务列表）
    /// </summary>
    public partial class GameHud : CanvasLayer
    {
        private Control m_Root;
        private PanelContainer m_InfoPanel;
        private ColorRect m_InfoTitleBar;
        private Label m_InfoTitle;
        private Label m_InfoBody;
        private Label m_PausedLabel;
        private Label m_TimeLabel;
        private Button m_PauseButton;
        private ColorRect m_PauseOverlay;
        private Label m_Toast;
        private double m_ToastLife;

        /// <summary>玩家点击暂停按钮时触发。</summary>
        public event System.Action PauseToggled;

        public override void _Ready()
        {
            Layer = 10;

            // 暂停遮罩：压暗世界（overlay.pause = ink 35%），在 HUD 之下
            m_PauseOverlay = new ColorRect
            {
                Name = "PauseOverlay",
                Color = Palette.OverlayPause,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                Visible = false,
            };
            AddChild(m_PauseOverlay);

            m_Root = new Control
            {
                Name = "Root",
                MouseFilter = Control.MouseFilterEnum.Ignore,
            };
            AddChild(m_Root);

            BuildInfoPanel();
            BuildTimeBox();
            BuildHelpPanel();
            BuildPausedLabel();
            BuildToast();

            GetViewport().SizeChanged += UpdateUiScale;
            UpdateUiScale();
        }

        public override void _Process(double delta)
        {
            if (m_ToastLife > 0)
            {
                m_ToastLife -= delta;
                if (m_ToastLife <= 0)
                {
                    m_Toast.Visible = false;
                }
            }
        }

        /// <summary>
        /// UI 整数缩放：scale = max(1, floor(min(w/960, h/540)))，
        /// Root 尺寸 = 屏幕 / scale，于是 1080p 下 Root 恰为 960 x 540。
        /// </summary>
        private void UpdateUiScale()
        {
            var s = GetViewport().GetVisibleRect().Size;
            int scale = Mathf.Max(1, Mathf.FloorToInt(Mathf.Min(s.X / Metrics.UICanvasW, s.Y / Metrics.UICanvasH)));
            m_Root.Scale = new Vector2(scale, scale);
            m_Root.Size = s / scale;
            m_PauseOverlay.Size = s;
        }

        // ==================== 面板样式 ====================

        /// <summary>钢板面板：steel.2 底、1 px ink 描边（07 "Panel Kit"）。</summary>
        private static StyleBoxFlat SteelPanel()
        {
            return new StyleBoxFlat
            {
                BgColor = Palette.Steel2,
                BorderColor = Palette.Ink,
                BorderWidthLeft = 1,
                BorderWidthTop = 1,
                BorderWidthRight = 1,
                BorderWidthBottom = 1,
                ContentMarginLeft = 4,
                ContentMarginRight = 4,
                ContentMarginTop = 3,
                ContentMarginBottom = 3,
            };
        }

        private static Label MakeLabel(string text, Color color, int size = Metrics.FontSizeUI)
        {
            var l = new Label
            {
                Text = text,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            };
            l.AddThemeColorOverride("font_color", color);
            l.AddThemeFontSizeOverride("font_size", size);
            return l;
        }

        // ==================== 信息面板 ====================

        private void BuildInfoPanel()
        {
            m_InfoPanel = new PanelContainer
            {
                Name = "InfoPanel",
                MouseFilter = Control.MouseFilterEnum.Ignore,
                CustomMinimumSize = new Vector2(Metrics.ListPanelW, Metrics.InfoPanelH),
            };
            m_InfoPanel.AddThemeStyleboxOverride("panel", SteelPanel());
            // 锚定左下角：x 4，距底 4
            m_InfoPanel.AnchorTop = 1;
            m_InfoPanel.AnchorBottom = 1;
            m_InfoPanel.OffsetLeft = Metrics.ListPanelX;
            m_InfoPanel.OffsetRight = Metrics.ListPanelX + Metrics.ListPanelW;
            m_InfoPanel.OffsetTop = -4 - Metrics.InfoPanelH;
            m_InfoPanel.OffsetBottom = -4;
            m_Root.AddChild(m_InfoPanel);

            var vbox = new VBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
            vbox.AddThemeConstantOverride("separation", 4);
            m_InfoPanel.AddChild(vbox);

            // 标题条：卡牌机箱色 + 标题色
            m_InfoTitleBar = new ColorRect
            {
                CustomMinimumSize = new Vector2(0, 16),
                MouseFilter = Control.MouseFilterEnum.Ignore,
                Color = Palette.Steel3,
            };
            vbox.AddChild(m_InfoTitleBar);

            m_InfoTitle = MakeLabel("", Palette.Bone);
            m_InfoTitle.Position = new Vector2(3, 0);
            m_InfoTitleBar.AddChild(m_InfoTitle);

            m_InfoBody = MakeLabel("", Palette.Bone);
            m_InfoBody.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            m_InfoBody.CustomMinimumSize = new Vector2(Metrics.ListPanelW - 10, 0);
            vbox.AddChild(m_InfoBody);

            ShowCard(null);
        }

        /// <summary>显示悬停卡；null = 清空（07："Empty when nothing is hovered"）。</summary>
        public void ShowCard(CardView card)
        {
            var data = card?.Data;
            if (data == null)
            {
                m_InfoTitleBar.Visible = false;
                m_InfoBody.Text = "";
                return;
            }

            var style = data.Style;
            m_InfoTitleBar.Visible = true;
            m_InfoTitleBar.Color = style.Body;
            m_InfoTitle.Text = data.DisplayName;
            m_InfoTitle.AddThemeColorOverride("font_color", style.TitleColor);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine(FamilyName(data.Family));
            if (data.BadgeLeft >= 0) sb.AppendLine($"{LeftBadgeName(data.Family)}: {data.BadgeLeft}");
            if (data.BadgeRight >= 0) sb.AppendLine($"{RightBadgeName(data.Family)}: {data.BadgeRight}");
            if (!string.IsNullOrEmpty(data.Description)) sb.Append(data.Description);
            m_InfoBody.Text = sb.ToString();
        }

        private static string FamilyName(CardFamily f) => f switch
        {
            CardFamily.Vehicle => "车辆",
            CardFamily.Enemy => "敌人",
            CardFamily.Site => "地点",
            CardFamily.Event => "事件",
            CardFamily.Facility => "设施",
            CardFamily.Part => "部件",
            CardFamily.Region => "区域",
            CardFamily.Cargo => "货物",
            CardFamily.Protagonist => "主角",
            _ => "",
        };

        /// <summary>徽章含义（11-card-taxonomy.md "Badge Defaults"）。</summary>
        private static string LeftBadgeName(CardFamily f) => f switch
        {
            CardFamily.Vehicle => "SP%",
            CardFamily.Enemy => "等级",
            CardFamily.Site => "剩余搜索",
            CardFamily.Region => "时限",
            CardFamily.Part => "重量",
            CardFamily.Cargo => "数量",
            CardFamily.Protagonist => "驾驶等级",
            _ => "左",
        };

        private static string RightBadgeName(CardFamily f) => f switch
        {
            CardFamily.Vehicle => "货舱",
            CardFamily.Enemy => "HP",
            CardFamily.Event => "倒计时",
            CardFamily.Cargo => "单价",
            CardFamily.Facility => "费用",
            _ => "右",
        };

        // ==================== 时间盒 ====================

        private void BuildTimeBox()
        {
            var box = new PanelContainer
            {
                Name = "TimeBox",
                CustomMinimumSize = new Vector2(Metrics.TimeBoxW, Metrics.TimeBoxH),
            };
            box.AddThemeStyleboxOverride("panel", SteelPanel());
            // 锚定右上角：距右 4，y 4
            box.AnchorLeft = 1;
            box.AnchorRight = 1;
            box.OffsetLeft = -4 - Metrics.TimeBoxW;
            box.OffsetRight = -4;
            box.OffsetTop = Metrics.TimeBoxY;
            box.OffsetBottom = Metrics.TimeBoxY + Metrics.TimeBoxH;
            m_Root.AddChild(box);

            var hbox = new HBoxContainer();
            box.AddChild(hbox);

            m_TimeLabel = MakeLabel("车库 · 第 1 天", Palette.Bone);
            m_TimeLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            m_TimeLabel.VerticalAlignment = VerticalAlignment.Center;
            hbox.AddChild(m_TimeLabel);

            // 16 x 16 暂停/播放按钮（普通按钮：steel.3 底 + bone 字）
            m_PauseButton = new Button
            {
                Text = "II",
                CustomMinimumSize = new Vector2(16, 16),
                FocusMode = Control.FocusModeEnum.None,
            };
            m_PauseButton.AddThemeFontSizeOverride("font_size", Metrics.FontSizeUI);
            m_PauseButton.AddThemeColorOverride("font_color", Palette.Bone);
            var btnStyle = new StyleBoxFlat
            {
                BgColor = Palette.Steel3,
                BorderColor = Palette.Steel1,
                BorderWidthBottom = 2,
            };
            m_PauseButton.AddThemeStyleboxOverride("normal", btnStyle);
            m_PauseButton.AddThemeStyleboxOverride("hover", btnStyle);
            m_PauseButton.AddThemeStyleboxOverride("pressed", new StyleBoxFlat { BgColor = Palette.Steel3 });
            m_PauseButton.Pressed += () => PauseToggled?.Invoke();
            hbox.AddChild(m_PauseButton);
        }

        public void SetTimeText(string text) => m_TimeLabel.Text = text;

        // ==================== 操作提示 ====================

        /// <summary>左上角操作说明（列表面板落地前的占位，尺寸与列表面板一致宽度）。</summary>
        private void BuildHelpPanel()
        {
            var panel = new PanelContainer
            {
                Name = "HelpPanel",
                MouseFilter = Control.MouseFilterEnum.Ignore,
                Position = new Vector2(Metrics.ListPanelX, Metrics.ListPanelY),
                CustomMinimumSize = new Vector2(Metrics.ListPanelW, 0),
            };
            panel.AddThemeStyleboxOverride("panel", SteelPanel());
            m_Root.AddChild(panel);

            var vbox = new VBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
            panel.AddChild(vbox);

            var title = MakeLabel("操作", Palette.Amber2);
            vbox.AddChild(title);

            string[] lines =
            {
                "左键拖卡：移动 / 叠放",
                "拖空白处：平移棋盘",
                "WASD / 方向键：平移",
                "滚轮：缩放",
                "点击卡：选中",
                "右键（拖拽中）：放下",
                "G：对齐网格",
                "N：生成随机卡",
                "空格 / 按钮：暂停",
            };
            foreach (var line in lines)
            {
                var l = MakeLabel(line, Palette.Steel7);
                vbox.AddChild(l);
            }
        }

        // ==================== PAUSED 标签 ====================

        private void BuildPausedLabel()
        {
            m_PausedLabel = MakeLabel("已暂停", Palette.Bone, Metrics.FontSizeHeading);
            m_PausedLabel.Name = "PausedLabel";
            m_PausedLabel.HorizontalAlignment = HorizontalAlignment.Center;
            m_PausedLabel.VerticalAlignment = VerticalAlignment.Center;
            m_PausedLabel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            m_PausedLabel.AddThemeColorOverride("font_shadow_color", Palette.Ink);
            m_PausedLabel.AddThemeConstantOverride("shadow_offset_x", 1);
            m_PausedLabel.AddThemeConstantOverride("shadow_offset_y", 1);
            m_PausedLabel.Visible = false;
            m_Root.AddChild(m_PausedLabel);
        }

        public void SetPaused(bool paused)
        {
            m_PausedLabel.Visible = paused;
            m_PauseOverlay.Visible = paused;
            m_PauseButton.Text = paused ? ">" : "II";
        }

        // ==================== 状态提示（tier 2 toast 的最小实现） ====================

        private void BuildToast()
        {
            m_Toast = MakeLabel("", Palette.Bone);
            m_Toast.Name = "Toast";
            m_Toast.HorizontalAlignment = HorizontalAlignment.Center;
            m_Toast.AnchorLeft = 0.5f;
            m_Toast.AnchorRight = 0.5f;
            m_Toast.AnchorTop = 1;
            m_Toast.AnchorBottom = 1;
            m_Toast.OffsetLeft = -110;
            m_Toast.OffsetRight = 110;
            m_Toast.OffsetTop = -24 - 24;
            m_Toast.OffsetBottom = -24;
            var style = SteelPanel();
            m_Toast.AddThemeStyleboxOverride("normal", style);
            m_Toast.Visible = false;
            m_Root.AddChild(m_Toast);
        }

        /// <summary>底部居中的一行提示，3 秒后消失（17-dialogs.md tier 2）。</summary>
        public void Toast(string text, Color color)
        {
            m_Toast.Text = text;
            m_Toast.AddThemeColorOverride("font_color", color);
            m_Toast.Visible = true;
            m_ToastLife = 3.0;
        }
    }
}
