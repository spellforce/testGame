using System.Collections.Generic;
using System.Linq;
using Shapes;
using TMPro;
using UnityEngine;

public class BuyBoosterBox : CardTarget
{
	public int Cost;

	public int StoredCostAmount;

	public string BoosterId;

	public Transform SpawnTarget;

	public TextMeshPro BuyText;

	public TextMeshPro NameText;

	public GameObject NewLabel;

	public Rectangle HighlightRectangle;

	public GameObject IdeaIcon;

	public BoardCurrency BoardCurrency;

	private int[] takeOrder = new int[4] { 10, 20, 50, 100 };

	public BoosterpackData Booster => WorldManager.instance.GetBoosterData(BoosterId);

	public int GetCost()
	{
		if (Booster.BoosterLocation == Location.Cities)
		{
			if (CitiesManager.instance.ActiveEvent == CardEventType.FinancialCrisis)
			{
				return Mathf.RoundToInt((float)Cost * 1.5f / 10f) * 10;
			}
			if (CitiesManager.instance.ActiveEvent == CardEventType.PackSale)
			{
				return Mathf.CeilToInt((float)Cost * 0.75f / 10f) * 10;
			}
		}
		return Cost;
	}

	public override bool CanHaveCard(GameCard card)
	{
		if (!MyBoard.IsCurrent)
		{
			return false;
		}
		if (!Booster.IsUnlocked)
		{
			return false;
		}
		if (WorldManager.instance.RemovingCards)
		{
			return false;
		}
		if (BoardCurrency == BoardCurrency.Gold)
		{
			if (WorldManager.instance.GetCardCountInStack(card, (CardData x) => x.Id == "gold") > 0 || WorldManager.instance.GetAmountInChest(card, "gold") > 0)
			{
				return true;
			}
		}
		else if (BoardCurrency == BoardCurrency.Shell)
		{
			if (WorldManager.instance.GetCardCountInStack(card, (CardData x) => x.Id == "shell") > 0 || WorldManager.instance.GetAmountInChest(card, "shell") > 0)
			{
				return true;
			}
		}
		else if (BoardCurrency == BoardCurrency.Dollar && (WorldManager.instance.GetCardCountInStack(card, (CardData x) => x is Dollar) > 0 || WorldManager.instance.GetDollarsInCreditcard(card) > 0))
		{
			return true;
		}
		return false;
	}

	public int GetCurrentCost()
	{
		return GetCost() - StoredCostAmount;
	}

	public override void CardDropped(GameCard card)
	{
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		int currentCost = GetCurrentCost();
		if (BoardCurrency == BoardCurrency.Gold)
		{
			int cardCountInStack = WorldManager.instance.GetCardCountInStack(card, (CardData x) => x.Id == "gold");
			int amountInChest = WorldManager.instance.GetAmountInChest(card, "gold");
			int num = 0;
			if (cardCountInStack > 0)
			{
				num = ((cardCountInStack > currentCost) ? currentCost : cardCountInStack);
				WorldManager.instance.RemoveCardsFromStackPred(card, num, (GameCard x) => x.CardData.Id == "gold");
				StoredCostAmount += num;
			}
			else if (amountInChest > 0)
			{
				num = ((amountInChest > currentCost) ? currentCost : amountInChest);
				WorldManager.instance.BuyWithChest(card, num);
				StoredCostAmount += num;
			}
		}
		else if (BoardCurrency == BoardCurrency.Shell)
		{
			int cardCountInStack2 = WorldManager.instance.GetCardCountInStack(card, (CardData x) => x.Id == "shell");
			int amountInChest2 = WorldManager.instance.GetAmountInChest(card, "shell");
			int num2 = 0;
			if (cardCountInStack2 > 0)
			{
				num2 = ((cardCountInStack2 > currentCost) ? currentCost : cardCountInStack2);
				WorldManager.instance.RemoveCardsFromStackPred(card, num2, (GameCard x) => x.CardData.Id == "shell");
				StoredCostAmount += num2;
			}
			else if (amountInChest2 > 0)
			{
				num2 = ((amountInChest2 > currentCost) ? currentCost : amountInChest2);
				WorldManager.instance.BuyWithChest(card, num2);
				StoredCostAmount += num2;
			}
		}
		else
		{
			List<Dollar> list = (from x in card.GetAllCardsInStack()
				where x.CardData is Dollar
				select x.CardData as Dollar).ToList();
			int num3 = list.Sum((Dollar x) => x.DollarValue);
			int dollarsInCreditcard = WorldManager.instance.GetDollarsInCreditcard(card);
			int num4 = 0;
			if (num3 > 0)
			{
				num4 = Mathf.Min(currentCost, num3);
				int num5 = num4;
				for (int num6 = 0; num6 < takeOrder.Length; num6++)
				{
					int curBillAmount = takeOrder[num6];
					int num7 = num5 / curBillAmount;
					num7 = Mathf.Min(list.Count((Dollar x) => x != null && x.DollarValue == curBillAmount), num7);
					num5 -= num7 * curBillAmount;
					for (int num8 = 0; num8 < num7; num8++)
					{
						Dollar dollar = list.Where((Dollar x) => x != null && x.DollarValue == curBillAmount).FirstOrDefault();
						list.Remove(dollar);
						dollar.MyGameCard.DestroyCard();
					}
					if (num5 <= 0)
					{
						break;
					}
				}
				if (num5 > 0 && list.Count > 0)
				{
					Dollar dollar2 = list.OrderBy((Dollar x) => x.DollarValue).FirstOrDefault();
					int value = dollar2.DollarValue - num5;
					list.Remove(dollar2);
					dollar2.MyGameCard.DestroyCard();
					num4 = currentCost;
					WorldManager.instance.CreateDollarsFromValue(value, ((Component)this).transform.position);
				}
				WorldManager.instance.Restack(list.Select((Dollar x) => x.MyGameCard).ToList());
				StoredCostAmount += num4;
			}
			if (dollarsInCreditcard > 0)
			{
				num4 = ((dollarsInCreditcard > currentCost) ? currentCost : dollarsInCreditcard);
				WorldManager.instance.BuyWithCreditcard(card, num4);
				StoredCostAmount += num4;
			}
		}
		WorldManager.instance.CreateSmoke(((Component)this).transform.position);
		if (StoredCostAmount == GetCost())
		{
			CreateBoosterPack();
			StoredCostAmount = 0;
		}
		base.CardDropped(card);
	}

