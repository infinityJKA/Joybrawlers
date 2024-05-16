using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Box : MonoBehaviour
{
    void Start(){
        if(GameObject.Find("Players").GetComponent<Players>().showFrameData == false){
            transform.GetComponent<MeshRenderer>().enabled = false;
        }

    }
}
