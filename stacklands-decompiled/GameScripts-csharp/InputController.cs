using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Users;

public class InputController : MonoBehaviour
{
	private class UserInput
	{
		public Vector2 ScreenPosition;

		public Vector2 DeltaPosition = Vector2.zero;

		public Vector2 StartPosition;

		public float StartTime;

		public int MouseId = -1;

		public int TouchId = -1;

		public int PenId = -1;

		public bool JustStarted = true;

		public bool JustEnded;

		public bool UpdatedThisFrame;

		public override string ToString()
		{
			return MouseId + " " + TouchId + ((object)Unsafe.As<Vector2, Vector2>(ref ScreenPosition)/*cast due to .constrained prefix*/).ToString() + " " + ((object)Unsafe.As<Vector2, Vector2>(ref DeltaPosition)/*cast due to .constrained prefix*/).ToString() + " " + JustStarted;
		}
	}

	public static InputController instance;

	public PlayerInput PlayerInput;

	public bool DisableAllInput;

	private string inputString;

	public string ActiveScheme;

	private List<UserInput> Inputs = new List<UserInput>();

	private List<UserInput> inputsToRemove = new List<UserInput>();

	public ControllerVibrator Vibrator;

	private InputAction cancel;

	private InputAction submit;

	private InputAction time_pause;

	private InputAction pause;

	private InputAction move;

	private InputAction snap_cards;

	private InputAction time_1;

	private InputAction time_2;

	private InputAction time_3;

	private InputAction zoom;

	private InputAction panel_collapse;

	private InputAction activate_ui;

	private InputAction time_toggle;

	private InputAction sell;

	private InputAction toggle_inventory;

	private InputAction toggle_view;

	private InputAction grab;

	private InputAction snap_move;

	private bool mouseIsDragging;

	private Vector2 lastMove;

	private float lastGrab;

	private Vector2 lastSnapMove;

	private ControlScheme lastControlScheme;

	public int LastInputCount;

	private Dictionary<string, string> bindingDisplayCache = new Dictionary<string, string>();

	public ControlScheme? SchemeOverride;

	private ControlScheme? _currentScheme;

	private InputDevice lastDevice;

	public string InputString => inputString;

	public bool IsUsingMouse
	{
		get
		{
			if (CurrentSchemeIsController || CurrentSchemeIsTouch)
			{
				return false;
			}
			return Mouse.current != null;
		}
	}

	public bool MouseIsDragging => mouseIsDragging;

	private float dragDistanceThreshold => (float)Screen.height * 0.025f;

	private float dragTimeThreshold => 0.4f;

	private bool TouchesEnabled => true;

	public int InputCount => Inputs.Count;

	public ControlScheme CurrentScheme
	{
		get
		{
			if (SchemeOverride.HasValue)
			{
				return SchemeOverride.Value;
			}
			if (!_currentScheme.HasValue)
			{
				_currentScheme = GetSchemeFromName(PlayerInput.currentControlScheme);
			}
			return _currentScheme.Value;
		}
	}

	public bool CurrentSchemeIsController => CurrentScheme == ControlScheme.Controller;

	public bool CurrentSchemeIsMouseKeyboard => CurrentScheme == ControlScheme.KeyboardMouse;

	public bool CurrentSchemeIsTouch => CurrentScheme == ControlScheme.Touch;

	public event Action<ControlScheme> ControlSchemeChanged;

