using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Screenshotter : MonoBehaviour
{
	public static Screenshotter instance;

	[HideInInspector]
	public List<ScreenshotDescription> Descriptions = new List<ScreenshotDescription>();

	public bool IsScreenshotting;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		List<ScreenshotDescription> list = new List<ScreenshotDescription>();
		list.Add(new ScreenshotDescription(1920, 1080)
		{
			Description = "schinese",
			Language = "Chinese (Simplified)"
		});
		list.Add(new ScreenshotDescription(1920, 1080)
		{
			Description = "tchinese",
			Language = "Chinese (Traditional)"
		});
		list.Add(new ScreenshotDescription(1920, 1080)
		{
			Description = "koreana",
			Language = "Korean"
		});
		list.Add(new ScreenshotDescription(1920, 1080)
		{
			Description = "english",
			Language = "English"
		});
		Descriptions.AddRange(list);
	}

	private void LateUpdate()
	{
		if (InputController.instance.GetKeyDown((Key)100))
		{
			((MonoBehaviour)this).StartCoroutine(TakeAllScreenshots());
		}
	}

	private IEnumerator TakeAllScreenshots()
	{
		DateTime curTime = DateTime.Now;
		foreach (ScreenshotDescription description in Descriptions)
		{
			if (description.IncludeInScreenshots)
			{
				description.TakenAt = curTime;
				yield return TakeScreenshot(description);
			}
		}
	}

	public IEnumerator TakeScreenshot(ScreenshotDescription sd)
	{
		IsScreenshotting = true;
		GameCanvas.instance.Canvas.renderMode = (RenderMode)1;
		GameCanvas.instance.Canvas.worldCamera = GameCamera.instance.MyCam;
		GameCanvas.instance.Canvas.sortingOrder = 2;
		GameCanvas.instance.Canvas.sortingLayerName = "Above";
		if (sd.ShowUI)
		{
			GameCanvas.instance.SetUIToggle(enabled: true);
		}
		else
		{
			GameCanvas.instance.SetUIToggle(enabled: false);
		}
		string originalLanguage = SokLoc.instance.CurrentLanguage;
		SokLoc.instance.SetLanguage(sd.Language);
		if (sd.ControlSchemeOverride.HasValue)
		{
			InputController.instance.SchemeOverride = sd.ControlSchemeOverride;
		}
		for (int i = 0; i < 5; i++)
		{
			Canvas.ForceUpdateCanvases();
		}
		if ((Object)(object)GameCanvas.instance != (Object)null)
		{
			Transform transform = ((Component)GameCanvas.instance).transform;
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)(object)((transform is RectTransform) ? transform : null));
		}
		yield return null;
		yield return (object)new WaitForEndOfFrame();
		MakeScreenshot(sd, out var success, out var targetPath);
		GameCanvas.instance.SetUIToggle(enabled: true);
		Canvas.ForceUpdateCanvases();
		SokLoc.instance.SetLanguage(originalLanguage);
		GameCanvas.instance.Canvas.renderMode = (RenderMode)0;
		InputController.instance.SchemeOverride = null;
		if (success)
		{
			Debug.Log((object)("Screenshot saved to " + targetPath));
		}
		IsScreenshotting = false;
	}

	public static void MakeScreenshot(ScreenshotDescription sd, out bool success, out string targetPath)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		RenderTextureDescriptor val = default(RenderTextureDescriptor);
		((RenderTextureDescriptor)(ref val))._002Ector(sd.Width, sd.Height, (RenderTextureFormat)0, 32);
		((RenderTextureDescriptor)(ref val)).sRGB = true;
		((RenderTextureDescriptor)(ref val)).stencilFormat = (GraphicsFormat)13;
		RenderTexture temporary = RenderTexture.GetTemporary(val);
		temporary.antiAliasing = 8;
		Camera.main.targetTexture = temporary;
		Camera.main.Render();
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		Texture2D val2 = new Texture2D(((Texture)temporary).width, ((Texture)temporary).height, (TextureFormat)(sd.AlphaBackground ? 5 : 3), false, true);
		val2.ReadPixels(new Rect(0f, 0f, (float)((Texture)temporary).width, (float)((Texture)temporary).height), 0, 0);
		val2.Apply();
		WriteTexture(val2, sd, out success, out targetPath);
		Camera.main.targetTexture = null;
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
	}

	private static void WriteTexture(Texture2D tex, ScreenshotDescription desc, out bool success, out string targetPath)
	{
		string text = Path.Combine(Application.persistentDataPath, "screenshots");
		Directory.CreateDirectory(text);
		success = true;
		string path = $"{desc.TakenAt.ToFileTimeUtc()} {desc.Description}.png";
		targetPath = Path.Combine(text, path);
		try
		{
			File.WriteAllBytes(targetPath, ImageConversion.EncodeToPNG(tex));
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"Saving screenshot failed\n{arg}");
			success = false;
		}
	}
}
