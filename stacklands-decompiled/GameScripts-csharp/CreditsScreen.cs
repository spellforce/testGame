using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class CreditsScreen : SokScreen
{
	public TextMeshProUGUI CreditsText;

	public float ScrollSpeed;

	private StringBuilder sb;

	private void OnEnable()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)CreditsText).text = GenerateCredits();
		Vector2 anchoredPosition = ((TMP_Text)CreditsText).rectTransform.anchoredPosition;
		anchoredPosition.y = GetHeight();
		((TMP_Text)CreditsText).rectTransform.anchoredPosition = anchoredPosition;
	}

	private float GetHeight()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return 0f - ((RectTransform)((Component)this).transform.parent).sizeDelta.y;
	}

	private void Update()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 anchoredPosition = ((TMP_Text)CreditsText).rectTransform.anchoredPosition;
		anchoredPosition.y += ScrollSpeed * Time.deltaTime;
		if (anchoredPosition.y >= ((TMP_Text)CreditsText).rectTransform.sizeDelta.y)
		{
			anchoredPosition.y = GetHeight();
		}
		((TMP_Text)CreditsText).rectTransform.anchoredPosition = anchoredPosition;
		if (InputController.instance.AnyInputDone())
		{
			GameCanvas.instance.SetScreen<OptionsScreen>();
		}
	}

	private void AddTitle(string term)
	{
		sb.Append("<color=#AAAAAA>");
		sb.Append(SokLoc.Translate(term));
		sb.Append("</color>");
		sb.AppendLine();
	}

	private void AddName(string name)
	{
		sb.Append(name);
		sb.AppendLine();
	}

	private void AddName(params string[] names)
	{
		foreach (string item in names.OrderBy((string x) => x))
		{
			AddName(item);
		}
	}

	private void AddNewLine()
	{
		sb.AppendLine();
	}

	public string GenerateCredits()
	{
		sb = new StringBuilder();
		sb.Append("<b><size=150%>Stacklands</size></b>");
		sb.AppendLine();
		sb.Append(SokLoc.Translate("credits_sokpop"));
		sb.AppendLine();
		sb.AppendLine();
		AddTitle("credits_aran");
		AddName("Aran Koning");
		AddNewLine();
		AddTitle("credits_lisa");
		AddName("Lisa Mantel");
		AddNewLine();
		AddTitle("credits_wouter");
		AddName("Wouter Janssen");
		AddNewLine();
		AddTitle("credits_cyber");
		AddName("Cyber");
		AddNewLine();
		AddTitle("credits_tumult");
		AddName("Tumult Kollektiv");
		AddNewLine();
		AddTitle("credits_local_heroes");
		AddName("Local Heroes");
		AddNewLine();
		AddTitle("language_chinese");
		AddName("Active Gaming Media");
		AddNewLine();
		AddTitle("language_dutch");
		AddName("Vincent Leeuw", "Iris Kuppen", "Lotte Busch");
		AddNewLine();
		AddTitle("language_french");
		AddName("Manuel Deroulers");
		AddNewLine();
		AddTitle("language_german");
		AddName("Jan Schäfer", "Regina Lurz", "Janina Zaghli");
		AddNewLine();
		AddTitle("language_italian");
		AddName("Michele Fantoni", "Gian Maria Battistini", "Gaetano Fabozzi");
		AddNewLine();
		AddTitle("language_japanese");
		AddName("Ziya Sarper Ekim", "Eugene Kamei-Oser", "Moeka Shimada");
		AddNewLine();
		AddTitle("language_korean");
		AddName("Ziya Sarper Ekim", "Junglim Kim", "Lim Yoon");
		AddNewLine();
		AddTitle("language_polish");
		AddName("Aleksandra Lubińska");
		AddNewLine();
		AddTitle("language_portuguese");
		AddName("Fábio Ludwig", "Thierry Banhete");
		AddNewLine();
		AddTitle("language_spanish");
		AddName("Isabel de la Mota Mendiola", "Alba Salgado Rivas", "Pedro Cortázar Pagalday");
		AddNewLine();
		AddTitle("credits_betatesting");
		AddName("Arjan \"Starchip\" Schipstra");
		AddName("Bor den Breejen");
		AddName("Benedikt \"1vader\" Werner");
		AddName("Lopidav");
		AddName("Marc de Jong");
		AddName("Margmas");
		AddName("NBK_RedSpy");
		AddName("Titouan \"Tit\" Nizet");
		AddName("Vsevolod \"Damglador\" Stopchanskyi");
		AddNewLine();
		AddTitle("credits_special_thanks");
		AddName("Boomhut", "Esther Bouma", "Neander Giljam", "Simon Naus", "Adriaan de Jongh", "Andel van Ophem", "Qkrisi");
		return sb.ToString();
	}
}
