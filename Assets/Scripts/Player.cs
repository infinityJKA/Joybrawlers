using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Playables;


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
    public double xVel,yVel;

    void Start(){
        playersManager = GameObject.Find("Players").GetComponent<Players>();
    }

    public void PlayerUpdate(){

        if(playersManager.battleState == BattleState.Battle){

            bool acted = false;

            RemoveInputs();
            CheckDirectionDown();
            CheckDirectionRelease();
            
            if(fighterActionState != FighterActionState.Attacking || fighterActionState != FighterActionState.Attacking){
                if(Input.GetButtonDown("p" + playerNumber + " x")){
                    AddInput("x");
                    // acted = CheckForInput();
                }
                else if(Input.GetButtonDown("p" + playerNumber + " y")){
                    AddInput("y");
                    // acted = CheckForInput();
                }
            }

            if(fighterActionState == FighterActionState.Attacking){
                if(fighterObject.GetComponentInChildren<PlayableDirector>().state != PlayState.Playing){
                    fighterActionState = FighterActionState.Neutral;
                }
            }

            // COMMAND INPUT CHECKING SYSTEM
            for(int i = 0; i < fighter.inputActions.Count; i++){
                ActionInput inputAction = fighter.inputActions[i];
                if(!acted){
                    if(fighterState == inputAction.validFighterState && fighterActionState == inputAction.validActionState){ // instantly skip if invalid state
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
                                    inputs.Clear();
                                    inputTime.Clear();

                                }
                            }
                        }
                    }
                }
            }

            
            if(fighterActionState == FighterActionState.Neutral){
                if(inputs.Count > 0){
                    if(inputs[inputs.Count-1] == "2" || inputs[inputs.Count-1] == "1" || inputs[inputs.Count-1] == "3"){
                        fighterState = FighterState.Crouching;
                    }
                    else if(inputs[inputs.Count-1] == "4" || inputs[inputs.Count-1] == "5" || inputs[inputs.Count-1] == "6"){
                        fighterState = FighterState.Standing;
                    }
                }
            }

            if(!acted){
                if(fighterState == FighterState.Standing && fighterActionState == FighterActionState.Neutral){
                    Action(fighter.Idle, true);
                }
                else if(fighterState == FighterState.Crouching && fighterActionState == FighterActionState.Neutral){
                    Action(fighter.Crouching, true);
                }
            }

        }


    }

    void Action(Action action, bool continous){
        if(actionTimeline.currentActionName == action.name){  // stop if the action is already in action
            return;
        }

        // set action state
        fighterActionState = action.actionStateDuringAction;

        // // erases inputs
        // if(fighterActionState != FighterActionState.Neutral){
        //     inputs.Clear();
        //     inputTime.Clear();
        // }
        
        // destroys preexisting animation timeline
        if(actionTimeline.transform.childCount > 0){
            Destroy(actionTimeline.transform.GetChild(0).gameObject);
        }

        BoxData boxData = Instantiate(action.boxData,actionTimeline.transform.position,actionTimeline.transform.rotation);
        boxData.transform.parent = actionTimeline.transform;
        boxData.PlayerNumber = playerNumber;
        actionTimeline.currentActionName = action.name;

        boxData.StartAnim(fighterObject);
    }

    

    public void InitializeBattleStart(Vector3 pos, Quaternion rot){
        fighterObject = fighter.model;
        fighterObject = Instantiate(fighterObject,pos, rot);  // THE CODE WILL SELF DESTRUCT AND INFINITELY SPAWN INCOMPLETE PLAYERS IF OBJECT IS NOT DEFINIED HERE, DO NOT CHANGE!!!
        fighterObject.AddComponent<Animator>();
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
            else{AddInput("5");}
        }

    }




}