using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    public LayerMask solidObjectslayer;
    public LayerMask grassLayer;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input;

    



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   public void HandleUpdate()
    {

        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //stops  diagnal movment
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(IsWalkable(targetPos))
                     StartCoroutine(Move(targetPos));
            }

        }

    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;


        }
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    //colition
    private bool IsWalkable(Vector3 targetPos)
    {

       if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectslayer) != null)
        {

            return false;
        }

        return true;
    }


    //looks for encoter when hits colider
    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
           if (UnityEngine.Random.Range(1, 101) <= 10)
           {
               // animation.SetBool("isMoving", false);
                OnEncountered();
                Debug.Log("enconter");
           }
        }
    }
}
