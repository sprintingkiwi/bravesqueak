using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class SkillTreeIcon : MonoBehaviour
{
    [TextArea(5, 10)]
    public string description;
    public Skill skill;
    public SkillTreeIcon[] dependencies;
    GameObject tracker;

	// Use this for initialization
	void Start ()
    {
        // Sync skill description
        if (skill != null)
            skill.description = description;

        // Display skill icon image if null
        if (gameObject.GetComponent<SpriteRenderer>().sprite == null)
            if (skill != null)
                gameObject.GetComponent<SpriteRenderer>().sprite = skill.GetComponent<SpriteRenderer>().sprite;
        gameObject.AddComponent<BoxCollider2D>();

        // Draw dependencies connections
        tracker = Resources.Load("Tracker") as GameObject;
        foreach (SkillTreeIcon d in dependencies)
        {
            Debug.Log("Tracking dependence " + d.name + " of " + name);

            GameObject t = Instantiate(tracker, d.transform.position, Quaternion.identity, transform);
            t.tag = "Tracker";
            LineRenderer l = t.GetComponent<LineRenderer>();
            Vector3[] twoPos = {d.transform.position, transform.position};
            l.SetPositions(twoPos);
            //StartCoroutine(TrackDependence(t, d, this));
        }
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    void OnMouseDown()
    {
        Jrpg.Log(name);
    }

    //IEnumerator TrackDependence(GameObject t, SkillTreeIcon start, SkillTreeIcon end, float speed=10)
    //{
    //    Debug.Log("Start tracking dependence " + start.name + " of " + end.name);
    //    while(Vector3.Distance(start.transform.position, end.transform.position) > 0.1f)
    //    {
    //        t.transform.position = Vector3.MoveTowards(t.transform.position, end.transform.position, speed * Time.deltaTime);
    //        yield return null;
    //    }
    //    Debug.Log("Finished tracking dependence " + start.name + " of " + end.name);
    //}
}
