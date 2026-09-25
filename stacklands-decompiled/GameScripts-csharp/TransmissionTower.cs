using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransmissionTower : CardData
{
	[ExtraData("has_energy")]
	public bool hasEnergy;

	private bool prevHasEnergy;

	public override bool HasEnergyOutput(CardConnector outputConnector, List<CardConnector> nodeTracker)
	{
		if (nodeTracker.Contains(outputConnector))
		{
			return false;
		}
		nodeTracker.Add(outputConnector);
		if (MyGameCard.CardConnectorChildren.Where((CardConnector x) => x.CardDirection == CardDirection.input).Count() <= 0)
		{
			hasEnergy = false;
		}
		if ((Object)(object)outputConnector != (Object)null)
		{
			int num = MyGameCard.CardConnectorChildren.Where((CardConnector x) => x.CardDirection == CardDirection.output).ToList().IndexOf(outputConnector);
			List<CardConnector> list = MyGameCard.CardConnectorChildren.Where((CardConnector x) => x.CardDirection == CardDirection.input).ToList();
			if (num >= 0 && num < list.Count)
			{
				CardConnector cardConnector = list[num];
				if ((Object)(object)cardConnector != (Object)null)
				{
					if ((Object)(object)cardConnector.ConnectedNode != (Object)null)
					{
						hasEnergy = cardConnector.ConnectedNode.Parent.CardData.HasEnergyOutput(cardConnector, nodeTracker);
					}
					else
					{
						hasEnergy = false;
					}
				}
				else
				{
					hasEnergy = false;
				}
			}
			else
			{
				hasEnergy = false;
			}
		}
		else
		{
			hasEnergy = false;
		}
		if (hasEnergy != prevHasEnergy)
		{
			NotifyEnergyConsumers();
		}
		prevHasEnergy = hasEnergy;
		return hasEnergy;
	}

	public override bool HasEnergyInput(CardConnector inputConnector)
	{
		if ((Object)(object)inputConnector != (Object)null && (Object)(object)inputConnector.ConnectedNode != (Object)null && inputConnector.ConnectedNode.HasEnergyOutput())
		{
			return true;
		}
		return false;
	}
}
