using UnityEngine;

public class LivingObject : MonoBehaviour
{
    float lookDistance = 5f;

    public float GetLookDistance()
    {
        return lookDistance*10;
    }
}
