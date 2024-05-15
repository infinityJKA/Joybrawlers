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
    public BattleState battleState;
    public Fighter p1_Fighter, p2_Fighter;
    public GameObject p1_Object, p2_Object,p1_Spawn,p2_Spawn;
    public Animator p1_animator,p2_animator;
    public int p1_HP,p1_MaxHP,p2_HP,p2_MaxHP;
    public int p1_meter,p2_meter;
    public FighterState p1_fighterState,p2_fighterState;
    public FighterActionState p1_fighterActionState,p2_fighterActionState;
    public int matchTime;
    public bool p1_facingInvert, p2_facingInvert;
    public List<string> p1_inputs,p2_inputs;
    public List<double> p1_inputTime,p2_inputTime;
    public double inputValidTime;

    void Update(){

        if(battleState == BattleState.Initialize){ /////////////////////////////////////////////
            p1_Object = p1_Fighter.model;
            p1_Object = Instantiate(p1_Object,p1_Spawn.transform.position,p1_Spawn.transform.rotation);  // THE CODE WILL SELF DESTRUCT AND INFINITELY SPAWN INCOMPLETE PLAYERS IF P1_OBJECT IS NOT DEFINIED HERE, DO NOT CHANGE!!!
            p1_Object.AddComponent<Animator>();
            p1_animator = p1_Object.GetComponent<Animator>();
            p1_animator.runtimeAnimatorController = p1_Fighter.animator;

            p1_MaxHP = p1_Fighter.maxHP;
            p1_HP = p1_MaxHP;
            p1_meter = 0;
            p1_facingInvert = false;
            p1_fighterState = FighterState.Standing;
            p1_fighterActionState = FighterActionState.Neutral;
            //pretend p2 is also here

            //change this to intro later
            battleState = BattleState.Battle;
        }
        else if(battleState == BattleState.Battle){

            RemoveInputs_P1();
            CheckDirectionDown_P1();
            CheckDirectionRelease_P1();
            
            if(Input.GetButtonDown("p1 x")){
                AddInput_P1("x");
            }
            if(Input.GetButtonDown("p1 y")){
                AddInput_P1("y");
            }

            if(p1_fighterActionState == FighterActionState.Neutral){
                if(p1_inputs.Count > 0){
                    if(p1_inputs[p1_inputs.Count-1] == "2" || p1_inputs[p1_inputs.Count-1] == "1" || p1_inputs[p1_inputs.Count-1] == "3"){
                        p1_fighterState = FighterState.Crouching;
                    }
                    else if(p1_inputs[p1_inputs.Count-1] == "4" || p1_inputs[p1_inputs.Count-1] == "5" || p1_inputs[p1_inputs.Count-1] == "6"){
                        p1_fighterState = FighterState.Standing;
                    }
                }
            }

            if(p1_fighterState == FighterState.Standing && p1_fighterActionState == FighterActionState.Neutral){
                p1_Action(p1_Fighter.Idle);
            }
            else if(p1_fighterState == FighterState.Crouching && p1_fighterActionState == FighterActionState.Neutral){
                p1_Action(p1_Fighter.Crouching);
            }

        }


    }

    void p1_Action(Action action){
        p1_animator.Play(action.modelAnimation);
    }

    void AddInput_P1(string input){
        p1_inputs.Add(input);
        p1_inputTime.Add(Time.time);
        Debug.Log(input);
    }

    void RemoveInputs_P1(){
        for(int i = 0; i < p1_inputTime.Count-1; i++){
            if(Time.time - p1_inputTime[i] > inputValidTime){
                p1_inputs.RemoveAt(i);
                p1_inputTime.RemoveAt(i);
            }
        }
    }

    void CheckDirectionDown_P1(){
        if(Input.GetButtonDown("p1 down")){
            if(Input.GetButton("p1 left")){
                if(p1_facingInvert == false){AddInput_P1("1");}
                else{AddInput_P1("3");}
            }
            else if(Input.GetButton("p1 right")){
                if(p1_facingInvert == false){AddInput_P1("3");}
                else{AddInput_P1("1");}
            }
            else{AddInput_P1("2");}
        }

        else if(Input.GetButtonDown("p1 left")){
            if(Input.GetButton("p1 down")){
                if(p1_facingInvert == false){AddInput_P1("1");}
                else{AddInput_P1("3");}
            }
            else if(Input.GetButton("p1 up")){
                if(p1_facingInvert == false){AddInput_P1("7");}
                else{AddInput_P1("9");}
            }
            else{
                if(p1_facingInvert == false){AddInput_P1("4");}
                else{AddInput_P1("6");}
            }
        }

        else if(Input.GetButtonDown("p1 right")){
            if(Input.GetButton("p1 down")){
                if(p1_facingInvert == false){AddInput_P1("3");}
                else{AddInput_P1("1");}
            }
            else if(Input.GetButton("p1 up")){
                if(p1_facingInvert == false){AddInput_P1("9");}
                else{AddInput_P1("7");}
            }
            else{
                if(p1_facingInvert == false){AddInput_P1("6");}
                else{AddInput_P1("4");}
            }
        }

        else if(Input.GetButtonDown("p1 up")){
            if(Input.GetButton("p1 left")){
                if(p1_facingInvert == false){AddInput_P1("7");}
                else{AddInput_P1("9");}
            }
            else if(Input.GetButton("p1 right")){
                if(p1_facingInvert == false){AddInput_P1("9");}
                else{AddInput_P1("7");}
            }
            else{AddInput_P1("8");}
        }
    }

    void CheckDirectionRelease_P1(){
        if(Input.GetButtonUp("p1 down")){
            if(Input.GetButton("p1 left")){
                if(p1_facingInvert == false){AddInput_P1("4");}
                else{AddInput_P1("6");}
            }
            else if(Input.GetButton("p1 right")){
                if(p1_facingInvert == false){AddInput_P1("6");}
                else{AddInput_P1("4");}
            }
            else{AddInput_P1("5");}
        }

        else if(Input.GetButtonUp("p1 left")){
            if(Input.GetButton("p1 down")){
                AddInput_P1("2");
            }
            else if(Input.GetButton("p1 up")){
                AddInput_P1("8");
            }
            else{AddInput_P1("5");}
        }

        else if(Input.GetButtonUp("p1 up")){
            if(Input.GetButton("p1 left")){
                if(p1_facingInvert == false){AddInput_P1("4");}
                else{AddInput_P1("6");}
            }
            else if(Input.GetButton("p1 right")){
                if(p1_facingInvert == false){AddInput_P1("6");}
                else{AddInput_P1("4");}
            }
            else{AddInput_P1("5");}
        }

        else if(Input.GetButtonUp("p1 right")){
            if(Input.GetButton("p1 down")){
                AddInput_P1("2");
            }
            else if(Input.GetButton("p1 up")){
                AddInput_P1("8");
            }
            else{AddInput_P1("5");}
        }

    }




}
