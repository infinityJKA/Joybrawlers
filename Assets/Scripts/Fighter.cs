using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Animations;


[CreateAssetMenu(fileName = "Fighter", menuName = "JOYOUS ASSETS/Fighter")]
public class Fighter : ScriptableObject
{
    public string fighterName;
    public int maxHP;
    public int jumpHeight;
    public int airJumps;
    public int airJumpHeight;
    public int walkSpeed,walkBackSpeed;
    public Sprite smallSelectIcon;
    public Sprite bigSelectIcon;
    public GameObject model;
    public AnimatorController animator;

    public Action Idle,Crouching,AirIdle,
    WalkForwards,WalkBackwards,
    Jump,AirJump,
    GroundHit,AirHit,
    KnockedDown,LaunchUp,LaunchDown,
    GroundShield,CrouchShield,AirShield,
    Victory,Lose;

    public List<ActionInput> inputActions;

}

