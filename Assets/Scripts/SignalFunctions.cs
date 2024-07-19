using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class SignalFunctions : MonoBehaviour
{
    public void xVelocityAdd(float x){
        Debug.Log("xVelAdd");
        if(GetComponent<BoxData>().PlayerNumber == 1){
            GameObject.Find("P1").GetComponent<Player>().AddVelocity(x,0,false);
        }
        else if(GetComponent<BoxData>().PlayerNumber == 2){
            GameObject.Find("P2").GetComponent<Player>().AddVelocity(x,0,false);
        }
    }

    public void yVelocityAdd(float y){
        Debug.Log("yVelAdd");
        if(GetComponent<BoxData>().PlayerNumber == 1){
            GameObject.Find("P1").GetComponent<Player>().AddVelocity(0,y,false);
        }
        else if(GetComponent<BoxData>().PlayerNumber == 2){
            GameObject.Find("P2").GetComponent<Player>().AddVelocity(0,y,false);
        }
    }

    public void xVelocitySet(float x){
        Debug.Log("xVelSet");
        if(GetComponent<BoxData>().PlayerNumber == 1){
            GameObject.Find("P1").GetComponent<Player>().SetVelocityX(x);
        }
        else if(GetComponent<BoxData>().PlayerNumber == 2){
            GameObject.Find("P2").GetComponent<Player>().SetVelocityX(x);
        }
    }

    public void Jump(){
        JumpingLogic(false);
    }
    public void SuperJump(){
        JumpingLogic(true);
    }

    private void JumpingLogic(bool superJump){
        Debug.Log("jump");
        Player p;
        if(GetComponent<BoxData>().PlayerNumber == 1){
            p = GameObject.Find("P1").GetComponent<Player>();
        }
        else{
            p = GameObject.Find("P2").GetComponent<Player>();
        }
        
        float reduced = 1f;
        if(p.holdingDown){
            reduced = 0.5f;
        }

        if(p.inputs.Count > 0){
            if(p.inputs[p.inputs.Count-1] == "9" || p.inputs[p.inputs.Count-1] == "6" || p.inputs[p.inputs.Count-1] == "3"){
                if(superJump){
                    p.AddVelocity(p.fighter.diagonalSuperJumpVertMultiplier,p.fighter.superJumpHeight*reduced,false);
                }
                else{
                    p.AddVelocity(p.fighter.diagonalJumpVertMultiplier,p.fighter.jumpHeight*reduced,false);
                }
            }
            else if(p.inputs[p.inputs.Count-1] == "7" || p.inputs[p.inputs.Count-1] == "4" || p.inputs[p.inputs.Count-1] == "1"){
                if(superJump){
                    p.AddVelocity(p.fighter.diagonalSuperJumpVertMultiplier,p.fighter.superJumpHeight*reduced,false);
                }
                else{
                    p.AddVelocity(p.fighter.diagonalJumpVertMultiplier*-1f,p.fighter.jumpHeight*reduced,false);
                }
            }
            else{
                if(superJump){
                    p.AddVelocity(0,p.fighter.superJumpHeight*reduced,false);
                }
                else{
                    p.AddVelocity(0,p.fighter.jumpHeight*reduced,false);
                }
            }
        }
        else{
            p.AddVelocity(0,p.fighter.jumpHeight*reduced,false);
        }
        p.fighterState = FighterState.InAir;
        p.fighterActionState = FighterActionState.Cancellable;
    }

    public void GrabRelease(){
        Debug.Log("Grab Release!");
        Player p;
        if(GetComponent<BoxData>().PlayerNumber == 1){
            p = GameObject.Find("P1").GetComponent<Player>();
        }
        else{
            p = GameObject.Find("P2").GetComponent<Player>();
        }
        p.fighterActionState = FighterActionState.Attacking;
        p.moveHasHit = true;
    }

    public void PlaySound(int n){
        SfxManager.instance.PlaySFX(SfxManager.instance.SoundsForSignalFunctions[n],SfxManager.instance.transform,1);
    }

}
