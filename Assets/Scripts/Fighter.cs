using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Fighter", menuName = "JOYOUS ASSETS/Fighter")]
public class Fighter : ScriptableObject
{
    public string fighterName;
    public int maxHP;
    public float jumpHeight;
    public int airJumps;
    public float airJumpHeight;
    public float walkSpeed,walkBackSpeed;
    public Sprite smallSelectIcon;
    public Sprite bigSelectIcon;
    public GameObject model;
    public RuntimeAnimatorController animator;

    public Action Idle,Crouching,AirIdle,
    WalkForwards,WalkBackwards,
    Jump,AirJump,
    GroundHit,AirHit,
    KnockedDown,LaunchUp,LaunchDown,
    GroundShield,CrouchShield,AirShield,
    Victory,Lose;

    public List<ActionInput> inputActions;

}

