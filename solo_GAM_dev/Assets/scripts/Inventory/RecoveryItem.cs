using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool resotoreMaxHP;

    [Header("AP")]
    [SerializeField] int apAmount;
    [SerializeField] bool resotoreMaxAP;

    [Header("status")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("rez")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Piece piece)
    {
        if(hpAmount > 0)
        {
            if (piece.HP == piece.MaxHP)
                return false;

            piece.IecreaseHP(hpAmount);
        }

        return true;
    }
}
