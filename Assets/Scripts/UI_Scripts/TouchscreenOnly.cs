using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchscreenOnly : MonoBehaviour
{    
    void Start()
    {
        gameObject.SetActive(GameController.Instance.touchControls);
        if (gameObject.activeSelf) StartCoroutine(CheckMainMenu());
    }

    IEnumerator CheckMainMenu()
    {
        Vector3 orignalScale = transform.localScale;
        if (GameController.Instance.situation == "")
            transform.localScale = Vector3.zero;
        while (GameController.Instance.situation == "")
            yield return null;
        transform.localScale = orignalScale;
    }
}
