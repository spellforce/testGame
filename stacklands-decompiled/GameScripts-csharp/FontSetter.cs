using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class FontSetter : MonoBehaviour
{
	public FontType MyFontType;

	[HideInInspector]
	public string LanguageOverride;

	private TextMeshPro tmPro;

	private void Start()
	{
		SetFont();
		if ((Object)(object)SokLoc.instance != (Object)null)
		{
			SokLoc.instance.LanguageChanged += SetFont;
		}
	}

	private void OnEnable()
	{
		SetFont();
	}

	private void OnDestroy()
	{
		if ((Object)(object)SokLoc.instance != (Object)null)
		{
			SokLoc.instance.LanguageChanged -= SetFont;
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			SetFont();
		}
		if ((Object)(object)tmPro != (Object)null && (Object)(object)FontManager.instance != (Object)null)
		{
			((TMP_Text)tmPro).fontSharedMaterial = FontManager.instance.WorldFontMaterial;
		}
	}

	private void SetFont()
	{
		if (!((Object)(object)FontManager.instance == (Object)null))
		{
			TMP_FontAsset font = FontManager.instance.GetFont(MyFontType, LanguageOverride);
			TextMeshProUGUI component = ((Component)this).GetComponent<TextMeshProUGUI>();
			if ((Object)(object)component != (Object)null)
			{
				((TMP_Text)component).font = font;
				((Graphic)component).material = ((TMP_Asset)font).material;
			}
			tmPro = ((Component)this).GetComponent<TextMeshPro>();
			if ((Object)(object)tmPro != (Object)null)
			{
				((TMP_Text)tmPro).font = font;
				((TMP_Text)tmPro).fontSharedMaterial = FontManager.instance.WorldFontMaterial;
			}
		}
	}
}
