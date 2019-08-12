using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCharacter : NPC
{
    [Header("Enemy Character")]
    public Encounter encounter;
    GameObject EncounterTrigger;

    public override void Start()
    {
        base.Start();

        // Instance encounter trigger
        EncounterTrigger = Instantiate(Resources.Load("TriggerSensor") as GameObject);
        EncounterTrigger.transform.parent = transform;
        EncounterTrigger.name = "Encounter Trigger";
        EncounterTrigger.transform.position = transform.position;
        EncounterTrigger.GetComponent<BoxCollider2D>().offset = gameObject.GetComponent<BoxCollider2D>().offset;
        EncounterTrigger.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<BoxCollider2D>().size.x + 0.5f, gameObject.GetComponent<BoxCollider2D>().size.y + 0.5f);

    }

    public override void TriggerSensor(string sensor, Collider2D other, string type)
    {
        base.TriggerSensor(sensor, other, type);

        if (type == "Enter")
            if (sensor == "Encounter Trigger")
                if (other.gameObject.name == "Player")
                    GameController.instance.StartCoroutine(GameController.instance.TriggerBattle(encounter, name));
    }

    void OnMouseDown()
    {
        GameController.instance.StartCoroutine(GameController.instance.TriggerBattle(encounter, name));
    }    
}
