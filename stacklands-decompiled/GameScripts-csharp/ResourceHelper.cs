using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class ResourceHelper
{
	public static Sprite LoadSpriteFromPath(string path)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(0, 0, (TextureFormat)4, false);
		ImageConversion.LoadImage(val, File.ReadAllBytes(path));
		return Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), Vector2.one / 2f);
	}

	public static IEnumerator LoadAudioClipFromPath(string path, Action<AudioClip> callback, Action onError = null)
	{
		return LoadAudioClipFromPath(path, (AudioType)20, callback, onError);
	}

	public static IEnumerator LoadAudioClipFromPath(string path, AudioType type, Action<AudioClip> callback, Action onError = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, type);
		try
		{
			www.timeout = 3;
			yield return www.SendWebRequest();
			if ((int)www.result == 2)
			{
				Debug.LogWarning((object)("Error while loading audio from " + path + ": " + www.error));
				onError?.Invoke();
			}
			else
			{
				callback(DownloadHandlerAudioClip.GetContent(www));
			}
		}
		finally
		{
			((IDisposable)www)?.Dispose();
		}
	}
}
