using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : BattleCameraController
{
    public float rightBound;
    public float leftBound;
    public float topBound;
    public float bottomBound;
    public Vector3 pos;
    public Transform target;
    public SpriteRenderer spriteBounds;
    public bool followPlayer;

    Camera cam;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        cam = gameObject.GetComponent<Camera>();
    }

    public void Setup(WorldMap map)
    {
        float screenRatio = Screen.width / Screen.height;
        float desiredRatio = 16f / 9f;
        float height = Screen.height;
        float width = Screen.width;
        if (screenRatio > desiredRatio)
        {
            height = Screen.height;
            width = (height * 16f) / 9f;
        }
        else if (screenRatio < desiredRatio)
        {
            width = Screen.width;
            height = (width * 9f) / 16f;
        }



        spriteBounds = map.cameraBoundary;
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * width / height; //Screen.width / Screen.height;
        leftBound = spriteBounds.transform.position.x + (horzExtent - spriteBounds.sprite.bounds.size.x / 2.0f);
        rightBound = spriteBounds.transform.position.x + (spriteBounds.sprite.bounds.size.x / 2.0f - horzExtent);
        bottomBound = spriteBounds.transform.position.y + (vertExtent - spriteBounds.sprite.bounds.size.y / 2.0f);
        topBound = spriteBounds.transform.position.y + (spriteBounds.sprite.bounds.size.y / 2.0f - vertExtent);

        followPlayer = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (followPlayer)
            FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        transform.position = pos;
    }
}