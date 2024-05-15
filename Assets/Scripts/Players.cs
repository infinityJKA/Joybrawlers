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
    public BattleState battleState;
    public Fighter p1_Fighter, p2_Fighter;
    public GameObject p1_Object, p2_Object,p1_Spawn,p2_Spawn;
    public int p1_HP,p1_MaxHP,p2_HP,p2_MaxHP;
    public int p1_meter,p2_meter;
    public int matchTime;
    public bool p1_facingInvert, p2_facingInvert;
    public List<string> p1_inputs,p2_inputs;
    public List<double> p1_inputTime,p2_inputTime;
    public double inputValidTime;

    void Update(){

        if(battleState == BattleState.Initialize){ /////////////////////////////////////////////
            p1_Object = p1_Fighter.model;
            Instantiate(p1_Object,p1_Spawn.transform.position,p1_Spawn.transform.rotation);
            p1_MaxHP = p1_Fighter.maxHP;
            p1_HP = p1_MaxHP;
            p1_meter = 0;
            p1_facingInvert = false;
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
        }


    }

    void AddInput_P1(string input){
        p1_inputs.Add(input);
        p1_inputTime.Add(Time.time);
        Debug.Log(input);
    }

    void RemoveInputs_P1(){
        for(int i = 0; i < p1_inputTime.Count; i++){
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
