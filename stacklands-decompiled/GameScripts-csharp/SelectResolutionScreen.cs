using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectResolutionScreen : SokScreen
{
	public RectTransform ButtonsParent;

	public CustomButton BackButton;

	private List<CustomButton> resolutionButtons = new List<CustomButton>();

	private List<Resolution> resolutions = new List<Resolution>();

	private int lastScreenWidth;

	private int lastHighestWidth;

	private void Start()
	{
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		};
		lastScreenWidth = Screen.width;
	}

	private void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (lastScreenWidth != Screen.width)
		{
			int num = lastHighestWidth;
			Resolution currentResolution = Screen.currentResolution;
			if (num != ((Resolution)(ref currentResolution)).width && resolutions != OptionsScreen.PossibleResolutions())
			{
				ResetResolutions();
			}
		}
		lastHighestWidth = ((Resolution)(ref Screen.resolutions[Screen.resolutions.Length - 1])).width;
		lastScreenWidth = Screen.width;
		resolutions = OptionsScreen.PossibleResolutions();
	}

	private void OnEnable()
	{
		ResetResolutions();
	}

	private void InitButtons()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Reset resolutions");
		List<Resolution> list = OptionsScreen.PossibleResolutions();
		for (int i = 0; i < list.Count; i++)
		{
			Resolution res = list[i];
			CustomButton customButton = Object.Instantiate<CustomButton>(PrefabManager.instance.ButtonPrefab);
			((Component)customButton).transform.SetParent((Transform)(object)ButtonsParent);
			((Component)customButton).transform.localScale = Vector3.one;
			((Component)customButton).transform.localPosition = Vector3.zero;
			((Component)customButton).transform.localRotation = Quaternion.identity;
			((TMP_Text)customButton.TextMeshPro).text = ((Resolution)(ref res)).width + "x" + ((Resolution)(ref res)).height;
			customButton.Clicked += delegate
			{
				OptionsScreen.CurrentWidth = ((Resolution)(ref res)).width;
				OptionsScreen.CurrentHeight = ((Resolution)(ref res)).height;
				OptionsScreen.SetResolution();
			};
			resolutionButtons.Add(customButton);
		}
	}

	public void ResetResolutions()
	{
		foreach (CustomButton resolutionButton in resolutionButtons)
		{
			Object.Destroy((Object)(object)((Component)resolutionButton).gameObject);
		}
		resolutionButtons.Clear();
		InitButtons();
	}
}
