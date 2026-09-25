using UnityEngine;

public class Enemy : Mob
{
	protected override void Move()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		if ((Object)(object)CurrentTarget != (Object)null)
		{
			val = ((Component)this).transform.position - ((Component)CurrentTarget).transform.position;
			val.y = 0f;
			((Vector3)(ref val)).Normalize();
			Debug.DrawLine(((Component)this).transform.position, ((Component)this).transform.position - val, Color.red, 1f);
			val = Wiggle(val, 45f);
			Debug.DrawLine(((Component)this).transform.position, ((Component)this).transform.position - val, Color.green, 1f);
			val = -val * 4f;
		}
		else if (WorldManager.instance.CurrentBoard.Id == "cities")
		{
			val = Vector3.zero;
		}
		else
		{
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			Vector2 val2 = ((Vector2)(ref insideUnitCircle)).normalized * 4f;
			((Vector3)(ref val))._002Ector(val2.x, 0f, val2.y);
		}
		MyGameCard.Velocity = new Vector3(val.x, 0f, val.z);
	}

	public override void UpdateCard()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		CardData cardData2;
		if (Id == "wolf")
		{
			if (HasCardOnTop("bone", out var cardData))
			{
				cardData.MyGameCard.DestroyCard();
				MyGameCard.DestroyCard();
				WorldManager.instance.CreateCard(((Component)this).transform.position, "dog", faceUp: true, checkAddToStack: false);
			}
		}
		else if (Id == "feral_cat" && HasCardOnTop("milk", out cardData2) && WorldManager.instance.IsSpiritDlcActive())
		{
			cardData2.MyGameCard.DestroyCard();
			MyGameCard.DestroyCard();
			WorldManager.instance.CreateCard(((Component)this).transform.position, "cat", faceUp: true, checkAddToStack: false);
		}
		base.UpdateCard();
	}

	private Vector3 Wiggle(Vector3 vec, float angle)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.AngleAxis(Random.Range(0f - angle, angle), Vector3.up) * vec;
	}
}
