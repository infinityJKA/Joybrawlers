using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Playables;
using System.Data.Common;
using TMPro;


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
    public float xVel,yVel,xVelContact,yVelContact;
    public float freezeTimer,freezeTime, stunTimer,stunTime, knockdownTimer;
    public bool knockedDownOnGround;
    public int comboed;
    public bool incomingKnockback;
    public float incomingXvel,incomingYvel;
    public float lastHitTime;
    public Action currentAction;
    public bool moveHasHit;
    Player otherPlayer;
    public GameObject grabTracker;
    public PlayerBattleUI playerUI;
    public bool holdingDown;
    private int maxMeter = 1000;

    void Start(){
        playersManager = GameObject.Find("Players").GetComponent<Players>();
        if(playerNumber == 1){
            otherPlayer = playersManager.player2;
        }
        else{
            otherPlayer = playersManager.player1;
        }
    }

    public void PlayerUpdate(){

        if(fighterActionState == FighterActionState.Neutral || fighterActionState == FighterActionState.Shield){
            if(currentAction != fighter.WalkForwards && currentAction != fighter.WalkBackwards){
                if(otherPlayer.fighterObject.transform.position.x > fighterObject.transform.position.x){
                    facingInvert = false;
                }
                else{
                    facingInvert = true;
                }
            }
        }

        if(fighterActionState == FighterActionState.Neutral || fighterActionState == FighterActionState.Knockdown){      // CHANGE FACING
            if(facingInvert){
                // fighterObject.transform.rotation = Quaternion.Euler(0,0,0);
                fighterObject.transform.localScale = new Vector3(-1,1,1);
            }
            else{
                // fighterObject.transform.rotation = Quaternion.Euler(0,180,0);
                fighterObject.transform.localScale = new Vector3(1,1,1);
            }
        }

        if(comboed > 0){
            if(fighterActionState == FighterActionState.Neutral || fighterActionState == FighterActionState.Attacking || fighterActionState == FighterActionState.NonattackAction){
                comboed = 0;
            }
        }

        // Update UI for HP and Super
        playerUI.HPGreen.fillAmount = (float)HP/MaxHP;
        if(comboed == 0){
            playerUI.HPRed.fillAmount = (float)HP/MaxHP;
        }
        playerUI.SuperBlue.fillAmount = (float)meter/maxMeter;
        
        if(meter == maxMeter){playerUI.SuperNumber.text = "4";}
        else if(meter >= (float)maxMeter*0.75){playerUI.SuperNumber.text = "3";}
        else if(meter >= (float)maxMeter*0.5){playerUI.SuperNumber.text = "2";}
        else if(meter >= (float)maxMeter*0.25){playerUI.SuperNumber.text = "1";}
        else{playerUI.SuperNumber.text = "0";}


        if(playersManager.battleState == BattleState.Battle){
            // Debug.Log("h");
            bool acted = false;

            if(inputTime.Count == 1){
                inputTime[0] = Time.time;
            }

            RemoveInputs();
            CheckDirectionDown();
            CheckDirectionRelease();

            if(fighterActionState == FighterActionState.Grabbed){           // GETTING GRABBED LOGIC
                if(otherPlayer.fighterActionState == FighterActionState.Grabbing){
                    if(grabTracker == null){
                        grabTracker = GameObject.Find("GrabTracker");
                    }
                    fighterObject.transform.position = grabTracker.transform.position;
                    fighterObject.transform.rotation = grabTracker.transform.rotation;
                    Action(fighter.Grabbed);
                    return;
                }
                fighterObject.transform.rotation = new Quaternion(0,180,0,0); // reset rotation after grab finishes
                if(fighterObject.transform.position.y < 0){
                    fighterObject.transform.position = new Vector3(fighterObject.transform.position.x, 0,0);
                    fighterState = FighterState.Standing;
                }
                else{
                    fighterObject.transform.position = new Vector3(fighterObject.transform.position.x, fighterObject.transform.position.y,0);
                    fighterState = FighterState.InAir;
                }
                Box grabHit = grabTracker.GetComponent<Box>();
                GetHit(grabHit.damage,grabHit.freeze,grabHit.hitstun,grabHit.xKnockback,grabHit.yKnockback,grabHit.trip,grabHit.knockdown,false,grabHit.attackType,grabHit.chipDamage,grabHit.xShieldKnockback,grabHit.yShieldKnockback);
            }

            if(fighterActionState != FighterActionState.Knockdown){
                knockedDownOnGround = false;
            }

            if(fighterActionState == FighterActionState.Shield){
                if(otherPlayer.fighterActionState != FighterActionState.Attacking){
                    fighterActionState = FighterActionState.Neutral;
                }
                else if(inputs[inputs.Count-1] != "7" && inputs[inputs.Count-1] != "4" && inputs[inputs.Count-1] != "1"){
                    fighterActionState = FighterActionState.Neutral;
                }
            }

            
            // if(fighterActionState != FighterActionState.Attacking || moveHasHit){
            if(Input.GetButtonDown("p" + playerNumber + " x")){
                AddInput("x");
            }
            else if(Input.GetButtonDown("p" + playerNumber + " y")){
                AddInput("y");
            }
            // }

            if(fighterState != FighterState.InAir && fighterActionState == FighterActionState.Knockdown && !knockedDownOnGround){
                knockdownTimer = Time.time;
                knockedDownOnGround = true;
            }

            if(fighterActionState == FighterActionState.Attacking || fighterActionState == FighterActionState.Cancellable || fighterActionState == FighterActionState.NonattackAction || fighterActionState == FighterActionState.Grabbing){
                if(fighterObject.GetComponentInChildren<PlayableDirector>().state != PlayState.Playing){
                    fighterActionState = FighterActionState.Neutral;
                }
            }

            // COMMAND INPUT CHECKING SYSTEM
            for(int i = 0; i < fighter.inputActions.Count; i++){
                ActionInput inputAction = fighter.inputActions[i];
                if(!acted){
                    if(fighterState == inputAction.validFighterState){ // instantly skip if invalid state
                        if(fighterActionState == inputAction.validActionState || fighterActionState == FighterActionState.Cancellable  &&  inputAction.validActionState == FighterActionState.Neutral || fighterActionState == FighterActionState.Attacking && currentAction.CancelInto.Count > 0 && moveHasHit){ // also checks for cancellable moves
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
                                        if(inputAction.validActionState == FighterActionState.Attacking || fighterActionState != FighterActionState.Attacking && fighterActionState != FighterActionState.Attacking){
                                            Action(inputAction.action);
                                            acted = true;
                                            ResetInputLists();
                                        }
                                        else{  // Check for attack cancelling inputs
                                            if(fighterActionState == FighterActionState.Attacking){
                                                validInput = false;
                                                foreach(Action a in currentAction.CancelInto){
                                                    if(a == inputAction.action){
                                                        validInput = true;
                                                    }
                                                }
                                                if(validInput){
                                                    Debug.Log("CANCELLED!");
                                                    Action(inputAction.action);
                                                acted = true;
                                                ResetInputLists();
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }

            if(moveHasHit){  // This is for move cancelling in combos
                if(fighterState != FighterState.InAir){
                    if(holdingDown){
                        fighterState = FighterState.Crouching;
                    }
                    else{
                        if(inputs.Count > 0){
                            if(inputs[inputs.Count-1] == "2" || inputs[inputs.Count-1] == "1" || inputs[inputs.Count-1] == "3"){
                                fighterState = FighterState.Crouching;
                            }
                            else if(inputs[inputs.Count-1] == "4" || inputs[inputs.Count-1] == "5" || inputs[inputs.Count-1] == "6"){
                                fighterState = FighterState.Standing;
                            }
                        }
                        else{
                            fighterState = FighterState.Standing;
                        }
                    }
                }
            }

            
            if(fighterActionState == FighterActionState.Neutral){  // CHECKS CROUCH/STANDING AND JUMP START
                if(fighterObject.transform.position.y > 0){
                    fighterState = FighterState.InAir;
                }
                else{
                    if(inputs.Count > 0){
                        if(inputs[inputs.Count-1] == "2" || inputs[inputs.Count-1] == "1" || inputs[inputs.Count-1] == "3"){
                            fighterState = FighterState.Crouching;
                        }
                        else if(inputs[inputs.Count-1] == "4" || inputs[inputs.Count-1] == "5" || inputs[inputs.Count-1] == "6"){
                            fighterState = FighterState.Standing;
                        }
                        
                        // if(inputs[inputs.Count-1] == "7" || inputs[inputs.Count-1] == "8" || inputs[inputs.Count-1] == "9"){
                        //     if(fighterState == FighterState.Crouching){
                        //         Debug.Log("SUPERJUMP 1");
                        //         Action(fighter.SuperJump);
                        //     }
                        //     else{
                        //         if(inputs.Count >= 3 && inputTime[inputs.Count-3]+0.1f > Time.time){
                        //             if(inputs[inputs.Count-3] == "2" || inputs[inputs.Count-3] == "1" || inputs[inputs.Count-3] == "3"){
                        //                 Debug.Log("SUPERJUMP 2");
                        //                 Action(fighter.SuperJump);
                        //             }
                        //             else{
                        //                 Action(fighter.Jump);
                        //             }
                        //         }
                        //         else{
                        //             Action(fighter.Jump);
                        //         }
                        //     }
                        // }
                    }
                }
            }

            if(fighterActionState == FighterActionState.Neutral || currentAction.cancelIntoJumpstart){  // CHECKS JUMP START
                if(inputs.Count > 0 && fighterState != FighterState.InAir){
                    if(inputs[inputs.Count-1] == "7" || inputs[inputs.Count-1] == "8" || inputs[inputs.Count-1] == "9"){
                        if(fighterState == FighterState.Crouching){
                            Debug.Log("SUPERJUMP 1");
                            Action(fighter.SuperJump);
                        }
                        else{
                            if(inputs.Count >= 3 && inputTime[inputs.Count-3]+0.1f > Time.time){
                                if(inputs[inputs.Count-3] == "2" || inputs[inputs.Count-3] == "1" || inputs[inputs.Count-3] == "3"){
                                    Debug.Log("SUPERJUMP 2");
                                    Action(fighter.SuperJump);
                                }
                                else{
                                    Action(fighter.Jump);
                                }
                            }
                            else{
                                Action(fighter.Jump);
                            }
                        }
                    }
                }
            }


            if(otherPlayer.fighterActionState == FighterActionState.Attacking){ // CODE TO ACTIVATE SHIELDING
                if(fighterActionState == FighterActionState.Neutral || fighterActionState == FighterActionState.Cancellable || fighterActionState == FighterActionState.Shield){
                    if(inputs.Count > 0){
                        if(inputs[inputs.Count-1] == "7" || inputs[inputs.Count-1] == "4" || inputs[inputs.Count-1] == "1"){
                            if(fighterState == FighterState.Crouching){
                                Action(fighter.CrouchShield);
                            }
                            else if(fighterState == FighterState.InAir){
                                Action(fighter.AirShield);
                            }               
                            else{
                                Action(fighter.GroundShield);
                            }
                        }
                    }
                }
            }

            if(!acted){
                if(fighterActionState == FighterActionState.Neutral){
                    if(fighterState == FighterState.Standing){
                        if(inputs.Count > 0){
                            if(inputs[inputs.Count-1] == "6"){         //  WALKING
                                
                                    Action(fighter.WalkForwards);
                                    if(xVel < fighter.walkSpeed){
                                        SetVelocityX(fighter.walkSpeed);
                                    }
                                
                            }
                            else if(inputs[inputs.Count-1] == "4"){    // WALKING BACKWARDS
                                
                                    Action(fighter.WalkBackwards);
                                    if(xVel > fighter.walkBackSpeed){
                                        SetVelocityX(fighter.walkBackSpeed);
                                    }
                                
                            }
                            else{
                                Action(fighter.Idle);           //  IDLE
                                if(inputs.Count >= 2){
                                    if(inputs[inputs.Count-2] == "4" || inputs[inputs.Count-2] == "6"){
                                        SetVelocityX(0);
                                    }
                                }
                            }
                        }
                        else{
                            Action(fighter.Idle);   //  ALT IDLE CONDITION
                        }
                    }
                    else if(fighterState == FighterState.Crouching){
                        Action(fighter.Crouching);
                    }
                    else if(fighterState == FighterState.InAir){
                        if(inputs.Count > 0){
                            if(inputs[inputs.Count-1] == "6"){         //  Air drift forwards
                                if(xVel < fighter.airDrift){
                                    SetVelocityX(fighter.airDrift);
                                }
                                
                            }
                            else if(inputs[inputs.Count-1] == "4"){    // Air drift backwards
                                if(xVel > fighter.airDriftBack){
                                    SetVelocityX(fighter.airDriftBack);
                                }
                                
                            }
                            else{
                                Action(fighter.AirIdle);
                            }
                        }
                        else{
                            Action(fighter.AirIdle);
                        }
                    }
                }
                else if(fighterActionState == FighterActionState.Knockdown){
                    if(inputs.Count > 0 && knockdownTimer + 1 < Time.time && fighterState != FighterState.InAir){
                        if(inputs[inputs.Count-1] == "7" || inputs[inputs.Count-1] == "8" || inputs[inputs.Count-1] == "9"){
                            Action(fighter.NeutralGetUp);
                        }
                        else{
                            Action(fighter.KnockedDown);
                        }
                    }
                    else{
                        Action(fighter.KnockedDown);
                    }
                }
            }

            if(fighterActionState == FighterActionState.Hit){         // deal with hitstun and related things

                if(incomingKnockback){
                    if(freezeTimer + freezeTime < Time.time){
                        AddVelocity(incomingXvel*-1,incomingYvel);
                        incomingKnockback = false;
                    }
                }

                if(stunTimer + stunTime < Time.time){
                    comboed = 0;
                    fighterActionState = FighterActionState.Neutral;
                }
                else if(fighterState == FighterState.Standing || fighterState == FighterState.Crouching){
                    Action(fighter.GroundHit);
                }
                else{
                    Action(fighter.AirHit);
                }
            }

            if(knockedDownOnGround && fighterActionState == FighterActionState.Knockdown){
                if(knockdownTimer + 4 < Time.time){
                    Action(fighter.NeutralGetUp);
                }
                // else{
                //     Debug.Log(knockdownTimer + 4 + " vs " + Time.time);
                // }
            }

        }
    }

    public void GetHit(int damage, float freeze, float stun, float xKnockback, float yKnockback, bool willTrip, bool willKnockdown, bool superArmored, AttackType attackType, int chipDamage, float xShieldKnockback, float yShieldKnockback){
        if(lastHitTime != Time.time){ // prevents overlapping hurtboxes getting hit multiple times at once
            lastHitTime = Time.time;

            bool shieldedSuccessfully = false;                       // CHECK FOR SHIELD HERE
            if(fighterActionState == FighterActionState.Shield){
                if(attackType == AttackType.Neutral){
                    shieldedSuccessfully = true;
                }
                else if(attackType == AttackType.Overhead){
                    if(fighterState == FighterState.Standing || fighterState == FighterState.InAir){
                        shieldedSuccessfully = true;
                    }
                }
                else if(attackType == AttackType.Low && fighterState == FighterState.Crouching){
                    shieldedSuccessfully = true;
                }
            }

            if(shieldedSuccessfully){
                Debug.Log("Shielded!");
                HP -= chipDamage;
                AddVelocity(xShieldKnockback*-1,yShieldKnockback);
            }
            else{
                Debug.Log("Got hit!");
                HP -= damage;
                if(!superArmored){
                    if(willTrip && fighterState == FighterState.Standing || willTrip && fighterState == FighterState.Crouching){
                        comboed += 1;
                        incomingKnockback = false;
                        fighterActionState = FighterActionState.Knockdown;
                    }
                    else{
                        incomingKnockback = true;
                        incomingXvel = xKnockback;
                        incomingYvel = yKnockback;

                        float t = Time.time;
                        freezeTime = freeze;
                        freezeTimer = t;
                        stunTime = stun;
                        stunTimer = t;

                        comboed += 1;

                        fighterActionState = FighterActionState.Hit;
                    }
                }
            }


        }

    }

    public void AddVelocity(float x, float y){
        if(facingInvert){
           x *= -1; 
        }
        xVel += x;
        yVel += y;
    }

    public void SetVelocityX(float x){
        if(facingInvert){
           x *= -1; 
        }
        xVel = x;
    }

    public void PlayerPhysics(){
        if(!incomingKnockback && fighterActionState != FighterActionState.Grabbed){ // pauses during freeze
            
            if(fighterState == FighterState.InAir){
                if(fighterObject.transform.position[1]+yVel < 0 && fighterObject.transform.position.y + yVel < fighterObject.transform.position.y){
                    Debug.Log("YOU LANDED");
                    yVel = 0;
                    fighterObject.transform.position = new Vector3(fighterObject.transform.position[0],0,fighterObject.transform.position[2]);
                    fighterState = FighterState.Standing;
                    if(currentAction.cancelOnLanding){
                        
                        // DONT USE Action(fighter.Idle) HERE IT WILL CAUSE INFINITE RANGE ATTACKS FOR SOME REASON
                        fighterActionState = FighterActionState.Neutral;
                    }
                }
                else{
                    yVel -= 0.1f;
                }
            }

            if(fighterObject.transform.position[1]+yVel >= 0){
                fighterObject.transform.position = new Vector3(fighterObject.transform.position[0]+xVel+xVelContact,fighterObject.transform.position[1]+yVel,0);
            }
            else{
                fighterObject.transform.position = new Vector3(fighterObject.transform.position[0]+xVel+xVelContact,0,0);
            }


            if(fighterState != FighterState.InAir && yVel > 0){
                fighterState = FighterState.InAir;
            }
            

            float n = 0.915f;
            // if(fighter.name == "Dark Gibson"){
            //     n = 0.965f;
            // }

            if(fighterState == FighterState.InAir){  // don't decrease xVel while during a self-instantiated action
                if(fighterActionState == FighterActionState.Attacking || fighterActionState == FighterActionState.NonattackAction){}
                else{xVel = xVel*n;}
            }
            else{
                xVel = xVel*n;
            }
            yVel = yVel*n;
        }
    }

    public void Action(Action action){
        if(actionTimeline.currentActionName == action.name){  // stop if the action is already in action
            return;
        }

        if(meter + action.meterGain > maxMeter){
            meter = maxMeter;
        }
        else{
            meter += action.meterGain;
        }

        if(action.actionStateDuringAction == FighterActionState.Shield){
            SetVelocityX(0);
        }

        // set action state
        fighterActionState = action.actionStateDuringAction;
        
        // destroys preexisting animation timeline
        if(actionTimeline.transform.childCount > 0){
            Destroy(actionTimeline.transform.GetChild(0).gameObject);
        }

        BoxData boxData = Instantiate(action.boxData,actionTimeline.transform.position,actionTimeline.transform.rotation);
        if(facingInvert){
            // boxData.transform.Rotate(0,180,0);
            boxData.transform.localScale = new Vector3(-1,1,1);
        }

        boxData.transform.parent = actionTimeline.transform;
        boxData.PlayerNumber = playerNumber;
        actionTimeline.currentActionName = action.name;

        currentAction = action;
        moveHasHit = false;

        boxData.StartAnim(fighterObject);
    }

    public void ResetInputLists(){
        inputs.Clear();
        inputTime.Clear();
    }

    public void InitializeBattleStart(Vector3 pos, Quaternion rot, PlayerBattleUI pUI){
        fighterObject = fighter.model;
        fighterObject = Instantiate(fighterObject,pos, rot);  // THE CODE WILL SELF DESTRUCT AND INFINITELY SPAWN INCOMPLETE PLAYERS IF OBJECT IS NOT DEFINIED HERE, DO NOT CHANGE!!!
        // fighterObject.AddComponent<Animator>();
        animator = fighterObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = fighter.animator;

        actionTimeline = Instantiate(timelineHandlerPrefab,fighterObject.transform.position,fighterObject.transform.rotation);  // THE CODE WILL SELF DESTRUCT AND INFINITELY SPAWN INCOMPLETE PLAYERS IF OBJECT IS NOT DEFINIED HERE, DO NOT CHANGE!!!
        actionTimeline.transform.parent = fighterObject.transform;

        playerUI = pUI;
        playerUI.FighterName.text = fighter.fighterName;
        playerUI.portrait.sprite = fighter.battlePortrait;

        MaxHP = fighter.maxHP;
        HP = MaxHP;
        meter = 0;
        facingInvert = false;
        fighterState = FighterState.Standing;
        fighterActionState = FighterActionState.Neutral;
    }

    void AddInput(string input){
        // Debug.Log("AddInput()");
        if(inputs.Count > 0){
            if(input == "1" || input == "2" || input == "3" || input == "4" || input == "5" || input == "6" || input == "7" || input == "8" || input == "9"){
                if(input == inputs[inputs.Count-1]){
                    return;  // Prevents double direct input glitch
                }
            }
        }

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
            holdingDown = true;
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
                holdingDown = true;
                if(facingInvert == false){AddInput("1");}
                else{AddInput("3");}
            }
            else if(Input.GetButton("p" + playerNumber + " up")){
                holdingDown = false;
                if(facingInvert == false){AddInput("7");}
                else{AddInput("9");}
            }
            else{
                holdingDown = false;
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
        }

        else if(Input.GetButtonDown("p" + playerNumber + " right")){
            if(Input.GetButton("p" + playerNumber + " down")){
                holdingDown = true;
                if(facingInvert == false){AddInput("3");}
                else{AddInput("1");}
            }
            else if(Input.GetButton("p" + playerNumber + " up")){
                holdingDown = false;
                if(facingInvert == false){AddInput("9");}
                else{AddInput("7");}
            }
            else{
                holdingDown = false;
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
            }
        }

        else if(Input.GetButtonDown("p" + playerNumber + " up")){
            holdingDown = false;
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
            holdingDown = false;
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
            else if(Input.GetButton("p" + playerNumber + " right")){
                if(facingInvert == false){AddInput("6");}
                else{AddInput("4");}
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
            else if(Input.GetButton("p" + playerNumber + " left")){
                if(facingInvert == false){AddInput("4");}
                else{AddInput("6");}
            }
            else{AddInput("5");}
        }

    }




}