using UnityEngine;

public class CityAdvisor : CardData
{
	public AudioClip AdvisorSound;

	public override void OnInitialCreate()
	{
		AudioManager.me.PlaySound2D(AdvisorSound, 1f, 0.1f);
		base.OnInitialCreate();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public override void UpdateCard()
	{
		if (!CutsceneScreen.instance.IsAdvisorCutscene && !MyGameCard.IsDemoCard)
		{
			MyGameCard.DestroyCard();
		}
		base.UpdateCard();
		AdvisorMovement();
	}

	public void SetAdditionalOffset()
	{
	}

	private void AdvisorMovement()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GameCamera.instance.ScreenPosToWorldPos(Vector2.op_Implicit(new Vector2((float)Screen.width, (float)Screen.height) * 0.5f));
		if (GameCamera.instance.TargetCardOverride != null)
		{
			val += Vector3.forward;
		}
		val += Vector3.left * 0.05f * Mathf.Cos(Time.time);
		val += Vector3.forward * 0.01f * Mathf.Cos(Time.time * 0.5f);
		MyGameCard.TargetPosition = val;
	}
}
