using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{

    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movmentPattern;
    [SerializeField] float timeBetweenPattern;

    NPCState state;
    float idleTimer = 0f;
    int currentMovePattern = 0;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowerds(initiator.position);

            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => 
            {
                idleTimer = 0;
                state = NPCState.Idle;
            }));
        }
            
        
    }

    private void Update()
    {
        if(state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if(movmentPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }
    
    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movmentPattern[currentMovePattern]);

        if(transform.position != oldPos)
           currentMovePattern = (currentMovePattern + 1) % movmentPattern.Count;

        state = NPCState.Idle;
    }

}

public enum NPCState { Idle, Walking, Dialog}
