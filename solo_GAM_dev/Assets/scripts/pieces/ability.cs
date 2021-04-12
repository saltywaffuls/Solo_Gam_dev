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

}
