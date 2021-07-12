using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState {FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused}

public class GameController : MonoBehaviour
{
    GameState state;

    GameState stateBeforePaused;

    public static GameController Instance { get; private set; }
    public SceneDetails currentScene { get; private set; }
    public SceneDetails prevScene { get; private set; }

    MenuController menuController;

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera mainCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    EnemyController enemy;

    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        PieceDB.Init();
        AbilityDB.Init();
        ConditionDB.Init();
    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };

        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += OnMenuSelected;
    }

    private void Update()
    {
        //checking overall game state
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }

            // save/load 
            if (Input.GetKeyDown(KeyCode.F1))
            {
                SavingSystem.i.Save("saveSlot1");
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                SavingSystem.i.Load("saveSlot1");
            }
        }
        else if( state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if ( state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if ( state == GameState.Menu)
        {
            menuController.HandleUpdates();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                //go to stat screen
            };

            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {

            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            inventoryUI.HandleUpdate(onBack);
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

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            //piece
            partyScreen.gameObject.SetActive(true);
            partyScreen.SetPartyData(playerController.GetComponent<PieceParty>().Pieces);
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            //inventory
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            //save
            SavingSystem.i.Save("saveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            //load
            SavingSystem.i.Load("saveSlot1");
            state = GameState.FreeRoam;
        }
    }
}
