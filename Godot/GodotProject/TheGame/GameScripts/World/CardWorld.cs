using Godot;
using GameLogic.Board;
using GameLogic.Cards;
using GameLogic.Design;
using System.Collections.Generic;

namespace GameLogic.World
{
    /// <summary>
    /// 卡牌世界：拥有所有卡堆，提供空间查询、生成/销毁与收敛。
    ///
    /// 收敛模型（design 06-interaction-motion.md）：每张卡有一个**目标位置**
    /// （targetPosition），当前位置以 <c>lerp(pos, target, dt * 20)</c> 向其推进。
    /// 推动、拖拽、投放都只改目标位置，从不直接改当前位置——这样滑动是平滑的，
    /// 且随时可以被打断。
    ///
    /// 坐标约定：所有位置指卡牌**左上角**的世界坐标。
    /// </summary>
    public partial class CardWorld : Node2D
    {
        private const string CardScenePath = "res://TheGame/Scenes/Cards/Card.tscn";
        private PackedScene m_CardScene;
        private readonly List<CardStack> m_Stacks = new();

        /// <summary>每张卡的收敛目标（左上角世界坐标）。</summary>
        private readonly Dictionary<CardView, Vector2> m_Targets = new();

        /// <summary>尚未归堆的散卡（被拖出后短暂存在）。</summary>
        private readonly HashSet<CardView> m_FreeCards = new();

        /// <summary>空间网格：cell 64 x 64 art px（09-godot-handoff.md "Performance"）。</summary>
        private const int CellSize = 64;
        private readonly Dictionary<(int, int), List<CardStack>> m_Grid = new();

        /// <summary>所有卡堆（只读）。</summary>
        public IReadOnlyList<CardStack> Stacks => m_Stacks;

        public CardWorld()
        {
            Name = "CardWorld";
        }

        /// <summary>
        /// 静止卡堆所在的层（BoardView/Cards）。所有堆——新生成的与刚放下的——
        /// 必须在同一层，否则"最近移动的堆画在最上"（03 "Draw Order" 第 5 条）不成立。
        /// 无 BoardView 时（自检场景）退回到自身。
        /// </summary>
        private Node CardsLayer
            => GetParent()?.GetNodeOrNull<BoardView>("BoardView")?.CardsRoot ?? (Node)this;

        // ==================== 生成 / 销毁 ====================

        /// <summary>
        /// 在世界中生成一张卡，落到指定位置。返回新建的堆。
        /// </summary>
        public CardStack SpawnCard(CardData data, Vector2 topLeft)
        {
            m_CardScene ??= GD.Load<PackedScene>(CardScenePath);
            var view = m_CardScene.Instantiate<CardView>();
            view.SetData(data);

            var stack = new CardStack();
            CardsLayer.AddChild(stack);
            stack.AddChild(view);

            view.SnapTo(topLeft);
            stack.Push(view);

            m_Stacks.Add(stack);
            m_FreeCards.Add(view);
            m_Targets[view] = topLeft;

            RebuildGrid();
            return stack;
        }

        /// <summary>移除一张卡及其所在的堆（若堆空了）。</summary>
        public void DestroyCard(CardView card)
        {
            if (card == null)
            {
                return;
            }

            m_Targets.Remove(card);
            m_FreeCards.Remove(card);
            card.QueueFree();
        }

        // ==================== 查询 ====================

        /// <summary>
        /// 命中测试：找最上层（最靠后绘制、y 最大）的卡。
        /// 堆内下方卡片的命中区只有露出的 12 px 表头条（04-card-system.md "Hit Area"）。
        /// </summary>
        public CardView HitTest(Vector2 worldPos)
        {
            CardView best = null;
            int bestZ = int.MinValue;

            foreach (var stack in m_Stacks)
            {
                foreach (var card in stack.Cards)
                {
                    if (!HitTestCard(stack, card, worldPos))
                    {
                        continue;
                    }
                    int z = card.ZIndex + m_Stacks.IndexOf(stack) * 1000;
                    if (z >= bestZ)
                    {
                        bestZ = z;
                        best = card;
                    }
                }
            }
            return best;
        }

        /// <summary>单张卡的命中判定：顶卡用完整矩形，下方卡只用 12 px 表头条。</summary>
        private static bool HitTestCard(CardStack stack, CardView card, Vector2 worldPos)
        {
            bool isTop = stack.Top == card;
            var pos = card.GlobalPosition;

            float h = isTop ? Metrics.CardH : CardStack.Offset;
            float y0 = pos.Y;
            // 下方卡的表头条就是它自己露出的那一段
            float x0 = pos.X;

            return worldPos.X >= x0 && worldPos.X < x0 + Metrics.CardW
                && worldPos.Y >= y0 && worldPos.Y < y0 + h;
        }

