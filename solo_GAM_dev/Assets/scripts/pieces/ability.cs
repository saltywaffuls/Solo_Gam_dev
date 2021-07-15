using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{

    //short way to make propperty does not show vars in inspector
    public AbilityBase Base { get; set; }

    public int AP { get; set; }

    public Ability(AbilityBase pBase)
    {
        Base = pBase;
        AP = pBase.Ap;
    }

    public Ability(AbilitySaveData saveData)
    {
        Base = AbilityDB.GetAbilityByName(saveData.name);
        AP = saveData.ap;
    }

    public AbilitySaveData GetSaveData()
    {
        var saveData = new AbilitySaveData()
        {
            name = Base.Name,
            ap = AP
        };
        return saveData;
    }
}

[System.Serializable]
public class AbilitySaveData
{
    public string name;
    public int ap;
}
