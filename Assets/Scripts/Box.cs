using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Box : MonoBehaviour
{
    public int damage;
    public float xknockback,yknockback;
    public bool cancellableOnHit;
    void Start(){
        if(GameObject.Find("Players").GetComponent<Players>().showFrameData == false){
            transform.GetComponent<MeshRenderer>().enabled = false;
        }

    }
}

public enum BoxType{hitbox,hurtbox}
