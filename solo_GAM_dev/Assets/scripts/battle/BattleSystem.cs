using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//defines states
public enum BattleState { Start, ActionSelection, AbilitySelection, PreformAbility, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyscreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentAbility;
    int currentUnit;

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
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.AbilitySelection)
        {
            HandleAbilitySelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    public IEnumerator SetUpBattle()
    {
        playerUnit.SetUp(playerParty.GetHealthyPiece());
        enemyUnit.SetUp(wildPiece);

        partyscreen.Init();

        dialogBox.SetAbilityName(playerUnit.Piece.abilities);

        yield return dialogBox.TypeDialog($"a {enemyUnit.Piece.Base.Name} is reveald.");
        

        ActionSelection();
        

    }

    // player turn phase fight/flee
    void ActionSelection()
    {

        state = BattleState.ActionSelection;
        dialogBox.SetDialog("action");
        dialogBox.EnableActionSelector(true);
    }


    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }


    //opens the party select ui
    void OpenPartyUI()
    {

        state = BattleState.PartyScreen;
        partyscreen.SetPartyData(playerParty.Pieces);
        partyscreen.gameObject.SetActive(true);
    }


    //player attack phase
    void AbilitySelection()
    {
        state = BattleState.AbilitySelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableAbilitySelector(true);
    }


    //use attack player selected
    IEnumerator PlayerAbility()
    {

        state = BattleState.PreformAbility;

        var ability = playerUnit.Piece.abilities[currentAbility];
        yield return RunAbility(playerUnit, enemyUnit, ability);
        
        //if the battle stat was not changed by RunAbility,  then go to next step
        if (state == BattleState.PreformAbility)
            StartCoroutine(EnemyAbility());
        
    }

    // runs enemy attack
    IEnumerator EnemyAbility()
    {
        state = BattleState.PreformAbility;

        var ability = enemyUnit.Piece.GetRandomAbility();
        yield return RunAbility(enemyUnit, playerUnit, ability);

        if (state == BattleState.PreformAbility)
            ActionSelection();
        
    }

    // runs the ability
    IEnumerator RunAbility(BattleUnit sourceUnit, BattleUnit targetUnit, Ability ability)
    {
       
        ability.AP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Piece.Base.Name} used {ability.Base.Name}");

        //sourceUnit.PlayAttackAnimation();
        //yield return new WaitForSeconds(1f);

        //targetUnit.PlayHitAnimation();

        //checks to see if the move is a status effect or not
        if(ability.Base.Category == AbilityCategory.Status)
        {
            //cheacks to see what type of effect
            var effects = ability.Base.Effects;
            if (effects.Boosts != null)
            {
                if (ability.Base.Target == AbilityTarget.Self)
                    sourceUnit.Piece.ApplyBoost(effects.Boosts);
                else
                    targetUnit.Piece.ApplyBoost(effects.Boosts);
            }
        }
        else
        {
            var damageDetails = targetUnit.Piece.TakeDamage(ability, sourceUnit.Piece);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        //cheeks to see if its dead
        if (targetUnit.Piece.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Piece.Base.Name} dead");
            //targetUnit.PlayDeathAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);

        }
    }

    void CheckForBattleOver(BattleUnit deadUnit)
    {
        if (deadUnit.IsPlayerUnit)
        {
            //sets up next unit in party
            var nextPiece = playerParty.GetHealthyPiece();

            if (nextPiece != null)
                OpenPartyUI();
            else
                BattleOver(false);
        }
        else
            BattleOver(true);
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
                AbilitySelection();
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
            StartCoroutine(PlayerAbility());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableAbilitySelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    //controls party selector
    void HandlePartySelection()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentUnit;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentUnit;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentUnit += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentUnit -= 2;

        currentUnit = Mathf.Clamp(currentUnit, 0, playerParty.Pieces.Count - 1);

        partyscreen.UpdateUnitSelection(currentUnit);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedUnit = playerParty.Pieces[currentUnit];
            if (selectedUnit.HP <= 0)
            {
                partyscreen.SetMessageText("he is no loger with us");
                return;
            }
            if (selectedUnit == playerUnit.Piece)
            {
                partyscreen.SetMessageText("he already in the fight");
                return;
            }

            partyscreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchUnit(selectedUnit));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyscreen.gameObject.SetActive(false);
            ActionSelection();
        }

    }

    //switches out the pices in play
    IEnumerator SwitchUnit(Piece newPiece)
    {

        if (playerUnit.Piece.HP > 0)
        {
            yield return dialogBox.TypeDialog($"tagging out {playerUnit.Piece.Base.name}");
            //playerUnit.PlayDeathAnimation();
            yield return new WaitForSeconds(2);
        }

        playerUnit.SetUp(newPiece);

        dialogBox.SetAbilityName(newPiece.abilities);

        yield return dialogBox.TypeDialog($" {newPiece.Base.Name} is up next.");

        AbilitySelection();
    }


}
