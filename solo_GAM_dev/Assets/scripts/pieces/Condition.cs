using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Action<Piece> OnAfterTurn { get; set; }
    public Action<Piece> OnStart { get; set; }
    public Func<Piece, bool> OnBeforeAbility { get; set; }
}
