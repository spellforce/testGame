using UnityEngine;

public class WaterTreatmentPlant : EnergyHarvestable
{
	protected override bool CanStartHarvesting()
	{
		for (int i = 0; i < MyGameCard.CardConnectorChildren.Count; i++)
		{
			CardConnector cardConnector = MyGameCard.CardConnectorChildren[i];
			if ((Object)(object)cardConnector != (Object)null && cardConnector.ConnectionType == ConnectionType.Sewer && (Object)(object)cardConnector.ConnectedNode == (Object)null)
			{
				return false;
			}
		}
		return base.CanStartHarvesting();
	}
}
