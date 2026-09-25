using System.IO;
using ImGuiNET;
using UImGui;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SchemaUI : MonoBehaviour
{
	private bool Show;

	private string FolderInput = "";

	private void Start()
	{
		UImGuiUtility.Layout += DrawSchemaUI;
	}

	private void Update()
	{
		if (((ButtonControl)Keyboard.current[(Key)99]).wasPressedThisFrame)
		{
			Show = !Show;
		}
	}

	private void DrawSchemaUI(UImGui _)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (Show)
		{
			ImGui.SetNextWindowSize(new Vector2(700f, 100f), (ImGuiCond)2);
			ImGui.Begin("JSON Schema Generator");
			ImGui.InputTextWithHint("Schema Folder", "C:/Users/cyber/Documents/stacklands-schemas", ref FolderInput, 500u);
			if (ImGui.Button("Generate card.schema.json"))
			{
				SchemaGenerator.GenerateCardSchema(Path.Combine(FolderInput, "card.schema.json"));
			}
			ImGui.SameLine();
			if (ImGui.Button("Generate blueprint.schema.json"))
			{
				SchemaGenerator.GenerateBlueprintSchema(Path.Combine(FolderInput, "blueprint.schema.json"));
			}
			ImGui.SameLine();
			if (ImGui.Button("Generate boosterpack.schema.json"))
			{
				SchemaGenerator.GenerateBoosterSchema(Path.Combine(FolderInput, "boosterpack.schema.json"));
			}
			if (ImGui.Button("Generate ALL schemas"))
			{
				SchemaGenerator.GenerateSchemas(FolderInput);
			}
			ImGui.End();
		}
	}
}
