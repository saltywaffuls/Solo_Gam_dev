using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{

   [SerializeField] Text messageText;


    PartyUI[] rosterSlots;


    public void Init()
    {
        rosterSlots = GetComponentsInChildren<PartyUI>();
    }

    public void SetPartyData(List<Piece> pieces)
    {

        for (int i = 0; i < rosterSlots.Length; i++)
        {
            if (i < pieces.Count)
                rosterSlots[i].SetData(pieces[i]);
            else
                rosterSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "swap memeber";

    }

}
