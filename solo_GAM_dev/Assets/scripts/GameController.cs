using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState {FreeRoam, Battle, Dialog, Cutscene, Paused}

public class GameController : MonoBehaviour
{
    GameState state;

    GameState stateBeforePaused;

    public static GameController Instance { get; private set; }
    public SceneDetails currentScene { get; private set; }
    public SceneDetails prevScene { get; private set; }

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
        battleSystem.OnBattleOver += EndBattle;


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

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePaused = state;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforePaused;
        }
    }

    //enables battle system
    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PieceParty>();
        var wildPiece = currentScene.GetComponent<MapArea>().GetRandomWildPiece();

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

    public void OnEnterEnemyView(EnemyController enemy)
    {
        state = GameState.Battle;
        StartCoroutine(enemy.TriggerEnemyBattle(playerController));
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

    public void SetCurrentScene(SceneDetails currScene)
    {
        prevScene = currentScene;
        currentScene = currScene;
    }
}
