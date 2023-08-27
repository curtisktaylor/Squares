using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour, IDamageable{

    [SerializeField] public float speed = 2000;
    [SerializeField] public int jumpScale = 1500;
    [SerializeField] public GameObject bulletPrefab;

    LayerMask platformLayer;

    GameObject pistolObject;
    ParticleSystem casingSpawner;
    GameObject handObject;
    GameObject parent;

    GameObject healthBar;

    GameObject lowerLeftLegTarget;
    GameObject lowerRightLegTarget;

    GameObject upperLeftLeg;
    GameObject upperRightLeg;
    GameObject lowerLeftLeg;
    GameObject lowerRightLeg;

    float boxCastOffset = 0f;

    float playerGravity = 4f;

    Vector3 transformVect = new Vector3(0, 0, 0);
    BoxCollider2D playerCollider;
    Rigidbody2D playerRB;

    bool readyToJump = false;

    bool leftLegAttached = true;
    bool rightLegAttached = true;

    List<GameObject> bulletClones = new List<GameObject>(100);

    float timePassed = 0;


    void Start(){

        playerCollider = GetComponent<BoxCollider2D>();
        playerRB = GetComponent<Rigidbody2D>();

        platformLayer = LayerMask.GetMask("damageable", "platforms");

        healthBar = transform.Find("Health Bar").gameObject;
        handObject = transform.Find("hand").gameObject;
        pistolObject = handObject.transform.Find("pistol").gameObject;
        casingSpawner = handObject.transform.Find("casing spawner").gameObject.GetComponent<ParticleSystem>();

        upperLeftLeg = transform.Find("Root").transform.Find("upper left leg").gameObject;
        upperRightLeg = transform.Find("Root").transform.Find("upper right leg").gameObject;
        lowerLeftLeg = upperLeftLeg.transform.Find("lower left leg").gameObject;
        lowerRightLeg = upperRightLeg.transform.Find("lower right leg").gameObject;

        lowerLeftLegTarget = lowerLeftLeg.transform.Find("Left Leg Effector").gameObject;
        lowerRightLegTarget = lowerRightLeg.transform.Find("Right Leg Effector").gameObject;

        parent = transform.parent.gameObject;

    }

    
    void Update(){

        checkMovement();
        movePlayer();

        timePassed += Time.deltaTime;

        if (timePassed > 5 && leftLegAttached) {
            //detachLeftLeg();
            //detachRightLeg();
        }

        

    }


    void checkMovement(){

        if (Input.GetKeyDown("a")){ 

            transformVect.x = -1;
        }
        if (Input.GetKeyDown("d")){

            transformVect.x = 1;
        }
        if (Input.GetKeyUp("a") && transformVect.x != 1|| Input.GetKeyUp("d") && transformVect.x != -1) {

            transformVect.x = 0;
        }


        if (Input.GetKeyDown("w")){

            transformVect.y = 3;
        }
        if (Input.GetKeyDown("s")){

            transformVect.y = -1;
        }
        if (Input.GetKeyUp("w") || Input.GetKeyUp("s")){

            transformVect.y = 0;
        }

        if(!isGrounded() || transformVect.y == -1) {
            crouchLegs();
        }
        else {
            unCrouchLegs();
        }

        //reset jump
        if (!readyToJump && isGrounded() && !Input.GetKeyDown(KeyCode.Space)) {
            readyToJump = true;
        }

        //start jump when space is pressed
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded() && readyToJump) {
            readyToJump = false;
            //if one leg is missing, jump half as high
            //otherwise, jump normally
            if (leftLegAttached && !rightLegAttached || !leftLegAttached && rightLegAttached) {
                applyJump(jumpScale / 2);
            }
            else if (leftLegAttached && rightLegAttached) {
                applyJump(jumpScale);
            }
        }

        //shoot gun
        if (Input.GetMouseButtonDown(0)) {
            shootWeapon();
        }

        //raise player up if they are on the ground and not jumping
        //also disable gravity while doing this
        if (rightLegAttached && leftLegAttached && isGrounded()) {

            //find the current and target position, then lerp to the target
            float currentYPosition = transform.position.y;
            float targetYPosition = (lowerLeftLegTarget.transform.position.y + lowerRightLegTarget.transform.position.y) / 2;
            
            playerRB.gravityScale = 0;
            if(transformVect.y >= 0) {
                playerRB.AddForce(new Vector2(0, (targetYPosition - currentYPosition) * 6f));
            }
        }
        else {
            //turn gravity back on if not on the ground 
            playerRB.gravityScale = playerGravity;
        }
        

    }


    void movePlayer(){

        //slows down player movement if a leg is missing
        float legModifier = 1f;

        if (leftLegAttached && !rightLegAttached || !leftLegAttached && rightLegAttached || transformVect.y < 0) {
            legModifier = 0.5f;
        } else if(!leftLegAttached && !rightLegAttached){
            //no moving if no legs are attached
            return;
        }

        GetComponent<Rigidbody2D>().AddForce(transformVect * speed * Time.deltaTime * legModifier);
    }


    void applyJump(int jumpScale){
        if(playerRB.velocity.y < (Vector2.up * jumpScale).y) {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * (jumpScale - playerRB.velocity.y));
        }
    }

    public bool isGrounded() {

        RaycastHit2D leftCast = Physics2D.BoxCast(lowerLeftLegTarget.transform.position, new Vector3(0.5f, 0.5f, 0f), 0f, Vector2.down, boxCastOffset, platformLayer);
        RaycastHit2D rightCast = Physics2D.BoxCast(lowerRightLegTarget.transform.position, new Vector3(0.5f, 0.5f, 0f), 0f, Vector2.down, boxCastOffset, platformLayer);

        bool leftCollision = leftLegAttached && leftCast.collider != null;
        bool rightCollision = rightLegAttached && rightCast.collider != null;

        return (leftCollision || rightCollision);

    }

    public Vector3 getTransformVect() {
        return transformVect;
    }

    public bool LLegAttached() {
        return leftLegAttached;
    }

    public bool RLegAttached() { 
        return rightLegAttached;
    }

    public bool isCrouching() {
        return transformVect.y < 0;
    }

    public void detachLeftLeg() {
        leftLegAttached = false;
        //other leg needs to be rotated so that it is not moving in the ground
        upperRightLeg.transform.localScale = new Vector3(upperRightLeg.transform.localScale.x * -1, upperRightLeg.transform.localScale.y * -1, upperRightLeg.transform.localScale.z);

        upperLeftLeg.GetComponent<BoxCollider2D>().enabled = true;
        //lowerLeftLeg.GetComponent<BoxCollider2D>().enabled = true;
        //unity gets mad if i dont put these inside variables
        Rigidbody2D UL = upperLeftLeg.AddComponent<Rigidbody2D>() as Rigidbody2D;
        //Rigidbody2D LL = lowerLeftLeg.AddComponent<Rigidbody2D>() as Rigidbody2D;

        upperLeftLeg.transform.SetParent(parent.transform);
    }

    public void detachRightLeg() {
        rightLegAttached = false;
        //other leg needs to be rotated so that it is not moving in the ground
        upperLeftLeg.transform.localScale = new Vector3(upperLeftLeg.transform.localScale.x * -1, upperLeftLeg.transform.localScale.y * -1, upperLeftLeg.transform.localScale.z);

        upperRightLeg.GetComponent<BoxCollider2D>().enabled = true;
        //lowerRightLeg.GetComponent<BoxCollider2D>().enabled = true;
        //unity gets mad if i dont put these inside variables
        Rigidbody2D UR = upperRightLeg.AddComponent<Rigidbody2D>() as Rigidbody2D;
        //Rigidbody2D LR = lowerRightLeg.AddComponent<Rigidbody2D>() as Rigidbody2D;

        upperRightLeg.transform.SetParent(parent.transform);
    }


    public void crouchLegs() {
        upperLeftLeg.transform.localScale = new Vector3(Mathf.Abs(upperLeftLeg.transform.localScale.x) * -1, Mathf.Abs(upperLeftLeg.transform.localScale.y) * -1, upperLeftLeg.transform.localScale.z);
        upperRightLeg.transform.localScale = new Vector3(Mathf.Abs(upperRightLeg.transform.localScale.x) * -1, Mathf.Abs(upperRightLeg.transform.localScale.y) * -1, upperRightLeg.transform.localScale.z);
    }

    public void unCrouchLegs() {
        upperLeftLeg.transform.localScale = new Vector3(Mathf.Abs(upperLeftLeg.transform.localScale.x), Mathf.Abs(upperLeftLeg.transform.localScale.y), upperLeftLeg.transform.localScale.z);
        upperRightLeg.transform.localScale = new Vector3(Mathf.Abs(upperRightLeg.transform.localScale.x), Mathf.Abs(upperRightLeg.transform.localScale.y), upperRightLeg.transform.localScale.z);
    }

    void shootWeapon() {
        float angle = handObject.transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 forceDirection = new Vector3(500f * Mathf.Cos(angle), 500f * Mathf.Sin(angle), 0f);

        //inverse bullet direction when aiming left
        if(handObject.transform.eulerAngles.x != 0f) {
            forceDirection.x *= -1;
        }

        pistolObject.GetComponent<Animator>().Play("PistolShoot");
        casingSpawner.Emit(1);

        bulletClones.Add(Instantiate(bulletPrefab, handObject.transform.position, handObject.transform.rotation, parent.transform));

        bulletClones[bulletClones.Count - 1].GetComponent<Rigidbody2D>().AddForce(forceDirection * 1.5f);
    }


    public float damage(float x) {
        return healthBar.GetComponent<HealthBarController>().damage(x);
    }

    public float getHealth() {
        return healthBar.GetComponent<HealthBarController>().getHealth();
    }


    public float getMaxHealth() {
        return healthBar.GetComponent<HealthBarController>().getMaxHealth();
    }


    public void kill() {

    }

}
