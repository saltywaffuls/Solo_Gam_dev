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
    [SerializeField] bool trueHit;
    [SerializeField] int ap;
    [SerializeField] AbilityCategory category;
    [SerializeField] AbilityEffects effects;
    [SerializeField] List<SecendaryAbilityEffects> secondaries;
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
    
    public bool TureHit
    {
        get { return trueHit; }
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
    
    public List<SecendaryAbilityEffects> Secondaries
    {
        get { return secondaries; }
    }

    public AbilityTarget Target{
        get { return target; }
    }

}

[System.Serializable]
public class AbilityEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status
    {
        get { return status; }
    } 
    
    public ConditionID VolatileStatus
    {
        get { return volatileStatus; }
    }

}

[System.Serializable]
public class SecendaryAbilityEffects : AbilityEffects
{
    [SerializeField] int chance;
    [SerializeField] AbilityTarget target;

    public int Chance
    {
        get { return chance; }
    }

    public AbilityTarget Target
    {
        get { return target; }
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
