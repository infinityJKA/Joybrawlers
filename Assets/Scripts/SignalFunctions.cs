using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SignalFunctions : MonoBehaviour
{
    public void xVelocityAdd(float x){
        Debug.Log("xVelAdd");
        if(GetComponent<BoxData>().PlayerNumber == 1){
            GameObject.Find("P1").GetComponent<Player>().xVel += x;
        }
        else if(GetComponent<BoxData>().PlayerNumber == 2){
            GameObject.Find("P2").GetComponent<Player>().xVel += x;
        }
    }

    public void yVelocityAdd(float x){

    }
}
