using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Selectable, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
	public Image Image;

	public bool EnableUnderline = true;

	public bool ButtonEnabled = true;

	private RectTransform rectTransform;

	private bool isDown;

	public Func<bool> IsSelectableAction;

	public SokScreen parentScreen;

	public bool SetColor = true;

	public AudioClip CustomSound;

	private Color startColor;

	public string TooltipText;

	private ScrollRect parentScrollRect;

	private bool? _isSelected;

	private bool triedFindingTmPro;

	private TextMeshProUGUI tmPro;

	private PointerEventData lastEventData;

	private float HorizontalStickTimer;

	public bool IsHovered;

	public bool ScrollToInRect = true;

	private bool tryFindParentScrollRect = true;

	public RectTransform RectTransform => (RectTransform)((Component)this).transform;

	public bool SelectableWithController
	{
		get
		{
			if (!ButtonEnabled)
			{
				return false;
			}
			if ((Object)(object)parentScreen != (Object)null && (GameCanvas.instance.ModalIsOpen || (TransitionScreen.InTransition && !TransitionScreen.instance.IsLeaving)))
			{
				return false;
			}
			if (IsSelectableAction == null)
			{
				return true;
			}
			return IsSelectableAction();
		}
	}

	public bool IsSelected
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I4
			if (!_isSelected.HasValue)
			{
				if (InputController.instance.CurrentSchemeIsController && (int)((Selectable)this).currentSelectionState == 3)
				{
					_isSelected = true;
				}
				else
				{
					_isSelected = false;
				}
			}
			return _isSelected.Value;
		}
	}

	public TextMeshProUGUI TextMeshPro
	{
		get
		{
			if (!triedFindingTmPro)
			{
				tmPro = ((Component)this).GetComponent<TextMeshProUGUI>();
				if ((Object)(object)tmPro == (Object)null)
				{
					tmPro = ((Component)this).GetComponentInChildren<TextMeshProUGUI>(true);
				}
				triedFindingTmPro = true;
			}
			return tmPro;
		}
	}

	private bool canBeClicked => !((Object)(object)parentScreen != (Object)null) || !GameCanvas.instance.ModalIsOpen;

	private Camera cam => null;

	public bool WasRightClick
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			if (lastEventData != null)
			{
				return (int)lastEventData.button == 1;
			}
			return false;
		}
	}

	public bool IsClicked
	{
		get
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			if (isDown && InputController.instance.InputCount > 0 && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, InputController.instance.GetInputPosition(0), cam))
			{
				result = true;
			}
			if (TransitionScreen.InTransition || !ButtonEnabled || !GameCanvas.instance.ScreenIsInteractable(parentScreen) || (InputController.instance.IsUsingMouse && InputController.instance.MouseIsDragging))
			{
				result = false;
			}
			return result;
		}
	}

	[HideInInspector]
	public event Action Clicked;

	[HideInInspector]
	public event Action<Vector2> StartDragging;

	public event Func<CustomButton, Navigation, Navigation> ExplicitNavigationChanged;

	protected override void Awake()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying)
		{
			Image = ((Component)this).GetComponent<Image>();
			rectTransform = ((Component)this).GetComponent<RectTransform>();
			startColor = ((Graphic)Image).color;
			((Selectable)this).Awake();
		}
	}

	protected override void Start()
	{
		if (Application.isPlaying)
		{
			parentScreen = GameCanvas.instance.GetParentScreen(rectTransform);
			if ((Object)(object)parentScreen == (Object)null && Application.isEditor)
			{
				Debug.LogWarning((object)("No parent screen found for " + ((Object)this).name));
			}
			((UIBehaviour)this).Start();
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (canBeClicked)
		{
			isDown = true;
		}
		((Selectable)this).OnPointerDown(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (isDown)
		{
			lastEventData = eventData;
			if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, cam) && canBeClicked)
			{
				SubmitClick();
			}
			isDown = false;
		}
		((Selectable)this).OnPointerUp(eventData);
	}

	private void HorizontalStick()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Slider componentInChildren = ((Component)this).GetComponentInChildren<Slider>();
		if (!((Object)(object)componentInChildren == (Object)null))
		{
			if (HorizontalStickTimer == 0f)
			{
				componentInChildren.value += ((InputController.instance.GetMove().x > 0f) ? 0.05f : (-0.05f));
			}
			HorizontalStickTimer += Time.deltaTime;
			if (HorizontalStickTimer > 1.15f - Mathf.Abs(InputController.instance.GetMove().x))
			{
				HorizontalStickTimer = 0f;
			}
		}
	}

	private void SubmitClick()
	{
		if ((Object)(object)parentScreen == (Object)null)
		{
			parentScreen = GameCanvas.instance.GetParentScreen(rectTransform);
		}
		bool flag = (Object)(object)parentScreen == (Object)null || GameCanvas.instance.ScreenIsInteractable(parentScreen);
		bool flag2 = InputController.instance.IsUsingMouse && InputController.instance.MouseIsDragging;
		if (this.Clicked != null && !TransitionScreen.InTransition && flag && !flag2 && ButtonEnabled)
		{
			this.Clicked();
			if ((Object)(object)CustomSound == (Object)null)
			{
				AudioManager.me.PlaySound2D(AudioManager.me.Click, 1f, 0.1f);
				return;
			}
			AudioManager.me.PlaySound2D(new List<AudioClip> { CustomSound }, 1f, 0.1f);
		}
	}

	protected override void OnDisable()
	{
		isDown = false;
		((Selectable)this).OnDisable();
	}

	protected override void OnEnable()
	{
		((Selectable)this).OnEnable();
	}

	public void Update()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isPlaying)
		{
			return;
		}
		if (tryFindParentScrollRect)
		{
			tryFindParentScrollRect = false;
			parentScrollRect = ((Component)this).GetComponentInParent<ScrollRect>();
		}
		if (ButtonEnabled && IsSelected && (Object)(object)InputController.instance != (Object)null && InputController.instance.CurrentSchemeIsController && SelectableWithController)
		{
			if (InputController.instance.SubmitTriggered())
			{
				SubmitClick();
			}
			if (InputController.instance.GetStickHorizontal())
			{
				HorizontalStick();
			}
			else
			{
				HorizontalStickTimer = 0f;
			}
			if ((Object)(object)parentScrollRect != (Object)null && ScrollToInRect)
			{
				ScrollToMe();
			}
		}
		if (IsHovered)
		{
			Tooltip.Text = TooltipText;
		}
		Color color = (SetColor ? ColorManager.instance.ButtonColor : startColor);
		if ((IsHovered || IsSelected) && ButtonEnabled && SetColor)
		{
			color = ColorManager.instance.HoverButtonColor;
		}
		if ((Object)(object)TextMeshPro != (Object)null)
		{
			if (!ButtonEnabled)
			{
				((Graphic)TextMeshPro).color = ColorManager.instance.DisabledButtonTextColor;
			}
			else
			{
				((Graphic)TextMeshPro).color = ColorManager.instance.ButtonTextColor;
			}
			FontStyles fontStyle = ((TMP_Text)TextMeshPro).fontStyle;
			fontStyle = (FontStyles)(((!IsHovered && !IsSelected) || !ButtonEnabled || !EnableUnderline) ? (fontStyle & -5) : (fontStyle | 4));
			((TMP_Text)TextMeshPro).fontStyle = fontStyle;
		}
		if (SetColor && (Object)(object)Image != (Object)null)
		{
			((Graphic)Image).color = color;
		}
		((Selectable)this).interactable = ButtonEnabled;
		if (!InputController.instance.CurrentSchemeIsController)
		{
			return;
		}
		if (!SelectableWithController)
		{
			Navigation navigation = ((Selectable)this).navigation;
			((Navigation)(ref navigation)).mode = (Mode)0;
			((Selectable)this).navigation = navigation;
			return;
		}
		Navigation val = ((Selectable)this).navigation;
		if (this.ExplicitNavigationChanged != null && IsSelected)
		{
			((Navigation)(ref val)).mode = (Mode)4;
			((Navigation)(ref val)).selectOnLeft = ((Selectable)this).FindSelectable(Vector3.left);
			((Navigation)(ref val)).selectOnRight = ((Selectable)this).FindSelectable(Vector3.right);
			((Navigation)(ref val)).selectOnUp = ((Selectable)this).FindSelectable(Vector3.up);
			((Navigation)(ref val)).selectOnDown = ((Selectable)this).FindSelectable(Vector3.down);
			val = this.ExplicitNavigationChanged(this, val);
		}
		else
		{
			((Navigation)(ref val)).mode = (Mode)3;
		}
		((Selectable)this).navigation = val;
	}

	public void ScrollToMe()
	{
		GameCanvas.SetScrollRectPosition(parentScrollRect, rectTransform, centerInView: true);
	}

	private void LateUpdate()
	{
		if (Application.isPlaying)
		{
			if (IsSelected && (!SelectableWithController || !ButtonEnabled) && InputController.instance.CurrentSchemeIsController)
			{
				EventSystem.current.SetSelectedGameObject((GameObject)null);
			}
			_isSelected = null;
		}
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (this.StartDragging != null)
		{
			this.StartDragging(eventData.position);
			return;
		}
		isDown = false;
		ScrollRect componentInParent = ((Component)this).GetComponentInParent<ScrollRect>();
		if (componentInParent != null)
		{
			componentInParent.OnBeginDrag(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		ScrollRect componentInParent = ((Component)this).GetComponentInParent<ScrollRect>();
		if (componentInParent != null)
		{
			componentInParent.OnDrag(eventData);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		ScrollRect componentInParent = ((Component)this).GetComponentInParent<ScrollRect>();
		if (componentInParent != null)
		{
			componentInParent.OnEndDrag(eventData);
		}
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
	}

	public void HardSetText(string text)
	{
		((TMP_Text)((Component)this).GetComponentInChildren<TextMeshProUGUI>()).text = text;
	}
}
