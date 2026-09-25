using System;
using UnityEngine;

public class ModLogger
{
	public ModManifest Manifest;

	public ModLogger(ModManifest manifest)
	{
		Manifest = manifest;
	}

	public void Log(LogType logType, string message)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Debug.unityLogger.Log(logType, (object)$"[{FormatTime(DateTime.Now)}] [{logType} : {Manifest.Id}] {message}");
	}

	public void Log(string message)
	{
		Log((LogType)3, message);
	}

	public void LogWarning(string message)
	{
		Log((LogType)2, message);
	}

	public void LogError(string message)
	{
		Log((LogType)0, message);
	}

	public void LogException(string message)
	{
		Log((LogType)4, message);
	}

	public static string FormatTime(DateTime dt)
	{
		return dt.ToString("HH:mm:ss");
	}
}
