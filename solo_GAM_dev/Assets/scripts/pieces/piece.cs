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

    public void Init()
    {
        HP = MaxHP;


        // generate abilties
        abilities = new List<Ability>();
        foreach (var ability in Base.LearnableAbilities)
        {
            if (ability.Level <= Level)
                abilities.Add(new Ability(ability.AbilityBase));

            if (abilities.Count >= 4)
                break;
        }
    }

    //property level formulas
    public int Attack
    {
                  //base attack mutpled by the level divided by 100 add five
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 ; }
    }


    public int MaxHP
    {
        
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10; }
    }

    public int Defense
    {
        
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int UltAttack
    {
        //base attack mutpled by the level divided by 100 add five
        get { return Mathf.FloorToInt((Base.UltAttack * Level) / 100f) + 5; }
    }

    public int UltDefense
    {
        //base attack mutpled by the level divided by 100 add five
        get { return Mathf.FloorToInt((Base.UltDefense * Level) / 100f) + 5; }
    }

    public int Speed
    {
        //base attack mutpled by the level divided by 100 add five
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
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