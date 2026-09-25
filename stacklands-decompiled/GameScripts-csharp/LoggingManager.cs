using System;
using System.Collections.Generic;
using ImGuiNET;
using UImGui;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class LoggingManager : MonoBehaviour
{
	public static List<(int, string)> Logs = new List<(int, string)>();

	public static LoggingManager instance;

	public bool LogViewerEnabled;

	private void Awake()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		instance = this;
		Application.logMessageReceived += new LogCallback(HandleLog);
		UImGuiUtility.Layout += DrawLogViewer;
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected I4, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		if (!logString.StartsWith("["))
		{
			logString = string.Format("[{0}] [{1} : Stacklands] {2}{3}", DateTime.Now.ToString("HH:mm:ss"), type, logString, ((int)type == 4) ? ("\n" + stackTrace) : "");
		}
		Logs.Add(((int)type, logString));
	}

	private void Update()
	{
		if (((ButtonControl)Keyboard.current[(Key)96]).wasPressedThisFrame)
		{
			LogViewerEnabled = !LogViewerEnabled;
		}
	}

	private void DrawLogViewer(UImGui _)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if (!LogViewerEnabled)
		{
			return;
		}
		ImGui.SetNextWindowSize(new Vector2(520f, 600f), (ImGuiCond)4);
		ImGui.Begin("Log");
		if (ImGui.Button("Open log file"))
		{
			Application.OpenURL("file:///" + Application.consoleLogPath);
		}
		ImGui.SameLine();
		if (ImGui.Button("Clear log"))
		{
			Logs.Clear();
		}
		ImGui.Separator();
		ImGuiStylePtr style = ImGui.GetStyle();
		if (ImGui.BeginChild("logsection", new Vector2(0f, 0f - (((ImGuiStylePtr)(ref style)).ItemSpacing.y + ImGui.GetFrameHeightWithSpacing())), false, (ImGuiWindowFlags)2048))
		{
			ImGui.PushStyleVar((ImGuiStyleVar)14, new Vector2(4f, 1f));
			foreach (var log in Logs)
			{
				int item = log.Item1;
				string item2 = log.Item2;
				bool flag = false;
				Vector4 zero = Vector4.zero;
				if (item == 2)
				{
					flag = true;
					((Vector4)(ref zero))._002Ector(1f, 1f, 0f, 1f);
				}
				if (item == 0 || item == 4)
				{
					flag = true;
					((Vector4)(ref zero))._002Ector(1f, 0f, 0f, 1f);
				}
				if (flag)
				{
					ImGui.PushStyleColor((ImGuiCol)0, zero);
				}
				ImGui.TextUnformatted(item2);
				if (flag)
				{
					ImGui.PopStyleColor();
				}
			}
			ImGui.PopStyleVar();
		}
		ImGui.EndChild();
		ImGui.End();
	}
}
