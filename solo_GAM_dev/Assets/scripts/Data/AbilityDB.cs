using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDB
{
    static Dictionary<string, AbilityBase> abilities;

    public static void Init()
    {
        abilities = new Dictionary<string, AbilityBase>();

        var abilityArray = Resources.LoadAll<AbilityBase>("");
        foreach(var ability in abilityArray)
        {
            if (abilities.ContainsKey(ability.Name))
            {
                Debug.LogError($"two ability with same name {ability.Name}");
                continue;
            }

            abilities[ability.Name] = ability;
        }
    }

    public static AbilityBase GetAbilityByName(string name)
    {
        if (!abilities.ContainsKey(name))
        {
            Debug.LogError($"ability with name {name} is not in database");
            return null;
        }

        return abilities[name];
    }
}
