using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEditor.VersionControl;


public class BoxData : MonoBehaviour
{
    public PlayableDirector frameData;
    public AnimationClip modelAnim;
    public int PlayerNumber; // either 1 for p1 or 2 for p2
    public int MaximumTracks;

    public void StartAnim(GameObject model){
        // adds the relevant model to the timeline and the animation to play
        TimelineAsset asset = frameData.playableAsset as TimelineAsset;
        // EraseExcessLayers(asset);

        if(asset.outputTrackCount > MaximumTracks){
            EraseOneExcessLayer(asset);
        }
        AnimationTrack newTrack = asset.CreateTrack<AnimationTrack>("modelAnim");
        TimelineClip clip = newTrack.CreateClip(modelAnim);
        frameData.SetGenericBinding(newTrack, model);

        frameData.Play();
        Debug.Log("Play!");
    }

    public void EraseExcessLayers(TimelineAsset asset){
        foreach(TrackAsset t in asset.GetOutputTracks()){ // this causes a million errors that don't seem to affect gameplay but without it the game lags like hell
             String s = t.ToString();
            if(s.Contains("modelAnim")){
                asset.DeleteTrack(t);
           }
        }
        Debug.Log(asset.outputTrackCount);
    }

    public void EraseOneExcessLayer(TimelineAsset asset){
        foreach(TrackAsset t in asset.GetOutputTracks()){ // same result as earlier
             String s = t.ToString();
            if(s.Contains("modelAnim")){
                asset.DeleteTrack(t);
                return;
           }
        }
    }

}

