using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour
{
    [SerializeField] float acceleration = .1f;
    [SerializeField] float speed = 0;
    float direction = 0;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] bool falling = false;
    int bounces = 0;
    // Start is called before the first frame update

    void OnCollisionEnter2D(Collision2D c) {
        if(c.gameObject.layer == 12){
            this.setSpeed(speed * -2f/3f);
        }
        bounces++;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(falling) {
            rb.velocity = new Vector3(direction,speed,-2);
            speed-=acceleration;
        }
        if(bounces == 3) {
            this.gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void setFalling(bool t) {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        falling = t;
    }
    public void setDirection(float d) {
        direction =d;
    }
    public void setSpeed(float s) {
        speed = s;
    }
}
