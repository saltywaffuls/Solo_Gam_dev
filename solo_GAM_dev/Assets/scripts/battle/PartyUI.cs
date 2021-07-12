using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour
{

    [SerializeReference] Text nameText;
    [SerializeReference] Text levelText;
    [SerializeReference] HPBar hpBar;

    Piece _piece;

    // shows data of pice in ui
    public void SetData(Piece piece)
    {
        _piece = piece;

        nameText.text = piece.Base.Name;
        levelText.text = "lvl" + piece.Level;
        hpBar.SetHP((float)piece.HP / piece.MaxHP);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else
            nameText.color = Color.black;
    }
}
