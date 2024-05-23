using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Playables;
using System.Data.Common;


public class Player : MonoBehaviour
{
    public Fighter fighter;
    public GameObject fighterObject;
    public Animator animator;
    public int HP,MaxHP;
    public int meter;
    public FighterState fighterState;
    public FighterActionState fighterActionState;
    public bool facingInvert;
    public List<string> inputs;
    public List<double> inputTime;
    public TimelineHandler timelineHandlerPrefab,actionTimeline;
    public int playerNumber; //"p1" or "p2"
    public Players playersManager;
    public float xVel,yVel,xVelContact,yVelContact;
    public float freezeTimer,freezeTime, stunTimer,stunTime;
    public int comboed;
    public bool incomingKnockback;
    public float incomingXvel,incomingYvel;
    public float lastHitTime;

    void Start(){
        playersManager = GameObject.Find("Players").GetComponent<Players>();
    }

    public void PlayerUpdate(){

        if(playerNumber == 1){
            if(playersManager.player2.fighterObject.transform.position.x > fighterObject.transform.position.x){
                facingInvert = false;
            }
            else{
                facingInvert = true;
            }
        }
        else if(playerNumber == 2){
            if(playersManager.player1.fighterObject.transform.position.x > fighterObject.transform.position.x){
                facingInvert = false;
            }
            else{
                facingInvert = true;
            }
        }

        if(fighterActionState == FighterActionState.Neutral){      // CHANGE FACING
            if(facingInvert){
                // fighterObject.transform.rotation = Quaternion.Euler(0,0,0);
                fighterObject.transform.localScale = new Vector3(-1,1,1);
            }
            else{
                // fighterObject.transform.rotation = Quaternion.Euler(0,180,0);
                fighterObject.transform.localScale = new Vector3(1,1,1);
            }
        }


        if(playersManager.battleState == BattleState.Battle){
            // Debug.Log("h");
            bool acted = false;

            if(inputTime.Count == 1){
                inputTime[0] = Time.time;
            }

            RemoveInputs();
            CheckDirectionDown();
            CheckDirectionRelease();
            
            if(fighterActionState != FighterActionState.Attacking || fighterActionState != FighterActionState.Attacking){
                if(Input.GetButtonDown("p" + playerNumber + " x")){
                    AddInput("x");
                }
                else if(Input.GetButtonDown("p" + playerNumber + " y")){
                    AddInput("y");
                }
            }

            if(fighterActionState == FighterActionState.Attacking || fighterActionState == FighterActionState.Cancellable){
                if(fighterObject.GetComponentInChildren<PlayableDirector>().state != PlayState.Playing){
                    fighterActionState = FighterActionState.Neutral;
                }
            }

            // COMMAND INPUT CHECKING SYSTEM
            for(int i = 0; i < fighter.inputActions.Count; i++){
                ActionInput inputAction = fighter.inputActions[i];
                if(!acted){
                    if(fighterState == inputAction.validFighterState){ // instantly skip if invalid state
                        if(fighterActionState == inputAction.validActionState || fighterActionState == FighterActionState.Cancellable  &&  inputAction.validActionState == FighterActionState.Neutral){ // also checks for cancellable moves
                            if(inputAction.meter <= meter ){ // instantly skip check if not enough meter
                                if(inputAction.requiredInputs.Count <= inputs.Count){  /// instantly skip check if physically not enough inputs
                                    bool validInput = true;
                                    for(int j = 0; j < inputAction.requiredInputs.Count; j++){  /// loop through each required input
                                        string a = inputAction.requiredInputs[j]; // input required
                                        string b =inputs[inputs.Count-inputAction.requiredInputs.Count+j]; // input given
                                        if(a != b){
                                            validInput = false;
                                        }
                                    }
                                    if(validInput){
                                        Action(inputAction.action,true);
                                        acted = true;
                                        ResetInputLists();

                                    }
                                }
                            }
                        }
                    }
                }
            }

            
            if(fighterActionState == FighterActionState.Neutral){
                if(fighterObject.transform.position.y > 0){
                    fighterState = FighterState.InAir;
                }
                else{
                    if(inputs.Count > 0){
                        if(inputs[inputs.Count-1] == "2" || inputs[inputs.Count-1] == "1" || inputs[inputs.Count-1] == "3"){
                            fighterState = FighterState.Crouching;
                        }
                        else if(inputs[inputs.Count-1] == "4" || inputs[inputs.Count-1] == "5" || inputs[inputs.Count-1] == "6"){
                            fighterState = FighterState.Standing;
                        }
                        
                        if(inputs[inputs.Count-1] == "7" || inputs[inputs.Count-1] == "8" || inputs[inputs.Count-1] == "9"){
                            Action(fighter.Jump, true);
                        }
                    }
                }
            }

            if(!acted){
                if(fighterActionState == FighterActionState.Neutral){
                    if(fighterState == FighterState.Standing){
                        if(inputs.Count > 0){
                            if(inputs[inputs.Count-1] == "6"){         //  WALKING
                                Action(fighter.WalkForwards, true);
                                if(xVel < fighter.walkSpeed){
                                    SetVelocityX(fighter.walkSpeed);
                                }
                            }
                            else if(inputs[inputs.Count-1] == "4"){    // WALKING BACKWARDS
                                Action(fighter.WalkBackwards, true);
                                if(xVel > fighter.walkBackSpeed){
                                    SetVelocityX(fighter.walkBackSpeed);
                                }
                            }
                            else{
                                Action(fighter.Idle, true);           //  IDLE
                                if(inputs.Count >= 2){
                                    if(inputs[inputs.Count-2] == "4" || inputs[inputs.Count-2] == "6"){
                                        SetVelocityX(0);
                                    }
                                }
                            }
                        }
                        else{
                            Action(fighter.Idle, true);   //  ALT IDLE CONDITION
                        }
                    }
                    else if(fighterState == FighterState.Crouching){
                        Action(fighter.Crouching, true);
                    }
                }
            }

            if(fighterActionState == FighterActionState.Hit){         // deal with hitstun and related things

                if(incomingKnockback){
                    if(freezeTimer + freezeTime < Time.time){
                        AddVelocity(incomingXvel*-1,incomingYvel);
                        incomingKnockback = false;
                    }
                }

                if(stunTimer + stunTime < Time.time){
                    comboed = 0;
                    fighterActionState = FighterActionState.Neutral;
                }
                else if(fighterState == FighterState.Standing || fighterState == FighterState.Crouching){
                    Action(fighter.GroundHit,true);
                }
            }

        }
    }

