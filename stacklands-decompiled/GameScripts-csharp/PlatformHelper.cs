using System;
using System.IO;
using System.Linq;
using Steamworks;
using UnityEngine;

public static class PlatformHelper
{
	public static bool UseSteam => SteamManager.Initialized;

	public static bool HasModdingSupport
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)Application.platform == 32)
			{
				return false;
			}
			if (Application.isEditor && !DebugOptions.Default.ModdingSupportEnabled)
			{
				return false;
			}
			if ((from s in Environment.GetCommandLineArgs()
				select s.ToLower()).Contains("--no-mods"))
			{
				return false;
			}
			return true;
		}
	}

	public static bool IsTestBuild
	{
		get
		{
			string text = default(string);
			if (SteamManager.Initialized)
			{
				return SteamApps.GetCurrentBetaName(ref text, 100);
			}
			return false;
		}
	}

	public static string CurrentSavesDirectory
	{
		get
		{
			string text = default(string);
			if (SteamManager.Initialized && SteamApps.GetCurrentBetaName(ref text, 100))
			{
				string text2 = Path.Combine(Application.persistentDataPath, text + "_Saves");
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				return text2;
			}
			return Application.persistentDataPath;
		}
	}
}
