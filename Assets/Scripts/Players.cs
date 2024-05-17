using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEditor.Animations;


public enum BattleState{NotInBattle,Initialize,Intro,Battle,RoundFinished};

public class Players : MonoBehaviour
{
    [SerializeField] private int frameCap = 30;
    public bool showFrameData;
    public BattleState battleState;
    public Player player1,player2;
    public GameObject p1_Spawn,p2_Spawn;
    public int matchTime;
    public double inputValidTime;

    // void Start() { Application.targetFrameRate = frameCap; }

    void Update(){
        if(battleState == BattleState.Initialize){ /////////////////////////////////////////////
            
            player1.InitializeBattleStart(p1_Spawn.transform.position,p1_Spawn.transform.rotation);
            //pretend p2 is also here

            //change this to intro later
            battleState = BattleState.Battle;
        }
        else{
            player1.PlayerUpdate();          
        }


    }

}
