using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeReference] pieceBase _base;
    [SerializeReference] int level;
    [SerializeReference] bool isPlayerUnit;

    public piece Piece { get; set; }


    public void SetUp()
    {
        Piece = new piece(_base, level);
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Piece.Base.BackSpriten;
        else
            GetComponent<Image>().sprite = Piece.Base.FrontSprite;
    }

}
