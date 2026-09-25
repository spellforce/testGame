using Godot;
using GameLogic.Design;

namespace GameLogic.Art
{
    /// <summary>
    /// 像素画布：以裸 RGBA8 字节缓冲逐像素作画，再一次性交给 Godot 生成纹理。
    ///
    /// 为什么要绕过 <c>Image.SetPixel</c>：那是逐像素的 P/Invoke，一张 1104 x 644
    /// 的棋盘要 70 万次调用。本类在托管数组上作画，只在最后调用一次
    /// <c>Image.CreateFromData</c>。
    ///
    /// 坐标系原点在左上，+y 向下，与设计文档的行号一致。
    /// </summary>
    public sealed class PixelCanvas
    {
        private readonly int m_W;
        private readonly int m_H;
        private readonly byte[] m_Px;

        public int Width => m_W;
        public int Height => m_H;

        public PixelCanvas(int w, int h, Color fill)
        {
            m_W = w;
            m_H = h;
            m_Px = new byte[w * h * 4];
            FillRect(0, 0, w, h, fill);
        }

        /// <summary>写入一个像素。越界坐标静默忽略，调用方无需裁剪。</summary>
        public void Set(int x, int y, Color c)
        {
            if ((uint)x >= (uint)m_W || (uint)y >= (uint)m_H)
            {
                return;
            }
            int i = (y * m_W + x) * 4;
            m_Px[i + 0] = ToByte(c.R);
            m_Px[i + 1] = ToByte(c.G);
            m_Px[i + 2] = ToByte(c.B);
            m_Px[i + 3] = ToByte(c.A);
        }

        /// <summary>读取一个像素（越界返回全透明）。</summary>
        public Color Get(int x, int y)
        {
            if ((uint)x >= (uint)m_W || (uint)y >= (uint)m_H)
            {
                return new Color(0, 0, 0, 0);
            }
            int i = (y * m_W + x) * 4;
            return new Color(m_Px[i] / 255f, m_Px[i + 1] / 255f, m_Px[i + 2] / 255f, m_Px[i + 3] / 255f);
        }

        public void FillRect(int x, int y, int w, int h, Color c)
        {
            int x0 = Mathf.Max(0, x);
            int y0 = Mathf.Max(0, y);
            int x1 = Mathf.Min(m_W, x + w);
            int y1 = Mathf.Min(m_H, y + h);
            byte r = ToByte(c.R), g = ToByte(c.G), b = ToByte(c.B), a = ToByte(c.A);

            for (int yy = y0; yy < y1; yy++)
            {
                int row = yy * m_W;
                for (int xx = x0; xx < x1; xx++)
                {
                    int i = (row + xx) * 4;
                    m_Px[i + 0] = r;
                    m_Px[i + 1] = g;
                    m_Px[i + 2] = b;
                    m_Px[i + 3] = a;
                }
            }
        }

        /// <summary>把整张画布的一次性固化成 Godot 纹理。</summary>
        public ImageTexture ToTexture(string name = "")
        {
            var img = Image.CreateFromData(m_W, m_H, false, Image.Format.Rgba8, m_Px);
            if (!string.IsNullOrEmpty(name))
            {
                img.SetMeta("name", name);
            }
            return ImageTexture.CreateFromImage(img);
        }

        public Image ToImage() => Image.CreateFromData(m_W, m_H, false, Image.Format.Rgba8, m_Px);

        /// <summary>从另一张画布贴入（跳过全透明像素），用于拼装。</summary>
        public void Blit(PixelCanvas src, int destX, int destY, bool skipTransparent = true)
        {
            for (int y = 0; y < src.m_H; y++)
            {
                for (int x = 0; x < src.m_W; x++)
                {
                    var c = src.Get(x, y);
                    if (skipTransparent && c.A <= 0f)
                    {
                        continue;
                    }
                    Set(destX + x, destY + y, c);
                }
            }
        }

        /// <summary>
        /// 在矩形内画 1 px 圆角描边（设计文档的"四角像素透明 = 1 px 圆角"）。
        /// </summary>
        public void StrokeRounded(int x, int y, int w, int h, Color c)
        {
            for (int i = 0; i < w; i++)
            {
                Set(x + i, y, c);
                Set(x + i, y + h - 1, c);
            }
            for (int j = 0; j < h; j++)
            {
                Set(x, y + j, c);
                Set(x + w - 1, y + j, c);
            }
            // 抹掉四角，形成 1 px 圆角
            Set(x, y, new Color(0, 0, 0, 0));
            Set(x + w - 1, y, new Color(0, 0, 0, 0));
            Set(x, y + h - 1, new Color(0, 0, 0, 0));
            Set(x + w - 1, y + h - 1, new Color(0, 0, 0, 0));
        }

        /// <summary>1 px 圆弧（步进采样），用于等高线、冷却弧、装饰。</summary>
        public void Arc(int cx, int cy, int radius, float fromDeg, float toDeg, Color c, float stepDeg = 6f)
        {
            for (float a = fromDeg; a <= toDeg; a += stepDeg)
            {
                float rad = Mathf.DegToRad(a);
                Set(cx + Mathf.RoundToInt(Mathf.Cos(rad) * radius),
                    cy + Mathf.RoundToInt(Mathf.Sin(rad) * radius), c);
            }
        }

        private static byte ToByte(float v) => (byte)Mathf.Clamp(Mathf.RoundToInt(v * 255f), 0, 255);
    }
}