    public void GetHit(int damage, float freeze, float stun, float xKnockback, float yKnockback, bool willTrip, bool willKnockdown, bool superArmored){
        if(lastHitTime != Time.time){ // prevents overlapping hurtboxes getting hit multiple times at once
            lastHitTime = Time.time;
            Debug.Log("Got hit!");
            HP -= damage;
            if(!superArmored){
                incomingKnockback = true;
                incomingXvel = xKnockback;
                incomingYvel = yKnockback;

                float t = Time.time;
                freezeTime = freeze;
                freezeTimer = t;
                stunTime = stun;
                stunTimer = t;

                comboed += 1;

                fighterActionState = FighterActionState.Hit;
            }
        }

    }

    public void AddVelocity(float x, float y){
        if(facingInvert){
           x *= -1; 
        }
        xVel += x;
        yVel += y;
    }

    public void SetVelocityX(float x){
        if(facingInvert){
           x *= -1; 
        }
        xVel = x;
    }

    public void PlayerPhysics(){
        if(!incomingKnockback){ // pauses during freeze
            
            if(fighterObject.transform.position[1]+yVel < 0 && fighterObject.transform.position.y + yVel < fighterObject.transform.position.y){
                Debug.Log("YOU LANDED");
                yVel = 0;
                fighterObject.transform.position = new Vector3(fighterObject.transform.position[0],0,fighterObject.transform.position[2]);
            }
            else{
                yVel -= 0.1f;
            }
            
        
            fighterObject.transform.position = new Vector3(fighterObject.transform.position[0]+xVel+xVelContact,fighterObject.transform.position[1]+yVel,0);
        
            

            float n = 0.915f;
            // if(fighter.name == "Dark Gibson"){
            //     n = 0.965f;
            // }

            xVel = xVel*n;
            yVel = yVel*n;
        }
    }

    void Action(Action action, bool continous){
        if(actionTimeline.currentActionName == action.name){  // stop if the action is already in action
            return;
        }

        // set action state
        fighterActionState = action.actionStateDuringAction;
        
        // destroys preexisting animation timeline
        if(actionTimeline.transform.childCount > 0){
            Destroy(actionTimeline.transform.GetChild(0).gameObject);
        }

        BoxData boxData = Instantiate(action.boxData,actionTimeline.transform.position,actionTimeline.transform.rotation);
        if(facingInvert){
            boxData.transform.Rotate(0,180,0);
        }

        boxData.transform.parent = actionTimeline.transform;
        boxData.PlayerNumber = playerNumber;
        actionTimeline.currentActionName = action.name;

        boxData.StartAnim(fighterObject);
    }

    public void ResetInputLists(){
        inputs.Clear();
        inputTime.Clear();
    }

