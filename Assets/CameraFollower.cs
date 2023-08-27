using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

    GameObject playerBody;
    Rigidbody2D playerRB;
    Vector2 mousePos;
    Vector2 playerPos;
    Vector2 targetPoint;
    Vector2 lerpedPoint;


    // Start is called before the first frame update
    void Start(){

        playerBody = transform.parent.Find("character body").gameObject;
        playerRB = playerBody.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update(){


    }

    //camera should move every physics step, not every frame
    private void FixedUpdate() {
        moveCamera();
    }

    Vector2 combinePoints(Vector2 playerPos, Vector2 mousePos) { 

        return new Vector2((playerPos.x + mousePos.x)/2, (playerPos.y + mousePos.y)/2);
    
    }

    void moveCamera() {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerPos = playerBody.transform.position;

        targetPoint = combinePoints(playerPos, mousePos);

        lerpedPoint = Vector2.Lerp(transform.position, targetPoint, 0.5f);

        transform.position = new Vector3(lerpedPoint.x, lerpedPoint.y, transform.position.z);
        //transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }


}
