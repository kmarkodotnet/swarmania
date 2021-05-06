using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;

    // Update is called once per frame
    void Update()
    {
        Instantiate(bullet, transform);
    }
}
