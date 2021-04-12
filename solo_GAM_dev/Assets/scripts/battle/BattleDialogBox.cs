using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeReference] int letterPerSecond;
    [SerializeReference] Color highlightedColor;

    [SerializeReference] Text dialogText;
    [SerializeReference] GameObject actionSelector;
    [SerializeReference] GameObject abilitySelector;
    [SerializeReference] GameObject abilityDetails;

    [SerializeReference] List<Text> actionText;
    [SerializeReference] List<Text> abilityText;

    [SerializeReference] Text apText;
    [SerializeReference] Text typeText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    //animates text and how long it takes to appre
    public IEnumerator TypeDialog(string dialog)
    {

        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/letterPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableAbilitySelector(bool enabled)
    {
        abilitySelector.SetActive(enabled);
        abilityDetails.SetActive(enabled);
    }


    //sets color
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i=0; i<actionText.Count; ++i)
        {
            if (i == selectedAction)
                actionText[i].color = highlightedColor;
            else
                actionText[i].color = Color.black;
        }

        
    }

    //sets color
    public void UpdateAbilitySelection(int selectedAbility, Ability ability)
    {
        for (int i = 0; i < abilityText.Count; ++i)
        {
            if (i == selectedAbility)
                abilityText[i].color = highlightedColor;
            else
                abilityText[i].color = Color.black;
        }

        apText.text = $"AP {ability.AP}/{ability.Base.Ap}";
        typeText.text = ability.Base.Type.ToString();
    }

    public void SetAbilityName(List<Ability> abilities)
    {
        for (int i=0; i<abilityText.Count; ++i)
        {
            if (i < abilities.Count)
                abilityText[i].text = abilities[i].Base.Name;
            else
                abilityText[i].text = "-";
        }
    }

}
