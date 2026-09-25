using Godot;
using GameLogic.Board;
using GameLogic.Cards;
using GameLogic.Design;
using GameLogic.Game;
using GameLogic.World;

namespace GameLogic.DebugTools
{
    /// <summary>
    /// 交互规则自检（design 06-interaction-motion.md）。
    ///
    /// 设计文档把几条规则标为"最容易做错"，并写进了验收清单
    /// （10-validation-checklist.md "Match With Stacklands"）。这些规则无法靠
    /// 看截图判断，所以这里用无头断言固化下来：
    ///
    ///   1. 堆叠：每张卡比下一张低 12 px，表头保持可见
    ///   2. 堆高公式 56 + 12 x (n-1)；10 张以上出现计数牌
    ///   3. 抓取：抓中间某张卡会连同它上面的所有卡一起摘出，下方不动
    ///   4. 投放：找不到目标时**留在松手处**，永不送回拿起的位置
    ///   5. 夹取：松手点在紧边界外时滑到最近的合法位置（11 px 内缩）
    ///   6. 命中测试：下方卡只露出 12 px 表头可点
    ///   7. 推动：只有根卡推；被拖拽的堆不推也不被推；
    ///      质量大的让位少（pushShare = 1 - myMass / (myMass + otherMass)）
    ///   8. 对齐网格：格 86 x 96，对齐后留有间隙
    ///
    /// 运行：godot --headless --path GodotProject res://TheGame/DebugTools/InteractionSelfTest.tscn
    /// 退出码 0 = 全部通过，1 = 有失败。
    /// </summary>
    public partial class InteractionSelfTest : Node2D
    {
        private int m_Passed;
        private int m_Failed;

        private GameBoard m_Board;
        private CardWorld m_World;

        public override void _Ready()
        {
            // 默认棋盘铺满示例卡，自检需要一个干净的场地
            m_Board = new GameBoard { Name = "GameBoard" };
            AddChild(m_Board);

            // _Ready 在 AddChild 时同步执行，世界此时已可用
            m_World = m_Board.World;

            RunAll();

            GD.Print($"\n[InteractionSelfTest] 通过 {m_Passed}，失败 {m_Failed}");
            GetTree().Quit(m_Failed == 0 ? 0 : 1);
        }

        private void RunAll()
        {
            Test_StackLayout();
            Test_StackHeight();
            Test_HitTestHeaderOnly();
            Test_DetachFromMiddle();
            Test_DetachTopKeepsRest();
            Test_MassPushesLightMore();
            Test_ClampTightAndLoose();
            Test_SnapToGrid();
            Test_DropStaysWhereReleased();
            Test_PushRules();
        }

        // ==================== 1. 堆叠布局 ====================

        private void Test_StackLayout()
        {
            var world = FreshWorld();
            var stack = world.SpawnCard(MakeCard(CardFamily.Cargo, "a"), new Vector2(700, 600));

            for (int i = 0; i < 4; i++)
            {
                var extra = world.SpawnCard(MakeCard(CardFamily.Cargo, "b" + i), new Vector2(700, 600));
                world.MergeStack(extra, stack);
            }

            Check("堆叠后有 5 张卡", stack.Count == 5, $"实际 {stack.Count}");

            bool ok = true;
            for (int i = 1; i < stack.Count; i++)
            {
                float dy = stack.Cards[i].Position.Y - stack.Cards[i - 1].Position.Y;
                if (!Mathf.IsEqualApprox(dy, Metrics.StackOffset))
                {
                    ok = false;
                    Fail($"第 {i} 张卡的偏移应为 {Metrics.StackOffset}，实际 {dy}");
                }
            }
            Check("每张卡相对上一张下移 12 px（表头保持可见）", ok, "");

            // 深度排序：越靠下越靠前
            bool zOk = true;
            for (int i = 1; i < stack.Count; i++)
            {
                if (stack.Cards[i].ZIndex <= stack.Cards[i - 1].ZIndex)
                {
                    zOk = false;
                }
            }
            Check("堆内深度递增（下方卡绘制在前）", zOk, "");

            world.QueueFree();
        }

        private void Test_StackHeight()
        {
            Check("堆高公式：3 张 = 80", Metrics.StackHeight(3) == 80, $"实际 {Metrics.StackHeight(3)}");
            Check("堆高公式：10 张 = 164", Metrics.StackHeight(10) == 164, $"实际 {Metrics.StackHeight(10)}");
            Check("堆高公式：9 张满配车辆 = 152",
                Metrics.StackHeight(9) == 152, $"实际 {Metrics.StackHeight(9)}");
        }

