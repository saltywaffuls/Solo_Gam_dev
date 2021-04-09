using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{

    public float moveSpeed;

    private bool isMoving;

    private Vector2 input;

    public LayerMask solidObjectslayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   private void Update()
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
}
