using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Pieces", menuName = "Pieces/Create new pieces")]
public class PieceBase : ScriptableObject
{
    [SerializeField] string pieceName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;


    [SerializeField] PieceType type1;
    [SerializeField] PieceType type2;


    //Base Stats
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int ultAttack;
    [SerializeField] int ultDefense;
    [SerializeField] int speed;


    [SerializeField] List<LearnableAbility> learnableAbilities;


    //propertys
    public string Name
    {
        get { return pieceName; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSpriten
    {
        get { return backSprite; }
    }

    public PieceType Type1
    {
        get { return type1; }
    }

    public PieceType Type2
    {
        get { return type2; }
    }

    public int MaxHP
    {
        get { return maxHP; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int UltAttack
    {
        get { return ultAttack; }
    
    }
    
    public int UltDefense
    {
        get { return ultDefense; }
    }
    
    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableAbility> LearnableAbilities
    {
        get { return learnableAbilities; }
    }

}

//types
public enum PieceType
{
    None,
    Humen,
    Space,
    Time,
    Creation,
    Destruction,
    Light,
    Dark,
}

public enum Stat
{
    Attack,
    Defense,
    UltAttack,
    UltDefense,
    Speed,

    //these 2 are not actual stats, they are used to boost abilityAccuracy
    Accuracy,
    Evasion
}

//what type is effective agenst other types
public class TypeChart
{
    //2d arry
    static float[][] chart =
    {
        // 1f = normal dmg 0.5f = 50% less damage 2 = 50% more damage
        //                  hum spa tim cre des lig dar
        /*hum*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f},
        /*SPA*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f},
        /*Tim*/new float[] {2f, 1f, 1f, 1f, 1f, 1f, 1f},
        /*CRE*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f},
        /*DES*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f},
        /*LIG*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f},
        /*DAR*/new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f},
        
    };

    // gets the stats from the chart
    public static float GetWeakness(PieceType attacktype, PieceType defensetype)
    {
        if (attacktype == PieceType.None || defensetype == PieceType.None)
            return 1;

        int row = (int)attacktype - 1;
        int col = (int)defensetype - 1;

        return chart[row][col];
    }
}


//list of ablitys they can learn
[System.Serializable]
public class LearnableAbility
{
    [SerializeReference] AbilityBase abilityBase;
    [SerializeReference] int level;

    public AbilityBase AbilityBase
    {
        get { return abilityBase; }
    }

    public int Level
    {
        get { return level; }
    }
}