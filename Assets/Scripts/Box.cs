using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Box : MonoBehaviour
{
    public BoxType boxType = BoxType.Hurtbox;
    public int damage = 0;
    public float hitstun,knockbackDelay,xKnockback,yKnockback;
    public bool trip = false,knockdown = false;
    public AttackType attackType;
    public int playerNumber;
    public int ActiveHurtCollisions = 0;
    public Players players;


    void Start(){
        players = GameObject.Find("Players").GetComponent<Players>();
        if(players.showFrameData == false){
            transform.GetComponent<MeshRenderer>().enabled = false;
        }
        playerNumber = transform.GetComponentInParent<BoxData>().PlayerNumber;
    }

    void OnTriggerEnter(Collider c){
        if(c.gameObject.GetComponent<Box>() == true){
            Box otherBox = c.gameObject.GetComponent<Box>(); 
            if(otherBox.playerNumber != playerNumber){
                if(boxType == BoxType.Hurtbox && otherBox.boxType == BoxType.Hurtbox){
                    ActiveHurtCollisions += 1;
                }
            }
        }
    }

    void OnTriggerStay(Collider c){
        Debug.Log("JJJ");
        if(boxType == BoxType.Hurtbox){
            if(c.gameObject.GetComponent<Box>() == true){
                Box otherBox = c.gameObject.GetComponent<Box>(); 
                if(otherBox.playerNumber != playerNumber && otherBox.boxType == BoxType.Hurtbox){
                    if(playerNumber == 1){
                        if(players.player1.facingInvert == false && players.player1.xVel > 0 || players.player1.facingInvert && players.player1.xVel < 0){
                            players.player2.xVelContact = players.player1.xVel;
                        }
                        else{
                            players.player2.xVelContact = 0;
                        }
                    }
                    else{
                        if(players.player2.facingInvert == false && players.player2.xVel > 0 || players.player2.facingInvert && players.player2.xVel < 0){
                            players.player1.xVelContact = players.player2.xVel;
                        }
                        else{
                            players.player1.xVelContact = 0;
                        }
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider c){
        if(boxType == BoxType.Hurtbox){
            if(c.gameObject.GetComponent<Box>() == true){
                Box otherBox = c.gameObject.GetComponent<Box>(); 
                if(otherBox.playerNumber != playerNumber && otherBox.boxType == BoxType.Hurtbox){
                    ActiveHurtCollisions -= 1;
                    // Players players = GameObject.Find("Players").GetComponent<Players>();
                    // if(playerNumber == 1){
                    //     players.player1.xVelContact = 0;
                    //     players.player1.yVelContact = 0;
                    // }
                    // else{
                    //     players.player2.xVelContact = 0;
                    //     players.player2.yVelContact = 0;
                    // }
                }
            }
        }
    }

    void Update(){
        if(ActiveHurtCollisions == 0){
            if(playerNumber == 1){
                players.player1.xVelContact = 0;
            }
            else{
                players.player2.xVelContact = 0;
            }
        }
    }

}

public enum BoxType{Hurtbox,Hitbox}
public enum AttackType{Neutral,Overhead,Low,Unshieldable}