    public void InitializeBattleStart(Vector3 pos, Quaternion rot){
        fighterObject = fighter.model;
        fighterObject = Instantiate(fighterObject,pos, rot);  // THE CODE WILL SELF DESTRUCT AND INFINITELY SPAWN INCOMPLETE PLAYERS IF OBJECT IS NOT DEFINIED HERE, DO NOT CHANGE!!!
        // fighterObject.AddComponent<Animator>();
        animator = fighterObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = fighter.animator;

        actionTimeline = Instantiate(timelineHandlerPrefab,fighterObject.transform.position,fighterObject.transform.rotation);  // THE CODE WILL SELF DESTRUCT AND INFINITELY SPAWN INCOMPLETE PLAYERS IF OBJECT IS NOT DEFINIED HERE, DO NOT CHANGE!!!
        actionTimeline.transform.parent = fighterObject.transform;

        MaxHP = fighter.maxHP;
        HP = MaxHP;
        meter = 0;
        facingInvert = false;
        fighterState = FighterState.Standing;
        fighterActionState = FighterActionState.Neutral;
    }

    void AddInput(string input){
        // Debug.Log("AddInput()");
        if(inputs.Count > 0){
            if(input == "1" || input == "2" || input == "3" || input == "4" || input == "5" || input == "6" || input == "7" || input == "8" || input == "9"){
                if(input == inputs[inputs.Count-1]){
                    return;  // Prevents double direct input glitch
                }
            }
        }

        inputs.Add(input);
        inputTime.Add(Time.time);
        // Debug.Log(input);
    }

    void RemoveInputs(){
        if(inputs.Count > 0){
            for(int i = 0; i < inputTime.Count-1; i++){
                if(Time.time - inputTime[i] > playersManager.inputValidTime){
                    inputs.RemoveAt(i);
                    inputTime.RemoveAt(i);
                }
            }
        }
    }

    void CheckDirectionDown(){
        if(Input.GetButtonDown("p" + playerNumber + " down")){
            if(Input.GetButton("p" + playerNumber + " left")){
                if(facingInvert == false){AddInput("1");}
                else{AddInput("3");}
            }
            else if(Input.GetButton("p" + playerNumber + " right")){
                if(facingInvert == false){AddInput("3");}
                else{AddInput("1");}
            }
            else{AddInput("2");}
        }

        else if(Input.GetButtonDown("p" + playerNumber + " left")){
            if(Input.GetButton("p" + playerNumber + " down")){
                if(facingInvert == false){AddInput("1");}
                else{AddInput("3");}
            }
            else if(Input.GetButton("p" + playerNumber + " up")){
                if(facingInvert == false){AddInput("7");}
                else{AddInput("9");}
            }
            else{
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
        }

        else if(Input.GetButtonDown("p" + playerNumber + " right")){
            if(Input.GetButton("p" + playerNumber + " down")){
                if(facingInvert == false){AddInput("3");}
                else{AddInput("1");}
            }
            else if(Input.GetButton("p" + playerNumber + " up")){
                if(facingInvert == false){AddInput("9");}
                else{AddInput("7");}
            }
            else{
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
            }
        }

        else if(Input.GetButtonDown("p" + playerNumber + " up")){
            if(Input.GetButton("p" + playerNumber + " left")){
                if(facingInvert == false){AddInput("7");}
                else{AddInput("9");}
            }
            else if(Input.GetButton("p" + playerNumber + " right")){
                if(facingInvert == false){AddInput("9");}
                else{AddInput("7");}
            }
            else{AddInput("8");}
        }
    }

    void CheckDirectionRelease(){
        if(Input.GetButtonUp("p" + playerNumber + " down")){
            if(Input.GetButton("p" + playerNumber + " left")){
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
            else if(Input.GetButton("p" + playerNumber + " right")){
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
            }
            else{AddInput("5");}
        }

        else if(Input.GetButtonUp("p" + playerNumber + " left")){
            if(Input.GetButton("p" + playerNumber + " down")){
                AddInput("2");
            }
            else if(Input.GetButton("p" + playerNumber + " up")){
                AddInput("8");
            }
            else if(Input.GetButton("p" + playerNumber + " right")){
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
            }
            else{AddInput("5");}
        }

        else if(Input.GetButtonUp("p" + playerNumber + " up")){
            if(Input.GetButton("p" + playerNumber + " left")){
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
            else if(Input.GetButton("p" + playerNumber + " right")){
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
            }
            else{AddInput("5");}
        }

        else if(Input.GetButtonUp("p" + playerNumber + " right")){
            if(Input.GetButton("p" + playerNumber + " down")){
                AddInput("2");
            }
            else if(Input.GetButton("p" + playerNumber + " up")){
                AddInput("8");
            }
            else if(Input.GetButton("p" + playerNumber + " left")){
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
            else{AddInput("5");}
        }

    }




}