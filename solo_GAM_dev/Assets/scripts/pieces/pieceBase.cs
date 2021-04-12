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

    public List<LearnableAbility> LearnableAbilities
    {
        get { return learnableAbilities; }
    }

}

//types
public enum PieceType
{
    None,
    Space,
    Time,
    Creation,
    Destruction,
    Light,
    Dark,
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