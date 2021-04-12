using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{

    [SerializeReference] Text nameText;
    [SerializeReference] Text levelText;
    [SerializeReference] HPBar hpBar;

    public void SetData(Piece piece)
    {
        nameText.text = piece.Base.Name;
        levelText.text = "lvl" + piece.Level;
        hpBar.SetHP((float)piece.HP / piece.MaxHP);
    }

}
