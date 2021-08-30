using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new key item")]
public class KeyItems : ItemBase
{
    public override bool Use(Piece piece)
    {
        return true;
    }
}
