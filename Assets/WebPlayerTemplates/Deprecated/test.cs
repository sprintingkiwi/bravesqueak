using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [System.Serializable]
    public class Stat : System.Object
    {
        public string name;
        public int value;
    }
    public Stat[] stats;
    public Stat str;
    public Dictionary<string, Stat> testStats = new Dictionary<string, Stat>();

    // Use this for initialization
    void Start ()
    {
        testStats.Add("Strenght", str);

        str.value = 8;
        Debug.Log(testStats["Strenght"].value);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
