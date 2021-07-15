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
            ConditionID.dot,
            new Condition()
            {
                Name = "damage over time",
                StartMessage = "A Dot is appied",
                OnAfterTurn = (Piece piece) =>
                {
                    piece.DecreaseHP(piece.MaxHP / 8);
                    piece.statusChanges.Enqueue($"{piece.Base.Name} Dot has ticked");
                }
            }
        },

        {
            ConditionID.sdot,
            new Condition()
            {
                Name = "super damage over time",
                StartMessage = "super dot",
                OnAfterTurn = (Piece piece) =>
                {
                    piece.DecreaseHP(piece.MaxHP / 16);
                    piece.statusChanges.Enqueue($"{piece.Base.Name} very soul hurts");
                }
            }
        },

        {
            ConditionID.fer,
            new Condition()
            {
                Name = "fear",
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
                    piece.DecreaseHP(piece.MaxHP / 8);
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
   none, dot, sdot, slp, fer, frz,
   confusion
}
