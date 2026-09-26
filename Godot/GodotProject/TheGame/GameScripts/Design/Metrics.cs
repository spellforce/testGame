using Godot;

namespace GameLogic.Design
{
    /// <summary>
    /// 设计令牌（design 02/03/06）：所有几何、尺寸与运动数值的唯一来源。
    /// 单位说明：
    /// - 世界（world）与卡牌一律使用 **art px**（美术像素），1 art px = 源图 1 像素。
    /// - UI 使用 **UI px**，逻辑画布 960 x 540，整数倍缩放到屏幕。
    /// 任何地方都不得再写死魔法数字；数值改动只在本文件发生。
    /// </summary>
    public static class Metrics
    {
        // ======================= 卡牌 =======================

        /// <summary>card.size — 卡牌 48 x 56 art px。所有家族共用同一footprint。</summary>
        public const int CardW = 48;
        public const int CardH = 56;

        /// <summary>card.header — 标题行 12 行（堆叠时保持可见的部分）。</summary>
        public const int CardHeader = 12;

        /// <summary>卡牌内部列数：1 描边 + 46 内容 + 1 描边。</summary>
        public const int CardContentW = 46;

        // 卡牌行分解（E1）：1 + 12 + 1 + 29 + 12 + 1 = 56
        public const int RowOutlineTop = 1;
        public const int RowHeader = 12;          // y 1..12
        public const int RowSeparator = 1;        // y 13
        public const int RowArtArea = 29;         // y 14..42
        public const int RowFooter = 12;          // y 43..54
        public const int RowOutlineBottom = 1;    // y 55

        // 行区间（含端点，direct 传给美术与贴图切片）
        public const int ArtAreaTop = RowOutlineTop + RowHeader + RowSeparator;      // 14
        public const int ArtAreaBottom = ArtAreaTop + RowArtArea - 1;                // 42
        public const int FooterTop = ArtAreaBottom + 1;                              // 43
        public const int FooterBottom = FooterTop + RowFooter - 1;                   // 54

        /// <summary>E2 — 通用图标预算 32 x 20（高瘦主体可用 20 x 24）。</summary>
        public const int IconMaxW = 32;
        public const int IconMaxH = 20;
        public const int IconTallW = 20;
        public const int IconTallH = 24;

        /// <summary>stack.offset — 堆叠偏移。E6 待定：12（当前设计值）vs 13（含分隔行）。</summary>
        public const int StackOffset = 12;

        /// <summary>堆叠高度公式 56 + StackOffset * (n - 1)。</summary>
        public static int StackHeight(int cardCount) => CardH + StackOffset * (cardCount - 1);

        // 徽章（04-card-system.md）
        public const int BadgeW = 13;
        public const int BadgeH = 9;
        public const int BadgeLeftX = 2;
        public const int BadgeRightX = 33;
        public const int BadgeY = 44;

        /// <summary>计时条 40 x 5，居中于根卡上方 4 px。</summary>
        public const int TimerBarW = 40;
        public const int TimerBarH = 5;
        public const int TimerBarGap = 4;

        // ======================= 棋盘 =======================

        /// <summary>board.size — 1104 x 644 art px（23 x 11.5 卡宽/高，比例 1.714）。</summary>
        public const int BoardW = 1104;
        public const int BoardH = 644;

        /// <summary>board.slot_band — 164（E3，原 104）。</summary>
        public const int BoardSlotBand = 164;

        /// <summary>游戏区高度 644 - 164 = 480。</summary>
        public const int PlayAreaH = BoardH - BoardSlotBand;

        /// <summary>棋盘围栏 3 px，位于棋盘边界内侧。</summary>
        public const int BoardRail = 3;

        /// <summary>分隔线 2 px（ink + steel.7），位于 y 164。</summary>
        public const int BandDividerH = 2;
        public const int BandDividerY = BoardSlotBand;

        // 槽位带两行（16-garage-and-expedition-boards.md）
        // 14 + 81 + 9 + 48 + 12 = 164
        public const int SlotW = 60;
        public const int SlotH = 81;
        public const int SlotRow1Y = 14;
        public const int SlotPitch = 64;
        public const int SlotCount = 9;
        public const int SlotRowWidth = SlotPitch * SlotCount;   // 576 → 实际 560 用 64 间距 9 个占 560

        public const int ChipW = 40;
        public const int ChipH = 48;
        public const int ChipRow2Y = SlotRow1Y + SlotH + 9;      // 104
        public const int ChipPitch = 48;

        // ======================= 世界画布 =======================

        /// <summary>world.size — 2208 x 1288 art px，棋盘居中。</summary>
        public const int WorldW = 2208;
        public const int WorldH = 1288;

        /// <summary>棋盘居中位置：x 552..1656，y 322..966。</summary>
        public static int BoardLeft => (WorldW - BoardW) / 2;    // 552
        public static int BoardTop => (WorldH - BoardH) / 2;     // 322
        public static int BoardRight => BoardLeft + BoardW;      // 1656
        public static int BoardBottom => BoardTop + BoardH;      // 966

        // ======================= 拖拽 / 物理 =======================

        /// <summary>clamp margin 0.1 unit = 11 art px，卡牌静止时的内缩边界。</summary>
        public const int ClampMargin = 11;

