using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutsceneScreen : SokScreen
{
	public static CutsceneScreen instance;

	public TextMeshProUGUI TitleText;

	public TextMeshProUGUI StatusText;

	public Image Background;

	private List<CustomButton> multipleButtons = new List<CustomButton>();

	public CustomButton ContinueButton;

	public RectTransform ButtonSpacer;

	public RectTransform MultipleButtonsParent;

	public RectTransform WellbeingBar;

	public Image WellbeingFill;

	public Image WellbeingFill2;

	public Image WellbeingBackground;

	public float TargetWellbeingFillAmount;

	public TextMeshProUGUI WellbeingFillText;

	public TextMeshProUGUI WellbeingStateText;

	public int WellbeingAmount;

	public RectTransform WellbeingLineParent;

	public bool CanMoveScreen;

	public bool IsEndOfMonthCutscene;

	public bool IsAdvisorCutscene;

	private bool blink;

	private float blinkTimer;

	public Image Crosshair;

	public Color Color2;

	private float fillVelo;

	private float fillVelo2;

	private bool isPossitive;

	private float wellbeingCounter;

	private float prevWellbeingCounter;

	private float timer;

	private int index;

	private string lastCutsceneText;

	public override bool IsFrameRateUncapped => true;

	private void Awake()
	{
		instance = this;
		GenerateWellbeingBarLines();
	}

	private void GenerateWellbeingBarLines()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		CityState[] array = (CityState[])Enum.GetValues(typeof(CityState));
		float num = 50f;
		for (int i = 0; i < array.Length - 1; i++)
		{
			LayoutElement obj = new GameObject
			{
				name = "Spacer"
			}.AddComponent<LayoutElement>();
			((Component)obj).transform.SetParentClean((Transform)(object)WellbeingLineParent);
			float flexibleWidth = ((float)array[i + 1] - (float)array[i]) / num * 10f;
			obj.flexibleWidth = flexibleWidth;
			if (i < array.Length - 2)
			{
				((Component)Object.Instantiate<LayoutElement>(PrefabManager.instance.WellbeingLinePrefab)).transform.SetParentClean((Transform)(object)WellbeingLineParent);
			}
		}
	}

	private void OnEnable()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)Background).rectTransform.anchoredPosition = new Vector2(20f, -200f);
		ContinueButton.Clicked += delegate
		{
			WorldManager.instance.ContinueClicked = true;
		};
	}

	public void ClearMultipleOptions()
	{
		foreach (CustomButton multipleButton in multipleButtons)
		{
			Object.Destroy((Object)(object)((Component)multipleButton).gameObject);
		}
		multipleButtons.Clear();
	}

	public void EnableWellbeingBar(int wellbeingAmount)
	{
		IsEndOfMonthCutscene = true;
		wellbeingCounter = wellbeingAmount;
		WellbeingAmount = wellbeingAmount;
		WellbeingFill.fillAmount = (float)wellbeingAmount / 50f;
		WellbeingFill2.fillAmount = (float)wellbeingAmount / 50f;
		((TMP_Text)WellbeingFillText).text = wellbeingCounter.ToString();
	}

	public void CreateMultipleOptions(params string[] options)
	{
		for (int i = 0; i < options.Length; i++)
		{
			string text = options[i];
			int cap = i;
			CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.CutsceneButtonPrefab);
			((Component)customButton).transform.SetParentClean((Transform)(object)MultipleButtonsParent);
			customButton.HardSetText(text);
			customButton.Clicked += delegate
			{
				WorldManager.instance.ContinueButtonIndex = cap;
				WorldManager.instance.ContinueClicked = true;
			};
			multipleButtons.Add(customButton);
		}
	}

	private void Update()
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		((Component)Crosshair).gameObject.SetActive(InputController.instance.CurrentSchemeIsController && (WorldManager.instance.RemovingCards || CanMoveScreen || WorldManager.instance.ConnectConnectors));
		((Component)WellbeingBar).gameObject.SetActive(WorldManager.instance.CurrentBoard?.Id == "cities" && IsEndOfMonthCutscene && WorldManager.instance.currentAnimationRoutine != null);
		if (((Component)WellbeingBar).gameObject.activeSelf)
		{
			((TMP_Text)WellbeingStateText).text = CitiesManager.GetCityStateTranslated(CitiesManager.GetCityStateForWellbeing(WellbeingAmount));
			float num = (float)WellbeingAmount / 50f;
			if ((double)Mathf.Abs(fillVelo2) < 0.001 && (double)Mathf.Abs(fillVelo) < 0.001)
			{
				if (num > WellbeingFill.fillAmount)
				{
					((Graphic)WellbeingFill).color = ColorManager.instance.FloatingTextColorSuccess;
					isPossitive = true;
				}
				else
				{
					((Graphic)WellbeingFill).color = ColorManager.instance.FloatingTextColorFailed;
					isPossitive = false;
				}
			}
			if (!isPossitive)
			{
				WellbeingFill2.fillAmount = FRILerp.Spring(WellbeingFill2.fillAmount, num, 2f, 5f, ref fillVelo2);
			}
			else
			{
				WellbeingFill.fillAmount = FRILerp.Spring(WellbeingFill.fillAmount, num, 2f, 5f, ref fillVelo);
			}
			wellbeingCounter = Mathf.Lerp(wellbeingCounter, (float)WellbeingAmount, Time.deltaTime * 2f);
			if (Mathf.Abs(prevWellbeingCounter - (float)Mathf.RoundToInt(wellbeingCounter)) >= 1f)
			{
				AudioManager.me.PlaySound2D(AudioManager.me.WellbeingCounter, 1f, 0.7f);
				prevWellbeingCounter = Mathf.RoundToInt(wellbeingCounter);
			}
			((TMP_Text)WellbeingFillText).text = Mathf.RoundToInt(wellbeingCounter).ToString();
		}
		blinkTimer += Time.deltaTime;
		if (blinkTimer >= 0.25f)
		{
			blinkTimer = 0f;
			blink = !blink;
		}
		foreach (CustomButton multipleButton in multipleButtons)
		{
			((Graphic)multipleButton.Image).color = (blink ? ColorManager.instance.ButtonColor : Color2);
		}
		((Graphic)ContinueButton.Image).color = (blink ? ColorManager.instance.ButtonColor : Color2);
		((Graphic)Background).rectTransform.anchoredPosition = Vector2.op_Implicit(Vector3.Lerp(Vector2.op_Implicit(((Graphic)Background).rectTransform.anchoredPosition), Vector2.op_Implicit(new Vector2(10f, 10f)), Time.deltaTime * 20f));
		((TMP_Text)ContinueButton.TextMeshPro).text = WorldManager.instance.ContinueButtonText;
		if (((Component)ContinueButton).gameObject.activeInHierarchy && (InputController.instance.GetKeyDown((Key)1) || InputController.instance.SubmitTriggered()))
		{
			WorldManager.instance.ContinueClicked = true;
		}
		CheckAdvisorCutscene();
	}

	public void CheckAdvisorCutscene()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		CardData card = WorldManager.instance.GetCard("city_advisor");
		if (IsAdvisorCutscene && (Object)(object)card == (Object)null)
		{
			Vector3 val = GameCamera.instance.ScreenPosToWorldPos(Vector2.op_Implicit(new Vector2((float)Screen.width, (float)Screen.height) * 0.5f));
			WorldManager.instance.CreateCard(val, "city_advisor", faceUp: true, checkAddToStack: false);
			WorldManager.instance.CreateSmoke(val);
		}
		if (!IsAdvisorCutscene && (Object)(object)card != (Object)null)
		{
			card.MyGameCard.DestroyCard(spawnSmoke: true);
		}
	}

	private bool IsPronounced(char c)
	{
		return c switch
		{
			' ' => false, 
			'.' => false, 
			',' => false, 
			_ => true, 
		};
	}

	private void LateUpdate()
	{
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)TitleText).text = WorldManager.instance.CutsceneTitle ?? "";
		((Component)StatusText).gameObject.SetActive(!string.IsNullOrEmpty(WorldManager.instance.CutsceneText));
		if (IsAdvisorCutscene)
		{
			if (lastCutsceneText != WorldManager.instance.CutsceneText)
			{
				index = 0;
			}
			char c = ((index < WorldManager.instance.CutsceneText.Length) ? WorldManager.instance.CutsceneText[index] : ' ');
			timer += Time.deltaTime;
			if (timer >= 0.02f)
			{
				index++;
				timer = 0f;
				CardData card = WorldManager.instance.GetCard("city_advisor");
				if ((Object)(object)card != (Object)null)
				{
					if (index >= WorldManager.instance.CutsceneText.Length)
					{
						card.MyGameCard.ZRotOffset = 0f;
					}
					else if (index % 4 == 0)
					{
						if (IsPronounced(c))
						{
							Transform transform = ((Component)card.MyGameCard).transform;
							transform.localScale *= 0.98f;
							if (card.MyGameCard.ZRotOffset == 5f)
							{
								card.MyGameCard.ZRotOffset = -5f;
							}
							else
							{
								card.MyGameCard.ZRotOffset = 5f;
							}
						}
						else
						{
							card.MyGameCard.ZRotOffset = 0f;
						}
					}
				}
			}
		}
		else
		{
			index = 99999;
		}
		((TMP_Text)StatusText).maxVisibleCharacters = index;
		((TMP_Text)StatusText).text = WorldManager.instance.CutsceneText;
		((Component)ContinueButton).gameObject.SetActive(WorldManager.instance.ShowContinueButton);
		((Component)ButtonSpacer).gameObject.SetActive(((Component)ContinueButton).gameObject.activeInHierarchy);
		lastCutsceneText = WorldManager.instance.CutsceneText;
	}
}
