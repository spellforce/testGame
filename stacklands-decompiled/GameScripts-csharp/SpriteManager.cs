using UnityEngine;

public class SpriteManager : MonoBehaviour
{
	public static SpriteManager instance;

	public Sprite HealthIcon;

	public Sprite FoodIcon;

	public Sprite PollutionIcon;

	public Sprite EmptyTexture;

	public Sprite Speed1;

	public Sprite Speed10;

	public Sprite Speed0;

	public Sprite IdeaIcon;

	public Sprite IslandIdeaIcon;

	public Sprite SpiritIdeaIcon;

	public Sprite CitiesIdeaIcon;

	public Texture2D CursorDefault;

	public Texture2D CursorCanDrag;

	public Texture2D CursorDragging;

	public Sprite PoisonEffect;

	public Sprite DrunkEffect;

	public Sprite StunnedEffect;

	public Sprite WellFedEffect;

	public Sprite SpoilingEffect;

	public Sprite BleedingEffect;

	public Sprite FrenzyEffect;

	public Sprite InvulnerableEffect;

	public Sprite DemandEffect;

	public Sprite SickEffect;

	public Sprite AnxiousEffect;

	public Sprite NoEnergyEffect;

	public Sprite NoWorkersEffect;

	public Sprite HomelessEffect;

	public Sprite MaxReachedEffect;

	public Sprite NoEducatedWorkersEffect;

	public Sprite DissolvingEffect;

	public Sprite RadarEffect;

	public Sprite DamagedEffect;

	public Sprite OnFireEffect;

	public Sprite EmptyEffect;

	public Sprite CardOffEffect;

	public Sprite DepletedEffect;

	public Sprite HousingSpaceEffect;

	public Sprite DroughtEffect;

	public Sprite NoSewerEffect;

	public Sprite CoinIcon;

	public Sprite ShellIcon;

	public Sprite DollarIcon;

	public Sprite MagicFightIcon;

	public Sprite MeleeFightIcon;

	public Sprite RangedFightIcon;

	public Sprite AirFightIcon;

	public Sprite FootFightIcon;

	public Sprite ArmourFightIcon;

	public Sprite HeadIconFilled;

	public Sprite TorsoIconFilled;

	public Sprite HandIconFilled;

	public Sprite ChestIconSecondary;

	public Sprite MagnetIconSecondary;

	public Sprite TopIcon;

	public Sprite BottomIcon;

	public Sprite LeftIcon;

	public Sprite RightIcon;

	[Header("Connectors")]
	public Sprite HighVoltageSprite;

	public Sprite LowVoltageSprite;

	public Sprite SewerSprite;

	public Sprite TransportSprite;

	private Texture2D currentCursor;

	private void Awake()
	{
		instance = this;
	}

	private void SetCursorFast(Texture2D cursor, Vector2 hotspot, CursorMode cursorMode)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)currentCursor == (Object)(object)cursor))
		{
			currentCursor = cursor;
			Cursor.SetCursor(cursor, hotspot, cursorMode);
		}
	}

	private void Update()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.0625f;
		if ((Object)(object)WorldManager.instance.DraggingDraggable != (Object)null || GameCamera.instance.IsDragging || (Object)(object)CitiesManager.instance.DrawingConnector != (Object)null)
		{
			SetCursorFast(CursorDragging, new Vector2(127f, 165f) * num, (CursorMode)0);
		}
		else if ((Object)(object)WorldManager.instance.HoveredDraggable != (Object)null && WorldManager.instance.HoveredDraggable.CanBeDragged() && WorldManager.instance.CanInteract)
		{
			bool flag = true;
			if (WorldManager.instance.HoveredDraggable is InventoryInteractable || WorldManager.instance.HoveredDraggable is OnOffInteractable || WorldManager.instance.HoveredInteractable is FloatingStatus)
			{
				flag = false;
			}
			if (flag)
			{
				SetCursorFast(CursorCanDrag, new Vector2(166f, 165f) * num, (CursorMode)0);
			}
			else
			{
				SetCursorFast(CursorDefault, new Vector2(80f, 70f) * num, (CursorMode)0);
			}
		}
		else if ((Object)(object)WorldManager.instance.HoveredDraggable != (Object)null && WorldManager.instance.HoveredDraggable is CardConnector)
		{
			SetCursorFast(CursorCanDrag, new Vector2(166f, 165f) * num, (CursorMode)0);
		}
		else
		{
			SetCursorFast(CursorDefault, new Vector2(80f, 70f) * num, (CursorMode)0);
		}
	}
}
