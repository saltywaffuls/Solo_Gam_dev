using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Abilitys", menuName = "Pieces/Create new abilitys")]
public class AbilityBase : ScriptableObject
{

    [SerializeField] string abilityName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PieceType type;


    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int ap;
    [SerializeField] AbilityCategory category;
    [SerializeField] AbilityEffects effects;
    [SerializeField] AbilityTarget target;

    //propertys
    public string Name
    {
        get { return abilityName; }
    }

    public string Description
    {
        get { return description; }
    }

    public PieceType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int Ap
    {
        get { return ap; }
    }

    public AbilityCategory Category {
        get { return category; }
    }

    public AbilityEffects Effects{
        get { return effects; }
    }
    
    public AbilityTarget Target{
        get { return target; }
    }

}

[System.Serializable]
public class AbilityEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
}

// class shows list in inspecter
[System.Serializable]
public class StatBoost 
{
    public Stat stat;
    public int boost;
}


// what type of ability it is
public enum AbilityCategory
{
    Physical, Ultimate, Status
}

public enum AbilityTarget
{
    Foe, Self
}
