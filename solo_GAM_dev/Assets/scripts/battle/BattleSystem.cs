using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//defines states
public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerhud;
    [SerializeField] BattleHud enemyhud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyscreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentAbility;

    PieceParty playerParty;
    Piece wildPiece;

    public void StartBattle(PieceParty playerParty, Piece wildPiece)
    {
        this.playerParty = playerParty;
        this.wildPiece = wildPiece;
        StartCoroutine( SetUpBattle());
    }

    public void HandleUpdate()
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
        playerUnit.SetUp(playerParty.GetHealthyPiece());
        enemyUnit.SetUp(wildPiece);
        playerhud.SetData(playerUnit.Piece);
        enemyhud.SetData(enemyUnit.Piece);

        partyscreen.Init();

        dialogBox.SetAbilityName(playerUnit.Piece.abilities);

        yield return dialogBox.TypeDialog($"a {enemyUnit.Piece.Base.Name} is reveald.");
        

        PlayerAction();
        

    }

    // player turn phase fight/flee
    void PlayerAction()
    {

        state = BattleState.PlayerAction;
        dialogBox.SetDialog("action");
        dialogBox.EnableActionSelector(true);
    }

    //opens the party select ui
    void OpenPartyUI()
    {
        partyscreen.SetPartyData(playerParty.Pieces);
        partyscreen.gameObject.SetActive(true);
    }


    //player attack phase
    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableAbilitySelector(true);
    }


    //use attack player selected
    IEnumerator PerformPlayerAbility()
    {

        state = BattleState.Busy;

        var ability = playerUnit.Piece.abilities[currentAbility];
        ability.AP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Piece.Base.Name} used {ability.Base.Name}");

        

        var damageDetails = enemyUnit.Piece.TakeDamage(ability, playerUnit.Piece);
        yield return enemyhud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Dead)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Piece.Base.Name} dead");


            yield return new WaitForSeconds(2f);
            OnBattleOver(true);

        }
        else
        {
            StartCoroutine(EnemyAbility());
        }
    }

    // runs enemy attack
    IEnumerator EnemyAbility()
    {
        state = BattleState.EnemyMove;

        var ability = enemyUnit.Piece.GetRandomAbility();
        ability.AP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Piece.Base.Name} used {ability.Base.Name}");

        

        var damageDetails = playerUnit.Piece.TakeDamage(ability, enemyUnit.Piece);
        yield return playerhud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Dead)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Piece.Base.Name} dead");


            yield return new WaitForSeconds(2f);

            //sets up next unit in party
           var nextPiece = playerParty.GetHealthyPiece();

            if (nextPiece != null)
            {
                playerUnit.SetUp(nextPiece);
                playerhud.SetData(nextPiece);

                dialogBox.SetAbilityName(nextPiece.abilities);

                yield return dialogBox.TypeDialog($" {nextPiece.Base.Name} is up next.");


                PlayerAction();
            }
            else
            {
                OnBattleOver(false);
            }

        }
        else
        {
            PlayerAction();
        }
    }

    //shows damage Details in dialog box
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Crit > 1f)
            yield return dialogBox.TypeDialog("BIG DAMAGE");

        if (damageDetails.TypeWeakness > 1f)
            yield return dialogBox.TypeDialog("Weak");
        else if (damageDetails.TypeWeakness < 1f)
            yield return dialogBox.TypeDialog("resist");
    }


    // controls for what the player picked run/fight
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

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
                //inventroy
            } 
            else if (currentAction == 2)
            {
                //party
                OpenPartyUI();
            } 
            else if (currentAction == 3)
            {
                //flee
            }
        }

    }


    //controls for what abilty is selected
    void HandleAbilitySelection()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAbility;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAbility;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAbility += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAbility -= 2;

        currentAbility = Mathf.Clamp(currentAbility, 0, playerUnit.Piece.abilities.Count - 1);

        dialogBox.UpdateAbilitySelection(currentAbility , playerUnit.Piece.abilities[currentAbility]);


        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableAbilitySelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerAbility());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableAbilitySelector(false);
            dialogBox.EnableDialogText(true);
            PlayerAction();
        }
    }


}
