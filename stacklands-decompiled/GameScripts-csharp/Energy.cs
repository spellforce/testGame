using UnityEngine;

public class Energy : Resource, IEnergy
{
	public int EnergyAmount => 1;

	public CardData GetCardData()
	{
		return this;
	}

	public override void OnInitialCreate()
	{
		Battery battery = (Battery)WorldManager.instance.GetNearestCardMatchingPred(MyGameCard, (GameCard x) => x.CardData is Battery);
		if ((Object)(object)battery != (Object)null && battery.StoredEnergy < battery.EnergyCapacity)
		{
			WorldManager.instance.StackSendTo(MyGameCard, battery.MyGameCard);
		}
		base.OnInitialCreate();
	}

	public void UseEnergy(int energyAmount)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		WorldManager.instance.CreateMinusElectricity(base.Position);
		MyGameCard.DestroyCard();
	}
}