	private void Awake()
	{
		instance = this;
		SetupInputActions();
		EnhancedTouchSupport.Enable();
		Vibrator = new ControllerVibrator();
		InputSystem.pollingFrequency = 120f;
		InputUser.onChange += InputUser_onChange;
		InputSystem.onActionChange += delegate(object obj, InputActionChange change)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if ((int)change == 5)
			{
				lastDevice = ((InputAction)obj).activeControl.device;
			}
		};
		if (Keyboard.current != null)
		{
			Keyboard.current.onTextInput += OnTextInput;
		}
	}

	private void OnTextInput(char c)
	{
		inputString += c;
	}

	private void SetupInputActions()
	{
		cancel = PlayerInput.actions["cancel"];
		submit = PlayerInput.actions["submit"];
		time_pause = PlayerInput.actions["time_pause"];
		pause = PlayerInput.actions["pause"];
		move = PlayerInput.actions["move"];
		snap_cards = PlayerInput.actions["snap_cards"];
		time_1 = PlayerInput.actions["time_1"];
		time_2 = PlayerInput.actions["time_2"];
		time_3 = PlayerInput.actions["time_3"];
		zoom = PlayerInput.actions["zoom"];
		panel_collapse = PlayerInput.actions["panel_collapse"];
		activate_ui = PlayerInput.actions["activate_ui"];
		time_toggle = PlayerInput.actions["time_toggle"];
		sell = PlayerInput.actions["sell"];
		toggle_inventory = PlayerInput.actions["toggle_inventory"];
		toggle_view = PlayerInput.actions["toggle_view"];
		grab = PlayerInput.actions["grab"];
		snap_move = PlayerInput.actions["snap_move"];
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			if (Keyboard.current != null)
			{
				InputSystem.ResetDevice((InputDevice)(object)Keyboard.current, false);
			}
			ClearInputs();
			if (Vibrator != null)
			{
				Vibrator.StopVibrate();
			}
		}
	}

	private void OnDestroy()
	{
		if (Vibrator != null)
		{
			Vibrator.StopVibrate();
		}
		InputUser.onChange -= InputUser_onChange;
	}

	public void ClearInputs()
	{
		Inputs.Clear();
	}

	public void LogCurrentState()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		string text = "Input controller state log\n";
		foreach (UserInput input in Inputs)
		{
			text = text + input.ToString() + "\n";
		}
		text += $"Active touches report: {Touch.activeTouches.Count} touches!";
		Debug.Log((object)text);
	}

	private ButtonControl GetMouseButton(int buttonId)
	{
		return (ButtonControl)(buttonId switch
		{
			0 => Mouse.current.leftButton, 
			1 => Mouse.current.rightButton, 
			_ => Mouse.current.middleButton, 
		});
	}

	public Vector2 ClampedMousePosition()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.current == null)
		{
			return new Vector2((float)Screen.width, (float)Screen.height) * 0.5f;
		}
		Vector2 val = ((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue();
		val.x = Mathf.Clamp(val.x, 0f, (float)Screen.width);
		val.y = Mathf.Clamp(val.y, 0f, (float)Screen.height);
		return val;
	}

	private bool MousePositionIsInScreen()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = ((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue();
		if (val.x > 0f && val.x < (float)Screen.width && val.y > 0f)
		{
			return val.y < (float)Screen.height;
		}
		return false;
	}

	public Vector2 GetSafeTouchPosition(int i)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!GetInput(i))
		{
			return new Vector2((float)Screen.width, (float)Screen.height) * 0.5f;
		}
		return GetInputPosition(i);
	}

	private void Update()
	{
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Invalid comparison between Unknown and I4
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Invalid comparison between Unknown and I4
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Invalid comparison between Unknown and I4
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		InputSystem.Update();
		Vibrator.UpdateVibrate(Time.unscaledDeltaTime);
		foreach (UserInput item in inputsToRemove)
		{
			Inputs.Remove(item);
		}
		inputsToRemove.Clear();
		foreach (UserInput input in Inputs)
		{
			input.JustStarted = false;
			input.UpdatedThisFrame = false;
		}
		if (Mouse.current != null)
		{
			int num = 0;
			if (AccessibilityScreen.ClickToDragEnabled)
			{
				num = 1;
			}
			int mouseId;
			for (mouseId = 0; mouseId <= num; mouseId++)
			{
				ButtonControl mouseButton = GetMouseButton(mouseId);
				if (mouseButton.wasPressedThisFrame)
				{
					if (MousePositionIsInScreen() && Inputs.Count <= 0)
					{
						mouseIsDragging = false;
						Inputs.Add(new UserInput
						{
							ScreenPosition = ClampedMousePosition(),
							MouseId = mouseId,
							JustStarted = true,
							StartPosition = ClampedMousePosition(),
							StartTime = Time.time,
							UpdatedThisFrame = true
						});
					}
					continue;
				}
				if (mouseButton.isPressed)
				{
					UserInput userInput = Inputs.FirstOrDefault((UserInput x) => x.MouseId == mouseId);
					if (userInput == null)
					{
						continue;
					}
					userInput.DeltaPosition = ClampedMousePosition() - userInput.ScreenPosition;
					userInput.ScreenPosition = ClampedMousePosition();
					userInput.UpdatedThisFrame = true;
					if (!mouseIsDragging)
					{
						Vector2 val = userInput.StartPosition - userInput.ScreenPosition;
						if (((Vector2)(ref val)).magnitude > dragDistanceThreshold || Time.time - userInput.StartTime >= dragTimeThreshold)
						{
							mouseIsDragging = true;
						}
					}
					continue;
				}
				for (int num2 = 0; num2 < Inputs.Count; num2++)
				{
					if (Inputs[num2].MouseId == mouseId)
					{
						Inputs[num2].JustEnded = true;
						Inputs[num2].UpdatedThisFrame = true;
						inputsToRemove.Add(Inputs[num2]);
					}
				}
			}
		}
		else
		{
			mouseIsDragging = false;
		}
		if (Inputs.Count == 0)
		{
			mouseIsDragging = false;
		}
		if (TouchesEnabled)
		{
			for (int num3 = 0; num3 < Touch.activeTouches.Count; num3++)
			{
				Touch touch = Touch.activeTouches[num3];
				if (!((Touch)(ref touch)).valid)
				{
					Debug.Log((object)"Invalid touch in active touches!");
					continue;
				}
				if ((int)((Touch)(ref touch)).phase == 1)
				{
					Inputs.Add(new UserInput
					{
						ScreenPosition = ((Touch)(ref touch)).screenPosition,
						TouchId = ((Touch)(ref touch)).touchId,
						JustStarted = true,
						StartPosition = ((Touch)(ref touch)).screenPosition,
						StartTime = Time.time,
						UpdatedThisFrame = true
					});
					continue;
				}
				if ((int)((Touch)(ref touch)).phase == 2 || (int)((Touch)(ref touch)).phase == 5)
				{
					UserInput userInput2 = Inputs.FirstOrDefault((UserInput x) => x.TouchId == ((Touch)(ref touch)).touchId);
					if (userInput2 != null)
					{
						userInput2.DeltaPosition = ((Touch)(ref touch)).delta;
						userInput2.ScreenPosition = ((Touch)(ref touch)).screenPosition;
						userInput2.UpdatedThisFrame = true;
					}
					continue;
				}
				for (int num4 = 0; num4 < Inputs.Count; num4++)
				{
					if (Inputs[num4].TouchId == ((Touch)(ref touch)).touchId)
					{
						Inputs[num4].JustEnded = true;
						Inputs[num4].UpdatedThisFrame = true;
						inputsToRemove.Add(Inputs[num4]);
					}
				}
			}
		}
		UpdatePen();
		for (int num5 = Inputs.Count - 1; num5 >= 0; num5--)
		{
			if (!Inputs[num5].UpdatedThisFrame)
			{
				Debug.Log((object)"Removed a non-updated input!");
				Inputs.RemoveAt(num5);
			}
		}
		Cursor.visible = !CurrentSchemeIsController;
		if (lastControlScheme != CurrentScheme)
		{
			ClearBindingDisplayCache();
		}
		ActiveScheme = PlayerInput.currentControlScheme;
		lastControlScheme = CurrentScheme;
	}

	private void UpdatePen()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (Pen.current == null)
		{
			return;
		}
		ButtonControl tip = Pen.current.tip;
		Vector2Control position = ((Pointer)Pen.current).position;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((InputControl<float>)(object)position.x).ReadValue(), ((InputControl<float>)(object)position.y).ReadValue());
		if (tip.wasPressedThisFrame)
		{
			Inputs.Add(new UserInput
			{
				ScreenPosition = val,
				PenId = ((InputDevice)Pen.current).deviceId,
				JustStarted = true,
				StartPosition = val,
				StartTime = Time.time,
				UpdatedThisFrame = true
			});
			return;
		}
		if (tip.isPressed)
		{
			UserInput userInput = Inputs.FirstOrDefault((UserInput x) => x.PenId == ((InputDevice)Pen.current).deviceId);
			if (userInput != null)
			{
				userInput.DeltaPosition = val - userInput.ScreenPosition;
				userInput.ScreenPosition = val;
				userInput.UpdatedThisFrame = true;
			}
			return;
		}
		for (int num = 0; num < Inputs.Count; num++)
		{
			if (Inputs[num].PenId == ((InputDevice)Pen.current).deviceId)
			{
				Inputs[num].JustEnded = true;
				Inputs[num].UpdatedThisFrame = true;
				inputsToRemove.Add(Inputs[num]);
			}
		}
	}

	private void LateUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		lastGrab = GetGrab();
		lastMove = GetMove();
		lastSnapMove = GetSnapMove();
		inputString = "";
		LastInputCount = InputCount;
		_currentScheme = null;
	}

	public Vector2 AllDeltaPos()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (DisableAllInput)
		{
			return Vector2.zero;
		}
		if (InputCount == 0)
		{
			return Vector2.zero;
		}
		Vector2 val = Vector2.zero;
		for (int i = 0; i < InputCount; i++)
		{
			val += GetDeltaPosition(i);
		}
		return val / (float)InputCount;
	}

	public bool GetInputBegan(int i)
	{
		if (GetInput(i))
		{
			return Inputs[i].JustStarted;
		}
		return false;
	}

	public bool GetRightMouseBegan()
	{
		if (GetInput(0) && Inputs[0].JustStarted)
		{
			return Inputs[0].MouseId == 1;
		}
		return false;
	}

	public bool GetLeftMouseBegan()
	{
		if (GetInput(0) && Inputs[0].JustStarted)
		{
			return Inputs[0].MouseId == 0;
		}
		return false;
	}

	public bool GetRightMouseEnded()
	{
		if (GetInput(0) && Inputs[0].JustEnded)
		{
			return Inputs[0].MouseId == 1;
		}
		return false;
	}

	public bool GetLeftMouseEnded()
	{
		if (GetInput(0) && Inputs[0].JustEnded)
		{
			return Inputs[0].MouseId == 0;
		}
		return false;
	}

	public bool GetInputTapped(int i)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		UserInput userInput = Inputs[i];
		if (userInput.JustEnded && Time.time - userInput.StartTime <= 0.15f)
		{
			Vector2 val = userInput.ScreenPosition - userInput.StartPosition;
			return ((Vector2)(ref val)).magnitude < (float)Screen.width * 0.05f;
		}
		return false;
	}

	public bool IsNotRightClick(int i)
	{
		return Inputs[i].MouseId != 1;
	}

	public bool GetInputEnded(int i)
	{
		if (GetInput(i))
		{
			return Inputs[i].JustEnded;
		}
		return false;
	}

	public bool GetInput(int i)
	{
		return Inputs.Count > i;
	}

	public bool GetInputMoving(int i)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (Inputs.Count > i)
		{
			return Inputs[i].DeltaPosition != Vector2.zero;
		}
		return false;
	}

	public Vector2 GetDeltaPosition(int i)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Inputs[i].DeltaPosition;
	}

	public Vector2 GetDeltaPositionSinceStart(int i)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return Inputs[i].ScreenPosition - Inputs[i].StartPosition;
	}

	public Vector2 GetInputPosition(int i)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Inputs[i].ScreenPosition;
	}

	public Vector2 GetStartPosition(int i)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Inputs[i].StartPosition;
	}

	public bool GetStickHorizontal()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!((double)move.ReadValue<Vector2>().x > 0.3))
		{
			return (double)move.ReadValue<Vector2>().x < -0.3;
		}
		return true;
	}

	public bool CancelTriggered()
	{
		return cancel.triggered;
	}

	public bool SubmitTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return submit.triggered;
	}

	public bool TimePauseTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_pause.triggered;
	}

	public bool PauseTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return pause.triggered;
	}

	public bool SnapCardsTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return snap_cards.triggered;
	}

	public bool Time1_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_1.triggered;
	}

	public bool Time2_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_2.triggered;
	}

	public bool Time3_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_3.triggered;
	}

	public float GetZoom()
	{
		if (DisableAllInput)
		{
			return 0f;
		}
		return zoom.ReadValue<float>();
	}

	public bool PanelCollapse_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return panel_collapse.triggered;
	}

	public bool ActivateUI_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return activate_ui.triggered;
	}

	public bool TimeToggleTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_toggle.triggered;
	}

	public bool SellTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return sell.triggered;
	}

	public bool ToggleInventoryTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return toggle_inventory.triggered;
	}

	public bool ToggleViewTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return toggle_view.triggered;
	}

	public Vector2 GetMove()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (DisableAllInput)
		{
			return Vector2.zero;
		}
		return move.ReadValue<Vector2>();
	}

	public Vector2 GetSnapMovePressed()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Vector2 snapMove = GetSnapMove();
		if (((Vector2)(ref snapMove)).magnitude == 0f)
		{
			return Vector2.zero;
		}
		Vector2 val = snapMove - lastSnapMove;
		return ((Vector2)(ref val)).normalized;
	}

	public Vector2 GetSnapMove()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (DisableAllInput)
		{
			return Vector2.zero;
		}
		return snap_move.ReadValue<Vector2>();
	}

	public float GetGrab()
	{
		if (DisableAllInput)
		{
			return 0f;
		}
		return grab.ReadValue<float>();
	}

	public Vector2 GetDeltaMove()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (DisableAllInput)
		{
			return Vector2.zero;
		}
		return GetMove() - lastMove;
	}

	public bool StartedGrabbing()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return grab.triggered;
	}

	public bool StoppedGrabbing()
	{
		if (DisableAllInput)
		{
			return false;
		}
		if (GetGrab() < 0.5f)
		{
			return lastGrab > 0.5f;
		}
		return false;
	}

	public string GetActionDisplayString(string name)
	{
		if (!bindingDisplayCache.ContainsKey(name))
		{
			bindingDisplayCache[name] = "[" + InputActionRebindingExtensions.GetBindingDisplayString(PlayerInput.actions[name], (DisplayStringOptions)0, (string)null) + "]";
		}
		return bindingDisplayCache[name];
	}

	public void ClearBindingDisplayCache()
	{
		bindingDisplayCache.Clear();
	}

	public bool GetKeyDown(Key key)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (Keyboard.current == null)
		{
			return false;
		}
		return ((ButtonControl)Keyboard.current[key]).wasPressedThisFrame;
	}

	public bool GetKey(Key key)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (Keyboard.current == null)
		{
			return false;
		}
		return ((ButtonControl)Keyboard.current[key]).isPressed;
	}

	public bool AnyInputDone()
	{
		if (InputCount > 0 && GetInputTapped(0))
		{
			return true;
		}
		if (Keyboard.current != null && ((ButtonControl)Keyboard.current.anyKey).wasPressedThisFrame)
		{
			return true;
		}
		if (SubmitTriggered())
		{
			return true;
		}
		return false;
	}

	private ControlScheme GetSchemeFromName(string scheme)
	{
		if (scheme == "Keyboard&Mouse")
		{
			return ControlScheme.KeyboardMouse;
		}
		if (scheme == "Gamepad")
		{
			return ControlScheme.Controller;
		}
		return ControlScheme.Touch;
	}

	private void InputUser_onChange(InputUser user, InputUserChange change, InputDevice device)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if ((int)change == 11)
		{
			InputControlScheme value = ((InputUser)(ref user)).controlScheme.Value;
			ControlScheme schemeFromName = GetSchemeFromName(((InputControlScheme)(ref value)).name);
			this.ControlSchemeChanged?.Invoke(schemeFromName);
		}
	}

	public void LogDevices()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < InputSystem.devices.Count; i++)
		{
			InputDevice val = InputSystem.devices[i];
			string[] obj = new string[10]
			{
				$"Device {i}\n",
				"Display name: ",
				((InputControl)val).displayName,
				"\nInterface name: ",
				null,
				null,
				null,
				null,
				null,
				null
			};
			InputDeviceDescription description = val.description;
			obj[4] = ((InputDeviceDescription)(ref description)).interfaceName;
			obj[5] = "\nDevice class: ";
			description = val.description;
			obj[6] = ((InputDeviceDescription)(ref description)).deviceClass;
			obj[7] = "\nProduct: ";
			description = val.description;
			obj[8] = ((InputDeviceDescription)(ref description)).product;
			obj[9] = "\n";
			Debug.Log((object)string.Concat(obj));
		}
	}
}
