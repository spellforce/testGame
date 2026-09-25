using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsScreen : SokScreen
{
	public static ControlsScreen instance;

	public CustomButton BackButton;

	public TextMeshProUGUI WaitingForInputText;

	public RectTransform RebindElementsParent;

	public List<ExcludedControl> ExcludedControls = new List<ExcludedControl>();

	public RebindInfo RebindInfo;

	public bool IsRebinding => RebindInfo != null;

	private void Awake()
	{
		instance = this;
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		LoadRebinds();
	}

	private void OnEnable()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		List<Transform> list = new List<Transform>();
		foreach (Transform item2 in (Transform)RebindElementsParent)
		{
			Transform item = item2;
			list.Add(item);
		}
		foreach (Transform item3 in list)
		{
			Object.Destroy((Object)(object)((Component)item3).gameObject);
		}
		CreateRebindElements();
	}

	private void CreateRebindElements()
	{
		MakeLabel(SokLoc.Translate("label_keyboard_mouse"));
		CreateElementsForScheme("Keyboard&Mouse");
		MakeLabel(SokLoc.Translate("label_controller"));
		CreateElementsForScheme("Gamepad");
	}

	private void MakeLabel(string s)
	{
		RectTransform obj = Object.Instantiate<RectTransform>(PrefabManager.instance.NormalLabelPrefab);
		((Transform)(object)obj).SetParentClean((Transform)(object)RebindElementsParent);
		TextMeshProUGUI componentInChildren = ((Component)obj).GetComponentInChildren<TextMeshProUGUI>();
		((TMP_Text)componentInChildren).text = s;
		((TMP_Text)componentInChildren).fontSize = 28f;
	}

	private void CreateElementsForScheme(string scheme)
	{
		foreach (InputAction action in InputController.instance.PlayerInput.actions)
		{
			if (!(action.actionMap.name == "UI") && ActionSupportsScheme(action, scheme) && !ExcludedControls.Any((ExcludedControl x) => x.ActionName == action.name && x.Scheme == scheme))
			{
				RebindElement rebindElement = Object.Instantiate<RebindElement>(PrefabManager.instance.RebindElementPrefab);
				((Component)rebindElement).transform.SetParentClean((Transform)(object)RebindElementsParent);
				rebindElement.MyAction = action.name;
				rebindElement.Scheme = scheme;
			}
		}
	}

	private bool ActionSupportsScheme(InputAction action, string scheme)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < action.bindings.Count; i++)
		{
			InputBinding val = action.bindings[i];
			if (((InputBinding)(ref val)).isComposite)
			{
				InputBinding val2 = action.bindings[i + 1];
				if (((InputBinding)(ref val2)).groups.Contains(scheme))
				{
					return true;
				}
			}
			else if (((InputBinding)(ref val)).groups.Contains(scheme))
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		if (RebindInfo != null)
		{
			InputBinding val = RebindInfo.Action.bindings[RebindInfo.BindingIndex];
			if (((InputBinding)(ref val)).isPartOfComposite)
			{
				LocParam[] array = new LocParam[1];
				val = RebindInfo.Action.bindings[RebindInfo.BindingIndex];
				array[0] = LocParam.Create("control", ((InputBinding)(ref val)).name);
				text = SokLoc.Translate("label_binding", (LocParam[])(object)array);
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			((TMP_Text)WaitingForInputText).text = SokLoc.Translate("label_waiting_for_input");
		}
		else
		{
			((TMP_Text)WaitingForInputText).text = text + "\n" + SokLoc.Translate("label_waiting_for_input");
		}
		((Component)((TMP_Text)WaitingForInputText).transform.parent).gameObject.SetActive(IsRebinding);
	}

	public void SaveRebinds()
	{
		string text = InputActionRebindingExtensions.SaveBindingOverridesAsJson((IInputActionCollection2)(object)InputController.instance.PlayerInput.actions);
		PlayerPrefs.SetString("rebinds", text);
	}

	private void LoadRebinds()
	{
		string text = PlayerPrefs.GetString("rebinds");
		InputActionRebindingExtensions.LoadBindingOverridesFromJson((IInputActionCollection2)(object)InputController.instance.PlayerInput.actions, text, true);
	}
}
