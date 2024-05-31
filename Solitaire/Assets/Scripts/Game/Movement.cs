using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float currentTime = 0;
    Vector3 location;

    public void moveCard(Transform s, Vector3 f, float time){
        StartCoroutine(moveCardCoroutine(s,f,time));
    }

    public void changeLocation(Vector3 loc) {
        location = loc;
    }



    //move Card
    IEnumerator moveCardCoroutine(Transform s, Vector3 f,float time) {
        
        float totalTime = 0;
        Vector3 tempVec = s.position;
        location = f;

        SpriteRenderer rend = s.gameObject.GetComponent<SpriteRenderer>();
        if(rend != null) {
            rend.sortingOrder = 2;
        }
        //while s isn't at f, then keep adding tim/ deltatime to position
        while(totalTime < time){
            yield return null;
            totalTime += Time.deltaTime;
            //acceleration
            float a = 1f;
            //place in curve
            float t = 1f;
            float x = (totalTime/time)*2*t;
            float y = 0;
            if(totalTime/time < 1f/2f){
                y = a*Mathf.Pow(x,2);
            } else{
                y = -a*Mathf.Pow(x-(2*t),2) + (2*a*Mathf.Pow(t,2));
            }
            y = y/(2*a*Mathf.Pow(t,2));

            s.position = Vector3.Lerp(tempVec,location,y);
        }
        s.position = location;
        if(rend!= null) {
            rend.sortingOrder = 1;
        }
    }

}
