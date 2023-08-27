using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableAlive : MonoBehaviour, IDamageable{

    GameObject healthBar;
    HealthBarController healthBarController;

    // Start is called before the first frame update
    void Start(){

        healthBar = transform.Find("Health Bar").gameObject;
        healthBarController = healthBar.GetComponent<HealthBarController>();
        
    }

    // Update is called once per frame
    void Update(){
        
    }


    public float damage(float damage) { 
        return healthBarController.damage(damage);
    }

    public float getHealth() {
        return healthBarController.getHealth();
    }


    public float getMaxHealth() {
        return healthBarController.getMaxHealth();
    }

    public void kill() {

    }
}
