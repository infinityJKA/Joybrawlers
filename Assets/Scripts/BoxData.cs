using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.EventSystems;


public class BoxData : MonoBehaviour
{
    public PlayableDirector frameData;
    public AnimationClip modelAnim;
    public int PlayerNumber; // either 1 for p1 or 2 for p2

    public void StartAnim(GameObject model){
        // adds the relevant model to the timeline and the animation to play
        TimelineAsset asset = frameData.playableAsset as TimelineAsset;
        AnimationTrack newTrack = asset.CreateTrack<AnimationTrack>("modelAnim");
        TimelineClip clip = newTrack.CreateClip(modelAnim);
        frameData.SetGenericBinding(newTrack, model);


        frameData.Play();
        Debug.Log("Play!");
    }
    
}
