using Godot;
using GameLogic.Design;

namespace GameLogic.Cards
{
	/// <summary>
	/// 卡框程序化绘制器（design 04-card-system.md + 05/11 号文档）。
	///
	/// 设计文档把一张成品卡拆成 6 层，其中"卡框"是唯一按家族不同的美术资源。
	/// 在美术资源到位之前，本类按文档的**像素解剖学**逐像素生成同规格的卡框，
	/// 使棋盘、堆叠、命中测试、家族识别全部可以先行验证：
	///
	///   y0       ink 描边（四角像素透明 = 1 px 圆角）
	///   y1-12    HEADER  标题行（机箱色）
	///   y13      separator（机箱暗色）
	///   y14-42   ART AREA 29 行（顶行亮色高光）
	///   y43-54   FOOTER 12 行
	///   y55      ink 描边
	///
	/// 行合计 1 + 12 + 1 + 29 + 12 + 1 = 56，列合计 1 + 46 + 1 = 48（E1 勘误后）。
	/// 美术介入时只需把 <see cref="Build"/> 的产物换成已绘制的 PNG，调用方不变。
	/// </summary>
	public static class CardFramePainter
	{
		private const int W = Metrics.CardW;
		private const int H = Metrics.CardH;

		// 行区间
		private const int YHeaderTop = 1;
		private const int YHeaderBottom = 12;
		private const int YSeparator = 13;
		private const int YArtTop = 14;
		private const int YArtBottom = 42;
		private const int YFooterTop = 43;
		private const int YFooterBottom = 54;
		private const int YOutlineBottom = 55;

		private static readonly System.Collections.Generic.Dictionary<string, ImageTexture> Cache = new();

		/// <summary>
		/// 取（家族, 品级）对应的卡框纹理。结果带缓存——同一张框在 200 张卡之间共享。
		/// </summary>
		public static ImageTexture Get(CardFamily family, CardGrade grade = CardGrade.Standard)
		{
			string key = $"{family}_{grade}";
			if (Cache.TryGetValue(key, out var cached) && cached != null)
			{
				return cached;
			}

			var tex = ImageTexture.CreateFromImage(Build(CardFamilies.Get(family), grade));
			Cache[key] = tex;
			return tex;
		}

		/// <summary>清空缓存（编辑器热重载或调色板改动后调用）。</summary>
		public static void ClearCache()
		{
			Cache.Clear();
		}

		/// <summary>
		/// 按解剖学生成一个卡框。返回 Rgba8 Image，未使用像素为全透明。
		/// </summary>
		public static Image Build(CardFamilyStyle style, CardGrade grade)
		{
			var img = Image.CreateEmpty(W, H, false, Image.Format.Rgba8);
			img.Fill(new Color(0, 0, 0, 0));

			// ---- 1. 机箱填充（不含描边） ----
			img.FillRect(new Rect2I(1, 1, W - 2, H - 2), style.Body);

			// ---- 2. 分隔行：机箱的下一档暗色 ----
			img.FillRect(new Rect2I(1, YSeparator, W - 2, 1), style.Shade);

			// ---- 3. 美术区顶行 1 px 高光 ----
			img.FillRect(new Rect2I(1, YArtTop, W - 2, 1), style.Highlight);

			// ---- 4. 形状暗示（家族可辨性，灰度下仍有效） ----
			PaintCue(img, style);

			// ---- 5. 品级描边 ----
			PaintGrade(img, style, grade);

			// ---- 6. ink 描边 + 1 px 圆角（压在一切之上） ----
			PaintOutline(img, style);

			// ---- 7. 剪影级暗示：必须在描边**之后**画，否则会被描边覆盖 ----
			if (style.Cue == ShapeCue.DogTagNotch)
			{
				PaintDogTagNotch(img);
			}

			return img;
		}

		// ==================== 形状暗示 ====================

