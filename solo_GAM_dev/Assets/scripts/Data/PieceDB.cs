using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceDB
{
    static Dictionary<string, PieceBase> pieces;

    public static void Init()
    {
        pieces = new Dictionary<string, PieceBase>();

        var PieceArray = Resources.LoadAll<PieceBase>("");
        foreach(var piece in PieceArray)
        {
            if (pieces.ContainsKey(piece.Name))
            {
                Debug.LogError($"two pieces with same name {piece.Name}");
                continue;
            }

            pieces[piece.Name] = piece;
        }
    }

    public static PieceBase GetPieceByName(string name)
    {
        if (!pieces.ContainsKey(name))
        {
            Debug.LogError($"piece with name {name} is not in database");
            return null;
        }

        return pieces[name];
    }
}
