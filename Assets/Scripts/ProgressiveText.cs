using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressiveText : MonoBehaviour
{
    Text text;
    public ProgressiveMessage[] messages;
    float startTime;
    float t;
    public GameObject afterObject;
    public bool persistent;
    public bool loop;
    public float loopPause;

    void Start()
    {
        // Clear the GUI text
        text = gameObject.GetComponent<Text>();
        text.text = "";

        //fade = Instantiate(Resources.Load("Fade") as GameObject, GameObject.Find("Canvas").transform).GetComponent<Image>();
        //fade.color = new Color(0f, 0f, 0f, 0f);
        startTime = Time.time;
        t = 0;

        StartCoroutine(TypeLetters());
    }

    IEnumerator TypeLetters()
    {
        Debug.Log("Starting typing coroutine");
        for (int i = 0; i < messages.Length; i++)
        {
            // Fade in
            if (i != 0 && messages[i - 1].fadeAfter)
            {
                
            }
            Debug.Log("Fade in");
            startTime = Time.time;
            t = 0;
            while (t < 1)
            {
                t = (Time.time - startTime) / 0.5f;
                text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }

            // Iterate over each letter
            foreach (char letter in messages[i].message.ToCharArray())
            {
                GetComponent<Text>().text += letter; // Add a single character to the GUI text
                yield return new WaitForSeconds(messages[i].charPause);
            }
            // After pause
            yield return new WaitForSeconds(messages[i].afterPause);

            // Fade out
            if (messages[i].fadeAfter)
            {
                Debug.Log("Fade out");
                startTime = Time.time;
                t = 0;
                while (t < 1)
                {
                    t = (Time.time - startTime) / 0.5f;
                    text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.SmoothStep(1, 0, t));
                    yield return null;
                }
            }            

            // Reset
            if (!persistent)
                GetComponent<Text>().text = ""; 
            else if (loop)
            {
                GetComponent<Text>().text = "";
                yield return new WaitForSeconds(loopPause);
                StartCoroutine(TypeLetters());
            }
        }     
        
        // Trigger after object
        if (afterObject != null)
        {
            afterObject.SetActive(true);

            Debug.Log("Fade in");
            startTime = Time.time;
            t = 0;
            while (t < 1)
            {
                t = (Time.time - startTime) / 0.5f;
                text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
            //Destroy(gameObject);            
        }
    }    
}
