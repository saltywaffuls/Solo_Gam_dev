using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] string names;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject alert;
    [SerializeField] GameObject fov;

    //state
    bool battleLost = false;
    [SerializeField] bool stillAlive = false;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefultDirection);
    }

    private void Update()
    {
        character.HandleUpdate(); 
    }

    public void Interact(Transform initiator)
    {
        character.LookTowerds(initiator.position);

        if (!battleLost)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                GameController.Instance.StartEnemyBattle(this);
            }));
        }
        else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle));
        }
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
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => 
        {
            GameController.Instance.StartEnemyBattle(this);
        }));
    }

    public void BattleLost()
    {
        battleLost = true;
        if(stillAlive)
        {
            fov.gameObject.SetActive(false);
        }
        else
        {
            //this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.SetActive(false);
        }
        
        
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

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        // fix when enemy dies battleLost gets set to true and game obkect turns off. the player then saves and relouds. this turns battle lost back to true tuning on the game object. ep 46 15:12
        if(battleLost)
            this.gameObject.SetActive(false);
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
