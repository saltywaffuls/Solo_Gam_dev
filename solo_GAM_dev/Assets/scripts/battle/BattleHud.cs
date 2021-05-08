using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Color dotColor;
    [SerializeField] Color sdotColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color ferColor;
    [SerializeField] Color frzColor;

    Piece _piece;

    Dictionary<ConditionID, Color> statusColors;

    // shows data of pice in ui
    public void SetData(Piece piece)
    {
        _piece = piece;

        nameText.text = piece.Base.Name;
        levelText.text = "lvl" + piece.Level;
        hpBar.SetHP((float)piece.HP / piece.MaxHP);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.dot, dotColor },
            {ConditionID.sdot, sdotColor },
            {ConditionID.slp, slpColor },
            {ConditionID.fer, ferColor },
            {ConditionID.frz, frzColor },
        };

        SetStatusText();
        _piece.OnStatusChanged += SetStatusText;
    }

    //sets the text for what status
    void SetStatusText()
    {
        if (_piece.status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _piece.status.Id.ToString().ToUpper();
            statusText.color = statusColors[_piece.status.Id];
        }
    }

    // updates HP bar
    public IEnumerator UpdateHP()
    {
        if (_piece.HPChange)
        {
            yield return hpBar.SetHPSmooth((float)_piece.HP / _piece.MaxHP);
            _piece.HPChange = false;
        }
       
    }

}
