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
    // public Fighter p1_Fighter, p2_Fighter;
    public GameObject p1_Spawn,p2_Spawn;
    // public Animator p1_animator,p2_animator;
    // public int p1_HP,p1_MaxHP,p2_HP,p2_MaxHP;
    // public int p1_meter,p2_meter;
    // public FighterState p1_fighterState,p2_fighterState;
    // public FighterActionState p1_fighterActionState,p2_fighterActionState;
    // public int matchTime;
    // public bool p1_facingInvert, p2_facingInvert;
    // public List<string> p1_inputs,p2_inputs;
    // public List<double> p1_inputTime,p2_inputTime;
    public double inputValidTime;
    public TimelineHandler timelineHandlerPrefab,p1_actionTimeline,p2_actionTimeline;

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
