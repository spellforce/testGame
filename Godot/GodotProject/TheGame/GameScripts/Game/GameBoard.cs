using Godot;
using GameLogic.Board;
using GameLogic.Cards;
using GameLogic.Design;
using GameLogic.Interaction;
using GameLogic.World;

namespace GameLogic.Game
{
    /// <summary>
    /// 游戏主场景（design 09-godot-handoff.md "Main Scene Tree"）。
    ///
    /// 节点结构：
    /// <code>
    /// Main (Node2D)
    ///  |- BoardView       世界地形 + 棋盘甲板 + 卡牌层 + 持握层
    ///  |- BoardCamera     Camera2D，连续缩放 0.5..2.0
    ///  |- CardWorld       卡堆的所有权与收敛
    ///  |- DragController  按下/点击/拖拽/平移/投放
    /// </code>
    ///
    /// 这个场景完全用代码搭建，不依赖 .tscn 里手摆节点，
    /// 因为棋盘尺寸、层序都由 <see cref="Metrics"/> 决定，
    /// 手摆的节点会与设计令牌脱节。
    /// </summary>
    public partial class GameBoard : Node2D
    {
        private BoardView m_Board;
        private BoardCamera m_Camera;
        private CardWorld m_World;
        private DragController m_Drag;

        public CardWorld World => m_World;
        public BoardCamera BoardCamera => m_Camera;

        public override void _Ready()
        {
            Name = "GameBoard";

            // ---- 1. 棋盘视图 ----
            m_Board = new BoardView { Name = "BoardView" };
            AddChild(m_Board);

            // ---- 2. 摄像机 ----
            m_Camera = new BoardCamera { Name = "BoardCamera" };
            AddChild(m_Camera);
            // 默认 0.5（最小）缩放：16-garage-and-expedition-boards.md 规定
            // 远征棋盘用 0.5，让整块棋盘一屏可见 —— 这也是 1080p 下
            // 1 art px = 1 屏幕像素、与参考截图一致的取景。
            m_Camera.SetZoomLevel(Metrics.ZoomMin);
            m_Camera.CenterOnBoard();

            // ---- 3. 卡牌世界（挂在 BoardView 之外，靠 Reparent 把堆分层） ----
            m_World = new CardWorld { Name = "CardWorld" };
            AddChild(m_World);

            // ---- 4. 拖拽控制器 ----
            m_Drag = new DragController { Name = "DragController" };
            AddChild(m_Drag);
            m_Drag.Setup(m_Camera, m_World, m_Board);

            // ---- 5. HUD ----
            m_Hud = new Hud.GameHud { Name = "GameHud" };
            AddChild(m_Hud);
            m_Drag.HoverChanged += m_Hud.ShowCard;
            m_Hud.PauseToggled += TogglePause;

            SpawnStarterCards();
        }

        private Hud.GameHud m_Hud;
        private bool m_Paused;
        private readonly System.Random m_Rng = new();

        /// <summary>
        /// 暂停：世界模拟停止（推动、计时），但卡牌仍可拖拽与投放
        /// （06 "Pause"："Cards can still be dragged and dropped while paused"）。
        /// 所以这里不用 GetTree().Paused，而是只停推动；收敛照常，
        /// 否则被放下的卡会悬在半空。
        /// </summary>
        private void TogglePause()
        {
            m_Paused = !m_Paused;
            m_Hud.SetPaused(m_Paused);
        }

        public override void _UnhandledKeyInput(InputEvent @event)
        {
            if (@event is not InputEventKey { Pressed: true, Echo: false } key)
            {
                return;
            }

            switch (key.Keycode)
            {
                case Key.Space:
                    TogglePause();
                    break;
                case Key.G:
                    AlignToGrid();
                    break;
                case Key.N:
                    SpawnRandomCard();
                    break;
                default:
                    return;
            }
            GetViewport().SetInputAsHandled();
        }

