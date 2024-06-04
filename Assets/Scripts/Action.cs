using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Action", menuName = "JOYOUS ASSETS/Action")]
public class Action : ScriptableObject
{
    public string moveName;
    //public string modelAnimation;
    public BoxData boxData; //prefab i will spawn when attack is instantiated 
    
    public FighterActionState actionStateDuringAction = FighterActionState.Attacking;
    public List<Action> CancelInto;
    public int meterGain;

}

[System.Serializable]
public class ActionInput{
    public List<string> requiredInputs;
    public FighterState validFighterState;
    public FighterActionState validActionState;
    public int meter;
    public Action action;
}