using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEditor.Animations;


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
    public double inputValidTime;
    public TimelineHandler timelineHandlerPrefab,actionTimeline;
    public string playerNumber; //"p1" or "p2"
    public Players playersManager;

    void Start(){
        playersManager = GameObject.Find("Players").GetComponent<Players>();
    }

    public void PlayerUpdate(){

        if(playersManager.battleState == BattleState.Battle){

            RemoveInputs();
            CheckDirectionDown();
            CheckDirectionRelease();
            
            if(Input.GetButtonDown(playerNumber + " x")){
                AddInput("x");
            }
            if(Input.GetButtonDown(playerNumber + " y")){
                AddInput("y");
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

            if(fighterState == FighterState.Standing && fighterActionState == FighterActionState.Neutral){
                Action(fighter.Idle, true);
            }
            else if(fighterState == FighterState.Crouching && fighterActionState == FighterActionState.Neutral){
                Action(fighter.Crouching, true);
            }

        }


    }

    void Action(Action action, bool continous){
        if(continous){
            //Debug.Log(actionTimeline.currentActionName + " vs " + action.name);
            if(actionTimeline.currentActionName == action.name){
                return;
            }
        }

        // destroys current animation timeline
        if(actionTimeline.transform.childCount > 0){
            Destroy(actionTimeline.transform.GetChild(0).gameObject);
            Debug.Log("destroy!");
        }

        BoxData boxData = Instantiate(action.boxData,actionTimeline.transform.position,actionTimeline.transform.rotation);
        boxData.transform.parent = actionTimeline.transform;
        boxData.PlayerNumber = 1;
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
        Debug.Log(input);
    }

    void RemoveInputs(){
        for(int i = 0; i < inputTime.Count-1; i++){
            if(Time.time - inputTime[i] > inputValidTime){
                inputs.RemoveAt(i);
                inputTime.RemoveAt(i);
            }
        }
    }

    void CheckDirectionDown(){
        if(Input.GetButtonDown(playerNumber + " down")){
            if(Input.GetButton(playerNumber + " left")){
                if(facingInvert == false){AddInput("1");}
                else{AddInput("3");}
            }
            else if(Input.GetButton(playerNumber + " right")){
                if(facingInvert == false){AddInput("3");}
                else{AddInput("1");}
            }
            else{AddInput("2");}
        }

        else if(Input.GetButtonDown(playerNumber + " left")){
            if(Input.GetButton(playerNumber + " down")){
                if(facingInvert == false){AddInput("1");}
                else{AddInput("3");}
            }
            else if(Input.GetButton(playerNumber + " up")){
                if(facingInvert == false){AddInput("7");}
                else{AddInput("9");}
            }
            else{
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
        }

        else if(Input.GetButtonDown(playerNumber + " right")){
            if(Input.GetButton(playerNumber + " down")){
                if(facingInvert == false){AddInput("3");}
                else{AddInput("1");}
            }
            else if(Input.GetButton(playerNumber + " up")){
                if(facingInvert == false){AddInput("9");}
                else{AddInput("7");}
            }
            else{
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
            }
        }

        else if(Input.GetButtonDown(playerNumber + " up")){
            if(Input.GetButton(playerNumber + " left")){
                if(facingInvert == false){AddInput("7");}
                else{AddInput("9");}
            }
            else if(Input.GetButton(playerNumber + " right")){
                if(facingInvert == false){AddInput("9");}
                else{AddInput("7");}
            }
            else{AddInput("8");}
        }
    }

    void CheckDirectionRelease(){
        if(Input.GetButtonUp(playerNumber + " down")){
            if(Input.GetButton(playerNumber + " left")){
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
            else if(Input.GetButton(playerNumber + " right")){
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
            }
            else{AddInput("5");}
        }

        else if(Input.GetButtonUp(playerNumber + " left")){
            if(Input.GetButton(playerNumber + " down")){
                AddInput("2");
            }
            else if(Input.GetButton(playerNumber + " up")){
                AddInput("8");
            }
            else{AddInput("5");}
        }

        else if(Input.GetButtonUp(playerNumber + " up")){
            if(Input.GetButton(playerNumber + " left")){
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
            else if(Input.GetButton(playerNumber + " right")){
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
            }
            else{AddInput("5");}
        }

        else if(Input.GetButtonUp(playerNumber + " right")){
            if(Input.GetButton(playerNumber + " down")){
                AddInput("2");
            }
            else if(Input.GetButton(playerNumber + " up")){
                AddInput("8");
            }
            else{AddInput("5");}
        }

    }




}
