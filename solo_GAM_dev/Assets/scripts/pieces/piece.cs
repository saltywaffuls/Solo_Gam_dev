using System.Collections;
using System.Collections.Generic;
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

    public int HP { get; set; }
    public List<Ability> abilities { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public void Init()
    {
        
        


        // generate abilties
        abilities = new List<Ability>();
        foreach (var ability in Base.LearnableAbilities)
        {
            if (ability.Level <= Level)
                abilities.Add(new Ability(ability.AbilityBase));

            if (abilities.Count >= 4)
                break;
        }

        CalculateStates();
        HP = MaxHP;

        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.UltAttack, 0},
            {Stat.UltDefense, 0},
            {Stat.Speed, 0},
        };
    }

    void CalculateStates()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 );
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5 );
        Stats.Add(Stat.UltAttack, Mathf.FloorToInt((Base.UltAttack * Level) / 100f) + 5 );
        Stats.Add(Stat.UltDefense, Mathf.FloorToInt((Base.UltDefense * Level) / 100f) + 5 );
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5 );

        MaxHP =  Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10;
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

    //property level formulas
    public int Attack
    {
        //base attack mutpled by the level divided by 100 add five
        get { return GetStat(Stat.Attack); }
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
       float attack = (ability.Base.IsUltimate) ? attacker.UltAttack : attacker.Attack;
        float defense = (ability.Base.IsUltimate) ? UltDefense : Defense;

        // damage formula
        float modifiers = Random.Range(0.85f, 1f) * type * crit;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * ability.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        //checks if its dead
        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            damageDetails.Dead = true;
        }
        return damageDetails;
    }

    //returns random ability for enemy
    public Ability GetRandomAbility()
    {
        int r = Random.Range(0, abilities.Count);
        return abilities[r];
    }

}

public class DamageDetails
{

    public bool Dead { get; set; }
    public float Crit { get; set; }
    public float TypeWeakness { get; set; }

}