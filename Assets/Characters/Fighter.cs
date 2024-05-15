using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Fighter", menuName = "JOYOUS ASSETS/Fighter")]
public class Fighter : ScriptableObject
{
    public string fighterName;
    public int maxHP;
    public int jumpHeight;
    public int airJumps;
    public int airJumpHeight;
    public Sprite smallSelectIcon;
    public Sprite bigSelectIcon;
    public GameObject model;

}
