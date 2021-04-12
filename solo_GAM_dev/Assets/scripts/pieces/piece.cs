using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class piece
{
   public pieceBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    public List<ability> abilitys { get; set; }

    public piece(pieceBase pBase, int plevel)
    {
        Base = pBase;
        Level = plevel;
        HP = MaxHP;


        // generate abilties
        abilitys = new List<ability>();
        foreach (var ability in Base.LearnableAbilities)
        {
            if (ability.Level <= Level)
                abilitys.Add(new ability(ability.AbilityBase));

            if (abilitys.Count >= 4)
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

}
