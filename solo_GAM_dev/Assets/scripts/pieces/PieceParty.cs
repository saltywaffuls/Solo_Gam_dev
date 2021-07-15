using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceParty : MonoBehaviour
{
    [SerializeField] List<Piece> pieces;

    public event Action onUpdated;

    public List<Piece> Pieces
    {
        get
        {
            return pieces;
        }
        set
        {
            pieces = value;
            onUpdated?.Invoke();
        }
    }


    private void Start()
    {
        foreach (var piece in pieces)
        {
            piece.Init();
        }
    }


    public Piece GetHealthyPiece()
    {
        // where loops thruoge thw list ep14 timestamp 11:40
       return pieces.Where(x => x.HP > 0).FirstOrDefault();
    }

    // skip the ep this wass added. hoevwer there is an update in ep 54 14:44
    public void AddPiece(Piece newPiece)
    {
        if(pieces.Count < 4)
        {
            pieces.Add(newPiece);
            onUpdated?.Invoke();
        }
        else
        {
            // to do add to PC once implmented
        }
    }

    public static PieceParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PieceParty>();
    }
}
