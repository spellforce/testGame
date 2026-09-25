using Godot;
using GameLogic.Cards;
using GameLogic.Design;

namespace GameLogic.DebugTools
{
	/// <summary>
	/// 卡框校验工具（design 19-ui-validation.md / 10-validation-checklist.md）。
	///
	/// 生成两张对照图并写到 res://.debug/：
	///
	/// 1. <c>card_frames_detail.png</c> — 全部家族 4x 放大，检查像素解剖、
	///    描边、圆角、形状暗示、品级线是否正确。
	/// 2. <c>card_frames_grayscale.png</c> — 上排彩色 1x、下排灰度 1x。
	///    设计文档要求："把全部家族的截图转灰度后，每个家族在 1 屏幕像素/art px 下
	///    仅凭形状暗示仍可辨识"。这张图就是那条验收项的判据。
	///
	/// 独立运行：<c>godot --path GodotProject res://TheGame/DebugTools/CardFrameSheet.tscn</c>
	/// </summary>
	public partial class CardFrameSheet : Node2D
	{
		private const string OutDir = "res://.debug";

		public override void _Ready()
		{
			DirAccess.MakeDirRecursiveAbsolute(OutDir);

			WriteDetailSheet();
			WriteGrayscaleSheet();

			GD.Print($"[CardFrameSheet] 已写出到 {OutDir}/");
			GetTree().Quit();
		}

		/// <summary>4x 放大对照图：3 列 x 3 行，覆盖 8 家族 + 主角。</summary>
		private void WriteDetailSheet()
		{
			const int scale = 4;
			const int cols = 3;
			const int gutter = 16;
			const int pad = 16;

			var styles = AllStyles();
			int rows = Mathf.CeilToInt((float)styles.Length / cols);

			int cellW = Metrics.CardW * scale + gutter;
			int cellH = Metrics.CardH * scale + gutter;

			var sheet = Image.CreateEmpty(
				cols * cellW + pad * 2 - gutter,
				rows * cellH + pad * 2 - gutter,
				false,
				Image.Format.Rgba8);
			sheet.Fill(Palette.Steel2);

			for (int i = 0; i < styles.Length; i++)
			{
				int col = i % cols;
				int row = i / cols;

				// 每格交替底色，便于看出描边是否贴边
				var cellBg = (col + row) % 2 == 0 ? Palette.Steel1 : Palette.Steel3;
				sheet.FillRect(new Rect2I(
					pad + col * cellW, pad + row * cellH,
					Metrics.CardW * scale, Metrics.CardH * scale), cellBg);

				var frame = CardFramePainter.Build(styles[i], CardGrade.Standard);
				BlitScaled(sheet, frame,
					pad + col * cellW,
					pad + row * cellH,
					scale);
			}

			Save(sheet, $"{OutDir}/card_frames_detail.png");
		}

		/// <summary>1x 彩色 + 1x 灰度的校验图（形状暗示可辨性）。</summary>
		private void WriteGrayscaleSheet()
		{
			const int gutter = 8;
			const int pad = 8;

			var styles = AllStyles();
			int width = styles.Length * (Metrics.CardW + gutter) + pad * 2 - gutter;
			int height = pad * 2 + Metrics.CardH * 2 + gutter;

			var sheet = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
			sheet.Fill(Palette.Steel1);

			for (int i = 0; i < styles.Length; i++)
			{
				int x = pad + i * (Metrics.CardW + gutter);

				// 棋盘底色：模拟卡牌在钢甲板上的实际背景
				sheet.FillRect(new Rect2I(x, pad, Metrics.CardW, Metrics.CardH), Palette.Steel6);
				sheet.FillRect(new Rect2I(x, pad + Metrics.CardH + gutter,
					Metrics.CardW, Metrics.CardH), Palette.Steel6);

				var frame = CardFramePainter.Build(styles[i], CardGrade.Standard);
				Blit(sheet, frame, x, pad);

				var gray = ToGrayscale(frame);
				Blit(sheet, gray, x, pad + Metrics.CardH + gutter);
			}

			Save(sheet, $"{OutDir}/card_frames_grayscale.png");
		}

		private static CardFamilyStyle[] AllStyles()
		{
			var list = new System.Collections.Generic.List<CardFamilyStyle>(CardFamilies.All);
			list.Add(CardFamilies.Protagonist);
			return list.ToArray();
		}

		// ==================== 图像工具 ====================

		/// <summary>整图按整数倍放大贴入（最近邻，保持像素锐利）。</summary>
		private static void BlitScaled(Image dst, Image src, int x, int y, int scale)
		{
			var scaled = Image.CreateEmpty(
				src.GetWidth() * scale, src.GetHeight() * scale, false, Image.Format.Rgba8);
			for (int sy = 0; sy < src.GetHeight(); sy++)
			{
				for (int sx = 0; sx < src.GetWidth(); sx++)
				{
					var c = src.GetPixel(sx, sy);
					if (c.A <= 0f)
					{
						continue;
					}
					scaled.FillRect(new Rect2I(sx * scale, sy * scale, scale, scale), c);
				}
			}
			Blit(dst, scaled, x, y);
		}

		/// <summary>1:1 贴入，跳过全透明像素。</summary>
		private static void Blit(Image dst, Image src, int x, int y)
		{
			for (int sy = 0; sy < src.GetHeight(); sy++)
			{
				for (int sx = 0; sx < src.GetWidth(); sx++)
				{
					var c = src.GetPixel(sx, sy);
					if (c.A <= 0f)
					{
						continue;
					}
					int dx = x + sx;
					int dy = y + sy;
					if (dx >= 0 && dx < dst.GetWidth() && dy >= 0 && dy < dst.GetHeight())
					{
						dst.SetPixel(dx, dy, c);
					}
				}
			}
		}

		/// <summary>Rec. 709 灰度化，与设计文档"转灰度截图"的口径一致。</summary>
		private static Image ToGrayscale(Image src)
		{
			var outImg = Image.CreateEmpty(src.GetWidth(), src.GetHeight(), false, Image.Format.Rgba8);
			outImg.Fill(new Color(0, 0, 0, 0));
			for (int y = 0; y < src.GetHeight(); y++)
			{
				for (int x = 0; x < src.GetWidth(); x++)
				{
					var c = src.GetPixel(x, y);
					if (c.A <= 0f)
					{
						continue;
					}
					float l = Palette.Luminance(c);
					outImg.SetPixel(x, y, new Color(l, l, l, c.A));
				}
			}
			return outImg;
		}

		private static void Save(Image img, string path)
		{
			var err = img.SavePng(path);
			if (err != Error.Ok)
			{
				GD.PushError($"[CardFrameSheet] 写出 {path} 失败: {err}");
			}
		}
	}
}
