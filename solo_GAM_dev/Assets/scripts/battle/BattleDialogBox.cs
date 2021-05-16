using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    [SerializeField] Color highlightedColor;

    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject abilitySelector;
    [SerializeField] GameObject abilityDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<Text> actionText;
    [SerializeField] List<Text> abilityText;

    [SerializeField] Text apText;
    [SerializeField] Text typeText;

    [SerializeField] Text yesText;
    [SerializeField] Text noText;

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

        yield return new WaitForSeconds(1f);
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

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    //sets color of action selector
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

    //sets color of ability selcetor
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

        if (ability.AP == 0)
            apText.color = Color.red;
        else
            apText.color = Color.black;
    }

    // shows ability name
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

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            noText.color = highlightedColor;
            yesText.color = Color.black;
        }
    }

}
