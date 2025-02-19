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
    public PlayerBattleUI p1UI,p2UI;
    public FighterCamera fighterCam;
    public int maxPlayerDist,wallDist;
    public float minWallBounceVel;
    public bool playersSpawned = false;
    public bool trainingMode;
    public Stage stage;

    void Start(){
        Application.targetFrameRate = 200; 
    }

    void Update(){
        if(battleState == BattleState.Initialize){ /////////////////////////////////////////////
            
            player1.InitializeBattleStart(p1_Spawn.transform.position,p1_Spawn.transform.rotation,p1UI);
            player2.InitializeBattleStart(p2_Spawn.transform.position,p2_Spawn.transform.rotation,p2UI);
            playersSpawned = true; // prevents non-gamebreaking missing object glitch in physics

            fighterCam.cameraFollowObjects.Clear();
            fighterCam.cameraFollowObjects.Add(player1.fighterObject.transform);
            fighterCam.cameraFollowObjects.Add(player2.fighterObject.transform);

            SfxManager.instance.PlayNewMusic(stage.music,stage.musicVolume);

            //change this to intro later
            battleState = BattleState.Battle;
        }
        else{
            player1.PlayerUpdate();     
            player2.PlayerUpdate();          
        }
    }

    void FixedUpdate(){
        if(playersSpawned){
            player1.PlayerPhysics();
            player2.PlayerPhysics();
        }
    }
}