        /// <summary>drop.snap_radius 23 art px（仅特殊投放区：pack、售卖区）。</summary>
        public const int DropSnapRadius = 23;

        /// <summary>auto-stack radius 2.0 units = 228 art px（机器发射卡片的自动堆叠）。</summary>
        public const int AutoStackRadius = 228;

        /// <summary>grid.cell 86 x 96（E5：刻意大于卡牌，对齐后留有间隙）。</summary>
        public const int GridCellW = 86;
        public const int GridCellH = 96;

        /// <summary>lift.hover 7 px。</summary>
        public const int LiftHover = 7;

        /// <summary>lift.drag 11 px。</summary>
        public const int LiftDrag = 11;

        /// <summary>拖拽阈值：0.2 秒 AND 11 px，两者都未超过才算点击。</summary>
        public const float DragThresholdTime = 0.2f;
        public const int DragThresholdPx = 11;

        /// <summary>位置插值收敛速率 pos = lerp(pos, target, dt * 20)。</summary>
        public const float SettleRate = 20f;

        /// <summary>推动速率 228 art px/s，按质量份额缩放。</summary>
        public const float PushSpeed = 228f;

        /// <summary>发射：水平 514 px/s，向上跳 571 px/s。</summary>
        public const float SendSpeedX = 514f;
        public const float SendSpeedY = 571f;

        /// <summary>重力 3429 px/s²（仅在空中生效）。</summary>
        public const float Gravity = 3429f;

        /// <summary>空气阻力：每 0.02 s 水平速度 x0.93。</summary>
        public const float AirDragPer20ms = 0.93f;

        /// <summary>弹跳系数 0.6，落地水平损失 x0.8。</summary>
        public const float Bounciness = 0.6f;
        public const float BounceHorizontalLoss = 0.8f;

        /// <summary>翻面时间 0.1 s。</summary>
        public const float FlipTime = 0.1f;

        // 质量（06-interaction-motion.md）
        public const float MassBase = 1f;
        public const float MassMob = 50f;
        public const float MassBuilding = 8f;
        public const float MassHeavyFoundation = 1000f;

        // ======================= 相机 =======================

        public const float ZoomMin = 0.5f;
        public const float ZoomDefault = 1.5f;
        public const float ZoomMax = 2.8f;

        /// <summary>每格滚轮 x1.15，缓动 120 ms。</summary>
        public const float ZoomStep = 1.15f;

        /// <summary>键盘平移 600 art px/s ÷ z。</summary>
        public const float PanKeySpeed = 600f;

        /// <summary>事件卡落在视野外时的 250 ms 微推。</summary>
        public const float CameraNudgeTime = 0.25f;

        /// <summary>
        /// z → 屏幕像素/art px。设计式：screen_px_per_art_px = z × screen_height / 540。
        /// 即 z=0.5 时在 1080p 下 1 art px = 1 屏幕像素。
        /// </summary>
        public static float ScreenPixelsPerArtPixel(float zoom, float screenHeight)
            => zoom * screenHeight / 540f;

        /// <summary>
        /// 屏幕像素/art px 吸附到整数（Pixel-perfect zoom 设置），下限 1。
        /// 对应 09-godot-handoff.md 的 maxf(1.0, roundf(spp))。
        /// </summary>
        public static float SnapToWholeArtPixel(float spp)
            => Mathf.Max(1f, Mathf.Round(spp));

        // ======================= UI =======================

        /// <summary>UI 逻辑画布 960 x 540，整数倍缩放。</summary>
        public const int UICanvasW = 960;
        public const int UICanvasH = 540;

        /// <summary>间距基数 4。</summary>
        public const int Space1 = 1;
        public const int Space2 = 2;
        public const int Space4 = 4;
        public const int Space8 = 8;
        public const int Space12 = 12;
        public const int Space16 = 16;

        /// <summary>按钮最小高度 16 UI px。</summary>
        public const int MinButtonH = 16;

        /// <summary>列表行高（窗口系统）。</summary>
        public const int ListRowH = 18;

        // HUD 布局（03-canvas-layout.md）
        public const int ListPanelX = 4;
        public const int ListPanelY = 4;
        public const int ListPanelW = 172;
        public const int ListPanelH = 380;

        public const int InfoPanelY = 389;
        public const int InfoPanelH = 147;

        public const int ResourceBoxX = 640;
        public const int ResourceBoxY = 4;
        public const int ResourceBoxW = 128;
        public const int ResourceBoxH = 27;

        public const int TimeBoxX = 774;
        public const int TimeBoxY = 4;
        public const int TimeBoxW = 182;
        public const int TimeBoxH = 27;


        // ======================= 运动（UI） =======================

        public const float MotionInstant = 0.06f;
        public const float MotionFast = 0.12f;
        public const float MotionStandard = 0.20f;
        public const float MotionSlow = 0.35f;

        /// <summary>12 fps 动画的单帧时长 83 ms。</summary>
        public const float MotionFrame = 0.083f;

        // ======================= 字体 =======================

        public const int FontSizeCard = 10;
        public const int FontSizeUI = 12;
        public const int FontLineHeightUI = 16;
        public const int FontSizeHeading = 24;
        public const int FontSizeTitle = 48;
    }
}
