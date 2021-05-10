using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{

    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;
    

    public bool IsPlayerUnit
    {
        get
        {
            return isPlayerUnit;
        }
    }

    public BattleHud Hud
    {
        get
        {
            return hud;
        }
    }

    public Piece Piece { get; set; }

    // sets up what pices are being used
    public void SetUp(Piece piece)
    {
        Piece = piece;
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Piece.Base.BackSpriten;
        else
            GetComponent<Image>().sprite = Piece.Base.FrontSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(piece);

        //eps 12 timestamp 12:50
        //image.color = originalColor;
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

}
