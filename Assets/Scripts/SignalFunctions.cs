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
}
