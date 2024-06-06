using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Fighter", menuName = "JOYOUS ASSETS/Fighter")]
public class Fighter : ScriptableObject
{
    public string fighterName;
    public int maxHP;
    public float jumpHeight,superJumpHeight,airJumpHeight;
    public int airJumps;
    public float walkSpeed,walkBackSpeed,airDrift,airDriftBack,diagonalJumpVertMultiplier,diagonalSuperJumpVertMultiplier;
    public Sprite battlePortrait;
    public GameObject model;
    public RuntimeAnimatorController animator;

    public Action Idle,Crouching,AirIdle,
    WalkForwards,WalkBackwards,
    Jump,AirJump,SuperJump,
    GroundHit,AirHit,Grabbed,
    KnockedDown,LaunchUp,LaunchDown,
    NeutralGetUp,GetUpAttack,GetUpRoll,
    GroundShield,CrouchShield,AirShield,
    Victory,Lose;

    public List<ActionInput> inputActions;

}

