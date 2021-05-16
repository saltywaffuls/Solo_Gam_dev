using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState {FreeRoam, Battle, Dialog, Cutscene }

public class GameController : MonoBehaviour
{
    GameState state;

    public static GameController Instance { get; private set; }

    [SerializeReference] PlayerController playerController;
    [SerializeReference] BattleSystem battleSystem;
    [SerializeReference] Camera mainCamera;

    EnemyController enemy;

    private void Awake()
    {
        Instance = this;
        ConditionDB.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnEnterEnemyView += (Collider2D enemyCollider) => 
        {
            var enemy = enemyCollider.GetComponentInParent<EnemyController>();
            if (enemy != null)
            {
                state = GameState.Battle;
                StartCoroutine (enemy.TriggerEnemyBattle(playerController));
            }
        };

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    private void Update()
    {
        //checking overall game state
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if( state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if ( state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    //enables battle system
    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PieceParty>();
        var wildPiece = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPiece();

        battleSystem.StartBattle(playerParty, wildPiece);
    }

    public void StartEnemyBattle(EnemyController enemy)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        this.enemy = enemy;
        var playerParty = playerController.GetComponent<PieceParty>();
        var enemyParty = enemy.GetComponent<PieceParty>();

        battleSystem.StartEnemyBattle(playerParty, enemyParty);
    }

    void EndBattle(bool won)
    {
        if (enemy != null && won == true)
        {
            enemy.BattleLost();
            enemy = null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }

}
