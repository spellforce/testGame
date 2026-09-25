using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combatable : CardData
{
	[Header("Combat")]
	public bool CanHaveInventory;

	public bool CanAttack = true;

	public List<string> PossibleEquipableIds = new List<string>();

	public bool InheritCombatStatsFromOtherCard;

	[Card]
	public string InheritCombatStatsFrom;

	public AttackType BaseAttackType;

	public CombatStats BaseCombatStats;

	private string _combatableDescription;

	[ExtraData("health")]
	public int HealthPoints = 3;

	private int previouseHealthPoints;

	[ExtraData("attack_timer")]
	[HideInInspector]
	public float AttackTimer;

	[HideInInspector]
	public bool BeingAttacked;

	[HideInInspector]
	public float StunTimer;

	protected List<Combatable> combatableTargets = new List<Combatable>();

	[HideInInspector]
	public bool InAttack;

	[HideInInspector]
	public AttackType CurrentAttackType;

	[HideInInspector]
	public bool Attacked;

	[HideInInspector]
	public List<Combatable> AttackTargets;

	[HideInInspector]
	public float InAttackTimer;

	[HideInInspector]
	public bool AttackIsHit;

	public Conflict MyConflict;

	private SpecialHit AttackSpecialHit;

	public List<AttackAnimation> AttackAnimations = new List<AttackAnimation>();

	[HideInInspector]
	public HitText CurrentHitText;

	[HideInInspector]
	private bool isDead;

	public List<Equipable> PossibleEquipables
	{
		get
		{
			if (!Application.isPlaying)
			{
				return (from id in PossibleEquipableIds
					select (Equipable)new GameDataLoader().GetCardFromId(id) into e
					where (Object)(object)e != (Object)null
					select e).ToList();
			}
			return (from id in PossibleEquipableIds
				select (Equipable)WorldManager.instance.GameDataLoader.GetCardFromId(id) into e
				where (Object)(object)e != (Object)null
				select e).ToList();
		}
	}

	public AttackType ProcessedAttackType
	{
		get
		{
			Equipable equipableOfEquipableType = GetEquipableOfEquipableType(EquipableType.Weapon);
			if ((Object)(object)equipableOfEquipableType != (Object)null)
			{
				return equipableOfEquipableType.AttackType;
			}
			AttackType baseAttackType = BaseAttackType;
			if (InheritCombatStatsFromOtherCard)
			{
				Combatable obj = WorldManager.instance.GameDataLoader.GetCardFromId(InheritCombatStatsFrom) as Combatable;
				if (obj.InheritCombatStatsFromOtherCard)
				{
					Debug.LogError((object)("The InheritCombatStatsFromOtherCard referenced by " + Id + " also inherits from another card"));
				}
				baseAttackType = obj.BaseAttackType;
			}
			return baseAttackType;
		}
	}

	public override bool HasInventory => CanHaveInventory;

	public CombatStats RealBaseCombatStats
	{
		get
		{
			CombatStats combatStats = new CombatStats();
			if (!InheritCombatStatsFromOtherCard)
			{
				combatStats.InitStats(BaseCombatStats);
			}
			else
			{
				Combatable combatable = WorldManager.instance.GameDataLoader.GetCardFromId(InheritCombatStatsFrom) as Combatable;
				if (!Object.op_Implicit((Object)(object)combatable))
				{
					Debug.LogError((object)("The InheritCombatStatsFromOtherCard referenced by " + Id + " is not set or incorrect"));
				}
				else if (combatable.InheritCombatStatsFromOtherCard)
				{
					Debug.LogError((object)("The InheritCombatStatsFromOtherCard referenced by " + Id + " also inherits from another card"));
				}
				else
				{
					combatStats.InitStats(combatable.BaseCombatStats);
				}
			}
			return combatStats;
		}
	}

	public CombatStats ProcessedCombatStats
	{
		get
		{
			CombatStats realBaseCombatStats = RealBaseCombatStats;
			foreach (Equipable allEquipable in GetAllEquipables())
			{
				realBaseCombatStats.AddStats(allEquipable.MyStats);
			}
			realBaseCombatStats.MaxHealth = ((realBaseCombatStats.MaxHealth < 1) ? 1 : realBaseCombatStats.MaxHealth);
			return realBaseCombatStats;
		}
	}

	[HideInInspector]
	public bool InConflict => MyConflict != null;

	public bool CanLeaveConflict
	{
		get
		{
			if (MyConflict != null)
			{
				return MyConflict.CanLeaveConflict(this);
			}
			return false;
		}
	}

	public float TimeToAttackNormalized => AttackTimer / GetAttackTime();

	public Team Team
	{
		get
		{
			if (this is BaseVillager || this is CitiesCombatable)
			{
				return Team.Player;
			}
			return Team.Enemy;
		}
	}

	public AttackAnimation CurrentAttackAnimation
	{
		get
		{
			if (AttackAnimations.Count > 0)
			{
				return AttackAnimations[0];
			}
			return null;
		}
	}

	public float DamageMultiplier
	{
		get
		{
			if (HasStatusEffectOfType<StatusEffect_Drunk>())
			{
				return 2f;
			}
			return 1f;
		}
	}

	public override void OnLanguageChange()
	{
		_combatableDescription = null;
		base.OnLanguageChange();
	}

	protected virtual float GetHitChance()
	{
		float num = ProcessedCombatStats.HitChance;
		if (HasStatusEffectOfType<StatusEffect_Drunk>())
		{
			num *= 0.6f;
		}
		return num;
	}

	protected virtual float GetAttackTime()
	{
		if (HasStatusEffectOfType<StatusEffect_Frenzy>())
		{
			return CombatStats.IncrementAttackSpeed(ProcessedCombatStats.AttackSpeed, 1);
		}
		return ProcessedCombatStats.AttackSpeed;
	}

	public string GetCombatTypeTitle()
	{
		if (ProcessedAttackType == AttackType.Melee)
		{
			return SokLoc.Translate("label_melee_title");
		}
		if (ProcessedAttackType == AttackType.Ranged)
		{
			return SokLoc.Translate("label_ranged_title");
		}
		if (ProcessedAttackType == AttackType.Magic)
		{
			return SokLoc.Translate("label_magic_title");
		}
		if (ProcessedAttackType == AttackType.Air)
		{
			return SokLoc.Translate("label_air_title");
		}
		if (ProcessedAttackType == AttackType.Foot)
		{
			return SokLoc.Translate("label_foot_title");
		}
		if (ProcessedAttackType == AttackType.Armour)
		{
			return SokLoc.Translate("label_armour_title");
		}
		return "";
	}

	public string GetCombatTypeDescription()
	{
		if (ProcessedAttackType == AttackType.Melee)
		{
			return SokLoc.Translate("label_melee_description");
		}
		if (ProcessedAttackType == AttackType.Ranged)
		{
			return SokLoc.Translate("label_ranged_description");
		}
		if (ProcessedAttackType == AttackType.Magic)
		{
			return SokLoc.Translate("label_magic_description");
		}
		if (ProcessedAttackType == AttackType.Air)
		{
			return SokLoc.Translate("label_air_description");
		}
		if (ProcessedAttackType == AttackType.Foot)
		{
			return SokLoc.Translate("label_foot_description");
		}
		if (ProcessedAttackType == AttackType.Armour)
		{
			return SokLoc.Translate("label_armour_description");
		}
		return "";
	}

	public string GetCombatTypeLore()
	{
		if (ProcessedAttackType == AttackType.Melee)
		{
			return SokLoc.Translate("label_melee_lore");
		}
		if (ProcessedAttackType == AttackType.Ranged)
		{
			return SokLoc.Translate("label_ranged_lore");
		}
		if (ProcessedAttackType == AttackType.Magic)
		{
			return SokLoc.Translate("label_magic_lore");
		}
		if (ProcessedAttackType == AttackType.Air)
		{
			return SokLoc.Translate("label_air_lore");
		}
		if (ProcessedAttackType == AttackType.Foot)
		{
			return SokLoc.Translate("label_foot_lore");
		}
		if (ProcessedAttackType == AttackType.Armour)
		{
			return SokLoc.Translate("label_armour_lore");
		}
		return "";
	}

	public override void OnEquipItem(Equipable equipable)
	{
		_combatableDescription = null;
		if (HealthPoints > ProcessedCombatStats.MaxHealth)
		{
			HealthPoints = ProcessedCombatStats.MaxHealth;
		}
	}

	public override void OnUnequipItem(Equipable equipable)
	{
		_combatableDescription = null;
	}

	private void StartOrJoinConflictInStack()
	{
		if (MyGameCard.HasTransportCard())
		{
			return;
		}
		Conflict conflictInStack = GetConflictInStack();
		if (conflictInStack != null)
		{
			conflictInStack.JoinConflict(this);
			return;
		}
		List<CardData> list = CardsInStackMatchingPredicate((CardData x) => x is Combatable && (Object)(object)x != (Object)(object)this);
		Conflict conflict = Conflict.StartConflict(this);
		foreach (CardData item in list)
		{
			conflict.JoinConflict(item as Combatable);
		}
	}

	public void OnHealthChange()
	{
		_combatableDescription = null;
	}

	private Conflict GetConflictInStack()
	{
		foreach (GameCard item in MyGameCard.GetAllCardsInStack())
		{
			if (item.CardData is Combatable { MyConflict: not null } combatable)
			{
				return combatable.MyConflict;
			}
		}
		return null;
	}

	public override void UpdateCard()
	{
		if (previouseHealthPoints != HealthPoints)
		{
			OnHealthChange();
		}
		MyGameCard.SpecialIcon.sprite = SpriteManager.instance.HealthIcon;
		if ((Object)(object)MyGameCard != (Object)null && MyGameCard.IsDemoCard)
		{
			MyGameCard.SpecialValue = ProcessedCombatStats.MaxHealth;
		}
		else
		{
			MyGameCard.SpecialValue = HealthPoints;
		}
		UpdateCombatableTargets();
		if ((combatableTargets.Count > 0 || GetConflictInStack() != null) && !InConflict)
		{
			StartOrJoinConflictInStack();
		}
		if (MyConflict != null && (Object)(object)MyConflict.Initiator == (Object)(object)this)
		{
			MyConflict.UpdateConflict();
		}
		if (InConflict)
		{
			bool flag = MyConflict.TimeSinceLastAttack <= 0.3f;
			if (CanAttack && !InAttack && !flag && !MyGameCard.BeingDragged)
			{
				AttackTimer += Time.deltaTime * WorldManager.instance.TimeScale;
			}
			bool flag2 = HasStatusEffectOfType<StatusEffect_Stunned>();
			if (!InAttack && StunTimer <= 0f && CanAttack && !flag2 && !flag && !BeingAttacked && AttackTimer >= GetAttackTime())
			{
				StartAttack();
			}
			if (InAttack)
			{
				UpdateAttackAnimations();
			}
		}
		StunTimer -= Time.deltaTime * WorldManager.instance.TimeScale;
		if (MyGameCard.BeingHovered && InConflict)
		{
			DrawConflictArrows(onlyVeryEffective: false);
		}
		previouseHealthPoints = HealthPoints;
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		descriptionOverride = SokLoc.Translate(DescriptionTerm);
		descriptionOverride = descriptionOverride + "\n\n<i>" + GetCombatableDescription() + "</i>";
		if (AdvancedSettingsScreen.AdvancedCombatStatsEnabled || GameCanvas.instance.CurrentScreen is CardopediaScreen)
		{
			descriptionOverride = descriptionOverride + "\n\n<i>" + GetCombatableDescriptionAdvanced() + "</i>";
		}
		base.UpdateCardText();
	}

	private void UpdateAttackAnimations()
	{
		for (int i = 0; i < AttackAnimations.Count; i++)
		{
			AttackAnimation attackAnimation = AttackAnimations[i];
			if (!attackAnimation.HasStarted)
			{
				attackAnimation.Start();
			}
			attackAnimation.Update();
			if (attackAnimation.IsDone)
			{
				AttackAnimations.RemoveAt(i);
				i--;
			}
			else if (attackAnimation.IsBlocking)
			{
				break;
			}
		}
		if (AttackAnimations.Count == 0)
		{
			CompleteAttack();
		}
	}

	private void StartAttack()
	{
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		if (InAttack)
		{
			Debug.LogError((object)"Already in attack!");
		}
		MyConflict.TimeSinceLastAttack = 0f;
		InAttack = true;
		InAttackTimer = 0f;
		Attacked = false;
		AttackTimer = 0f;
		Combatable target = MyConflict.GetTarget(this);
		AttackIsHit = Random.value <= GetHitChance();
		CurrentAttackType = ProcessedAttackType;
		if (!AttackIsHit)
		{
			AttackSpecialHit = null;
			AttackTargets = target.AsList();
		}
		else
		{
			AttackSpecialHit = DetermineSpecialHit();
			if (AttackSpecialHit == null || AttackSpecialHit.HitType == SpecialHitType.None)
			{
				AttackTargets = target.AsList();
			}
			else
			{
				Debug.Log((object)$"Special hit by {base.Name}: {AttackSpecialHit.HitType}");
				AttackTargets = GetSpecialHitTargets(AttackSpecialHit, target);
			}
		}
		foreach (Combatable attackTarget in AttackTargets)
		{
			attackTarget.BeingAttacked = true;
		}
		foreach (Combatable attackTarget2 in AttackTargets)
		{
			Vector3 attackTargetPosition;
			if (AttackIsHit)
			{
				attackTargetPosition = ((Component)attackTarget2).transform.position;
			}
			else
			{
				Vector2 insideUnitCircle = Random.insideUnitCircle;
				Vector2 val = ((Vector2)(ref insideUnitCircle)).normalized * WorldManager.instance.CombatMissOffset;
				attackTargetPosition = ((Component)attackTarget2).transform.position + new Vector3(val.x, 0f, val.y);
			}
			AttackAnimation attackAnimation = ((CurrentAttackType != AttackType.Ranged) ? ((CurrentAttackType != AttackType.Magic) ? ((CurrentAttackType != AttackType.Armour) ? ((CurrentAttackType != AttackType.Foot) ? ((CurrentAttackType != AttackType.Air) ? ((AttackAnimation)new AttackAnimationMelee()) : ((AttackAnimation)new AttackAnimationBullet())) : new AttackAnimationBullet()) : new AttackAnimationBullet()) : new AttackAnimationMagic()) : new AttackAnimationRanged());
			attackAnimation.Origin = this;
			attackAnimation.Target = attackTarget2;
			attackAnimation.AttackStartPosition = ((Component)MyGameCard).transform.position;
			attackAnimation.AttackTargetPosition = attackTargetPosition;
			AttackAnimations.Add(attackAnimation);
		}
	}

	public virtual void NotifyParticipantUpdate(Combatable oldParticipant, Combatable newParticipant)
	{
		if (AttackTargets != null)
		{
			for (int i = 0; i < AttackTargets.Count; i++)
			{
				if ((Object)(object)AttackTargets[i] == (Object)(object)oldParticipant)
				{
					AttackTargets[i] = newParticipant;
				}
			}
		}
		if (AttackAnimations.Count <= 0)
		{
			return;
		}
		foreach (AttackAnimation attackAnimation in AttackAnimations)
		{
			if ((Object)(object)attackAnimation.Target == (Object)(object)oldParticipant)
			{
				attackAnimation.Target = newParticipant;
			}
		}
	}

	public Projectile CreateProjectile(Projectile projectilePrefab, Combatable target, AttackAnimation originAnimation)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Projectile projectile = Object.Instantiate<Projectile>(projectilePrefab);
		projectile.ShotBy = this;
		projectile.Target = target;
		((Component)projectile).transform.position = (projectile.StartPosition = ((Component)this).transform.position);
		projectile.TargetPosition = originAnimation.AttackTargetPosition;
		projectile.OriginAnimation = originAnimation;
		return projectile;
	}

	public void CompleteAttack()
	{
		foreach (Combatable attackTarget in AttackTargets)
		{
			attackTarget.BeingAttacked = false;
		}
		AttackTargets = null;
		InAttack = false;
		Attacked = false;
	}

	public void DrawConflictArrows(bool onlyVeryEffective)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		if (InAttack || Attacked || MyGameCard.BeingDragged)
		{
			return;
		}
		foreach (Combatable combatableTarget in MyConflict.GetCombatableTargets(this))
		{
			if (!((Object)(object)combatableTarget == (Object)null) && (!combatableTarget.InAttack || combatableTarget.CurrentAttackAnimation == null || !(combatableTarget.CurrentAttackAnimation is AttackAnimationMelee)) && !combatableTarget.MyGameCard.BeingDragged)
			{
				bool flag = IsVeryEffective(ProcessedAttackType, combatableTarget.ProcessedAttackType);
				if (!onlyVeryEffective || flag)
				{
					Vector3 val = ((Component)this).transform.position + Vector3.up * 0.1f;
					Vector3 val2 = ((Component)combatableTarget).transform.position + Vector3.up * 0.1f;
					Vector3 val3 = val2 - val;
					Vector3 normalized = ((Vector3)(ref val3)).normalized;
					float conflictArrowLengthDecrease = WorldManager.instance.ConflictArrowLengthDecrease;
					Color color = (onlyVeryEffective ? ColorManager.instance.EffectiveCombatLineColor : ColorManager.instance.CombatLineColor);
					ConflictArrow conflictArrow = new ConflictArrow
					{
						Start = val + normalized * conflictArrowLengthDecrease,
						End = val2 - normalized * conflictArrowLengthDecrease,
						Color = color,
						VeryEffective = flag
					};
					DrawManager.instance.DrawShape(conflictArrow);
				}
			}
		}
	}

	public override void StoppedDragging()
	{
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		Combatable combatable = MyGameCard.Parent?.Combatable;
		if (InConflict)
		{
			if ((Object)(object)combatable != (Object)null && combatable.InConflict)
			{
				if (combatable.MyConflict == MyConflict)
				{
					MyConflict.SetParticipantTeamIndex(this, MyConflict.GetIndexInTeam(combatable));
				}
				else
				{
					MyConflict.LeaveConflict(this);
					StartOrJoinConflictInStack();
				}
				MyGameCard.RemoveFromStack();
				return;
			}
			if (!CanLeaveConflict)
			{
				MyGameCard.RemoveFromStack();
				return;
			}
			Conflict overlappingConflict = MyGameCard.GetOverlappingConflict();
			if (overlappingConflict != null && overlappingConflict != MyConflict)
			{
				MyConflict.LeaveConflict(this);
				overlappingConflict.JoinConflict(this);
			}
			if (overlappingConflict == null)
			{
				MyConflict.LeaveConflict(this);
			}
		}
		else
		{
			if ((Object)(object)combatable != (Object)null && combatable.Team != Team)
			{
				((Component)MyGameCard).transform.position = ((Component)combatable).transform.position;
				StartOrJoinConflictInStack();
			}
			Conflict overlappingConflict2 = MyGameCard.GetOverlappingConflict();
			if (overlappingConflict2 != null && !InConflict)
			{
				overlappingConflict2.JoinConflict(this);
			}
		}
	}

	public void CreateAndEquipCard(string cardId, bool markAsFound)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		CardData cardPrefab = WorldManager.instance.GetCardPrefab(cardId);
		if (!(cardPrefab is Equipable))
		{
			Debug.LogError((object)("Can't give " + cardId + " to " + Id + " because it is not an Equipable"));
		}
		else
		{
			CardData cardData = WorldManager.instance.CreateCard(((Component)this).transform.position, cardPrefab, faceUp: false, checkAddToStack: true, playSound: true, markAsFound);
			cardData.MyGameCard.MyBoard = MyGameCard.MyBoard;
			EquipItem(cardData as Equipable);
			((Component)cardData.MyGameCard.Visuals).gameObject.SetActive(false);
		}
	}

	public void ExitConflict()
	{
		AttackAnimations.Clear();
		AttackTimer = 0f;
		BeingAttacked = false;
		InAttack = false;
		Attacked = false;
	}

	public SpecialHit DetermineSpecialHit()
	{
		WeightedRandomBag<SpecialHit> weightedRandomBag = new WeightedRandomBag<SpecialHit>();
		float num = 0f;
		foreach (SpecialHit specialHit2 in ProcessedCombatStats.SpecialHits)
		{
			num += specialHit2.Chance;
			weightedRandomBag.AddEntry(specialHit2, specialHit2.Chance);
		}
		SpecialHit specialHit = new SpecialHit();
		specialHit.HitType = SpecialHitType.None;
		specialHit.Target = SpecialHitTarget.Target;
		specialHit.Chance = 100f - num;
		weightedRandomBag.AddEntry(specialHit, specialHit.Chance);
		return weightedRandomBag.Choose();
	}

	public int GetDamage(Combatable target)
	{
		if (target.HasStatusEffectOfType<StatusEffect_Invulnerable>())
		{
			return 0;
		}
		int attackDamage = ProcessedCombatStats.AttackDamage;
		int num = CombatStats.IncrementAttackDefence(ProcessedCombatStats.AttackDamage, 1);
		int num2 = ((Random.value < 0.5f) ? attackDamage : num);
		int defence = target.ProcessedCombatStats.Defence;
		int num3 = num2 - Mathf.CeilToInt((float)defence * 0.5f);
		num3 = Mathf.RoundToInt((float)num3 * GetCombatRuleMultiplier(target, this) * DamageMultiplier);
		if (num3 > 0)
		{
			return num3;
		}
		return Mathf.RoundToInt(Random.value);
	}

	public bool IsVeryEffective(AttackType self, AttackType target)
	{
		if (self == AttackType.Melee && target == AttackType.Magic)
		{
			return true;
		}
		if (self == AttackType.Magic && target == AttackType.Ranged)
		{
			return true;
		}
		if (self == AttackType.Ranged && target == AttackType.Melee)
		{
			return true;
		}
		return false;
	}

	public float GetCombatRuleMultiplier(Combatable target, Combatable self)
	{
		if (!IsVeryEffective(self.ProcessedAttackType, target.ProcessedAttackType))
		{
			return 1f;
		}
		return 1.4f;
	}

	private List<Combatable> GetSpecialHitTargets(SpecialHit specialHit, Combatable target)
	{
		if (specialHit.HitType == SpecialHitType.HealLowest)
		{
			List<Combatable> list = (from x in MyConflict.GetFriendlyParticipants(this)
				orderby x.HealthPoints
				select x).ToList();
			if (list.Count > 0)
			{
				return list[0].AsList();
			}
		}
		return specialHit.Target switch
		{
			SpecialHitTarget.Self => this.AsList(), 
			SpecialHitTarget.Target => target.AsList(), 
			SpecialHitTarget.RandomFriendly => MyConflict.GetFriendlyParticipants(this).Choose().AsList(), 
			SpecialHitTarget.RandomEnemy => MyConflict.GetEnemyParticipants(this).Choose().AsList(), 
			SpecialHitTarget.AllFriendly => MyConflict.GetFriendlyParticipants(this), 
			SpecialHitTarget.AllEnemy => MyConflict.GetEnemyParticipants(this), 
			_ => target.AsList(), 
		};
	}

	public virtual void PerformAttack(Combatable target, Vector3 attackPos)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)target == (Object)null)
		{
			return;
		}
		target.StunTimer = 0.05f;
		if (AttackIsHit)
		{
			int num = Mathf.Clamp(GetDamage(target), 0, 100);
			if (AttackSpecialHit != null && AttackSpecialHit.HitType != SpecialHitType.None)
			{
				if (!target.HasStatusEffectOfType<StatusEffect_Invulnerable>())
				{
					PerformSpecialHit(AttackSpecialHit, target, num);
				}
				else
				{
					target.Damage(num);
				}
				ShowHitText(this, target, attackPos, AttackIsHit, num, AttackSpecialHit.HitType);
			}
			else
			{
				target.Damage(num);
				ShowHitText(this, target, attackPos, AttackIsHit, num);
			}
		}
		else
		{
			ShowHitText(this, target, attackPos, AttackIsHit, -1);
		}
	}

	private void PerformSpecialHit(SpecialHit specialHit, Combatable target, int dmg)
	{
		Debug.Log((object)$"Special hit by {base.Name}: {specialHit.HitType}");
		if (specialHit.HitType == SpecialHitType.Poison)
		{
			if (!target.HasStatusEffectOfType<StatusEffect_Poison>())
			{
				target.AddStatusEffect(new StatusEffect_Poison());
			}
		}
		else if (specialHit.HitType == SpecialHitType.Crit)
		{
			dmg *= 2;
		}
		else if (specialHit.HitType == SpecialHitType.Stun)
		{
			target.RemoveStatusEffect<StatusEffect_Stunned>();
			target.AddStatusEffect(new StatusEffect_Stunned());
		}
		else if (specialHit.HitType == SpecialHitType.Bleeding)
		{
			if (!target.HasStatusEffectOfType<StatusEffect_Bleeding>())
			{
				target.AddStatusEffect(new StatusEffect_Bleeding());
			}
		}
		else if (specialHit.HitType == SpecialHitType.Frenzy)
		{
			target.RemoveStatusEffect<StatusEffect_Frenzy>();
			target.AddStatusEffect(new StatusEffect_Frenzy());
		}
		else if (specialHit.HitType == SpecialHitType.Sick)
		{
			if (!target.HasEquipableWithId("plague_mask"))
			{
				target.RemoveStatusEffect<StatusEffect_Sick>();
				target.AddStatusEffect(new StatusEffect_Sick());
				AudioManager.me.PlaySound2D(AudioManager.me.GetSick, 1f, 0.5f);
			}
		}
		else if (specialHit.HitType == SpecialHitType.HealLowest)
		{
			target.HealthPoints = Mathf.Clamp(target.HealthPoints + 2, 0, target.ProcessedCombatStats.MaxHealth);
		}
		else if (specialHit.HitType == SpecialHitType.Heal)
		{
			target.HealthPoints = Mathf.Clamp(target.HealthPoints + 2, 0, target.ProcessedCombatStats.MaxHealth);
		}
		else if (specialHit.HitType == SpecialHitType.LifeSteal)
		{
			HealthPoints = Mathf.Clamp(HealthPoints + dmg, 0, ProcessedCombatStats.MaxHealth);
		}
		else if (specialHit.HitType == SpecialHitType.Invulnerable)
		{
			if (!target.HasStatusEffectOfType<StatusEffect_Invulnerable>())
			{
				target.AddStatusEffect(new StatusEffect_Invulnerable());
			}
		}
		else if (specialHit.HitType == SpecialHitType.Anxious && !target.HasStatusEffectOfType<StatusEffect_Anxious>())
		{
			target.AddStatusEffect(new StatusEffect_Anxious());
		}
		bool flag = specialHit.Target == SpecialHitTarget.Target || specialHit.Target == SpecialHitTarget.RandomEnemy || specialHit.Target == SpecialHitTarget.AllEnemy;
		if (specialHit.Target == SpecialHitTarget.Self && (specialHit.HitType == SpecialHitType.Crit || specialHit.HitType == SpecialHitType.Stun || specialHit.HitType == SpecialHitType.Bleeding))
		{
			flag = true;
		}
		if (specialHit.HitType == SpecialHitType.HealLowest)
		{
			flag = false;
		}
		if (flag)
		{
			target.Damage(dmg);
		}
	}

	private void ShowHitText(Combatable origin, Combatable effectTarget, Vector3 targetPosition, bool isHit, int damage, SpecialHitType? type = null)
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		bool veryEffective = IsVeryEffective(origin.ProcessedAttackType, effectTarget.ProcessedAttackType);
		if (!type.HasValue)
		{
			if (isHit)
			{
				if (damage == 0)
				{
					if (effectTarget.HasStatusEffectOfType<StatusEffect_Invulnerable>())
					{
						effectTarget.CreateHitText("block", PrefabManager.instance.BlockHitText);
					}
					else
					{
						effectTarget.CreateHitText("block", PrefabManager.instance.BlockHitText);
					}
					AudioManager.me.PlaySound2D(AudioManager.me.Block, Random.Range(0.8f, 1.2f), 0.2f);
				}
				else
				{
					effectTarget.CreateHitText($"{damage}").SetVeryEffective(veryEffective);
					AudioManager.me.PlaySound2D(GetAttackTypeHitSound(), Random.Range(0.8f, 1.2f), 0.2f);
				}
			}
			else
			{
				((Component)effectTarget.CreateHitText("miss", PrefabManager.instance.MissHitText)).transform.position = targetPosition;
				if (WorldManager.instance.CurrentBoard.Id == "cities")
				{
					AudioManager.me.PlaySound2D(AudioManager.me.MissCities, Random.Range(0.8f, 1.2f), 0.5f);
				}
				else
				{
					AudioManager.me.PlaySound2D(AudioManager.me.Miss, Random.Range(0.8f, 1.2f), 0.5f);
				}
			}
			return;
		}
		switch (type)
		{
		case SpecialHitType.Heal:
			AudioManager.me.PlaySound2D(AudioManager.me.Buff, Random.Range(0.8f, 1.2f), 0.2f);
			effectTarget.CreateHitText("2", PrefabManager.instance.HealHitText);
			break;
		case SpecialHitType.HealLowest:
			AudioManager.me.PlaySound2D(AudioManager.me.Buff, Random.Range(0.8f, 1.2f), 0.2f);
			effectTarget.CreateHitText("2", PrefabManager.instance.HealHitText);
			break;
		case SpecialHitType.Crit:
			AudioManager.me.PlaySound2D(AudioManager.me.Crit, Random.Range(0.8f, 1.2f), 0.2f);
			effectTarget.CreateHitText($"{damage}!", PrefabManager.instance.CritHitText).SetVeryEffective(veryEffective);
			break;
		case SpecialHitType.Stun:
			AudioManager.me.PlaySound2D(GetAttackTypeHitSound(), Random.Range(0.8f, 1.2f), 0.2f);
			effectTarget.CreateHitText("stun", PrefabManager.instance.CritHitText).SetVeryEffective(veryEffective);
			break;
		case SpecialHitType.Damage:
			AudioManager.me.PlaySound2D(GetAttackTypeHitSound(), Random.Range(0.8f, 1.2f), 0.2f);
			effectTarget.CreateHitText($"{damage}").SetVeryEffective(veryEffective);
			break;
		case SpecialHitType.LifeSteal:
			AudioManager.me.PlaySound2D(GetAttackTypeHitSound(), Random.Range(0.8f, 1.2f), 0.2f);
			AudioManager.me.PlaySound2D(AudioManager.me.Buff, Random.Range(0.8f, 1.2f), 0.2f);
			effectTarget.CreateHitText($"{damage}", PrefabManager.instance.BleedHitText).SetVeryEffective(veryEffective);
			origin.CreateHitText($"{damage}", PrefabManager.instance.HealHitText);
			break;
		case SpecialHitType.Frenzy:
		case SpecialHitType.Invulnerable:
			effectTarget.CreateHitText("buff", PrefabManager.instance.HitTextPrefab);
			AudioManager.me.PlaySound2D(AudioManager.me.Buff, Random.Range(0.8f, 1.2f), 0.2f);
			break;
		case SpecialHitType.Poison:
		case SpecialHitType.Bleeding:
		case SpecialHitType.Sick:
		case SpecialHitType.Anxious:
			effectTarget.CreateHitText($"{damage}", PrefabManager.instance.HitTextPrefab).SetVeryEffective(veryEffective);
			AudioManager.me.PlaySound2D(GetAttackTypeHitSound(), Random.Range(0.8f, 1.2f), 0.2f);
			break;
		default:
			effectTarget.CreateHitText("NYI");
			AudioManager.me.PlaySound2D(GetAttackTypeHitSound(), Random.Range(0.8f, 1.2f), 0.2f);
			break;
		}
	}

	public List<AudioClip> GetAttackTypeHitSound()
	{
		if (ProcessedAttackType == AttackType.Melee)
		{
			return AudioManager.me.HitMelee;
		}
		if (ProcessedAttackType == AttackType.Ranged)
		{
			return AudioManager.me.HitRanged;
		}
		if (ProcessedAttackType == AttackType.Magic)
		{
			return AudioManager.me.HitMagic;
		}
		if (ProcessedAttackType == AttackType.Foot)
		{
			return AudioManager.me.HitFoot;
		}
		if (ProcessedAttackType == AttackType.Armour)
		{
			return AudioManager.me.HitArmour;
		}
		if (ProcessedAttackType == AttackType.Air)
		{
			return AudioManager.me.HitAir;
		}
		return AudioManager.me.HitMelee;
	}

	public HitText CreateHitText(string txt, HitText prefab = null)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)prefab == (Object)null)
		{
			prefab = PrefabManager.instance.NormalHitText;
		}
		HitText hitText = WorldManager.instance.CreateHitText(((Component)this).transform.position, txt, prefab);
		hitText.TargetCombatable = this;
		CurrentHitText = hitText;
		return hitText;
	}

	public virtual void Damage(int damage)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		HealthPoints -= damage;
		HealthPoints = Mathf.Max(HealthPoints, 0);
		StunTimer = 0.05f;
		GameCamera.instance.Screenshake = 0.3f;
		MyGameCard.SetHitEffect(delegate
		{
			CheckDeath();
		});
		MyGameCard.RotWobble(0.5f);
		Transform transform = ((Component)MyGameCard).transform;
		transform.localScale *= 1.5f;
	}

	private void CheckDeath()
	{
		if (!isDead && HealthPoints <= 0)
		{
			isDead = true;
			InAttack = false;
			QuestManager.instance.SpecialActionComplete(Id + "_killed");
			Die();
		}
	}

	public virtual void Die()
	{
		if (MyConflict != null)
		{
			MyConflict.LeaveConflict(this);
		}
		MyGameCard.GetAllCardsInStack().Remove(MyGameCard);
		MyGameCard.DestroyCard(spawnSmoke: true);
	}

	public virtual void UpdateCombatableTargets()
	{
		combatableTargets.Clear();
		GameCard gameCard = MyGameCard.GetRootCard();
		while ((Object)(object)gameCard != (Object)null)
		{
			if (gameCard.CardData is Combatable combatable && (Object)(object)gameCard.CardData != (Object)(object)this && combatable.Team != Team)
			{
				combatableTargets.Add(combatable);
			}
			gameCard = gameCard.Child;
		}
	}

	public string GetCombatableDescription()
	{
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(_combatableDescription))
		{
			return _combatableDescription;
		}
		string text = "";
		if ((Object)(object)MyGameCard != (Object)null && !MyGameCard.IsDemoCard)
		{
			text = text + SokLoc.Translate("label_health_info", (LocParam[])(object)new LocParam[2]
			{
				LocParam.Create("health", HealthPoints.ToString()),
				LocParam.Create("maxhealth", ProcessedCombatStats.MaxHealth.ToString())
			}) + "\n";
		}
		int num = Mathf.RoundToInt(RealBaseCombatStats.CombatLevel);
		int num2 = Mathf.RoundToInt(ProcessedCombatStats.CombatLevel);
		if (num2 != num)
		{
			text = text + SokLoc.Translate("label_base_combatlevel", (LocParam[])(object)new LocParam[1] { LocParam.Create("level", num.ToString()) }) + "\n";
			text += SokLoc.Translate("label_total_combatlevel", (LocParam[])(object)new LocParam[1] { LocParam.Create("level", num2.ToString()) });
		}
		else
		{
			text += SokLoc.Translate("label_combatlevel", (LocParam[])(object)new LocParam[1] { LocParam.Create("level", num2.ToString()) });
		}
		string text2 = ProcessedCombatStats.SummarizeSpecialHits();
		if (text2.Length > 0)
		{
			text = text + "\n\n" + text2;
		}
		_combatableDescription = text;
		return text;
	}

	public string GetCombatableDescriptionAdvanced()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		string text = SokLoc.Translate("label_combat_speed");
		string text2 = SokLoc.Translate("label_hit_chance");
		string text3 = SokLoc.Translate("label_damage");
		string text4 = SokLoc.Translate("label_defence");
		CombatStats processedCombatStats = ProcessedCombatStats;
		string attackSpeedTranslation = processedCombatStats.GetAttackSpeedTranslation();
		string attackDamageTranslation = processedCombatStats.GetAttackDamageTranslation();
		string hitChanceTranslation = processedCombatStats.GetHitChanceTranslation();
		string defenceTranslation = processedCombatStats.GetDefenceTranslation();
		string text5 = SokLoc.Translate("label_seconds_format", (LocParam[])(object)new LocParam[1] { LocParam.Create("seconds", processedCombatStats.AttackSpeed.ToString()) });
		return $"<size=80%>{text} {attackSpeedTranslation} ({text5})\n{text2} {hitChanceTranslation} ({processedCombatStats.HitChance * 100f}%)\n{text3} {attackDamageTranslation} ({processedCombatStats.AttackDamage})\n{text4}: {defenceTranslation} ({processedCombatStats.Defence})</size>";
	}

	private void OnDrawGizmos()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isPlaying || !InConflict)
		{
			return;
		}
		foreach (Combatable combatableTarget in MyConflict.GetCombatableTargets(this))
		{
			Gizmos.DrawLine(((Component)this).transform.position, ((Component)combatableTarget).transform.position);
		}
		Bounds bounds = MyConflict.GetBounds();
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).size);
	}

	public static bool SpecialHitTypeIsAttack(SpecialHitType hitType)
	{
		if (hitType == SpecialHitType.Heal || hitType == SpecialHitType.HealLowest || hitType == SpecialHitType.Invulnerable)
		{
			return false;
		}
		return true;
	}

	public void LogBaseCombatLevel()
	{
		Debug.Log((object)$"Base combat level: {BaseCombatStats.CombatLevel}");
	}
}
