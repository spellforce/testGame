using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class RebindElement : MonoBehaviour
{
	public string MyAction;

	public string Scheme;

	public TextMeshProUGUI ActionName;

	public CustomButton ResetButton;

	public CustomButton SetBindingButton;

	private void Start()
	{
		if (!ResolveActionAndBinding(out var action, out var bindingIndex))
		{
			return;
		}
		SetBindingButton.Clicked += delegate
		{
			if (!ControlsScreen.instance.IsRebinding)
			{
				StartRebind();
			}
		};
		ResetButton.Clicked += delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			InputBinding val = action.bindings[bindingIndex];
			if (((InputBinding)(ref val)).isComposite)
			{
				for (int i = bindingIndex + 1; i < action.bindings.Count; i++)
				{
					val = action.bindings[i];
					if (!((InputBinding)(ref val)).isPartOfComposite)
					{
						break;
					}
					InputActionRebindingExtensions.RemoveBindingOverride(action, i);
				}
			}
			else
			{
				InputActionRebindingExtensions.RemoveBindingOverride(action, bindingIndex);
			}
			ControlsScreen.instance.SaveRebinds();
		};
	}

	public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		bindingIndex = -1;
		action = InputController.instance.PlayerInput.actions[MyAction];
		for (int i = 0; i < action.bindings.Count; i++)
		{
			InputBinding val = action.bindings[i];
			if (((InputBinding)(ref val)).isComposite)
			{
				InputBinding val2 = action.bindings[i + 1];
				if (((InputBinding)(ref val2)).groups.Contains(Scheme))
				{
					bindingIndex = i;
					return true;
				}
			}
			else if (((InputBinding)(ref val)).groups.Contains(Scheme))
			{
				bindingIndex = i;
				return true;
			}
		}
		Debug.LogError((object)("No action found for " + MyAction + " in " + Scheme));
		return false;
	}

	private void StartRebind()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveActionAndBinding(out var action, out var bindingIndex))
		{
			return;
		}
		InputBinding val = action.bindings[bindingIndex];
		if (((InputBinding)(ref val)).isComposite)
		{
			int num = bindingIndex + 1;
			if (num < action.bindings.Count)
			{
				val = action.bindings[num];
				if (((InputBinding)(ref val)).isPartOfComposite)
				{
					Rebind(action, num, isComposite: true);
				}
			}
		}
		else
		{
			Rebind(action, bindingIndex);
		}
	}

	private void Rebind(InputAction action, int bindingIndex, bool isComposite = false)
	{
		action.Disable();
		RebindingOperation rebindOperation = InputActionRebindingExtensions.PerformInteractiveRebinding(action, bindingIndex).WithCancelingThrough("<Keyboard>/escape").OnMatchWaitForAnother(0.1f)
			.WithControlsExcluding("<Mouse>/leftButton");
		rebindOperation.OnComplete((Action<RebindingOperation>)delegate
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			if (CheckDuplicateBinding(action, bindingIndex))
			{
				InputActionRebindingExtensions.RemoveBindingOverride(action, bindingIndex);
			}
			RebindingOperation obj = rebindOperation;
			if (obj != null)
			{
				obj.Dispose();
			}
			rebindOperation = null;
			action.Enable();
			ControlsScreen.instance.RebindInfo = null;
			ControlsScreen.instance.SaveRebinds();
			if (isComposite)
			{
				int num = bindingIndex + 1;
				if (num < action.bindings.Count)
				{
					InputBinding val = action.bindings[num];
					if (((InputBinding)(ref val)).isPartOfComposite)
					{
						Rebind(action, num, isComposite: true);
					}
				}
			}
			InputController.instance.ClearBindingDisplayCache();
		});
		rebindOperation.OnCancel((Action<RebindingOperation>)delegate
		{
			RebindingOperation obj = rebindOperation;
			if (obj != null)
			{
				obj.Dispose();
			}
			rebindOperation = null;
			ControlsScreen.instance.RebindInfo = null;
			action.Enable();
		});
		ControlsScreen.instance.RebindInfo = new RebindInfo
		{
			Action = action,
			BindingIndex = bindingIndex
		};
		rebindOperation.Start();
	}

	private bool CheckDuplicateBinding(InputAction action, int bindingIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		InputBinding val = action.bindings[bindingIndex];
		Enumerator<InputBinding> enumerator = action.actionMap.bindings.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				InputBinding current = enumerator.Current;
				if (!(((InputBinding)(ref current)).action == ((InputBinding)(ref val)).action) && ((InputBinding)(ref current)).effectivePath == ((InputBinding)(ref val)).effectivePath)
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (((InputBinding)(ref val)).isPartOfComposite)
		{
			for (int i = 1; i < bindingIndex; i++)
			{
				InputBinding val2 = action.bindings[i];
				if (((InputBinding)(ref val2)).overridePath == ((InputBinding)(ref val)).overridePath)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void Update()
	{
		if (ResolveActionAndBinding(out var action, out var bindingIndex))
		{
			((TMP_Text)ActionName).text = SokLoc.Translate("control_" + action.name);
			((TMP_Text)SetBindingButton.TextMeshPro).text = InputActionRebindingExtensions.GetBindingDisplayString(action, bindingIndex, (DisplayStringOptions)0);
		}
	}
}