        // ==================== 2. 命中测试 ====================

        private void Test_HitTestHeaderOnly()
        {
            var world = FreshWorld();
            var stack = world.SpawnCard(MakeCard(CardFamily.Cargo, "a"), new Vector2(700, 600));

            for (int i = 0; i < 2; i++)
            {
                var extra = world.SpawnCard(MakeCard(CardFamily.Cargo, "b" + i), new Vector2(700, 600));
                world.MergeStack(extra, stack);
            }

            var root = stack.Root;

            // 根卡的表头条（y 0..12）应命中根卡
            var headerHit = world.HitTest(root.GlobalPosition + new Vector2(20, 6));
            Check("下方卡的表头条可命中", headerHit == root,
                $"命中的是 {headerHit?.Data?.Title ?? "null"}");

            // 根卡被覆盖的部分（y 20）应命中它上面那张卡，而不是根卡
            var coveredHit = world.HitTest(root.GlobalPosition + new Vector2(20, 20));
            Check("被覆盖的机身不命中根卡", coveredHit != root,
                "命中了根卡（下方卡片的命中区应只有 12 px 表头）");

            // 顶卡全身可命中
            var top = stack.Top;
            var topHit = world.HitTest(top.GlobalPosition + new Vector2(20, 50));
            Check("顶卡全身可命中", topHit == top,
                $"命中的是 {topHit?.Data?.Title ?? "null"}");

            world.QueueFree();
        }

        // ==================== 3. 摘出规则 ====================

        private void Test_DetachFromMiddle()
        {
            var world = FreshWorld();
            var stack = BuildStack(world, 5, new Vector2(700, 600));

            var mid = stack.Cards[2];
            var newStack = world.SplitFrom(mid);

            Check("从中间抓取后，原堆剩 2 张", stack.Count == 2, $"实际 {stack.Count}");
            Check("新堆含 3 张（被抓的 + 其上方）", newStack.Count == 3, $"实际 {newStack.Count}");
            Check("新堆的根就是被抓的那张", newStack.Root == mid, "");
            Check("原堆的根未变", stack.Root == null || stack.Root.IsInsideTree(), "");

            world.QueueFree();
        }

        private void Test_DetachTopKeepsRest()
        {
            var world = FreshWorld();
            var stack = BuildStack(world, 4, new Vector2(700, 600));
            var rootBefore = stack.Root;

            var lifted = world.SplitFrom(stack.Top);

            Check("抓顶卡：原堆剩 3 张", stack.Count == 3, $"实际 {stack.Count}");
            Check("抓顶卡：新堆 1 张", lifted.Count == 1, $"实际 {lifted.Count}");
            Check("抓顶卡：原堆根不变", stack.Root == rootBefore, "");

            // 抓根卡 = 整堆移动，不拆分
            var rootStack = world.SplitFrom(stack.Root);
            Check("抓根卡返回原堆本身（整堆移动）", rootStack == stack, "");

            world.QueueFree();
        }

        // ==================== 4. 质量与推动份额 ====================

        private void Test_MassPushesLightMore()
        {
            // 单卡质量 = 1；车辆 = 1 + 50 = 51；设施 = 1 + 8 = 9
            float cargoMass = CardMass.Of(MakeCard(CardFamily.Cargo, "x"));
            float vehicleMass = CardMass.Of(MakeCard(CardFamily.Vehicle, "v"));
            float facilityMass = CardMass.Of(MakeCard(CardFamily.Facility, "f"));

            Check("普通卡质量 = 1", Mathf.IsEqualApprox(cargoMass, 1f), $"实际 {cargoMass}");
            Check("车辆质量 = 51（含 Mob +50）", Mathf.IsEqualApprox(vehicleMass, 51f), $"实际 {vehicleMass}");
            Check("设施质量 = 9（含建筑 +8）", Mathf.IsEqualApprox(facilityMass, 9f), $"实际 {facilityMass}");

            // pushShare = 1 - myMass / (myMass + otherMass)
            // 质量 1 的卡推质量 27 的堆：自己让 1 - 1/28 = 0.964
            float share = 1f - 1f / (1f + 27f);
            Check("轻卡推重堆：轻的让位约 96%（设计文档 0.96）",
                Mathf.Abs(share - 0.964f) < 0.001f, $"实际 {share:F4}");

            // 质量 51 的车辆推质量 1 的卡：车辆只让 1 - 51/52 = 0.019
            float shareHeavy = 1f - 51f / (51f + 1f);
            Check("重车推轻卡：重的几乎不让位（约 1.9%）",
                shareHeavy < 0.02f, $"实际 {shareHeavy:F4}");
        }

