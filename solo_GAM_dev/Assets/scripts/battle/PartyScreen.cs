using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{

   [SerializeField] Text messageText;


    PartyUI[] rosterSlots;
    List<Piece> pieces;


    public void Init()
    {
        rosterSlots = GetComponentsInChildren<PartyUI>();
    }

    public void SetPartyData(List<Piece> pieces)
    {

        this.pieces = pieces;

        for (int i = 0; i < rosterSlots.Length; i++)
        {
            if (i < pieces.Count)
                rosterSlots[i].SetData(pieces[i]);
            else
                rosterSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "swap unit";

    }

    public void UpdateUnitSelection(int selectedUnit)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (i == selectedUnit)
                rosterSlots[i].SetSelected(true);
            else
                rosterSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }


}
