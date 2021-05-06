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


        ChooseFirstTurn();
        

    }

    //picks you goes first based of speed stat
    void ChooseFirstTurn()
    {
        if (playerUnit.Piece.Speed >= enemyUnit.Piece.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyAbility());
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
        playerParty.Pieces.ForEach(p => p.OnBattleOver());
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

        bool canRunAbility = sourceUnit.Piece.OnBeforeAbility();
        if (!canRunAbility)
        {
            yield return ShowStatusChanges(sourceUnit.Piece);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Piece);

        ability.AP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Piece.Base.Name} used {ability.Base.Name}");

        if (CheckIfAbilityHit(ability, sourceUnit.Piece, targetUnit.Piece))
        {

            //sourceUnit.PlayAttackAnimation();
            //yield return new WaitForSeconds(1f);
            //targetUnit.PlayHitAnimation();

            //checks to see if the move is a status effect or not
            if (ability.Base.Category == AbilityCategory.Status)
            {
                yield return RunAbilityEffects(ability.Base.Effects, sourceUnit.Piece, targetUnit.Piece, ability.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Piece.TakeDamage(ability, sourceUnit.Piece);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (ability.Base.Secondaries != null && ability.Base.Secondaries.Count > 0 && targetUnit.Piece.HP > 0)
            {
                foreach(var secondary in ability.Base.Secondaries)
                {
                    var rng = UnityEngine.Random.Range(1, 101);
                    if(rng <= secondary.Chance)
                        yield return RunAbilityEffects(secondary, sourceUnit.Piece, targetUnit.Piece, secondary.Target);
                }
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
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Piece.Base.Name} attack did not land");
        }

        //statuses like burn(DOTS) will hurt after a turn
        sourceUnit.Piece.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Piece);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Piece.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Piece.Base.Name} dead");
            //targetUnit.PlayDeathAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);

        }
    }

    //shows stat message in dialog box
    IEnumerator ShowStatusChanges(Piece piece)
    {
        while (piece.statusChanges.Count > 0)
        {
            var message = piece.statusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
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

    // runs debuff & buffs
    IEnumerator RunAbilityEffects(AbilityEffects effects, Piece source, Piece target, AbilityTarget abilityTarget)
    {
        //stat boosting
        if (effects.Boosts != null)
        {
            if (abilityTarget == AbilityTarget.Self)
                source.ApplyBoost(effects.Boosts);
            else
                target.ApplyBoost(effects.Boosts);
        }

        // cheecks to see what type of status condtion effect
        if(effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        // cheecks to see what type of volatile status condtion effect
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        //calls function to show after applying stat
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    // cheecks the acrecy of ability
    bool CheckIfAbilityHit(Ability ability, Piece source, Piece target)
    {

        if (ability.Base.TureHit)
            return true;

        float abilityAccuracy = ability.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            abilityAccuracy *= boostValues[accuracy];
        else
            abilityAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            abilityAccuracy /= boostValues[evasion];
        else
            abilityAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= abilityAccuracy;
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
        bool currentPieceDead = true;
        if (playerUnit.Piece.HP > 0)
        {
            currentPieceDead = false;
            yield return dialogBox.TypeDialog($"tagging out {playerUnit.Piece.Base.name}");
            //playerUnit.PlayDeathAnimation();
            yield return new WaitForSeconds(2);
        }

        playerUnit.SetUp(newPiece);

        dialogBox.SetAbilityName(newPiece.abilities);

        yield return dialogBox.TypeDialog($" {newPiece.Base.Name} is up next.");

        if (currentPieceDead)
            ChooseFirstTurn();
        else
            AbilitySelection();
    }


}
