using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;


public enum BattleState{NotInBattle,Initialize,Intro,Battle,RoundFinished};

public class Players : MonoBehaviour
{
    public bool showFrameData;
    public BattleState battleState;
    public Player player1,player2;
    public GameObject p1_Spawn,p2_Spawn;
    public double inputValidTime;
    public TimelineHandler timelineHandlerPrefab,p1_actionTimeline,p2_actionTimeline;

    void Start(){
        Application.targetFrameRate = 60; 
    }

    void Update(){
        if(battleState == BattleState.Initialize){ /////////////////////////////////////////////
            
            player1.InitializeBattleStart(p1_Spawn.transform.position,p1_Spawn.transform.rotation);
            player2.InitializeBattleStart(p2_Spawn.transform.position,p2_Spawn.transform.rotation);

            //change this to intro later
            battleState = BattleState.Battle;
        }
        else{
            player1.PlayerUpdate();     
            player2.PlayerUpdate();          
        }
    }

    void FixedUpdate(){
        player1.PlayerPhysics();
        player2.PlayerPhysics();
    }
}
