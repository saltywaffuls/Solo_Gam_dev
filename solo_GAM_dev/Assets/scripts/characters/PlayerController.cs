using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{

    [SerializeField] string names;
    [SerializeField] Sprite sprite;



    private bool isMoving;
    private Vector2 input;

    
    private Character character;


    private void Awake()
    {
        character = GetComponent<Character>();
    }


    // Update is called once per frame
   public void HandleUpdate()
    {

        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //stops  diagnal movment
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
               StartCoroutine(character.Move(input, OnMoveOver));
            }

        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();

    }

    // colistion for interacting
    void Interact()
    {
        var faceingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var intractPos = transform.position + faceingDir;

        //Debug.DrawLine(transform.position, intractPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(intractPos, 0.3f, GameLayer.i.InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    public void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.offSetY), 0.2f, GameLayer.i.TriggerableLayer);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            pieces = GetComponent<PieceParty>().Pieces.Select(p => p.GetSaveData()).ToList()
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        // restore postion
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        // restore party
        GetComponent<PieceParty>().Pieces = saveData.pieces.Select(s => new Piece(s)).ToList();
    }

    public string Name
    {
        get { return names; }
    }

    public Sprite Sprite
    {
        get { return sprite; }
    }

    public Character Character => character;
}

[Serializable]
public class PlayerSaveData 
{
    public float[] position;
    public List<PieceSaveData> pieces;
}
