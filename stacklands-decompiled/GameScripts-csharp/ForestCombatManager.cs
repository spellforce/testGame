using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForestCombatManager : MonoBehaviour
{
	public static ForestCombatManager instance;

	public ForestCombatState CombatState = ForestCombatState.Idle;

	public List<AudioClip> WitchSounds;

	public float FirstWaveStrength = 10f;

	public float WaveStrengthIncrement = 10f;

	public int WickedWitchWave = 10;

	public List<string> BlacklistedDropIds;

	private List<SetCardBagType> enemiesAdvanced = new List<SetCardBagType>
	{
		SetCardBagType.Forest_BasicEnemy,
		SetCardBagType.Forest_AdvancedEnemy
	};

	private List<SetCardBagType> enemiesBasic = new List<SetCardBagType> { SetCardBagType.Forest_BasicEnemy };

	private void Awake()
	{
		instance = this;
		VerifyBlacklistedDrops();
	}

	private void MinimizeUI()
	{
		GameScreen.instance.SetMinimize(minimized: true);
		GameScreen.instance.UpdateSidePanelPosition();
	}

	public void ResumeForestCombat()
	{
		if (WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			LeaveForest();
		}
		if (IsWaveOver())
		{
			CombatState = ForestCombatState.Cutscene;
			LayoutVillagers(hardSetPosition: true);
			WorldManager.instance.QueueCutscene(Cutscenes.ForestResumeIntro());
		}
		else
		{
			CombatState = ForestCombatState.InWave;
		}
		MinimizeUI();
	}

	public void InitForestCombat()
	{
		CombatState = ForestCombatState.Cutscene;
		MinimizeUI();
		LayoutVillagers(hardSetPosition: true);
		WorldManager.instance.QueueCutscene(Cutscenes.ForestIntro());
		QuestManager.instance.SpecialActionComplete("find_dark_forest");
	}

	public void PrepareWave()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		WorldManager.instance.CurrentRunVariables.CanDropItem = true;
		int forestWave = WorldManager.instance.CurrentRunVariables.ForestWave;
		Debug.Log((object)$"Start wave {forestWave} with wicked witch at wave {WickedWitchWave}");
		GameCamera.instance.Screenshake = 0.3f;
		if (forestWave < WickedWitchWave)
		{
			SpawnWave(forestWave);
		}
		else if (forestWave == WickedWitchWave)
		{
			WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "wicked_witch", faceUp: true, checkAddToStack: false);
			SpawnWave(forestWave);
		}
		else
		{
			SpawnWave(forestWave);
		}
		StartWaveConflict(forestWave == WickedWitchWave);
	}

	private List<SetCardBagType> GetPossibleEnemies(int wave)
	{
		if (wave < 4)
		{
			return enemiesBasic;
		}
		return enemiesAdvanced;
	}

	private float GetStrengthForWave(int wave)
	{
		return WaveStrengthIncrement * (float)wave + FirstWaveStrength;
	}

	private void SpawnWave(int wave)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		foreach (CardIdWithEquipment item in SpawnHelper.GetEnemiesToSpawn(strength: (wave > WickedWitchWave) ? Random.Range(GetStrengthForWave(3), GetStrengthForWave(15)) : GetStrengthForWave(wave), cardbags: GetPossibleEnemies(wave)))
		{
			Combatable obj = WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), item, faceUp: true, checkAddToStack: false) as Combatable;
			obj.HealthPoints = obj.ProcessedCombatStats.MaxHealth;
		}
	}

	public void StartWave()
	{
		CombatState = ForestCombatState.InWave;
	}

	private static void StartWaveConflict(bool wickedWitchWave)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		List<Combatable> cards = WorldManager.instance.GetCards<Combatable>();
		cards = ((!wickedWitchWave) ? (from x in cards
			orderby x.Team descending
			where x.Id != "wicked_witch"
			select x).ToList() : cards.OrderByDescending((Combatable x) => x.Team).ToList());
		Conflict conflict = Conflict.StartConflict(cards[0]);
		for (int num = 1; num < cards.Count; num++)
		{
			conflict.JoinConflict(cards[num]);
		}
		Vector3 conflictStartPosition = DetermineVillagerPositionAverage(cards);
		conflict.ConflictStartPosition = conflictStartPosition;
		for (int num2 = 0; num2 < cards.Count; num2++)
		{
			Vector3 positionInConflict = conflict.GetPositionInConflict(cards[num2]);
			((Component)cards[num2].MyGameCard).transform.position = (cards[num2].MyGameCard.TargetPosition = positionInConflict);
			if (!(cards[num2] is BaseVillager))
			{
				WorldManager.instance.CreateSmoke(positionInConflict);
			}
		}
	}

	private static Vector3 DetermineVillagerPositionAverage(List<Combatable> combatables)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		int num = 0;
		for (int i = 0; i < combatables.Count; i++)
		{
			if (combatables[i] is BaseVillager)
			{
				val += ((Component)combatables[i]).transform.position;
				num++;
			}
		}
		return val /= (float)num;
	}

	private void FinishWave()
	{
		DeleteAllCorpses();
		QuestManager.instance.SpecialActionComplete("completed_forest_wave");
		WorldManager.instance.CurrentRunVariables.ForestWave++;
		WorldManager.instance.CurrentRunVariables.CanDropItem = true;
		int forestWave = WorldManager.instance.CurrentRunVariables.ForestWave;
		CombatState = ForestCombatState.Finished;
		if (forestWave < WickedWitchWave)
		{
			LayoutVillagers();
			WorldManager.instance.QueueCutscene(Cutscenes.ForestWaveEnd());
		}
		else if (forestWave == WickedWitchWave)
		{
			LayoutVillagers();
			WorldManager.instance.QueueCutscene(Cutscenes.ForestLastWaveEnd());
		}
		else
		{
			LayoutVillagers();
			WorldManager.instance.QueueCutscene(Cutscenes.ForestEndlessWaveEnd());
		}
	}

	private void Update()
	{
		CheckResumeCombat();
		if (WorldManager.instance.CurrentGameState != WorldManager.GameState.Playing || !(WorldManager.instance.CurrentBoard.Id == "forest"))
		{
			return;
		}
		if (WorldManager.instance.InAnimation)
		{
			LayoutVillagers();
		}
		if (CombatState == ForestCombatState.InWave)
		{
			if (IsWaveOver())
			{
				FinishWave();
			}
			else if (AllVillagersInForestDied())
			{
				CombatState = ForestCombatState.Lost;
				WorldManager.instance.QueueCutscene(Cutscenes.ForestWaveLost());
			}
		}
	}

	private bool IsWaveOver()
	{
		bool result = true;
		foreach (GameCard item in WorldManager.instance.GetAllCardsOnBoard("forest"))
		{
			if (item.CardData is Enemy || item.CardData is Mob { IsAggressive: not false })
			{
				if (item.CardData.Id == "wicked_witch" && WorldManager.instance.CurrentRunVariables.ForestWave != WickedWitchWave)
				{
					break;
				}
				result = false;
			}
		}
		return result;
	}

	private bool AllVillagersInForestDied()
	{
		foreach (GameCard item in WorldManager.instance.GetAllCardsOnBoard("forest"))
		{
			if (item.CardData is BaseVillager)
			{
				return false;
			}
		}
		return true;
	}

	public void LeaveForest()
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		DeleteAllCorpses();
		List<GameCard> list = (from x in WorldManager.instance.GetAllCardsOnBoard("forest")
			where !x.IsEquipped && x.CardData.MyCardType != CardType.Humans
			select x).ToList();
		list.RemoveAll((GameCard x) => !CanDropCard(x.CardData.Id));
		List<GameCard> list2 = (from x in WorldManager.instance.GetAllCardsOnBoard("forest")
			where x.CardData.MyCardType == CardType.Humans
			select x).ToList();
		WorldManager.instance.Restack(list);
		WorldManager.instance.Restack(list2);
		GameBoard boardWithId = WorldManager.instance.GetBoardWithId(WorldManager.instance.CurrentRunVariables.PreviouseBoard);
		if (list.Count > 0)
		{
			WorldManager.instance.SendStackToBoard(list[0], boardWithId, new Vector2(0.4f, 0.5f));
		}
		WorldManager.instance.SendStackToBoard(list2[0], boardWithId, new Vector2(0.5f, 0.5f));
		WorldManager.instance.GoToBoard(boardWithId, delegate
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!WorldManager.instance.HasFoundCard("blueprint_stable_portal"))
			{
				WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), "blueprint_stable_portal");
			}
		});
	}

	public void ForestWaveLost()
	{
		GameBoard boardWithId = WorldManager.instance.GetBoardWithId(WorldManager.instance.CurrentRunVariables.PreviouseBoard);
		WorldManager.instance.GoToBoard(boardWithId, delegate
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!WorldManager.instance.HasFoundCard("blueprint_stable_portal"))
			{
				WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), "blueprint_stable_portal");
			}
			DeleteAllCorpses();
			RemoveForestCards();
			CombatState = ForestCombatState.Idle;
		});
	}

	private static void RemoveForestCards()
	{
		foreach (GameCard item in WorldManager.instance.GetAllCardsOnBoard("forest"))
		{
			item.DestroyCard();
		}
	}

	private void CheckResumeCombat()
	{
		if ((Object)(object)WorldManager.instance.CurrentBoard != (Object)null && WorldManager.instance.CurrentBoard.Id == "forest")
		{
			if (WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
			{
				LeaveForest();
			}
			if (WorldManager.instance.CurrentRunVariables.VisitedForest && CombatState == ForestCombatState.Idle)
			{
				ResumeForestCombat();
			}
			else if (!WorldManager.instance.CurrentRunVariables.VisitedForest && CombatState == ForestCombatState.Idle)
			{
				InitForestCombat();
				WorldManager.instance.CurrentRunVariables.VisitedForest = true;
			}
		}
		else if (CombatState != ForestCombatState.Idle)
		{
			CombatState = ForestCombatState.Idle;
		}
	}

	public bool CanDropCard(string cardId)
	{
		return !BlacklistedDropIds.Contains(cardId);
	}

	private void VerifyBlacklistedDrops()
	{
		foreach (string blacklistedDropId in BlacklistedDropIds)
		{
			if ((Object)(object)WorldManager.instance.GameDataLoader.GetCardFromId(blacklistedDropId) == (Object)null)
			{
				Debug.LogError((object)(blacklistedDropId + " is not a valid card id"));
			}
		}
	}

	public static Vector3 GetWitchPosition()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return GetVillagersPosition() + new Vector3(0f, 0f, GameCard.CardHeight * 1.2f);
	}

	public static void DeleteAllCorpses()
	{
		foreach (GameCard item in (from x in WorldManager.instance.GetAllCardsOnBoard("forest")
			where x.CardData is Corpse
			select x).ToList())
		{
			item.DestroyCard();
		}
	}

	public static Vector3 GetVillagersPosition()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = WorldManager.instance.GetBoardWithId("forest").MiddleOfBoard();
		float conflictHeight = Conflict.GetConflictHeight();
		result.z += conflictHeight * 0.25f;
		return result;
	}

	private void LayoutVillagers(bool hardSetPosition = false)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		List<BaseVillager> cards = WorldManager.instance.GetCards<BaseVillager>();
		if (cards.Count == 0)
		{
			return;
		}
		Vector3 villagersPosition = GetVillagersPosition();
		Vector3 val = default(Vector3);
		for (int i = 0; i < cards.Count; i++)
		{
			float num = (float)i - ((float)cards.Count - 1f) * 0.5f;
			((Vector3)(ref val))._002Ector(num * WorldManager.instance.HorizonalCombatOffset, 0f, 0f);
			cards[i].MyGameCard.RemoveFromStack();
			cards[i].MyGameCard.TargetPosition = villagersPosition + val;
			if (hardSetPosition)
			{
				((Component)cards[i].MyGameCard).transform.position = cards[i].MyGameCard.TargetPosition;
			}
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(GetVillagersPosition(), 0.3f);
		}
	}
}
