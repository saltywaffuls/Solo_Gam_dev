using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ability
{

    //short way to make propperty does not show vars in inspector
    public abilityBase Base { get; set; }

    public int AP { get; set; }

    public ability(abilityBase pBase)
    {
        Base = pBase;
        AP = pBase.Ap;
    }

}