	private void CreateBoosterPack(GameCard card = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		QuestManager.instance.SpecialActionComplete("buy_" + BoosterId + "_pack");
		WorldManager.instance.BoughtBoosterIds.Add(BoosterId);
		Boosterpack boosterpack = WorldManager.instance.CreateBoosterpack(((Component)this).transform.position, BoosterId);
		((Component)boosterpack).transform.position = (boosterpack.TargetPosition = SpawnTarget.position);
		if ((Object)(object)card != (Object)null)
		{
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(0.4f, 0f, 0f);
			((Component)boosterpack).transform.position = (boosterpack.TargetPosition = SpawnTarget.position + val);
			((Component)card).transform.position = (card.TargetPosition = SpawnTarget.position - val);
		}
		UpdateUndiscoveredCards();
	}

	protected override void Update()
	{
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		if (!MyBoard.IsCurrent)
		{
			return;
		}
		if (Booster.IsUnlocked)
		{
			((Object)((Component)this).gameObject).name = Booster.Name;
			if (BoardCurrency == BoardCurrency.Gold)
			{
				((TMP_Text)BuyText).text = $"{GetCost() - StoredCostAmount}{Icons.Gold}";
			}
			else if (BoardCurrency == BoardCurrency.Shell)
			{
				((TMP_Text)BuyText).text = $"{GetCost() - StoredCostAmount}{Icons.Shell}";
			}
			else if (BoardCurrency == BoardCurrency.Dollar)
			{
				((TMP_Text)BuyText).text = $"{GetCost() - StoredCostAmount}{Icons.Dollar}";
			}
			((TMP_Text)NameText).text = Booster.Name;
			NewLabel.gameObject.SetActive(!WorldManager.instance.CurrentSave.FoundBoosterIds.Contains(BoosterId));
		}
		else
		{
			((Object)((Component)this).gameObject).name = "???";
			((TMP_Text)NameText).text = "???";
			((TMP_Text)BuyText).text = "";
			NewLabel.gameObject.SetActive(false);
		}
		if ((Object)(object)WorldManager.instance.CurrentBoard != (Object)null)
		{
			((ShapeRenderer)HighlightRectangle).Color = WorldManager.instance.CurrentBoard.CardHighlightColor;
		}
		((Behaviour)HighlightRectangle).enabled = (Object)(object)WorldManager.instance.DraggingCard != (Object)null && CanHaveCard(WorldManager.instance.DraggingCard);
		Rectangle highlightRectangle = HighlightRectangle;
		highlightRectangle.DashOffset += Time.deltaTime;
		if (HighlightRectangle.DashOffset >= 1f)
		{
			Rectangle highlightRectangle2 = HighlightRectangle;
			highlightRectangle2.DashOffset -= 1f;
		}
		base.Update();
	}

	public void UpdateUndiscoveredCards()
	{
		if (Booster.IsUnlocked && Booster.UndiscoveredCardCount >= 1 && WorldManager.instance.CurrentSave.FoundBoosterIds.Contains(BoosterId))
		{
			IdeaIcon.SetActive(true);
		}
		else
		{
			IdeaIcon.SetActive(false);
		}
	}

	public override string GetTooltipText()
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (Booster.IsUnlocked)
		{
			string text = Icons.Gold;
			if (BoardCurrency == BoardCurrency.Shell)
			{
				text = Icons.Shell;
			}
			else if (BoardCurrency == BoardCurrency.Dollar)
			{
				text = Icons.Dollar;
			}
			return SokLoc.Translate("label_drag_coins_to_buy_pack", (LocParam[])(object)new LocParam[2]
			{
				LocParam.Create("goldicon", text),
				LocParam.Create("cost", GetCost().ToString())
			}) + "\n\n" + Booster.GetSummary();
		}
		string text2 = "label_complete_more_quests_for_pack";
		if (Booster.BoosterLocation == Location.Island)
		{
			text2 = "label_complete_more_island_quests_for_pack";
		}
		return SokLoc.Translate(text2, (LocParam[])(object)new LocParam[1] { LocParam.Plural("remaining", Booster.RemainingAchievementCountToUnlock) });
	}
}
