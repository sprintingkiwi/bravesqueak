using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlerHUD : MonoBehaviour
{
    public Slider HP;
    public Slider SP;

	// Use this for initialization
	public void Start ()
    {
        HP = transform.Find("HP").GetComponent<Slider>();
        SP = transform.Find("SP").GetComponent<Slider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
