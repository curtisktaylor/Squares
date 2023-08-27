using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{

    float maxHealth = 100f;
    float health;
    float prevHealth;
    float lerp = 0;

    GameObject healthBar;
    GameObject damageBar;

    Vector3 damageBarStartScale;
    Vector3 damageBarStartPosition;

    // Start is called before the first frame update
    void Start(){

        healthBar = transform.Find("health").gameObject;
        damageBar = transform.Find("damage").gameObject;

        health = maxHealth;
        prevHealth = maxHealth;

    }

    // Update is called once per frame
    void Update(){

        if(prevHealth != health) {

            float size = (health / maxHealth);

            if (lerp < 1) {
                damageBar.transform.localScale = Vector3.Lerp(damageBarStartScale, new Vector3(size, 0.1f, 1f), lerp);
                damageBar.transform.localPosition = Vector3.Lerp(damageBarStartPosition, new Vector3((size - 1) / 2, 0, 0), lerp);
                lerp += 0.01f;
            }
            else {
                prevHealth = health;
            }
        }

    }

    public float damage(float x) {

        health -= x;

        if (health < 0) {
            health = 0;
        }

        float hSize = (health / maxHealth);
        float dSize = (prevHealth / maxHealth);

        healthBar.transform.localScale = new Vector3(hSize, 0.1f, 1f);
        healthBar.transform.localPosition = new Vector3((hSize - 1) / 2, 0, 0);

        //if the damage bar isn't going down or is already below the current health, restart position, size, and lerp

        if(lerp > 1 || ((dSize - 1) / 2) <= ((hSize - 1) / 2) || prevHealth == maxHealth) {
            damageBarStartScale = new Vector3(dSize, 0.1f, 1f);
            damageBarStartPosition = new Vector3((dSize - 1) / 2, 0, 0);

            damageBar.transform.localScale = damageBarStartScale;
            damageBar.transform.localPosition = damageBarStartPosition;
            lerp = 0;
        }
        

        return health;
    }


    public float getHealth() {
        return health;
    }


    public float getMaxHealth() {
        return maxHealth;
    }
}