		private static void PaintCue(Image img, CardFamilyStyle style)
		{
			switch (style.Cue)
			{
				case ShapeCue.WheelNotches:
					// 底边 2 个 3x2 轮齿切口，位于两枚徽章之间（x 15..32）
					CutNotch(img, 17, YFooterBottom - 1, 3, 2);
					CutNotch(img, 28, YFooterBottom - 1, 3, 2);
					break;

				case ShapeCue.HeaderTeeth:
					// 表头下沿 3 颗齿，向下咬进分隔行
					PaintTeeth(img, style);
					break;

				case ShapeCue.Keyhole:
					PaintKeyhole(img);
					break;

				case ShapeCue.TornTop:
					PaintTornTop(img, style);
					break;

				case ShapeCue.CornerRivets:
					// 美术区四角 2x2 steel.7 铆钉
					PaintRivet(img, 2, YArtTop + 1);
					PaintRivet(img, W - 4, YArtTop + 1);
					PaintRivet(img, 2, YArtBottom - 2);
					PaintRivet(img, W - 4, YArtBottom - 2);
					break;

				case ShapeCue.HazardStripe:
					PaintHazardStripe(img);
					break;

				case ShapeCue.Contours:
					PaintContours(img);
					break;

				case ShapeCue.StencilBand:
					PaintStencilBand(img);
					break;

				case ShapeCue.DogTagNotch:
					// 剪影级暗示，由 Build 在描边之后统一处理（PaintDogTagNotch）
					break;
			}

			// 主角专属：完整 1 px cyan.3 内描边（在 ink 描边内 1 px）
			if (style.Family == CardFamily.Protagonist)
			{
				PaintInnerBorder(img, Palette.Cyan3);
			}
		}

		private static void PaintTeeth(Image img, CardFamilyStyle style)
		{
			// 3 齿，等距分布在 46 px 内容宽上
			int[] starts = { 12, 22, 32 };
			foreach (int x in starts)
			{
				img.FillRect(new Rect2I(x, YHeaderBottom, 3, 1), Palette.Ink);
				if (style.Family == CardFamily.Enemy)
				{
					// 齿尖再深一档，避免在 rust.2 上糊掉
					img.FillRect(new Rect2I(x + 1, YSeparator, 1, 1), Palette.Ink);
				}
			}
		}

		private static void PaintKeyhole(Image img)
		{
			const int cx = W / 2;                 // 24
			int y = YFooterTop + 2;               // 45
			img.SetPixel(cx, y, Palette.Ink);                       // 圆顶上沿
			img.FillRect(new Rect2I(cx - 1, y + 1, 3, 2), Palette.Ink); // 圆身
			img.SetPixel(cx, y + 3, Palette.Ink);                   // 杆
			img.SetPixel(cx, y + 4, Palette.Ink);
		}

		private static void PaintTornTop(Image img, CardFamilyStyle style)
		{
			// 顶边 1 px 阶梯锯齿：ink 在 y0 / y1 之间交替，缺口填机箱色
			for (int x = 1; x < W - 1; x++)
			{
				bool up = (x / 2) % 2 == 0;
				if (up)
				{
					img.SetPixel(x, 0, Palette.Ink);
					img.SetPixel(x, 1, style.Body);
				}
				else
				{
					img.SetPixel(x, 0, style.Body);
					img.SetPixel(x, 1, Palette.Ink);
				}
			}
		}

		private static void PaintRivet(Image img, int x, int y)
		{
			img.FillRect(new Rect2I(x, y, 2, 2), Palette.Steel7);
		}

		/// <summary>
		/// 主角的军牌缺角（11-card-taxonomy.md）：左上角 3x3 像素块被切掉，
		/// 填入棋盘色，使**仅凭剪影**即可识别主角。
		/// 必须在 ink 描边之后调用——描边会覆盖 0,0 处的圆角与外圈。
		/// </summary>
		private static void PaintDogTagNotch(Image img)
		{
			img.FillRect(new Rect2I(0, 0, 3, 3), Palette.Steel6);
		}

		private static void PaintHazardStripe(Image img)
		{
			// 美术区左沿 4 px 斜纹，hazard / ink 交替
			for (int y = YArtTop; y <= YArtBottom; y++)
			{
				for (int x = 1; x <= 4; x++)
				{
					img.SetPixel(x, y, ((x + y) / 2) % 2 == 0 ? Palette.Hazard : Palette.Ink);
				}
			}
		}