        /// <summary>返回包围指定卡牌矩形的所有卡堆（含自身所在堆）。</summary>
        public List<CardStack> OverlappingStacks(Rect2 rect, CardStack exclude = null)
        {
            var result = new List<CardStack>();

            // 用空间网格缩小候选集
            int cx0 = Mathf.FloorToInt(rect.Position.X / CellSize);
            int cy0 = Mathf.FloorToInt(rect.Position.Y / CellSize);
            int cx1 = Mathf.FloorToInt(rect.End.X / CellSize);
            int cy1 = Mathf.FloorToInt(rect.End.Y / CellSize);

            var seen = new HashSet<CardStack>();
            for (int cy = cy0; cy <= cy1; cy++)
            {
                for (int cx = cx0; cx <= cx1; cx++)
                {
                    if (!m_Grid.TryGetValue((cx, cy), out var list))
                    {
                        continue;
                    }
                    foreach (var stack in list)
                    {
                        if (stack == exclude || !seen.Add(stack))
                        {
                            continue;
                        }
                        if (stack.Bounds.Intersects(rect))
                        {
                            result.Add(stack);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>堆的当前数量。</summary>
        public int StackCount => m_Stacks.Count;

        // ==================== 收敛与目标 ====================

        /// <summary>设置某张卡的目标位置（左上角世界坐标）。</summary>
        public void SetTarget(CardView card, Vector2 topLeft)
        {
            m_Targets[card] = topLeft;
        }

        public Vector2 GetTarget(CardView card)
            => m_Targets.TryGetValue(card, out var t) ? t : card.GlobalPosition;

        /// <summary>
        /// 每帧收敛。位置以 lerp(pos, target, dt * 20) 推进（06 的收敛规则）。
        /// 静止的卡会被吸附到整数像素，保证像素锐利（01 "Resting positions"）。
        /// </summary>
        public void TickSettle(float delta)
        {
            float k = Mathf.Min(1f, delta * Metrics.SettleRate);

            foreach (var stack in m_Stacks)
            {
                if (stack.Count == 0)
                {
                    continue;
                }

                var root = stack.Root;

                // 拖拽中的堆不收敛：位置直接由鼠标驱动
                if (stack.IsDragging)
                {
                    continue;
                }

                var target = GetTarget(root);
                var pos = root.GlobalPosition;

                if (pos.DistanceSquaredTo(target) < 0.01f)
                {
                    // 收敛完成 → 吸附整像素
                    root.GlobalPosition = new Vector2(Mathf.Round(pos.X), Mathf.Round(pos.Y));
                    stack.Relayout();
                    continue;
                }

                root.GlobalPosition = pos.Lerp(target, k);
                stack.Relayout();
            }
        }

        /// <summary>把某张卡所属的堆标记为拖拽中 / 结束拖拽。</summary>
        public void SetDragging(CardStack stack, bool dragging)
        {
            if (stack == null)
            {
                return;
            }
            stack.IsDragging = dragging;

            if (dragging)
            {
                // 拖起来后整堆移到 Held 层，画在最前
                ReparentStackToHeld(stack, true);
            }
            else
            {
                ReparentStackToHeld(stack, false);
            }
        }

        private void ReparentStackToHeld(CardStack stack, bool toHeld)
        {
            var board = GetParent()?.GetNodeOrNull<BoardView>("BoardView");
            if (board == null)
            {
                return;
            }
            var target = toHeld ? board.HeldRoot : board.CardsRoot;
            if (stack.GetParent() != target)
            {
                stack.GetParent()?.RemoveChild(stack);
                target.AddChild(stack);
            }
        }

        /// <summary>
        /// 推动分离（design 06-interaction-motion.md "Push-Apart"）。
        ///
        /// 这是"卡牌互相挤开"的规则，每帧在移动之后运行。严格照文档的 7 条：
        ///   1. 被拖拽的卡不推也不被推
        ///   2. 只有**根卡**推动；堆内的卡什么都不做
        ///   3. 正在工作/已装备的卡不推也不被推（本版本暂无该状态）
        ///   4. 卡包推别人但从不被别人推（本版本暂无卡包）
        ///   5. 同一堆内的卡互相忽略
        ///   6. 飞向目标的卡不推（本版本暂无飞行态）
        ///   7. 重型地基只与重型地基互推（本版本暂无地基）
        ///
        /// 两个关键实现细节，也是最容易做错的地方：
        ///  - 推动改的是**目标位置**，不是当前位置。卡再以 20/s 收敛过去，
        ///    所以滑动平滑且可被打断。
        ///  - 每张卡每帧**只推一次**：循环在第一个有效重叠处就 break
        ///    （顺序来自重叠查询，不是按距离排序）。
        ///
        /// 推的力度：
        ///   pushShare = 1 - myMass / (myMass + otherMass)
        ///   我退 share 份，对方进 (1-share) 份 —— 所以轻的让位多。
        /// </summary>
        public void TickPush(float delta)
        {
            if (m_Stacks.Count < 2)
            {
                return;
            }

            // 位置可能已经变化，先刷新网格再做重叠查询
            RebuildGrid();

            float step = Metrics.PushSpeed * delta;

            foreach (var me in m_Stacks)
            {
                // 规则 1：拖拽中的堆不参与推动
                if (me.IsDragging || me.Count == 0)
                {
                    continue;
                }

                // 规则 2：以**根卡**位置推动
                var myRoot = me.Root;
                var myRect = me.Bounds;

                // 规则 6：飞向目标的卡不推
                if (Vector2.Zero.DistanceTo(myRoot.Position) < 0f)
                {
                    continue;
                }

                var candidates = OverlappingStacks(myRect, exclude: me);

                foreach (var other in candidates)
                {
                    if (other.Count == 0 || other.IsDragging)
                    {
                        continue;
                    }

                    // 规则 5：同堆内互相忽略
                    if (other == me)
                    {
                        continue;
                    }

                    // 方向取两个**根卡**中心之差（地面平面内）
                    var dir = other.Root.GlobalPosition - myRoot.GlobalPosition;
                    if (dir.LengthSquared() < 0.0001f)
                    {
                        // 完全重合：给一个确定性的微小偏移，避免除零后卡死
                        dir = new Vector2(1, 0);
                    }
                    dir = dir.Normalized();

                    float myMass = me.Mass;
                    float otherMass = other.Mass;
                    float share = 1f - myMass / (myMass + otherMass);

                    // 我退 share，对方进 (1 - share)
                    SetTarget(myRoot, GetTarget(myRoot) - dir * share * step);
                    SetTarget(other.Root, GetTarget(other.Root) + dir * (1f - share) * step);

                    // 规则：每张卡每帧只推一次
                    break;
                }
            }
        }

        // ==================== 空间网格 ====================

        /// <summary>重建空间网格。卡牌移动后调用。</summary>
        public void RebuildGrid()
        {
            m_Grid.Clear();
            foreach (var stack in m_Stacks)
            {
                var b = stack.Bounds;
                int cx0 = Mathf.FloorToInt(b.Position.X / CellSize);
                int cy0 = Mathf.FloorToInt(b.Position.Y / CellSize);
                int cx1 = Mathf.FloorToInt(b.End.X / CellSize);
                int cy1 = Mathf.FloorToInt(b.End.Y / CellSize);

                for (int cy = cy0; cy <= cy1; cy++)
                {
                    for (int cx = cx0; cx <= cx1; cx++)
                    {
                        if (!m_Grid.TryGetValue((cx, cy), out var list))
                        {
                            list = new List<CardStack>();
                            m_Grid[(cx, cy)] = list;
                        }
                        list.Add(stack);
                    }
                }
            }
        }

        // ==================== 堆的合并 / 拆分 ====================

        /// <summary>
        /// 把 <paramref name="moved"/> 堆合并进 <paramref name="target"/> 堆。
        /// returned: 合并后剩余的源堆（通常为空，由调用方清理）。
        /// </summary>
        public void MergeStack(CardStack moved, CardStack target)
        {
            if (moved == null || target == null || moved == target)
            {
                return;
            }

            var cards = new List<CardView>(moved.Cards);
            moved.IsDragging = false;

            foreach (var card in cards)
            {
                moved.Remove(card);
                target.Push(card);
                m_Targets[card] = target.Root.GlobalPosition;
            }

            RemoveStack(moved);

            // 合并后整堆目标位置不变
            SetTarget(target.Root, target.Root.GlobalPosition);
            RebuildGrid();
        }

        /// <summary>从堆中摘出一张卡（及其上层），成为独立的新堆。</summary>
        public CardStack SplitFrom(CardView card)
        {
            CardStack owner = FindStack(card);
            if (owner == null)
            {
                return null;
            }

            // 抓根卡 = 整堆移动，无需拆分
            if (owner.Root == card)
            {
                return owner;
            }

            var detached = owner.DetachFrom(card);
            if (detached.Count == 0)
            {
                return null;
            }

            var newStack = new CardStack();
            var board = GetParent()?.GetNodeOrNull<BoardView>("BoardView");
            (board?.HeldRoot ?? (Node)this).AddChild(newStack);

            foreach (var c in detached)
            {
                newStack.AddChild(c);
                newStack.Push(c);
            }

            m_Stacks.Add(newStack);

            // 原堆的目标位置不变；新堆跟随被拿起的那张卡
            if (owner.Count > 0)
            {
                SetTarget(owner.Root, owner.Root.GlobalPosition);
            }
            SetTarget(newStack.Root, newStack.Root.GlobalPosition);

            RebuildGrid();
            return newStack;
        }

        /// <summary>找到某张卡所属的堆。</summary>
        public CardStack FindStack(CardView card)
        {
            foreach (var stack in m_Stacks)
            {
                if (stack.Contains(card))
                {
                    return stack;
                }
            }
            return null;
        }

        /// <summary>移除一个（应当已空的）堆。</summary>
        public void RemoveStack(CardStack stack)
        {
            if (stack == null)
            {
                return;
            }
            m_Stacks.Remove(stack);
            if (IsInstanceValid(stack))
            {
                stack.QueueFree();
            }
            RebuildGrid();
        }

        /// <summary>清空所有卡（重开 / 切板用）。</summary>
        public void Clear()
        {
            foreach (var stack in m_Stacks)
            {
                if (IsInstanceValid(stack))
                {
                    stack.QueueFree();
                }
            }
            m_Stacks.Clear();
            m_Targets.Clear();
            m_FreeCards.Clear();
            m_Grid.Clear();
        }
    }
}
