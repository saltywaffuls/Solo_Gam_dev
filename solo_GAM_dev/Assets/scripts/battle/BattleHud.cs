using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
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

    // updates HP bar
    public IEnumerator UpdateHP()
    {
       yield return hpBar.SetHPSmooth((float)_piece.HP / _piece.MaxHP);
    }

}
