using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ModalScreen : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public static ModalScreen instance;

	public CustomButton ButtonPrefab;

	public RectTransform ButtonParent;

	public TMP_InputField InputPrefab;

	public RectTransform InputParent;

	public TextMeshProUGUI TitleText;

	public TextMeshProUGUI TextText;

	private void Awake()
	{
		instance = this;
	}

	public void Clear()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		foreach (RectTransform item in (Transform)ButtonParent)
		{
			Object.Destroy((Object)(object)((Component)item).gameObject);
		}
		foreach (RectTransform item2 in (Transform)InputParent)
		{
			Object.Destroy((Object)(object)((Component)item2).gameObject);
		}
	}

	public void SetTexts(string title, string text)
	{
		((TMP_Text)TitleText).text = title;
		((TMP_Text)TextText).text = text;
	}

	public void AddOption(string text, Action action)
	{
		CustomButton customButton = Object.Instantiate<CustomButton>(ButtonPrefab);
		((Component)customButton).transform.SetParentClean((Transform)(object)ButtonParent);
		((TMP_Text)customButton.TextMeshPro).text = text;
		customButton.Clicked += action;
	}

	public TMP_InputField AddInput(string confirmText, Action<string> action)
	{
		TMP_InputField input = Object.Instantiate<TMP_InputField>(InputPrefab);
		((Component)input).transform.SetParentClean((Transform)(object)InputParent);
		CustomButton customButton = Object.Instantiate<CustomButton>(ButtonPrefab);
		((Component)customButton).transform.SetParentClean((Transform)(object)ButtonParent);
		((TMP_Text)customButton.TextMeshPro).text = confirmText;
		customButton.Clicked += delegate
		{
			action(input.text);
		};
		return input;
	}

	public TMP_InputField AddInputNoButton()
	{
		TMP_InputField obj = Object.Instantiate<TMP_InputField>(InputPrefab);
		((Component)obj).transform.SetParentClean((Transform)(object)InputParent);
		return obj;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		if (Mouse.current != null)
		{
			num = TMP_TextUtilities.FindIntersectingLink((TMP_Text)(object)TextText, new Vector3(((InputControl<float>)(object)((Pointer)Mouse.current).position.x).ReadValue(), ((InputControl<float>)(object)((Pointer)Mouse.current).position.y).ReadValue(), 0f), (Camera)null);
		}
		if (num != -1)
		{
			TMP_LinkInfo val = ((TMP_Text)TextText).textInfo.linkInfo[num];
			if (((TMP_LinkInfo)(ref val)).GetLinkID().StartsWith("https://"))
			{
				Debug.Log((object)("Clicked '" + ((TMP_LinkInfo)(ref val)).GetLinkText() + ", opening '" + ((TMP_LinkInfo)(ref val)).GetLinkID() + "' in browser"));
				Application.OpenURL(((TMP_LinkInfo)(ref val)).GetLinkID());
			}
		}
	}
}
