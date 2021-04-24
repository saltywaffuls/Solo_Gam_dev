using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    //probaly dont want this but for ref ep14 timestamp 6:14

    [SerializeField] List<Piece> wildPieces;

    public Piece GetRandomWildPiece()
    {
        var WildPiece = wildPieces[Random.Range(0, wildPieces.Count)];
        WildPiece.Init();
        return WildPiece;
    }

}