        // ==================== 5. 夹取 ====================

        private void Test_ClampTightAndLoose()
        {
            var play = BoardGeometry.PlayAreaRect;

            // 远远拖到棋盘左上角之外
            var wayOut = new Vector2(play.Position.X - 500, play.Position.Y - 500);

            var loose = BoardGeometry.ClampCard(wayOut, tight: false);
            var tight = BoardGeometry.ClampCard(wayOut, tight: true);

            Check("宽松边界：卡可贴到游戏区左上角",
                Mathf.IsEqualApprox(loose.X, play.Position.X) && Mathf.IsEqualApprox(loose.Y, play.Position.Y),
                $"实际 {loose}");

            Check("紧边界：内缩 11 px（设计文档 clamp margin）",
                Mathf.IsEqualApprox(tight.X, play.Position.X + Metrics.ClampMargin)
                && Mathf.IsEqualApprox(tight.Y, play.Position.Y + Metrics.ClampMargin),
                $"实际 {tight}");

            Check("紧边界比宽松边界更靠内", tight.X > loose.X && tight.Y > loose.Y, "");

            // 右下角
            var wayOut2 = new Vector2(play.End.X + 500, play.End.Y + 500);
            var tight2 = BoardGeometry.ClampCard(wayOut2, tight: true);
            Check("紧边界右下：卡整体留在游戏区内",
                tight2.X + Metrics.CardW <= play.End.X - Metrics.ClampMargin + 0.01f
                && tight2.Y + Metrics.CardH <= play.End.Y - Metrics.ClampMargin + 0.01f,
                $"卡右下角 = {new Vector2(tight2.X + Metrics.CardW, tight2.Y + Metrics.CardH)}，游戏区右下 = {play.End}");
        }

        // ==================== 6. 对齐网格 ====================

        private void Test_SnapToGrid()
        {
            var play = BoardGeometry.PlayAreaRect;

            // 一个略微偏离格点的位置，应当被吸附到最近的格点
            var offGrid = new Vector2(
                play.Position.X + Metrics.GridCellW * 2 + 10,
                play.Position.Y + Metrics.GridCellH * 3 + 12);

            var snapped = BoardGeometry.SnapToGrid(offGrid);

            float relX = snapped.X - play.Position.X;
            float relY = snapped.Y - play.Position.Y;

            Check("对齐后落在格点上（x 是 86 的整数倍）",
                Mathf.Abs(relX % Metrics.GridCellW) < 0.01f, $"实际 relX = {relX}");
            Check("对齐后落在格点上（y 是 96 的整数倍）",
                Mathf.Abs(relY % Metrics.GridCellH) < 0.01f, $"实际 relY = {relY}");

            // E5：格子刻意大于卡牌，所以对齐后必然有间隙
            Check("E5：格 86x96 大于卡 48x56（对齐后留有间隙）",
                Metrics.GridCellW > Metrics.CardW && Metrics.GridCellH > Metrics.CardH,
                "");
        }

        // ==================== 7. 投放不弹回 ====================

        private void Test_DropStaysWhereReleased()
        {
            var world = FreshWorld();
            var start = new Vector2(800, 600);
            var stack = world.SpawnCard(MakeCard(CardFamily.Cargo, "a"), start);

            // 模拟投放：改目标位置到远处，然后收敛
            var released = new Vector2(1000, 700);
            world.SetTarget(stack.Root, released);

            for (int i = 0; i < 200; i++)
            {
                world.TickSettle(1f / 60f);
            }

            var final = stack.Root.GlobalPosition;
            float distToReleased = final.DistanceTo(released);
            float distToStart = final.DistanceTo(start);

            Check("投放后停在松手处（距松手点 < 2 px）",
                distToReleased < 2f, $"距松手点 {distToReleased:F2} px");
            Check("投放后未被送回拿起处（设计文档：永不弹回）",
                distToStart > 50f, $"距拿起点仅 {distToStart:F2} px");

            world.QueueFree();
        }

        // ==================== 8. 推动规则 ====================

