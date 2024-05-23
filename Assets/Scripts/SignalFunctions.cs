using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SignalFunctions : MonoBehaviour
{
    public void xVelocityAdd(float x){
        Debug.Log("xVelAdd");
        if(GetComponent<BoxData>().PlayerNumber == 1){
            GameObject.Find("P1").GetComponent<Player>().AddVelocity(x,0);
        }
        else if(GetComponent<BoxData>().PlayerNumber == 2){
            GameObject.Find("P2").GetComponent<Player>().AddVelocity(x,0);
        }
    }

    public void yVelocityAdd(float y){
        Debug.Log("yVelAdd");
        if(GetComponent<BoxData>().PlayerNumber == 1){
            GameObject.Find("P1").GetComponent<Player>().AddVelocity(0,y);
        }
        else if(GetComponent<BoxData>().PlayerNumber == 2){
            GameObject.Find("P2").GetComponent<Player>().AddVelocity(0,y);
        }
    }

    public void Jump(){
        Debug.Log("jump");
        Player p;
        if(GetComponent<BoxData>().PlayerNumber == 1){
            p = GameObject.Find("P1").GetComponent<Player>();
        }
        else{
            p = GameObject.Find("P2").GetComponent<Player>();
        }

        if(p.inputs.Count > 0){
            if(p.inputs[p.inputs.Count-1] == "9" || p.inputs[p.inputs.Count-1] == "6" || p.inputs[p.inputs.Count-1] == "3"){
                p.AddVelocity(p.fighter.walkSpeed*2.5f,p.fighter.jumpHeight);
            }
            else if(p.inputs[p.inputs.Count-1] == "7" || p.inputs[p.inputs.Count-1] == "4" || p.inputs[p.inputs.Count-1] == "1"){
                p.AddVelocity(p.fighter.walkBackSpeed*2.5f,p.fighter.jumpHeight);
            }
            else{
                p.AddVelocity(0,p.fighter.jumpHeight);
            }
        }
        else{
            p.AddVelocity(0,p.fighter.jumpHeight);
        }
    }
}
