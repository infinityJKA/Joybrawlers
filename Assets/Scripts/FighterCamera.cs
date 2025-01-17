using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class FighterCamera : MonoBehaviour
{
    public List<Transform> cameraFollowObjects;
    public float yOffset,minimumDistance,zOffset,zMax,yPotentialMax;
    private float xMin,xMax,yMin,yMax;

    private void LateUpdate(){
        if(cameraFollowObjects.Count == 0){
            return;
        }

        xMin = xMax = cameraFollowObjects[0].position.x;
        yMin = yMax = cameraFollowObjects[0].position.y;

        for(int i = 1; i < cameraFollowObjects.Count; i++){
            Transform obj = cameraFollowObjects[i];
            if(obj.position.x < xMin){
                xMin = obj.position.x;
            }
            else if(obj.position.x > xMax){
                xMax = obj.position.x;
            }
            if(obj.position.y < yMin){
                yMin = obj.position.y;
            }
            else if(obj.position.y > yMax){
                yMax = obj.position.y;
            }
        }

        float xMiddle = (xMin+xMax)/2;
        float yMiddle = (yMin+yMax)/2;

        float distance = xMax-xMin;
        if(yMax-yMin > distance){
            distance = yMax-yMin;
        }

        float yMod = 0;
        float zMod = 0;
        if(distance < minimumDistance){
            distance = minimumDistance;
        }
        else{
            yMod = (distance-minimumDistance)/4f;
            zMod = (distance-minimumDistance)*0.5f;
        }

        float x = xMiddle;
        float y = yMiddle+yOffset+yMod;
        float z = -distance-zOffset+zMod;

        if(z < -zMax){
            z = -zMax;
        }

        // if(y > yPotentialMax){
        //     y = yPotentialMax;
        // }

        
        transform.position = new Vector3(x,y,z);
        


    }

}
