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

    // for ultimate abilitys or in simp terms magic basd on type
    public bool IsUltimate
    {
        get
        {
            // add new  ult typses here 
            if(type == PieceType.Time || type == PieceType.Destruction || type == PieceType.Dark)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}

//18 time stamp 13:22
