using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//defines states
public enum BattleState { Start, ActionSelection, AbilitySelection, RunningTurn, Busy, PartyScreen, AboutToUse, AbilityForget, BattleOver }
public enum BattleAction { Ability, SwitchPiece, Useitem, flee}

public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyscreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image enemyImage;
    [SerializeField] AbilitySelectionUI AbilitySelectionUI;

    public event Action<bool> OnBattleOver;

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentAbility;
    int currentUnit;
    bool aboutToUseChoice = true;

    PieceParty playerParty;
    PieceParty enemyParty;
    Piece wildPiece;

    bool isEnemyBattle = false;

    PlayerController player;
    EnemyController enemy;

    int escapeAttempts;
    AbilityBase abilityToLearn;

    public void StartBattle(PieceParty playerParty, Piece wildPiece)
    {
        this.playerParty = playerParty;
        this.wildPiece = wildPiece;
        isEnemyBattle = false;
        StartCoroutine( SetUpBattle());
    }

    public void StartEnemyBattle(PieceParty playerParty, PieceParty enemyParty)
    {
        this.playerParty = playerParty;
        this.enemyParty = enemyParty;

        isEnemyBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        enemy = enemyParty.GetComponent<EnemyController>();

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
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.AbilityForget)
        {
            Action<int> onAbilitySelected = (abilityIndex) =>
            {
                AbilitySelectionUI.gameObject.SetActive(false);
                if (abilityIndex == PieceBase.MaxNumOfAbilties)
                {
                    //dont leanr ability
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Piece.Base.Name} did not devlope {abilityToLearn.Name}"));
                }
                else
                {
                    //forget move learns it
                    var selectedAbility = playerUnit.Piece.abilities[abilityIndex].Base;
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Piece.Base.Name} scrapted {selectedAbility.Name} and devloped {abilityToLearn.Name}"));

                    playerUnit.Piece.abilities[abilityIndex] = new Ability(abilityToLearn);
                }

                abilityToLearn = null;
                state = BattleState.RunningTurn;
            };

            AbilitySelectionUI.HandleAbilitySelection(onAbilitySelected);
        }
    }

    public IEnumerator SetUpBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isEnemyBattle)
        {
            //random battle
            playerUnit.SetUp(playerParty.GetHealthyPiece());
            enemyUnit.SetUp(wildPiece);

            dialogBox.SetAbilityName(playerUnit.Piece.abilities);
            yield return dialogBox.TypeDialog($"a {enemyUnit.Piece.Base.Name} is reveald.");
        }
        else
        {
            //enemy battle

            //show player/enemy
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            enemyImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            enemyImage.sprite = enemy.Sprite;

            yield return dialogBox.TypeDialog($"{enemy.Name}");

            //send out first piece of enemy
            enemyImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPiece = enemyParty.GetHealthyPiece();
            enemyUnit.SetUp(enemyPiece);

            yield return dialogBox.TypeDialog($"{enemy.Name} is {enemyPiece.Base.Name}");

            //send out first piece of player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPiece = playerParty.GetHealthyPiece();
            playerUnit.SetUp(playerPiece);

            yield return dialogBox.TypeDialog($"tag in {playerPiece.Base.Name}");
            dialogBox.SetAbilityName(playerUnit.Piece.abilities);
        }

        escapeAttempts = 0;
        partyscreen.Init();
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

    IEnumerator AboutToUse(Piece newPiece)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"a member of {enemy.Name} {newPiece.Base.Name} steps up to fight you. do you want to swap a piece?");

        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    IEnumerator ChooseAbilityToForget(Piece piece, AbilityBase newAbility)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Devlope new ablity");
        AbilitySelectionUI.gameObject.SetActive(true);
        AbilitySelectionUI.SetAbilityData(piece.abilities.Select(x => x.Base).ToList(), newAbility);
        abilityToLearn = newAbility;

        state = BattleState.AbilityForget;
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if(playerAction == BattleAction.Ability)
        {
            //gets ability
            playerUnit.Piece.CurrentAbility = playerUnit.Piece.abilities[currentAbility];
            enemyUnit.Piece.CurrentAbility = enemyUnit.Piece.GetRandomAbility();

            int playerAbilityPriority = playerUnit.Piece.CurrentAbility.Base.Priority;
            int enemyAbilityPriority = enemyUnit.Piece.CurrentAbility.Base.Priority;

            //cheeks who goes first
            bool PlayerGoesFirst = true;
            if (enemyAbilityPriority > playerAbilityPriority)
                PlayerGoesFirst = false;
            else if(enemyAbilityPriority == playerAbilityPriority)
                PlayerGoesFirst = playerUnit.Piece.Speed >= enemyUnit.Piece.Speed;


            var firstUnit = (PlayerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (PlayerGoesFirst) ? enemyUnit : playerUnit;

            var secondPiece = secondUnit.Piece;

            //first turn
            yield return RunAbility(firstUnit, secondUnit, firstUnit.Piece.CurrentAbility);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPiece.HP > 0)
            {
                //second turn
                yield return RunAbility(secondUnit, firstUnit, secondUnit.Piece.CurrentAbility);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            //switch pieces code
            if( playerAction == BattleAction.SwitchPiece)
            {
                var selectedUnit = playerParty.Pieces[currentUnit];
                state = BattleState.Busy;
                yield return SwitchUnit(selectedUnit);

                //enemy turn ep 24 14:16
                var enemyAbility = enemyUnit.Piece.GetRandomAbility();
                  yield return RunAbility(enemyUnit, playerUnit, enemyAbility);
                  yield return RunAfterTurn(enemyUnit);
                  if (state == BattleState.BattleOver) yield break;
                
            }
            else if(playerAction == BattleAction.flee)
            {
                yield return TryToEscape();
            }

        }
        if (state != BattleState.BattleOver)
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

            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

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
                yield return HandlePieceDeath(targetUnit);
            }

        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Piece.Base.Name} attack did not land");
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


    IEnumerator HandlePieceDeath(BattleUnit deadUnit)
    {
        yield return dialogBox.TypeDialog($"{deadUnit.Piece.Base.Name} dead");
        deadUnit.PlayDeathAnimation();
        yield return new WaitForSeconds(2f);

        if (!deadUnit.IsPlayerUnit)
        {
            //exp
            int expYield = deadUnit.Piece.Base.ExpYield;
            int enemyLevel = deadUnit.Piece.Level;
            float enemyBonus = (isEnemyBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * enemyBonus) / 7);
            playerUnit.Piece.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Piece.Base.Name} gained {expGain} knowledge");
            yield return playerUnit.Hud.SetExpSmooth();

            //level up check
            while (playerUnit.Piece.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Piece.Base.Name} grew to knowledge {playerUnit.Piece.Level}");

                // try to learn new ability
                var newAbility = playerUnit.Piece.GetLearnableAbilityAtCurrLevel();
                if(newAbility != null)
                {
                    if(playerUnit.Piece.abilities.Count < PieceBase.MaxNumOfAbilties)
                    {
                        playerUnit.Piece.LearnAbility(newAbility);
                        yield return dialogBox.TypeDialog($"{playerUnit.Piece.Base.Name} developed {newAbility.AbilityBase.Name}");
                        dialogBox.SetAbilityName(playerUnit.Piece.abilities);
                    }
                    else
                    {
                        //froget ablity
                        yield return dialogBox.TypeDialog($"{playerUnit.Piece.Base.Name}  can developed {newAbility.AbilityBase.Name}");
                        yield return dialogBox.TypeDialog($"but can only have {PieceBase.MaxNumOfAbilties} abilitys");
                        yield return ChooseAbilityToForget(playerUnit.Piece, newAbility.AbilityBase);
                        yield return new WaitUntil(() => state != BattleState.AbilityForget);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(deadUnit);

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
        {
            if (!isEnemyBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextPiece = enemyParty.GetHealthyPiece();
                if (nextPiece != null)
                    StartCoroutine(AboutToUse(nextPiece));
                else
                    BattleOver(true);
            }
        }
            
    }

    //runs code after a turn ep 24 8:55
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //statuses like burn(DOTS) will hurt after a turn
        sourceUnit.Piece.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Piece);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Piece.HP <= 0)
        {
            yield return HandlePieceDeath(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
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
                prevState = state;
                OpenPartyUI();
            } 
            else if (currentAction == 3)
            {
                //flee
                StartCoroutine(RunTurns(BattleAction.flee));
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
            var ability = playerUnit.Piece.abilities[currentAbility];
            if (ability.AP == 0) return;

            dialogBox.EnableAbilitySelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Ability));
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

            if(prevState == BattleState.ActionSelection)
            {
                // if switch during turn
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPiece));
            }
            else
            {
                //if piece is dead
                state = BattleState.Busy;
                StartCoroutine(SwitchUnit(selectedUnit));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if(playerUnit.Piece.HP <= 0)
            {
                partyscreen.SetMessageText("piece must be in play");
                return;
            }

            partyscreen.gameObject.SetActive(false);

            if (prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextEnemyPiece());
            }
            else
                ActionSelection();
        }

    }

    // handles the switch option of party after enemy piece dies
    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if(aboutToUseChoice == true)
            {
                //yes
                prevState = BattleState.AboutToUse;
                OpenPartyUI();
            }
            else
            {
                //no
                StartCoroutine(SendNextEnemyPiece());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextEnemyPiece());
        }
    }


    //switches out the pices in play
    IEnumerator SwitchUnit(Piece newPiece)
    {
        if (playerUnit.Piece.HP > 0)
        {
            yield return dialogBox.TypeDialog($"tagging out {playerUnit.Piece.Base.name}");
            playerUnit.PlayDeathAnimation();
            yield return new WaitForSeconds(2);
        }

        playerUnit.SetUp(newPiece);

        dialogBox.SetAbilityName(newPiece.abilities);

        yield return dialogBox.TypeDialog($" {newPiece.Base.Name} is up next.");

        if (prevState == null)
        {
            state = BattleState.RunningTurn;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextEnemyPiece());
        }
    }

    IEnumerator SendNextEnemyPiece()
    {
        state = BattleState.Busy;

        var nextPiece = enemyParty.GetHealthyPiece();
        enemyUnit.SetUp(nextPiece);
        yield return dialogBox.TypeDialog($" {nextPiece.Base.Name} comes from the shadows");

        state = BattleState.RunningTurn;
    }

    //flee 
    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        // try and add some boolian here 
        // ep 35 5:33 if we don't want player to flee during enemy battle
        /*
         if (isEnemyBattle)
        {
            yield return dialogBox.TypeDialog($"text here");
            state = BattleState.RunningTurn;
            yield break;
        }
         */

        ++escapeAttempts;

        int playerSpeed = playerUnit.Piece.Speed;
        int enemySpeed = enemyUnit.Piece.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"you got away");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"you got away");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"enemy stop you from getting away");
                state = BattleState.RunningTurn;
            }
        }
    }
}
