using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] string names;
    [SerializeField] Sprite sprite;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterEnemyView;

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
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    public void OnMoveOver()
    {
        CheckForEncounters();
        CheckIfInEnemyView();
    }

    //looks for encoter when hits colider land tile
    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, GameLayer.i.GrassLayer) != null)
        {
           if (UnityEngine.Random.Range(1, 101) <= 10)
           {
               character.Animator.IsMoving =false;
                OnEncountered();
                Debug.Log("enconter");
           }
        }
    }

    //looks for encoter when hits colider of enemy
    private void CheckIfInEnemyView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayer.i.FovLayer);
        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterEnemyView?.Invoke(collider);
            Debug.Log("i see you");
        }
    }

    public string Name
    {
        get { return names; }
    }

    public Sprite Sprite
    {
        get { return sprite; }
    }
}
