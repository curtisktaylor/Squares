using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMover : MonoBehaviour{

    float time = 0;
    float dir = 1;

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
        time += Time.deltaTime;
        if(time > 10) {
            time = 0;
            dir *= -1;
        }

        transform.GetComponent<Rigidbody2D>().AddForce(new Vector3(50 * dir, -50, 0));

    }

    void OnCollisionStay2D(Collision2D collision) {
        if(collision.gameObject.GetComponent<IDamageable>() != null) {

            Vector3 scale = collision.gameObject.transform.localScale;
            if(collision.gameObject.GetComponent<Rigidbody2D>().velocity.x >= -0.01f && collision.gameObject.GetComponent<Rigidbody2D>().velocity.x <= 0.01f) {

                collision.gameObject.GetComponent<IDamageable>().damage(1);
                
                if(scale.x > 0.25f) {
                    collision.gameObject.transform.localScale = new Vector3(scale.x - 0.01f, scale.y, scale.z);
                }
                else {
                    collision.gameObject.GetComponent<IDamageable>().kill();
                }
            }
        }
    }
}
