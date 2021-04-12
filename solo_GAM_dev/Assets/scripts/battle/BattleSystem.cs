using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    [SerializeReference] BattleUnit playerUnit;
    [SerializeReference] BattleUnit enemyUnit;
    [SerializeReference] BattleHud playerhud;
    [SerializeReference] BattleHud enemyhud;


    private void Start()
    {
        SetUpBattle();
    }

    public void SetUpBattle()
    {
        playerUnit.SetUp();
        enemyUnit.SetUp();
        playerhud.SetData(playerUnit.Piece);
        enemyhud.SetData(enemyUnit.Piece);
    }
}