        /// <summary>
        /// 对齐网格（06 "Align to Grid"）：把每个空闲根卡吸附到 86 x 96 网格。
        /// 拖拽中的堆跳过；固定（Pinned）卡跳过。
        /// </summary>
        private void AlignToGrid()
        {
            foreach (var stack in m_World.Stacks)
            {
                if (stack.IsDragging || stack.Root == null
                    || stack.Root.State.HasFlag(CardState.Pinned))
                {
                    continue;
                }
                var target = BoardGeometry.SnapToGrid(m_World.GetTarget(stack.Root));
                m_World.SetTarget(stack.Root, target);
            }
            m_Hud.Toast("已对齐网格", Palette.Olive2);
        }

        /// <summary>在游戏区随机位置生成一张随机家族的卡（调试/演示）。</summary>
        private void SpawnRandomCard()
        {
            var all = SampleCards.CreateAll();
            var data = all[m_Rng.Next(all.Length)];
            var b = BoardGeometry.CardPositionBounds(tight: true);
            var pos = new Vector2(
                b.Position.X + (float)m_Rng.NextDouble() * b.Size.X,
                b.Position.Y + (float)m_Rng.NextDouble() * b.Size.Y);
            var stack = m_World.SpawnCard(data, pos);
            stack.Root.SetStateFlag(CardState.New, true);
            m_Hud.Toast($"生成：{data.Title}", Palette.Cyan3);
        }

        public override void _Process(double delta)
        {
            if (m_World == null)
            {
                return;
            }

            float dt = (float)delta;

            // 顺序照 design 06：先推动（改目标位置），再收敛（向目标移动）。
            // 反过来的话推动会发生在收敛之后，分离看起来会有一帧延迟。
            // 暂停只停推动（世界模拟），收敛照常。
            if (!m_Paused)
            {
                m_World.TickPush(dt);
            }
            m_World.TickSettle(dt);
        }

        /// <summary>
        /// 生成一副起始牌，用来验证整个交互链路。
        /// 正式版本会由 ProcedureGame 从配置表构建。
        ///
        /// 所有坐标经 <see cref="BoardGeometry.ClampCard"/> 夹取，
        /// 保证不会落在游戏区之外 —— 卡牌在游戏区外是不可交互的。
        /// </summary>
        private void SpawnStarterCards()
        {
            var play = BoardGeometry.PlayAreaRect;
            float x0 = play.Position.X + 70;
            float y0 = play.Position.Y + 60;

            // 9 张示例卡（8 家族 + 主角）排成 3 列，逐列向右、逐行向下。
            // 行距 120、列距 170，可以在 480 高的游戏区里放下 3 行。
            var samples = SampleCards.CreateAll();
            for (int i = 0; i < samples.Length; i++)
            {
                int col = i % 3;
                int row = i / 3;
                var pos = new Vector2(x0 + col * 170, y0 + row * 120);
                m_World.SpawnCard(samples[i], BoardGeometry.ClampCard(pos, tight: true));
            }

            // ---- 一组预置堆叠：车辆 + 2 件装备，验证 12 px 偏移的表头可读性 ----
            // 放在游戏区右下角，远离散卡。
            var stackBase = new Vector2(
                play.End.X - 260,
                play.End.Y - 200);

            var vehicle = m_World.SpawnCard(
                SampleCards.Create(CardFamily.Vehicle, "scout", "侦察车"), stackBase);

            // 依次叠上两张装备卡，模拟"车辆 + 装备"的堆叠
            var part1 = SampleCards.Create(CardFamily.Part, "cannon", "主炮");
            part1.PartKind = PartKind.MainCannon;
            var part2 = SampleCards.Create(CardFamily.Part, "engine", "引擎");
            part2.PartKind = PartKind.Engine;

            var s1 = m_World.SpawnCard(part1, stackBase);
            var s2 = m_World.SpawnCard(part2, stackBase);

            // 合并成一个堆：车辆在根，装备依次在下
            m_World.MergeStack(s1, vehicle);
            m_World.MergeStack(s2, vehicle);

            m_World.RebuildGrid();
        }
    }
}
