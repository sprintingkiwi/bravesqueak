using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickUI : MonoBehaviour
{
    [Header("Drag and Drop")]
    public Camera mapCamera;

    [Header("System")]
    public bool dragging;
    public Vector2 direction;
    public float deadzone;
    public float sensibility;
    public float maxOffset;
    Vector3 touchPos;
    
    //public GameObject pointTest;

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (dragging)
        {
            Vector2 distance = transform.position - transform.parent.position;
            if (distance.magnitude > deadzone)
                direction = distance.normalized;
            else
                direction = Vector2.zero;

            //if (Jrpg.CheckPlatform() == "Mobile" && !Debug.isDebugBuild)
            //{
            //    Touch touch = Input.GetTouch(0);
            //    touchPos = mapCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 1f));
            //}
            //else
            //{
                Vector3 mousePos = Input.mousePosition;
                touchPos = new Vector3(mousePos.x, mousePos.y, 1f);
                //Instantiate(pointTest, touchPos, Quaternion.identity);
            //}
            Vector2 stickMovement = (touchPos - transform.parent.position) * sensibility;
            if (stickMovement.magnitude > maxOffset)
                stickMovement = stickMovement.normalized * maxOffset;
            transform.position = new Vector3(transform.parent.position.x + stickMovement.x, transform.parent.position.y + stickMovement.y, transform.position.z);
        }
    }

    public void PointerDown()
    {
        dragging = true;
        Debug.LogWarning("Dragging");
    }

    public void PointerUp()
    {
        dragging = false;
        transform.localPosition = Vector3.zero;
        direction = Vector2.zero;
    }
}
