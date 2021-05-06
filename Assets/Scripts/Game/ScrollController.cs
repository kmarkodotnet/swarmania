using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollController : MonoBehaviour
{
    public float zoomSpeed = 1;
    public float targetOrtho;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 100.0f;
    public bool scrolled = false;

    void Start()
    {
        targetOrtho = Mathf.Clamp(Camera.main.orthographicSize, minOrtho, maxOrtho);
    }

    // Update is called once per frame
    void Update()
    {
        MouseScrolling(); 
        KeyboardScrolling();
    }

    private void MouseScrolling()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
            scrolled = true;
        }
        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
    }
    private void KeyboardScrolling()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            var scroll = 0.1f; 
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
            scrolled = true;
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            var scroll = -0.1f;
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
            scrolled = true;
        }
        if(Input.GetKeyUp(KeyCode.KeypadPlus) || Input.GetKeyUp(KeyCode.KeypadMinus))
        {
            targetOrtho = Camera.main.orthographicSize;
        }
        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
    }
}
