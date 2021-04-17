using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeReference] PieceBase _base;
    [SerializeReference] int level;
    [SerializeReference] bool isPlayerUnit;

    public Piece Piece { get; set; }

    // sets up what pices are being used
    public void SetUp()
    {
        Piece = new Piece(_base, level);
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Piece.Base.BackSpriten;
        else
            GetComponent<Image>().sprite = Piece.Base.FrontSprite;
    }

}
