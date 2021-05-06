using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMoveController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.6f;
    void Update()
    {
        //TODO: clamp
        float xAxisValue = Input.GetAxis("Horizontal");
        float yAxisValue = Input.GetAxis("Vertical");
        Camera.main.transform.Translate(new Vector3(xAxisValue * moveSpeed, yAxisValue * moveSpeed, 0.0f));
    }
}
