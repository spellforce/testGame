using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CitiesManager : MonoBehaviour
{
	public static CitiesManager instance;

	[HideInInspector]
	private int _wellbeing;

	public int WellbeingStart = 30;

	[HideInInspector]
	public CityState CityState;

	public int TriggerEventFromMonth = 10;

	public CardBag EventList;

	public int MonthsBetweenConflict = 6;

	public int MaxRandomOffset = 1;

	public CardEventType? ActiveEvent;

	[HideInInspector]
	public int NextConflictMonth;

	[HideInInspector]
	public CardConnector DrawingConnector;

	public Material EnergyCableMaterial;

	[HideInInspector]
	public List<HousingConsumer> HousingConsumers = new List<HousingConsumer>();

	[HideInInspector]
	public List<Worker> WorkersOnBoard = new List<Worker>();

	[HideInInspector]
	public List<HousingConsumer> HomelessHousingConsumers = new List<HousingConsumer>();

	[HideInInspector]
	public int PreviousWellbeing;

	private float targetVolume;

	private float targetPitch = 1f;

	private float prevDist;

	private AudioSource stretchSource;

	private int[] takeOrder = new int[4] { 10, 20, 50, 100 };

	public int Wellbeing
	{
		get
		{
			return _wellbeing;
		}
		set
		{
			PreviousWellbeing = _wellbeing;
			_wellbeing = Mathf.Clamp(value, 0, 200);
			UpdateCityState();
		}
	}

	private void Start()
	{
		instance = this;
		InitStrechSource();
		UpdateCityState();
	}

	private void InitStrechSource()
	{
		stretchSource = AudioManager.me.GetSource(((Component)this).transform, claim: true);
		stretchSource.pitch = 0f;
		stretchSource.volume = 0f;
		stretchSource.clip = AudioManager.me.EnergyStrech;
		stretchSource.reverbZoneMix = 0f;
		stretchSource.spatialBlend = 0f;
		stretchSource.bypassListenerEffects = false;
		stretchSource.loop = true;
		stretchSource.Play();
	}

	private void Update()
	{
		if ((Object)(object)WorldManager.instance.CurrentBoard == (Object)null || WorldManager.instance.CurrentBoard?.Id != "cities")
		{
			if (WorldManager.instance.CanUseTransport)
			{
				DrawConnectors();
			}
			return;
		}
		WorldManager.instance.GetCardsNonAlloc(WorkersOnBoard);
		WorldManager.instance.GetCardsImplementingInterfaceNonAlloc(HousingConsumers);
		HomelessHousingConsumers.Clear();
		for (int num = HousingConsumers.Count - 1; num >= 0; num--)
		{
			if ((Object)(object)HousingConsumers[num].Housing == (Object)null && HousingConsumers[num].GetHousingSpaceRequired() > 0)
			{
				HomelessHousingConsumers.Add(HousingConsumers[num]);
			}
		}
		CheckConflict();
		CheckCutscenes();
		CheckForEvents();
		DrawConnectors();
		DrawConnectorAudio();
	}

	public void DrawConnectorAudio()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)DrawingConnector != (Object)null)
		{
			AudioClip stretchSoundForType = DrawingConnector.GetStretchSoundForType(DrawingConnector.ConnectionType);
			stretchSource.clip = stretchSoundForType;
			if (!stretchSource.isPlaying)
			{
				stretchSource.Play();
			}
			Vector3 val = ((Component)DrawingConnector).transform.position + Vector3.down * 0.01f;
			Vector3 mouseWorldPosition = WorldManager.instance.mouseWorldPosition;
			float num = Vector3.Distance(val, mouseWorldPosition);
			float num2 = Mathf.Abs(num - prevDist);
			prevDist = num;
			if (num2 > 0.001f)
			{
				targetVolume = 1f;
			}
			else
			{
				targetVolume = 0f;
			}
			if (DrawingConnector.ConnectionType == ConnectionType.LV || DrawingConnector.ConnectionType == ConnectionType.HV)
			{
				targetPitch = Mathf.Lerp(1f, 1.5f, Mathf.InverseLerp(1f, 8f, num));
			}
			else
			{
				targetPitch = 1f;
			}
		}
		else
		{
			targetVolume = 0f;
		}
		stretchSource.volume = Mathf.Lerp(stretchSource.volume, targetVolume, Time.deltaTime * 15f);
		stretchSource.pitch = Mathf.Lerp(stretchSource.pitch, targetPitch, Time.deltaTime * 5f);
	}

	public void CheckForEvents()
	{
		EventCard card = WorldManager.instance.GetCard<EventCard>();
		if ((Object)(object)card != (Object)null && card.EventIsActive)
		{
			ActiveEvent = card.EventType;
		}
		else
		{
			ActiveEvent = null;
		}
	}

	public void CheckCutscenes()
	{
		if (!TransitionScreen.InTransition && _wellbeing > 0 && !WorldManager.instance.InAnimation)
		{
			if (_wellbeing >= 40)
			{
				WorldManager.instance.QueueCutsceneIfNotPlayed("cities_wellbeing_30");
			}
			if (_wellbeing >= 20)
			{
				WorldManager.instance.QueueCutsceneIfNotPlayed("cities_wellbeing_20");
			}
			if (_wellbeing < 10)
			{
				WorldManager.instance.QueueCutsceneIfNotPlayed("cities_wellbeing_10");
			}
		}
	}

	public void StartDrawCable(CardConnector connector)
	{
		DrawingConnector = connector;
	}

	public void StopDrawCable(CardConnector endConnector)
	{
		if ((Object)(object)DrawingConnector != (Object)null && (Object)(object)endConnector != (Object)null && endConnector.CardDirection != DrawingConnector.CardDirection && endConnector.ConnectionType == DrawingConnector.ConnectionType && (Object)(object)DrawingConnector.Parent != (Object)(object)endConnector.Parent)
		{
			DrawingConnector.SetConnectedNode(endConnector);
			QuestManager.instance.SpecialActionComplete("cities_cable_connected", DrawingConnector.Parent.CardData);
			DrawingConnector.Parent.CardData.NotifyEnergyConsumers();
		}
		DrawingConnector = null;
	}

	private Vector3 DetermineConnectorMiddle(Vector3 start, Vector3 end, ConnectionType conn)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (conn == ConnectionType.LV || conn == ConnectionType.HV)
		{
			float num = Mathf.Abs(start.x - end.x);
			float num2 = 1f - Mathf.InverseLerp(0f, 3f, num);
			float num3 = (end.z - start.z) * 0.3f;
			float num4 = 0.75f;
			return Vector3.Lerp(start, end, 0.5f + num3) + new Vector3(0f, 0f, 0f - num4) * num2;
		}
		return Vector3.Lerp(start, end, 0.5f);
	}

	public void DrawConnectors()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val2;
		if ((Object)(object)DrawingConnector != (Object)null)
		{
			Vector3 start = ((Component)DrawingConnector).transform.position + Vector3.down * 0.01f;
			Vector3 mouseWorldPosition = WorldManager.instance.mouseWorldPosition;
			Vector3 val = DetermineConnectorMiddle(start, mouseWorldPosition, DrawingConnector.ConnectionType);
			if (DrawingConnector.ConnectionType == ConnectionType.LV || DrawingConnector.ConnectionType == ConnectionType.HV)
			{
				val2 = DrawingConnector.Middle - val;
				if (((Vector3)(ref val2)).sqrMagnitude >= 100f)
				{
					DrawingConnector.MiddleVelo = Vector3.zero;
					DrawingConnector.Middle = val;
				}
				DrawingConnector.Middle = FRILerp.Spring(DrawingConnector.Middle, val, 25f, 10f, ref DrawingConnector.MiddleVelo);
			}
			else if (DrawingConnector.ConnectionType == ConnectionType.Sewer)
			{
				DrawingConnector.Middle = val;
			}
			else if (DrawingConnector.ConnectionType == ConnectionType.Transport)
			{
				DrawingConnector.Middle = val;
			}
			DrawManager.instance.DrawShape(GetShapeForConnectionType(DrawingConnector.ConnectionType, start, DrawingConnector.Middle, mouseWorldPosition));
		}
		GameBoard currentBoard = WorldManager.instance.CurrentBoard;
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard.CardConnectorChildren.Count == 0 || (Object)(object)allCard.MyBoard != (Object)(object)currentBoard)
			{
				continue;
			}
			foreach (CardConnector cardConnectorChild in allCard.CardConnectorChildren)
			{
				if ((Object)(object)cardConnectorChild.ConnectedNode != (Object)null && cardConnectorChild.CardDirection == CardDirection.output)
				{
					Vector3 start2 = ((Component)cardConnectorChild).transform.position + Vector3.down * 0.01f;
					Vector3 end = ((Component)cardConnectorChild.ConnectedNode).transform.position + Vector3.down * 0.01f;
					Vector3 val3 = DetermineConnectorMiddle(start2, end, cardConnectorChild.ConnectionType);
					val2 = cardConnectorChild.Middle - val3;
					if (((Vector3)(ref val2)).sqrMagnitude >= 100f)
					{
						cardConnectorChild.MiddleVelo = Vector3.zero;
						cardConnectorChild.Middle = val3;
					}
					if (cardConnectorChild.ConnectionType == ConnectionType.LV || cardConnectorChild.ConnectionType == ConnectionType.HV)
					{
						cardConnectorChild.Middle = FRILerp.Spring(cardConnectorChild.Middle, val3, 25f, 10f, ref cardConnectorChild.MiddleVelo);
					}
					else if (cardConnectorChild.ConnectionType == ConnectionType.Sewer)
					{
						cardConnectorChild.Middle = val3;
					}
					else if (cardConnectorChild.ConnectionType == ConnectionType.Transport)
					{
						cardConnectorChild.Middle = FRILerp.Spring(cardConnectorChild.Middle, val3, 30f, 30f, ref cardConnectorChild.MiddleVelo);
					}
					DrawManager.instance.DrawShape(GetShapeForConnectionType(cardConnectorChild.ConnectionType, start2, cardConnectorChild.Middle, end));
				}
			}
		}
	}

	private IShape GetShapeForConnectionType(ConnectionType connectionType, Vector3 start, Vector3 middle, Vector3 end)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		switch (connectionType)
		{
		case ConnectionType.LV:
		case ConnectionType.HV:
			return new EnergyCable
			{
				Start = start,
				Middle = middle,
				End = end,
				IsLowVoltage = (connectionType == ConnectionType.LV)
			};
		case ConnectionType.Sewer:
			return new SewerPipe
			{
				Start = start,
				Middle = middle,
				End = end
			};
		case ConnectionType.Transport:
			if (WorldManager.instance.CurrentBoard.Id == "cities")
			{
				return new TransportArrow
				{
					Start = start,
					Middle = middle,
					End = end
				};
			}
			return new TransportArrowMainland
			{
				Start = start,
				Middle = middle,
				End = end
			};
		default:
			return null;
		}
	}

	public void CheckConflict()
	{
		if (WorldManager.instance.HasFoundCard("blueprint_barrack") && Wellbeing >= 25)
		{
			int currentMonth = WorldManager.instance.CurrentMonth;
			if (NextConflictMonth < currentMonth - 1)
			{
				NextConflictMonth = GetNextConflictMonth(currentMonth);
				Debug.Log((object)("Updated Conflict Month to : " + NextConflictMonth));
			}
		}
		else
		{
			NextConflictMonth = -1;
		}
	}

	public int GetNextConflictMonth(int currentMonth)
	{
		return currentMonth + Random.Range(MonthsBetweenConflict - MaxRandomOffset, MonthsBetweenConflict + MaxRandomOffset);
	}

	public static string GetCityStateTranslated(CityState state)
	{
		return SokLoc.Translate("label_wellbeing_" + state.ToString().ToLower());
	}

	public void CheckCityHealth()
	{
	}

	public void AddWellbeing(int wellbeing)
	{
		Wellbeing += wellbeing;
	}

	public bool ShouldTriggerEvent()
	{
		if (WorldManager.instance.CurrentMonth > TriggerEventFromMonth)
		{
			return true;
		}
		return false;
	}

	public CardId GetEvent()
	{
		List<string> list = (from x in EventList.GetCardsInBag()
			where !WorldManager.instance.CurrentRunVariables.SpawnedEventIds.Contains(x)
			select x).ToList();
		if (list.Count <= 0)
		{
			WorldManager.instance.CurrentRunVariables.SpawnedEventIds = new List<string>();
			list = EventList.GetCardsInBag();
		}
		string text = list.Choose();
		WorldManager.instance.CurrentRunVariables.SpawnedEventIds.Add(text);
		return new CardId(text);
	}

	public void UpdateCityState()
	{
		if (_wellbeing < 0)
		{
			_wellbeing = 0;
		}
		CityState = GetCityStateForWellbeing(_wellbeing);
	}

	public static CityState GetCityStateForWellbeing(int amount)
	{
		if (amount <= 10 && amount > 0)
		{
			return CityState.Miserable;
		}
		if (amount <= 20 && amount > 10)
		{
			return CityState.Unhappy;
		}
		if (amount < 40 && amount > 20)
		{
			return CityState.Normal;
		}
		if (amount >= 40 && amount < 50)
		{
			return CityState.Happy;
		}
		if (amount >= 50)
		{
			return CityState.Euphoric;
		}
		return CityState.Gameover;
	}

	private void UpdateEnergyAmount()
	{
	}

	public bool TryConsumeEnergy(int amount, GameCard consumer)
	{
		if (WorldManager.instance.DebugNoEnergyEnabled)
		{
			return true;
		}
		if (amount == 0)
		{
			return true;
		}
		List<IEnergy> cardsImplementingInterface = WorldManager.instance.GetCardsImplementingInterface<IEnergy>();
		if (cardsImplementingInterface.Sum((IEnergy x) => x.EnergyAmount) < amount)
		{
			return false;
		}
		cardsImplementingInterface = cardsImplementingInterface.OrderBy(delegate(IEnergy x)
		{
			CardData cardData = x as CardData;
			return (cardData is Battery) ? (-1000 - cardData.MyGameCard.GetCardIndex()) : (-cardData.MyGameCard.GetCardIndex());
		}).ToList();
		foreach (IEnergy item in cardsImplementingInterface)
		{
			int num = Mathf.Min(item.EnergyAmount, amount);
			item.UseEnergy(num);
			amount -= num;
			if (amount == 0)
			{
				break;
			}
		}
		return true;
	}

	public int TryUseDollars(List<ICurrency> currencyList, int cost, bool onlyTakeIfAmountMet = false, bool spawnSmoke = false, bool keepOnStack = false)
	{
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		if (WorldManager.instance.DebugNoFoodEnabled)
		{
			return 0;
		}
		if (cost == 0)
		{
			return cost;
		}
		if (currencyList.Count <= 0)
		{
			return cost;
		}
		if (onlyTakeIfAmountMet && currencyList.Sum((ICurrency x) => x.CurrencyValue) < cost)
		{
			return cost;
		}
		int num = currencyList.Sum((ICurrency x) => x.CurrencyValue);
		int num2 = Mathf.Min(cost, num);
		GameCard rootCard = currencyList[0].Card.MyGameCard.GetRootCard();
		if (num > 0)
		{
			foreach (Creditcard item in (from Creditcard x in currencyList.Where((ICurrency x) => x is Creditcard creditcard && creditcard.DollarCount > 0)
				orderby x.DollarCount
				select x).ToList())
			{
				int num3 = Mathf.Min(item.CurrencyValue, cost);
				item.UseCurrency(num3, spawnSmoke);
				num2 -= num3;
				if (num2 == 0)
				{
					break;
				}
			}
			if (num2 > 0)
			{
				for (int num4 = 0; num4 < takeOrder.Length; num4++)
				{
					int curBillAmount = takeOrder[num4];
					int num5 = num2 / curBillAmount;
					num5 = Mathf.Min(currencyList.Count((ICurrency x) => x is Dollar dollar3 && dollar3.DollarValue == curBillAmount), num5);
					num2 -= num5 * curBillAmount;
					for (int num6 = 0; num6 < num5; num6++)
					{
						Dollar dollar = currencyList.Where((ICurrency x) => x is Dollar dollar3 && dollar3.DollarValue == curBillAmount).FirstOrDefault() as Dollar;
						currencyList.Remove(dollar);
						dollar.UseCurrency(dollar.CurrencyValue, spawnSmoke);
					}
					if (num2 <= 0)
					{
						break;
					}
				}
			}
			if (num2 > 0 && currencyList.Count > 0)
			{
				Dollar dollar2 = (from x in currencyList
					where x is Dollar
					orderby x.CurrencyValue
					select x).FirstOrDefault() as Dollar;
				int value = dollar2.DollarValue - num2;
				List<GameCard> source = WorldManager.instance.CreateDollarsFromValue(value, dollar2.Position);
				currencyList.AddRange(source.Select((GameCard x) => x.CardData as ICurrency));
				currencyList.Remove(dollar2);
				dollar2.UseCurrency(dollar2.CurrencyValue, spawnSmoke);
				num2 = 0;
			}
		}
		if (keepOnStack && currencyList.Count > 0 && (Object)(object)rootCard != (Object)null)
		{
			currencyList[0].Card.MyGameCard.GetRootCard().SetParent(rootCard);
		}
		else if (currencyList.Count > 0)
		{
			currencyList[0].Card.MyGameCard.RemoveFromParent();
			currencyList[0].Card.MyGameCard.SendIt();
		}
		return cost - num2;
	}

	public IEnumerator ConsumeFood(int amount, Vector3 targetPos)
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		List<Food> foodToUse = GetFoodToUse(amount);
		int num = amount;
		foreach (Food food in foodToUse)
		{
			if (num <= 0)
			{
				break;
			}
			food.IsReserved = true;
			int num2 = Mathf.Min(num, food.FoodValue);
			num -= num2;
			food.FoodValue -= num2;
			food.MyGameCard.PushEnabled = false;
			if (food.FoodValue <= 0)
			{
				if (food.MyGameCard.HasParent && food.MyGameCard.HasChild)
				{
					GameCard parent = food.MyGameCard.Parent;
					GameCard child = food.MyGameCard.Child;
					food.MyGameCard.RemoveFromStack();
					parent.SetChild(child);
				}
				else
				{
					food.MyGameCard.RemoveFromStack();
				}
				_ = food.Position;
				if (!(food is FoodWarehouse))
				{
					food.IsConsumed = true;
					food.MyGameCard.SendToPositionCallback(targetPos, delegate
					{
						food.MyGameCard.DestroyCard();
					});
				}
				else
				{
					WorldManager.instance.CreateSmoke(food.Position);
				}
			}
			food.MyGameCard.PushEnabled = true;
			food.IsReserved = false;
		}
		return null;
	}

	public List<Food> GetFoodToUse(int amount)
	{
		List<Food> source = (from x in WorldManager.instance.GetCards<Food>()
			where !x.IsReserved
			select x).ToList();
		if (source.Sum((Food x) => x.FoodValue) < amount)
		{
			return new List<Food>();
		}
		return source.Where((Food x) => x.FoodValue > 0).OrderByDescending(delegate(Food x)
		{
			bool flag = (Object)(object)x.MyGameCard.GetCardWithStatusInStack() != (Object)null;
			if (x is FoodWarehouse foodWarehouse && WorldManager.instance.GameDataLoader.GetCardFromId(foodWarehouse.HeldCardId) is Food food)
			{
				return food.FoodValue;
			}
			if (flag)
			{
				return -3;
			}
			return (!x.IsCookedFood) ? (-2) : 0;
		}).ThenBy((Food x) => x.FoodValue)
			.ThenBy((Food x) => -x.MyGameCard.GetCardIndex())
			.ToList();
	}

	public static string GetAmountPrefix(int amount)
	{
		if (amount > 0)
		{
			return "+";
		}
		return "";
	}
}
