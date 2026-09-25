using Shapes;
using TMPro;
using UnityEngine;

public class SellBox : CardTarget
{
	public Transform GoldSpawnPosition;

	public SpriteRenderer ImageSpriteRenderer;

	public Rectangle HighlightRectangle;

	public TextMeshPro SellText;

	public override void CardDropped(GameCard card)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		WorldManager.instance.SellCard(GoldSpawnPosition.position, card);
		base.CardDropped(card);
	}

	public override bool CanHaveCard(GameCard card)
	{
		if (!WorldManager.instance.CardCanBeSold(card))
		{
			return false;
		}
		return true;
	}

	protected override void Update()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		ImageSpriteRenderer.sprite = WorldManager.instance.GetCurrencyIcon(WorldManager.instance.CurrentBoard?.BoardOptions?.Currency);
		if ((Object)(object)WorldManager.instance.CurrentBoard != (Object)null)
		{
			((TMP_Text)SellText).text = SokLoc.Translate(WorldManager.instance.CurrentBoard.BoardOptions.SellBoxTerm);
			((Object)((Component)this).gameObject).name = SokLoc.Translate(WorldManager.instance.CurrentBoard.BoardOptions.SellBoxTerm);
		}
		else
		{
			((TMP_Text)SellText).text = SokLoc.Translate("label_sell");
			((Object)((Component)this).gameObject).name = SokLoc.Translate("label_sellbox_title");
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

	protected override void LateUpdate()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.LateUpdate();
		Vector3 localPosition = ((Component)this).transform.localPosition;
		localPosition.z = 0f;
		((Component)this).transform.localPosition = localPosition;
	}

	public override string GetTooltipText()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)WorldManager.instance.CurrentBoard != (Object)null)
		{
			return SokLoc.Translate(WorldManager.instance.CurrentBoard.BoardOptions.SellBoxDescription);
		}
		return SokLoc.Translate("label_sellbox_description", (LocParam[])(object)new LocParam[1] { Extensions.LocParam_Action("sell") });
	}
}