		private static void PaintContours(Image img)
		{
			// 美术区左上 3 道嵌套的 1 px 等高线弧（sand.3）。
			// 圆心取美术区左上角外侧，弧扫第四象限 → 在角落形成同心弧，
			// 半径 5 / 7 / 9，彼此间隔 1 px，肉眼可辨。
			const int cx = 1;
			const int cy = YArtTop;
			for (int i = 0; i < 3; i++)
			{
				DrawArc(img, cx, cy, 5 + i * 2, 20, 88, Palette.Sand3);
			}
		}

		private static void PaintStencilBand(Image img)
		{
			// 美术区底边 3 px steel.7 模板带，带 3 个 2 px 缺口
			const int y = YArtBottom - 2;          // 40..42
			for (int x = 1; x < W - 1; x++)
			{
				bool gap = x is >= 10 and <= 11 or >= 23 and <= 24 or >= 36 and <= 37;
				if (gap)
				{
					continue;
				}
				img.FillRect(new Rect2I(x, y, 1, 3), Palette.Steel7);
			}
		}

		// ==================== 品级 / 描边 ====================

		private static void PaintGrade(Image img, CardFamilyStyle style, CardGrade grade)
		{
			switch (grade)
			{
				case CardGrade.Rare:
					// ink 内侧再一条 1 px amber.3，仅顶边
					img.FillRect(new Rect2I(2, 2, W - 4, 1), Palette.Amber3);
					break;

				case CardGrade.Prototype:
					PaintInnerBorder(img, Palette.Cyan3);
					break;
			}
		}

		/// <summary>在 ink 描边内 1 px 画一圈内描边。</summary>
		private static void PaintInnerBorder(Image img, Color color)
		{
			img.FillRect(new Rect2I(1, 1, W - 2, 1), color);            // 顶
			img.FillRect(new Rect2I(1, H - 2, W - 2, 1), color);        // 底
			img.FillRect(new Rect2I(1, 1, 1, H - 2), color);            // 左
			img.FillRect(new Rect2I(W - 2, 1, 1, H - 2), color);        // 右
		}

		private static void PaintOutline(Image img, CardFamilyStyle style)
		{
			// 四边 ink
			img.FillRect(new Rect2I(0, 0, W, 1), Palette.Ink);
			img.FillRect(new Rect2I(0, H - 1, W, 1), Palette.Ink);
			img.FillRect(new Rect2I(0, 0, 1, H), Palette.Ink);
			img.FillRect(new Rect2I(W - 1, 0, 1, H), Palette.Ink);

			// 1 px 圆角：四角像素透明
			img.SetPixel(0, 0, new Color(0, 0, 0, 0));
			img.SetPixel(W - 1, 0, new Color(0, 0, 0, 0));
			img.SetPixel(0, H - 1, new Color(0, 0, 0, 0));
			img.SetPixel(W - 1, H - 1, new Color(0, 0, 0, 0));

			// Event 家族的撕裂顶边：锯齿已覆盖 y0/y1，这里只补两端
			if (style.Cue == ShapeCue.TornTop)
			{
				img.SetPixel(0, 0, Palette.Ink);
				img.SetPixel(W - 1, 0, Palette.Ink);
			}
		}

		// ==================== 小工具 ====================

		private static void CutNotch(Image img, int x, int y, int w, int h)
		{
			img.FillRect(new Rect2I(x, y, w, h), new Color(0, 0, 0, 0));
		}

		/// <summary>1 px 圆弧（Bresenham 式步进），用于等高线弧、冷却弧。</summary>
		private static void DrawArc(Image img, int cx, int cy, int radius, float fromDeg, float toDeg, Color color)
		{
			for (float a = fromDeg; a <= toDeg; a += 8f)
			{
				float rad = Mathf.DegToRad(a);
				int x = cx + Mathf.RoundToInt(Mathf.Cos(rad) * radius);
				int y = cy + Mathf.RoundToInt(Mathf.Sin(rad) * radius);
				if (x >= 0 && x < W && y >= 0 && y < H)
				{
					img.SetPixel(x, y, color);
				}
			}
		}
	}
}