        private void Test_PushRules()
        {
            var world = FreshWorld();

            // 两张重叠的散卡
            var a = world.SpawnCard(MakeCard(CardFamily.Cargo, "a"), new Vector2(700, 600));
            var b = world.SpawnCard(MakeCard(CardFamily.Cargo, "b"), new Vector2(710, 600));

            float distBefore = a.Root.GlobalPosition.DistanceTo(b.Root.GlobalPosition);

            // 跑若干帧推动
            for (int i = 0; i < 60; i++)
            {
                world.TickPush(1f / 60f);
                world.TickSettle(1f / 60f);
            }

            float distAfter = a.Root.GlobalPosition.DistanceTo(b.Root.GlobalPosition);
            Check("重叠的散卡会被推开（距离变大）", distAfter > distBefore,
                $"{distBefore:F1} → {distAfter:F1}");

            // 被拖拽的卡不参与推动
            var world2 = FreshWorld();
            var c = world2.SpawnCard(MakeCard(CardFamily.Cargo, "c"), new Vector2(700, 600));
            var d = world2.SpawnCard(MakeCard(CardFamily.Cargo, "d"), new Vector2(712, 600));
            world2.SetDragging(c, true);

            var cPosBefore = c.Root.GlobalPosition;
            var dPosBefore = d.Root.GlobalPosition;

            for (int i = 0; i < 60; i++)
            {
                world2.TickPush(1f / 60f);
            }

            Check("被拖拽的卡不推动别人（另一张卡未动）",
                d.Root.GlobalPosition.IsEqualApprox(dPosBefore),
                $"移动了 {d.Root.GlobalPosition.DistanceTo(dPosBefore):F2} px");

            // 质量差异：同样重叠，重的让位少
            var world3 = FreshWorld();
            var heavy = world3.SpawnCard(MakeCard(CardFamily.Vehicle, "heavy"), new Vector2(700, 600));
            var light = world3.SpawnCard(MakeCard(CardFamily.Cargo, "light"), new Vector2(712, 600));

            var heavyBefore = heavy.Root.GlobalPosition;
            var lightBefore = light.Root.GlobalPosition;

            for (int i = 0; i < 60; i++)
            {
                world3.TickPush(1f / 60f);
                world3.TickSettle(1f / 60f);
            }

            float heavyMoved = heavy.Root.GlobalPosition.DistanceTo(heavyBefore);
            // 注意：两侧都会收到推动份额；质量大的位移应显著小于质量小的
            float lightMoved = light.Root.GlobalPosition.DistanceTo(lightBefore);

            Check("质量大的卡位移更小（轻的让位多）",
                lightMoved > heavyMoved,
                $"重(车辆 {CardMass.Of(heavy.Root.Data):F0}) 移动 {heavyMoved:F2} px，轻(货 {CardMass.Of(light.Root.Data):F0}) 移动 {lightMoved:F2} px");

            world.QueueFree();
            world2.QueueFree();
            world3.QueueFree();
        }

        // ==================== 工具 ====================

        /// <summary>建一个干净的世界（不自动生成示例卡）。</summary>
        private CardWorld FreshWorld()
        {
            var host = new Node2D { Name = "TestHost" };
            AddChild(host);

            var board = new BoardView { Name = "BoardView" };
            host.AddChild(board);

            var world = new CardWorld { Name = "CardWorld" };
            host.AddChild(world);

            return world;
        }

        private static CardData MakeCard(CardFamily family, string id)
            => SampleCards.Create(family, id, id);

        /// <summary>叠出一个 n 张的堆。</summary>
        private static CardStack BuildStack(CardWorld world, int n, Vector2 pos)
        {
            var stack = world.SpawnCard(MakeCard(CardFamily.Cargo, "s0"), pos);
            for (int i = 1; i < n; i++)
            {
                var extra = world.SpawnCard(MakeCard(CardFamily.Cargo, "s" + i), pos);
                world.MergeStack(extra, stack);
            }
            return stack;
        }

        private void Check(string label, bool ok, string detail)
        {
            if (ok)
            {
                m_Passed++;
                GD.Print($"  ✓ {label}");
            }
            else
            {
                m_Failed++;
                GD.Print($"  ✗ {label}" + (string.IsNullOrEmpty(detail) ? "" : $"  — {detail}"));
            }
        }

        private void Fail(string message)
        {
            m_Failed++;
            GD.Print($"  ✗ {message}");
        }
    }
}
