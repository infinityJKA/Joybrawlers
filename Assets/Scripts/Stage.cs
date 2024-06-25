using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Stage", menuName = "JOYOUS ASSETS/Stage")]
public class Stage : ScriptableObject
{
    public string stageName;
    public Transform stagePrefab;
    public AudioClip music;
    public float musicVolume = 1;

}

