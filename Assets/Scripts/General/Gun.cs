using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;

    // Update is called once per frame
    void Update()
    {
        Instantiate(bullet, transform);
    }
}
