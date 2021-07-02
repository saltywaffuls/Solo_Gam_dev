using DG.Tweening;
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
    [SerializeField] GameObject expBar;

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
        SetLevel();
        hpBar.SetHP((float)piece.HP / piece.MaxHP);
        SetExp();

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
        if (_piece.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _piece.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_piece.Status.Id];
        }
    }

    public void SetLevel()
    {
        levelText.text = "lvl" + _piece.Level;
    }

    //updats exp bar
    public void SetExp()
    {
        if (expBar == null) return;

        float nomalizeExp = GetNormalizeExp();
        expBar.transform.localScale = new Vector3(nomalizeExp, 1, 1);

    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float nomalizeExp = GetNormalizeExp();
        yield return expBar.transform.DOScaleX(nomalizeExp, 1.5f).WaitForCompletion();

    }

    float GetNormalizeExp()
    {
        int currentLevelExp = _piece.Base.GetExpForLevel(_piece.Level);
        int nextLevelExp = _piece.Base.GetExpForLevel(_piece.Level + 1);

        float normalizeExp = (float)(_piece.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizeExp);
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
