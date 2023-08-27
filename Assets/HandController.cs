using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour{

    Vector2 mousePos;
    Vector2 playerCenter;
    GameObject playerObject;
    BoxCollider2D playerRenderer;
    ParticleSystem casingSpawner;

    ParticleSystem.VelocityOverLifetimeModule casingParticleVelocityModule;
    ParticleSystem.MinMaxCurve casingDirection;

    private float originalCasingVelocity;
    private float handDist = 0.9f;

    // Start is called before the first frame update
    void Start(){

        playerObject = transform.parent.gameObject;
        casingSpawner = transform.Find("casing spawner").gameObject.GetComponent<ParticleSystem>();

        playerRenderer = playerObject.GetComponent<BoxCollider2D>();

        casingParticleVelocityModule = casingSpawner.velocityOverLifetime;
        casingDirection = new ParticleSystem.MinMaxCurve();
        originalCasingVelocity = casingParticleVelocityModule.x.constantMax;

    }

    // Update is called once per frame
    void Update(){

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerCenter = playerRenderer.bounds.center + new Vector3(0f, playerRenderer.bounds.size.y/4);

        pointHand();

    }

    float getHandRotation() {
        float offset = 0;
        float x = playerCenter.x - mousePos.x;
        float y = playerCenter.y - mousePos.y;

        if(y < 0 && x >= 0 || y > 0 && x > 0) {
            offset = Mathf.PI;
        }

        return Mathf.Atan(y / x) + offset;
    }

    Vector2 getHandPosition(float rotation) {
        return new Vector2(handDist * Mathf.Cos(rotation), handDist * Mathf.Sin(rotation));
    }

    void pointHand() {
        float rotation = getHandRotation();
        float rotationDeg = getHandRotation() * Mathf.Rad2Deg;
        gameObject.transform.position = playerCenter + getHandPosition(rotation);

        if(rotationDeg > 90 && rotationDeg <= 270) {
            gameObject.transform.eulerAngles = new Vector3(180f, 0f, -rotationDeg);
            casingDirection.constantMax = originalCasingVelocity;
        }
        else {
            gameObject.transform.eulerAngles = new Vector3(0f, 0f, rotationDeg);
            casingDirection.constantMax = -originalCasingVelocity;
        }

        casingParticleVelocityModule.x = casingDirection;

    }
}
