using Godot;
using GameLogic.Art;
using GameLogic.Design;

namespace GameLogic.Board
{
    /// <summary>
    /// 棋盘视图（design 01/03 号文档）。
    ///
    /// 绘制顺序（03-canvas-layout.md "Draw Order"，自后向前）：
    ///   1. 世界地形  2. 世界道具  3. 棋盘甲板/围栏/槽位带
    ///   4. 卡牌投影  5. 静止卡牌与堆叠  6. 计时条
    ///   7. 持握中的卡牌  8. 世界特效  9. HUD  10. 暂停与菜单  11. 光标
    ///
    /// 本节点负责 1-3 层；卡牌由 <see cref="Godot.Node2D"/> 子节点承载。
    /// </summary>
    public partial class BoardView : Node2D
    {
        private Sprite2D m_BoardSprite;
        private Node2D m_CardsRoot;
        private Node2D m_HeldRoot;

        /// <summary>所有卡堆的容器。卡牌按 y 排序绘制。</summary>
        public Node2D CardsRoot => m_CardsRoot;

        /// <summary>持握中/飞行中的卡牌容器，永远画在最上层。</summary>
        public Node2D HeldRoot => m_HeldRoot;

        public override void _Ready()
        {
            Name = "BoardView";

            // ---- 1. 世界地形底色 ----
            // 设计：terrain base = sand.2，阴影 sand.1，高光 sand.3。
            // 世界比棋盘更暗更暖，保证棋盘始终是视觉焦点。
            var terrain = new ColorRect
            {
                Name = "Terrain",
                Color = Palette.Sand2,
                Position = Vector2.Zero,
                Size = new Vector2(Metrics.WorldW, Metrics.WorldH),
                MouseFilter = Control.MouseFilterEnum.Ignore,
                ZIndex = -100,
            };
            AddChild(terrain);
            PaintTerrainDetail();

            // ---- 3. 棋盘甲板 ----
            m_BoardSprite = new Sprite2D
            {
                Name = "BoardPlate",
                Texture = BoardPainter.GetTexture(),
                Centered = false,
                Position = new Vector2(Metrics.BoardLeft, Metrics.BoardTop),
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                ZIndex = -50,
            };
            AddChild(m_BoardSprite);

            // ---- 4/5. 卡牌层（在棋盘之上） ----
            m_CardsRoot = new Node2D { Name = "Cards", ZIndex = 0 };
            AddChild(m_CardsRoot);

            // ---- 7. 持握层（永远最上） ----
            m_HeldRoot = new Node2D { Name = "Held", ZIndex = 1000 };
            AddChild(m_HeldRoot);
        }

        /// <summary>
        /// 世界地形的细节：稀疏的沙丘斑与车道痕迹。
        ///
        /// 密度刻意压得很低。设计文档（01-visual-direction.md）要求
        /// "the world is darker and warmer than the board, so the board always
        /// reads as the focus" —— 地形若布满细节，棋盘就不再是焦点。
        /// 所以这里只用 ~3% 的瓦片点缀，且全部使用 sand 斜坡内的相邻色。
        /// </summary>
        private void PaintTerrainDetail()
        {
            const int tile = 32;   // 用 32 px 瓦片，斑块更大、数量更少
            int cols = Metrics.WorldW / tile;
            int rows = Metrics.WorldH / tile;

            var canvas = new PixelCanvas(Metrics.WorldW, Metrics.WorldH, new Color(0, 0, 0, 0));

            for (int ty = 0; ty < rows; ty++)
            {
                for (int tx = 0; tx < cols; tx++)
                {
                    int px = tx * tile;
                    int py = ty * tile;

                    // 棋盘范围内不画（甲板会完全盖住）
                    if (px + tile >= Metrics.BoardLeft - 8 && px <= Metrics.BoardRight + 8 &&
                        py + tile >= Metrics.BoardTop - 8 && py <= Metrics.BoardBottom + 8)
                    {
                        continue;
                    }

                    int h = Hash(tx, ty);

                    // 约 1/12 的瓦片得到一个浅色沙丘斑（高光）
                    if (h % 12 == 0)
                    {
                        canvas.FillRect(px + 6, py + 8, 14, 7, Palette.Sand3);
                        canvas.FillRect(px + 9, py + 6, 8, 3, Palette.Sand3);
                    }

                    // 约 1/9 的瓦片得到一个暗色阴影斑
                    if (h % 9 == 0)
                    {
                        canvas.FillRect(px + 17, py + 18, 10, 5, Palette.Sand1);
                    }

                    // 少量孤立高光点（碎石反光）
                    if (h % 23 == 0)
                    {
                        canvas.Set(px + 24, py + 5, Palette.Sand4);
                        canvas.Set(px + 25, py + 6, Palette.Sand4);
                    }
                }
            }

            var sprite = new Sprite2D
            {
                Name = "TerrainDetail",
                Texture = canvas.ToTexture("TerrainDetail"),
                Centered = false,
                Position = Vector2.Zero,
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                ZIndex = -99,
            };
            AddChild(sprite);
        }

        /// <summary>确定性哈希，保证地形每次一致。</summary>
        private static int Hash(int x, int y)
        {
            unchecked
            {
                int h = x * 73856093 ^ y * 19349663;
                h ^= h >> 13;
                h *= 0x5bd1e995;
                h ^= h >> 15;
                return Mathf.Abs(h);
            }
        }
    }
}
