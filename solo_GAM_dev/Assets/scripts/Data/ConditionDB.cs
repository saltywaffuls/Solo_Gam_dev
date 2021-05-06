using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{

    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    //status conditons moves
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() 
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "poison",
                StartMessage = "not feeling to well",
                OnAfterTurn = (Piece piece) =>
                {
                    piece.UpdateHP(piece.MaxHP / 8);
                    piece.statusChanges.Enqueue($"{piece.Base.Name} Dot has ticked");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "burn",
                StartMessage = "start flaming",
                OnAfterTurn = (Piece piece) =>
                {
                    piece.UpdateHP(piece.MaxHP / 16);
                    piece.statusChanges.Enqueue($"{piece.Base.Name} is being rosted");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "paralyzed",
                StartMessage = "is overwelmdd",
                OnBeforeAbility = (Piece piece) =>
                {
                   if (Random.Range(1, 5) == 1)
                    {
                        piece.statusChanges.Enqueue($"{piece.Base.Name} is scared");
                        return false;
                    }

                    return true;
                }
            }
        },
         {
            ConditionID.frz,
            new Condition()
            {
                Name = "frozen",
                StartMessage = "is on ice",
                OnBeforeAbility = (Piece piece) =>
                {
                   if (Random.Range(1, 5) == 1)
                    {
                        piece.CureStatus();
                        piece.statusChanges.Enqueue($"{piece.Base.Name} has chilled out");
                        return true;
                    }

                    return false;
                }
            }
        },
         {
            ConditionID.slp,
            new Condition()
            {
                Name = "sleep",
                StartMessage = "is on dreaming",
                OnStart = (Piece piece) =>
                {
                    // sleep for 1-3 turns
                    piece.statusTime = Random.Range(1, 4);
                    Debug.Log($"sleep for {piece.statusTime} ");
                },
                OnBeforeAbility = (Piece piece) =>
                {
                    if(piece.statusTime <= 0)
                    {
                        piece.CureStatus();
                        piece.statusChanges.Enqueue($"{piece.Base.Name} had a good nap");
                        return true;
                    }

                    piece.statusTime--;
                    piece.statusChanges.Enqueue($"{piece.Base.Name} is dreaming");
                    return false;
                }
            }
         },

         // Volati;e status condition
         {
            ConditionID.confusion,
            new Condition()
            {
                Name = "confusion",
                StartMessage = "is confused",
                OnStart = (Piece piece) =>
                {
                    // sleep for 1-4 turns
                    piece.VolatileStatusTime = Random.Range(1, 5);
                    Debug.Log($"confusion for {piece.VolatileStatusTime} ");
                },
                OnBeforeAbility = (Piece piece) =>
                {
                    if(piece.VolatileStatusTime <= 0)
                    {
                        piece.CureVolatileStatus();
                        piece.statusChanges.Enqueue($"{piece.Base.Name} can see stright");
                        return true;
                    }
                    piece.VolatileStatusTime--;

                    //50% change to use ability
                    if(Random.Range(1,3) ==1)
                        return true;

                    // hurt by confution
                    piece.statusChanges.Enqueue($"{piece.Base.Name} is confused");
                    piece.UpdateHP(piece.MaxHP / 8);
                    piece.statusChanges.Enqueue($"hit itself");
                    return false;
                }
            }
         },
    };
}

// all stat condtions
public enum ConditionID
{
    // ep20 timestamp 2:44
   none, psn, brn, slp, par, frz,
   confusion
}
