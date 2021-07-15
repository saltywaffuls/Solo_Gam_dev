using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{

   [SerializeField] Text messageText;


    PartyUI[] rosterSlots;
    List<Piece> pieces;
    PieceParty party;

    int selection = 0;
    public Piece SelectedUnit => pieces[selection];

    /// <summary>
    /// party screen can be called from diffrent states like ActionSelection, RunningTurns, AboutToUse
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        rosterSlots = GetComponentsInChildren<PartyUI>();

        party = PieceParty.GetPlayerParty();
        SetPartyData();

        party.onUpdated += SetPartyData;
    }

    public void SetPartyData()
    {

        pieces = party.Pieces;

        for (int i = 0; i < rosterSlots.Length; i++)
        {
            if (i < pieces.Count)
                rosterSlots[i].Init(pieces[i]);
            else
                rosterSlots[i].gameObject.SetActive(false);
        }

        UpdateUnitSelection(selection);

        messageText.text = "swap unit";

    }

    //controls party selector
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selection -= 2;

        selection = Mathf.Clamp(selection, 0, pieces.Count - 1);

        if(selection != prevSelection)
            UpdateUnitSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }

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
