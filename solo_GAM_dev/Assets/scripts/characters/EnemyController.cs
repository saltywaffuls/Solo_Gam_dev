using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject alert;
    [SerializeField] GameObject fov;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefultDirection);
    }

    public IEnumerator TriggerEnemyBattle(PlayerController player)
    {
        //shows alet
        alert.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        alert.SetActive(false);

        //moves npc to player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // show dialog ep 31 13:35 MAYBE A PROBLEM HERE
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
