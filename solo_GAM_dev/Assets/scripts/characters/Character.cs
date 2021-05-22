using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterAnimator animator;

    public float moveSpeed;
    public bool IsMoving { get; private set; }

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.x, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;


        // rplace with !IsPathClear
        if (!IsWalkable(targetPos))
            yield break;

        // ep 29 6:50
        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;


        }
        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    //colition ep 30 some type of bug players cant move up and down 
    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayer.i.SolidObjectsLayer | GameLayer.i.InteractableLayer | GameLayer.i.PlayerLayer) == true)
            return false;

        return true;
    }

    //colition
    private bool IsWalkable(Vector3 targetPos)
    {

        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayer.i.SolidObjectsLayer | GameLayer.i.InteractableLayer) != null)
        {

            return false;
        }

        return true;
    }

    public void LookTowerds(Vector3 targetPos)
    {
        var xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xDiff == 0 || yDiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        }
        else
            Debug.LogError("erroe in look towards: you can't look diagonally");
    }

    public CharacterAnimator Animator
    {
        get => animator;
    }

}
