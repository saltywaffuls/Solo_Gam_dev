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

        // revieves piece
        if (revive || maxRevive)
        {
            if (piece.HP > 0)
                return false;

            if (revive)
                piece.IecreaseHP(piece.MaxHP / 2);
            else if (maxRevive)
                piece.IecreaseHP(piece.MaxHP);

            piece.CureStatus();

            return true;
        }

        // no other items can be used on dead unit
        if (piece.HP == 0)
            return false;

        //resores HP and or maxHp
        if(resotoreMaxHP || hpAmount > 0)
        {
            if (piece.HP == piece.MaxHP)
                return false;

            if (resotoreMaxHP)
                piece.IecreaseHP(piece.MaxHP);
            else
                piece.IecreaseHP(hpAmount);
        }

        //recover status
        if(recoverAllStatus || status != ConditionID.none)
        {
            if (piece.Status == null && piece.VolatileStatus != null)
                return false;


            if (recoverAllStatus)
            {
                piece.CureStatus();
                piece.CureVolatileStatus();
            }
            else
            {
                if (piece.Status.Id == status)
                    piece.CureStatus();
                else if (piece.VolatileStatus.Id == status)
                    piece.CureVolatileStatus();
                else
                    return false;
            }
        }

        //restore AP
        if (resotoreMaxAP)
        {
            piece.Abilities.ForEach(m => m.IncreasAP(m.Base.Ap));
        }
        else if (apAmount > 0)
        {
            piece.Abilities.ForEach(m => m.IncreasAP(apAmount));
        }

        return true;
    }
}
