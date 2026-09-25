using System.Collections.Generic;
using UnityEngine;

public class CreatePackLine : MonoBehaviour
{
	public float Distance;

	public float TotalWidth;

	public void CreateBoosterBoxes(List<string> boosters, BoardCurrency currency)
	{
		((Component)Object.Instantiate<SellBox>(PrefabManager.instance.SellBoxPrefab)).transform.SetParent(((Component)this).transform, true);
		foreach (string booster in boosters)
		{
			BoosterpackData boosterData = WorldManager.instance.GetBoosterData(booster);
			if (!((Object)(object)boosterData == (Object)null))
			{
				BuyBoosterBox buyBoosterBox = Object.Instantiate<BuyBoosterBox>(PrefabManager.instance.BoosterBoxPrefab);
				buyBoosterBox.BoosterId = booster;
				buyBoosterBox.Cost = boosterData.Cost;
				buyBoosterBox.BoardCurrency = currency;
				((Component)buyBoosterBox).transform.SetParent(((Component)this).transform, true);
				WorldManager.instance.AllBoosterBoxes.Add(buyBoosterBox);
			}
		}
		SetPositions();
		TotalWidth = (float)boosters.Count * Distance + 0.375f;
	}

	private void SetPositions()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < ((Component)this).transform.childCount; i++)
		{
			if (((Component)((Component)this).transform.GetChild(i)).gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		int num2 = 0;
		for (int j = 0; j < ((Component)this).transform.childCount; j++)
		{
			Transform child = ((Component)this).transform.GetChild(j);
			if (((Component)child).gameObject.activeInHierarchy)
			{
				float num3 = (float)num2 * Distance - (float)(num - 1) * Distance * 0.5f;
				child.localPosition = new Vector3(num3, 0f, 0f);
				num2++;
			}
		}
	}
}
