using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionUI : MonoBehaviour
{
    [SerializeField] List<Text> abilityText;
    [SerializeField] Color highlightColor;

    int currentSelction = 0;

    public void SetAbilityData(List<AbilityBase> currentAbilites, AbilityBase newAbility)
    {
        for( int i=0; i<currentAbilites.Count; ++i)
        {
            abilityText[i].text = currentAbilites[i].Name;
        }

        abilityText[currentAbilites.Count].text = newAbility.Name;
    }

    public void HandleAbilitySelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelction;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelction;

        currentSelction = Mathf.Clamp(currentSelction, 0, PieceBase.MaxNumOfAbilties);

        UpdateAbilitySelection(currentSelction);

        if (Input.GetKeyDown(KeyCode.Z))
            onSelected?.Invoke(currentSelction);
    }

    public void UpdateAbilitySelection(int selection)
    {
        for (int i = 0; i < PieceBase.MaxNumOfAbilties + 1; i++)
        {
            if (i == selection)
                abilityText[i].color = highlightColor;
            else
                abilityText[i].color = Color.black;
        }
    }
}
