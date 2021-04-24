using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceParty : MonoBehaviour
{
    [SerializeField] List<Piece> pieces;


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
}
