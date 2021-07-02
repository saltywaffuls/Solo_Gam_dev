using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Piece
{

    [SerializeField] PieceBase _base;
    [SerializeField] int level;

   public PieceBase Base 
    {
        get
        {
            return _base;
        }
    }
    public int Level 
    { 
        get
        {
            return level;
        }
     }

    public int Exp { get; set; }
    public int HP { get; set; }
    public List<Ability> Abilities { get; set; }
    public Ability CurrentAbility { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int statusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }


    public Queue<string> statusChanges { get; private set; } = new Queue<string>();

    public bool HPChange { get; set; }
    public event System.Action OnStatusChanged;

    public void Init()
    {
        
        // generate abilties
        Abilities = new List<Ability>();
        foreach (var ability in Base.LearnableAbilities)
        {
            if (ability.Level <= Level)
                Abilities.Add(new Ability(ability.AbilityBase));

            if (Abilities.Count >= PieceBase.MaxNumOfAbilties)
                break;
        }

        Exp = Base.GetExpForLevel(Level);

        CalculateStates();
        HP = MaxHP;

        statusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    // restores the save data
    public Piece(PieceSaveData saveData)
    {
        _base = PieceDB.GetPieceByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;

        if (saveData.statusId != null)
            Status = ConditionDB.Conditions[saveData.statusId.Value];
        else
            Status = null;

        Abilities = saveData.abilities.Select(s => new Ability(s)).ToList();

        CalculateStates();
        statusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }

    //coverst curret pice class into save data class
    public PieceSaveData GetSaveData()
    {
        var saveData = new PieceSaveData()
        {
            name = Base.Name,
            hp = HP,
            level = Level,
            exp = Exp,
            statusId = Status?.Id,
            abilities = Abilities.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

    //math and formula for stats
    void CalculateStates()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 );
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5 );
        Stats.Add(Stat.UltAttack, Mathf.FloorToInt((Base.UltAttack * Level) / 100f) + 5 );
        Stats.Add(Stat.UltDefense, Mathf.FloorToInt((Base.UltDefense * Level) / 100f) + 5 );
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5 );

        MaxHP =  Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10 + Level;
    }

    // resets stats back to orignal
    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.UltAttack, 0},
            {Stat.UltDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //todo stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal =  Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    //apply sate buff/debuff by 6 levels
    public void ApplyBoost(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp (StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                statusChanges.Enqueue($"{Base.Name}'s {stat} has overcome its limit ");
            else
                statusChanges.Enqueue($"{Base.Name}'s {stat} has fell below its limit ");

            Debug.Log($"{stat} has been bossted to {StatBoosts[stat]}");
        }
    }

    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }

        return false;
    }

    //property level formulas
    public int Attack
    {
        //base attack mutpled by the level divided by 100 add five
        get { return GetStat(Stat.Attack); }
    }

    public LearnableAbility GetLearnableAbilityAtCurrLevel() 
    {
        return Base.LearnableAbilities.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnAbility(LearnableAbility abilityToLearn)
    {
        if (Abilities.Count > PieceBase.MaxNumOfAbilties)
            return;

        //dont know if code is right in base ep 38 6:47
        Abilities.Add(new Ability(abilityToLearn.AbilityBase));
    }

    public int MaxHP { get; private set; }

    public int Defense
    {
        
        get { return GetStat(Stat.Defense); }
    }

    public int UltAttack
    {
        //base attack mutpled by the level divided by 100 add five
        get { return GetStat(Stat.UltAttack); }
    }

    public int UltDefense
    {
        //base attack mutpled by the level divided by 100 add five
        get { return GetStat(Stat.UltDefense); }
    }

    public int Speed
    {
        //base attack mutpled by the level divided by 100 add five
        get { return GetStat(Stat.Speed); }
    }

    //function that deals damage
    public DamageDetails TakeDamage(Ability ability, Piece attacker)
    {

        // critcal hit 
        float crit = 1f;
        if (Random.value * 100f <= 6.25f)
            crit = 2f;

        //gets type weakness
        float type = TypeChart.GetWeakness(ability.Base.Type, this.Base.Type1) * TypeChart.GetWeakness(ability.Base.Type, this.Base.Type2);


        var damageDetails = new DamageDetails()
        {
            TypeWeakness = type,
            Crit = crit,
            Dead = false
        };

        //checks if the move is ult or not
       float attack = (ability.Base.Category == AbilityCategory.Ultimate) ? attacker.UltAttack : attacker.Attack;
        float defense = (ability.Base.Category == AbilityCategory.Ultimate) ? UltDefense : Defense;

        // damage formula
        float modifiers = Random.Range(0.85f, 1f) * type * crit;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * ability.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HPChange = true;
    }

    // sets the status effect
    public void SetStatus(ConditionID conditionID)
    {

        //prevents status from applying if alredy on
        if (Status != null) return;

        Status = ConditionDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        statusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    // sets the volatile status effect
    public void SetVolatileStatus(ConditionID conditionID)
    {

        //prevents status from applying if alredy on
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        statusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    //returns random ability for enemy
    public Ability GetRandomAbility()
    {
        var abilitesWithAP = Abilities.Where(x => x.AP > 0).ToList();

        int r = Random.Range(0, abilitesWithAP.Count);
        return abilitesWithAP[r];
    }

    public bool OnBeforeAbility()
    {
        bool canPreformAbility = true;
        if (Status?.OnBeforeAbility != null)
        {
            if (!Status.OnBeforeAbility(this))
                canPreformAbility = false;
        }

        if (VolatileStatus?.OnBeforeAbility != null)
        {
            if (!VolatileStatus.OnBeforeAbility(this))
                canPreformAbility = false;
        }

        return canPreformAbility;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

}

public class DamageDetails
{

    public bool Dead { get; set; }
    public float Crit { get; set; }
    public float TypeWeakness { get; set; }

}

[System.Serializable]
public class PieceSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public List<AbilitySaveData> abilities;
}