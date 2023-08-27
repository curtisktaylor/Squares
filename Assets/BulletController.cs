using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour{

    Rigidbody2D rb;
    ParticleSystem particleSys;

    void Start(){

        rb = GetComponent<Rigidbody2D>();
        particleSys = GetComponent<ParticleSystem>();
        
    }

    

    void Update(){

        //rotates the bullet to face the direction its moving
        transform.right = rb.velocity.normalized;

        //sets the particles lifetime to change based on how fast the bullet is moving
        var particleSystemMain = particleSys.main;
        particleSystemMain.startLifetime = rb.velocity.magnitude / 500f;

        //deletes bullet if it goes too far out of bounds
        if (outOfBoundsCheck()) {
            Destroy(gameObject);
        }

    }

    //destroy bullet on hit with a collider
    private void OnCollisionEnter2D(Collision2D collision) {

        //check if the colliding entity has a rigidbody first (aka if the colliding object has collision)
        if (collision.rigidbody != null) {

            //check if the colliding object is damageable
            if(collision.transform.GetComponent<IDamageable>() != null) {
                collision.transform.GetComponent<IDamageable>().damage(15);
            }

            Destroy(gameObject);
        }

    }


    //returns true if bullet is out of bounds
    bool outOfBoundsCheck() {
        return rb.position.y < -500;
    }
}
