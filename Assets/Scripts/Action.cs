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
    public AnimationClip animation;

}

public class ActionInput{
    public String[] requiredInputs;
    public float inputTime;
    public State[] validStates;
    public int meter;
    public Action action;
}