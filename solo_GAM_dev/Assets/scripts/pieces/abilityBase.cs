using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Abilitys", menuName = "Pieces/Create new abilitys")]
public class abilityBase : ScriptableObject
{

    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PieceType type;


    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int ap;

    //propertys
    public string Name
    {
        get { return name; }
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


}
