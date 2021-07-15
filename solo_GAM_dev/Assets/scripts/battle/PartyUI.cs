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

    
    public void Init(Piece piece)
    {
        _piece = piece;
        UpdateData();

        _piece.OnHPChanged += UpdateData;
    }

    // shows data of pice in ui
    void UpdateData()
    {
        nameText.text = _piece.Base.Name;
        levelText.text = "lvl" + _piece.Level;
        hpBar.SetHP((float)_piece.HP / _piece.MaxHP);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else
            nameText.color = Color.black;
    }
}
