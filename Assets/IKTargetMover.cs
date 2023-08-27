using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTargetMover : MonoBehaviour{


    public Vector2 oldLeftTarget = new Vector2();
    public Vector2 oldRightTarget = new Vector2();

    public Vector2 newLeftTarget = new Vector2();
    public Vector2 newRightTarget = new Vector2();

    public bool lerpingLeft = false;
    public bool lerpingRight = false;

    public float lerp = 0;
    public float lerpSpeed = 0.02f;

    public Vector3 playerPos;

    GameObject player;
    PlayerMovement playerScript;
    GameObject playerRoot;

    GameObject leftTargetObject;
    GameObject rightTargetObject;

    LayerMask platformLayer;

    // Start is called before the first frame update
    void Start(){

        player = transform.Find("character body").gameObject;
        playerScript = player.GetComponent<PlayerMovement>();
        playerRoot = player.transform.Find("Root").gameObject;

        leftTargetObject = transform.Find("Left Leg Target").gameObject;
        rightTargetObject = transform.Find("Right Leg Target").gameObject;

        platformLayer = LayerMask.GetMask("damageable", "platforms");

    }

    // Update is called once per frame
    void FixedUpdate(){
        playerPos = playerRoot.transform.position;

        setTarget();

        if (canLerpLeft()) {
            lerpingLeft = true;
        }

        if (lerpingLeft) {
            lerpSpeed = Mathf.Abs(oldLeftTarget.x - newLeftTarget.x) / 3f;

            if (lerpSpeed < 0.02f) {
                lerpSpeed = 0.02f;
            }

            lerp += lerpSpeed;
            if (Vector3.Distance(oldLeftTarget, newLeftTarget) > 1f) {
                lerpLeft(0.3f);
            }
            else {
                lerpLeft(0.1f);
            }
        }

        if (canLerpRight()) {
            lerpingRight = true;
        }

        if (lerpingRight) {
            lerpSpeed = Mathf.Abs(oldRightTarget.x - newRightTarget.x) / 3f;

            if (lerpSpeed < 0.02f) {
                lerpSpeed = 0.02f;
            }

            lerp += lerpSpeed;
            if (Vector3.Distance(oldRightTarget, newRightTarget) > 1f) {
                lerpRight(0.3f);
            }
            else {
                lerpRight(0.1f);
            }

        }


    }

    RaycastHit2D rayCast(float offset) {
        Debug.DrawRay(playerPos + new Vector3(offset, 0, 0), Vector2.down * 2f, Color.red);

        return Physics2D.Raycast(playerPos + new Vector3(offset, 0, 0), Vector2.down, 2f, platformLayer);

    }
    
    void setTarget() {

        float leftLegOffset = 0f;
        float rightLegOffset = 0f;

        if (!playerScript.LLegAttached() || playerScript.isCrouching()) {
            rightLegOffset = 0.8f;
        }
        if (!playerScript.RLegAttached() || playerScript.isCrouching()) {
            leftLegOffset = -0.8f;
        }

        float maxLength = 1.5f;
        RaycastHit2D rayL;
        RaycastHit2D rayR;
        
        //dead code for making legs move right/left more when changing direction
        //disbaled because it causes both legs to sometimes be above the ground even though the the player should be on the ground, resulting in missed jumps
        //only the else statement should execute
        if(playerScript.getTransformVect().x == -1 && false) {
            rayL = rayCast(-0.8f + leftLegOffset);
            rayR = rayCast(-0.5f + rightLegOffset);
        } else if (playerScript.getTransformVect().x == 1 && false) {
            rayL = rayCast(0.5f + leftLegOffset);
            rayR = rayCast(0.8f + rightLegOffset);
        }
        else {
            rayL = rayCast(-0.3f + leftLegOffset );
            rayR = rayCast(0.3f + rightLegOffset);
            maxLength = 0.3f;
        }

        if (!playerScript.isGrounded()) {
            setAirTarget();
            return;
        }

        float distL = Vector3.Distance(leftTargetObject.transform.position, rayL.point);
        float distR = Vector3.Distance(rightTargetObject.transform.position, rayR.point);

        if(distL > maxLength && rayL.point != new Vector2(0, 0)) {
            //leftTargetObject.transform.position = rayL.point;
            newLeftTarget = rayL.point;
            oldLeftTarget = leftTargetObject.transform.position;
        }
        if(distR > maxLength && rayR.point != new Vector2(0, 0)) {
            //rightTargetObject.transform.position = rayR.point;
            newRightTarget = rayR.point;
            oldRightTarget = rightTargetObject.transform.position;
        }

        
    }


    void setAirTarget() {
        //Debug.Log(newLeftTarget + " " + oldLeftTarget + " " + lerp);
        newLeftTarget = (new Vector2(playerPos.x, playerPos.y) + new Vector2(-0.6f, -0.6f) + oldLeftTarget)/2f;
        oldLeftTarget = leftTargetObject.transform.position;
        newRightTarget = (new Vector2(playerPos.x, playerPos.y) + new Vector2(0.6f, -0.6f) + oldRightTarget) / 2f;
        oldRightTarget = rightTargetObject.transform.position;
        //Debug.Log(playerPos);

    }


    bool canLerpLeft() {
        return (!lerpingRight && newLeftTarget != oldLeftTarget && playerScript.LLegAttached() || !playerScript.isGrounded());
    }

    bool canLerpRight() {
        return (!lerpingLeft && newRightTarget != oldRightTarget && playerScript.RLegAttached() || !playerScript.isGrounded());
    }

    void lerpLeft(float walkHeight) {

        if (!playerScript.isGrounded()) {
            leftTargetObject.transform.position = Vector2.Lerp(new Vector2(playerPos.x, playerPos.y) + new Vector2(-0.6f, -0.6f), newLeftTarget, 0.5f * Time.deltaTime);
            return;
        }

            if (lerp > 1) {
            lerp = 0;
            lerpingLeft = false;
            leftTargetObject.transform.position = newLeftTarget;
            oldLeftTarget = leftTargetObject.transform.position;
            return;
        }

        leftTargetObject.transform.position = Vector2.Lerp(oldLeftTarget, newLeftTarget, lerp);
        //Debug.Log(oldLeftTarget + "  |  " + Vector2.Lerp(oldLeftTarget, newLeftTarget, lerp) + "  |  " + newLeftTarget + "  |  " + lerp);
        leftTargetObject.transform.position += new Vector3(0, Mathf.Sin(lerp * Mathf.PI)) * walkHeight;
    }

    void lerpRight(float walkHeight) {

        if (!playerScript.isGrounded()) {
            rightTargetObject.transform.position = Vector2.Lerp(new Vector2(playerPos.x, playerPos.y) + new Vector2(0.6f, -0.6f), newRightTarget, 0.5f * Time.deltaTime);
            return;
        }

        if (lerp > 1) {
            lerp = 0;
            lerpingRight = false;
            rightTargetObject.transform.position = newRightTarget;
            oldRightTarget = rightTargetObject.transform.position;
            return;
        }

        rightTargetObject.transform.position = Vector2.Lerp(oldRightTarget, newRightTarget, lerp);
        rightTargetObject.transform.position += new Vector3(0, Mathf.Sin(lerp * Mathf.PI)) * walkHeight;
    }
}
