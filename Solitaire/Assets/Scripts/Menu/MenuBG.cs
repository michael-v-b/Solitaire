using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBG : MonoBehaviour
{
    [SerializeField] GameObject cardSprite;
    [SerializeField] Sprite[] suits = new Sprite[4];
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Color cardColor;
    // Start is called before the first frame update
    void Start()
    {
        int iter = 0;
        for(int i = -7; i < 7; i++) {
            iter++;
            for(int j = -7; j < 7; j++) {
                GameObject temp = Instantiate(cardSprite, this.gameObject.transform.position - new Vector3(i*2,j*2,0),Quaternion.identity,this.gameObject.transform);
                iter++;
                Debug.Log("thing: " + ((i+j) % 3));
                temp.GetComponent<SpriteRenderer>().sprite = suits[iter % 4];
                temp.GetComponent<SpriteRenderer>().color = cardColor;
            }
        }
        rb.velocity = new Vector3(2,2,0)*.5f;
        
    }

    public IEnumerator fallingCard() {
        while(true) {
            yield return new WaitForSeconds(.25f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.transform.position.x > 4) {
            this.gameObject.transform.position = new Vector3(0,0,90);
        }
    }
}
