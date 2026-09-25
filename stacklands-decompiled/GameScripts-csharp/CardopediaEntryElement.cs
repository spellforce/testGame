using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardopediaEntryElement : MonoBehaviour
{
	public CustomButton Button;

	[HideInInspector]
	public CardData MyCardData;

	public RectTransform NewTextTransform;

	public RectTransform NewBackgroundTransform;

	public RectTransform UndiscoveredTransform;

	public RectTransform UpdateTypeTransform;

	public ShowTooltip tooltip;

	public Image UpdateImage;

	public Sprite MainIcon;

	public Sprite SpiritIcon;

	public Sprite ForestIcon;

	public Sprite IslandIcon;

	public Sprite ModIcon;

	public Sprite OrderIcon;

	public Sprite CitiesIcon;

	public bool wasFound;

	public bool IsFiltered;

	public bool IsFilteredUpdate;

	public bool HasUndiscoveredCards;

	public bool IsNew;

	public bool IsEnabled;

	private bool wasHoveredAndNew;

	public List<CanvasRenderer> CanvasRenderers;

	private bool isCulled;

	public void SetCardData(CardData cardData)
	{
		MyCardData = cardData;
		wasFound = WorldManager.instance.CurrentSave.FoundCardIds.Contains(cardData.Id) || (DebugOptions.Default.UnlockAllInCardopedia && Application.isEditor);
		IsNew = WorldManager.instance.CurrentSave.NewCardopediaIds.Contains(cardData.Id);
		HasUndiscoveredCards = cardData.HasUndiscoveredCardInDrops() && wasFound;
		UpdateUndiscoveredCardsIcon();
		UpdateIsNew();
		if (cardData.CardUpdateType == CardUpdateType.Spirit)
		{
			UpdateImage.sprite = SpiritIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_spirit";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Forest)
		{
			UpdateImage.sprite = ForestIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_forest";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Island)
		{
			UpdateImage.sprite = IslandIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_island";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Order)
		{
			UpdateImage.sprite = OrderIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_order";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Cities)
		{
			UpdateImage.sprite = CitiesIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_cities";
		}
		else if (cardData.CardUpdateType == CardUpdateType.Mod)
		{
			UpdateImage.sprite = ModIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_modded";
		}
		else
		{
			UpdateImage.sprite = MainIcon;
			tooltip.MyTooltipTerm = "label_cardopedia_main";
		}
		UpdateText();
	}

	private void UpdateUndiscoveredCardsIcon()
	{
		((Component)UndiscoveredTransform).gameObject.SetActive(HasUndiscoveredCards && !IsNew);
	}

	private void Update()
	{
		UpdateIsNew();
		if (Button.IsHovered || Button.IsSelected)
		{
			if (IsNew)
			{
				wasHoveredAndNew = true;
			}
		}
		else if (wasHoveredAndNew)
		{
			wasHoveredAndNew = false;
			IsNew = false;
			WorldManager.instance.CurrentSave.NewCardopediaIds.Remove(MyCardData.Id);
			SaveManager.instance.Save(saveRound: false);
			UpdateUndiscoveredCardsIcon();
		}
	}

	private void UpdateIsNew()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		((Component)NewTextTransform).gameObject.SetActive(IsNew);
		((Component)NewBackgroundTransform).gameObject.SetActive(IsNew);
		if (IsNew)
		{
			RectTransform newBackgroundTransform = NewBackgroundTransform;
			Rect rect = NewTextTransform.rect;
			float width = ((Rect)(ref rect)).width;
			rect = NewTextTransform.rect;
			newBackgroundTransform.sizeDelta = new Vector2(width, ((Rect)(ref rect)).height);
			((Transform)NewBackgroundTransform).position = ((Transform)NewTextTransform).position;
		}
	}

	public void Cull(bool cull)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (isCulled != cull)
		{
			isCulled = cull;
			for (int i = 0; i < CanvasRenderers.Count; i++)
			{
				CanvasRenderers[i].cull = cull;
			}
			if (!isCulled)
			{
				RectTransform newBackgroundTransform = NewBackgroundTransform;
				Rect rect = NewTextTransform.rect;
				float num = ((Rect)(ref rect)).width + 0.001f;
				rect = NewTextTransform.rect;
				newBackgroundTransform.sizeDelta = new Vector2(num, ((Rect)(ref rect)).height);
				RectTransform rectTransform = ((Graphic)UpdateImage).rectTransform;
				rect = ((Graphic)UpdateImage).rectTransform.rect;
				float num2 = ((Rect)(ref rect)).width + 0.001f;
				rect = ((Graphic)UpdateImage).rectTransform.rect;
				rectTransform.sizeDelta = new Vector2(num2, ((Rect)(ref rect)).height);
				RectTransform undiscoveredTransform = UndiscoveredTransform;
				rect = UndiscoveredTransform.rect;
				float num3 = ((Rect)(ref rect)).width + 0.001f;
				rect = UndiscoveredTransform.rect;
				undiscoveredTransform.sizeDelta = new Vector2(num3, ((Rect)(ref rect)).height);
			}
		}
	}

	public void UpdateText()
	{
		if ((Object)(object)MyCardData != (Object)null)
		{
			MyCardData.UpdateCardText();
		}
		if (wasFound)
		{
			((TMP_Text)Button.TextMeshPro).text = "• " + MyCardData.Name;
		}
		else
		{
			((TMP_Text)Button.TextMeshPro).text = "• ???";
		}
	}
}
