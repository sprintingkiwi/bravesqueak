using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScrollController : MonoBehaviour
{
    [Header("Options")]
    public float contentEndY;
    public float maxContentY;
    public UnityEvent[] contentReadEvents;

    [Header("Drag and Drop")]
    public RectTransform content;

    bool readEventTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        content.anchoredPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CheckContentRead();
        RestrainContentPosition();
    }

    void CheckContentRead()
    {
        if (content.anchoredPosition.y > contentEndY && !readEventTriggered)
        {
            foreach (UnityEvent readEvent in contentReadEvents) readEvent.Invoke();
            readEventTriggered = true;
        }
    }

    void RestrainContentPosition()
    {
        if (content.anchoredPosition.y < 0)
            content.anchoredPosition = Vector2.zero;
        else if (content.anchoredPosition.y > maxContentY)
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, maxContentY);
    }
}
