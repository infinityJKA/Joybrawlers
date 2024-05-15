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
    public string modelAnimation;
    public BoxData boxData; //prefab i will spawn when attack is instantiated 

}

public class ActionInput{
    public List<String> requiredInputs;
    public float inputTime;
    public List<FighterState> validFighterStates;
    public List<FighterActionState> validFighterActionStates;
    public int meter;
    public Action action;
}