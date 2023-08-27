using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile{

    public GameObject bulletPrefab;
    GameObject bullet;

    float damage;
    float speed;
    float velocity;
    Vector3 position;
    Vector3 rotation;

    
    BulletProjectile(GameObject bulletPrefab, Vector3 position, Vector3 rotation) {
        this.bulletPrefab = bulletPrefab;
        //this.damage = damage;
        //this.speed = speed;
        //this.velocity = velocity;
        this.position = position;
        this.rotation = rotation;
    }

    

    
}
