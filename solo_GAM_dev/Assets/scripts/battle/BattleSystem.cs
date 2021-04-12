using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//defines states
public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{

    [SerializeReference] BattleUnit playerUnit;
    [SerializeReference] BattleUnit enemyUnit;
    [SerializeReference] BattleHud playerhud;
    [SerializeReference] BattleHud enemyhud;
    [SerializeReference] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentAbility;


    private void Start()
    {
        StartCoroutine( SetUpBattle());
    }

    private void Update()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleAbilitySelection();
        }
    }

    public IEnumerator SetUpBattle()
    {
        playerUnit.SetUp();
        enemyUnit.SetUp();
        playerhud.SetData(playerUnit.Piece);
        enemyhud.SetData(enemyUnit.Piece);

        dialogBox.SetAbilityName(playerUnit.Piece.abilities);

        yield return dialogBox.TypeDialog($"a {enemyUnit.Piece.Base.Name} is reveald.");
        yield return new WaitForSeconds(1f);

        PlayerAction();
        

    }

    void PlayerAction()
    {

        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableAbilitySelector(true);
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //fight
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //flee
            }
        }

    }

    void HandleAbilitySelection()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentAbility < playerUnit.Piece.abilities.Count - 1)
                ++currentAbility;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentAbility > 0)
                --currentAbility;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAbility < playerUnit.Piece.abilities.Count - 2)
                currentAbility += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAbility > 1)
                currentAbility -= 2;
        }

        dialogBox.UpdateAbilitySelection(currentAbility , playerUnit.Piece.abilities[currentAbility]);
    }


}
