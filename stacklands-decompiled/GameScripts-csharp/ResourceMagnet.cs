using UnityEngine;

public class ResourceMagnet : CardData
{
	[ExtraData("resource_id")]
	[HideInInspector]
	public string PullCardId;

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if ((otherCard.MyCardType == CardType.Resources || otherCard.MyCardType == CardType.Food || otherCard is Animal) && (otherCard.Id == PullCardId || !MyGameCard.HasChild))
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}

	public override void Clicked()
	{
		PullCardId = null;
	}

	protected override bool CanToggleOnOff()
	{
		if (WorldManager.instance.CurrentBoard.Id == "cities")
		{
			return true;
		}
		return false;
	}

	public override void UpdateCard()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (MyGameCard.HasChild && (string.IsNullOrEmpty(PullCardId) || PullCardId != MyGameCard.Child.CardData.Id))
		{
			PullCardId = MyGameCard.Child.CardData.Id;
		}
		if (!string.IsNullOrEmpty(PullCardId))
		{
			nameOverride = SokLoc.Translate("card_resource_magnet_name_override", (LocParam[])(object)new LocParam[1] { LocParam.Create("resource", WorldManager.instance.GameDataLoader.GetCardFromId(PullCardId).Name) });
			descriptionOverride = SokLoc.Translate("card_resource_magnet_description_long", (LocParam[])(object)new LocParam[1] { LocParam.Create("resource", WorldManager.instance.GameDataLoader.GetCardFromId(PullCardId).Name) });
		}
		else
		{
			nameOverride = SokLoc.Translate("card_resource_magnet_name");
			descriptionOverride = null;
		}
		base.UpdateCard();
		if (string.IsNullOrEmpty(PullCardId))
		{
			Icon = SpriteManager.instance.EmptyTexture;
		}
		else
		{
			Icon = WorldManager.instance.GetCardPrefab(PullCardId).Icon;
		}
		MyGameCard.UpdateIcon();
	}
}
