using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] float speed = 5;
    
    // Update is called once per frame
    void Update()
    {
        //var p = projectile.transform.position;
        //projectile.transform.position = new Vector3(p.x + speed * Time.deltaTime, p.y, p.z);
    }
}
