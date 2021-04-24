using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    
    [SerializeReference] bool isPlayerUnit;

    public Piece Piece { get; set; }

    // sets up what pices are being used
    public void SetUp(Piece piece)
    {
        Piece = piece;
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Piece.Base.BackSpriten;
        else
            GetComponent<Image>().sprite = Piece.Base.FrontSprite;

        //eps 12 timestamp 12:50
        //image.color = originalColor;
    }

}
