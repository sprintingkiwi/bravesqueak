using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSensorTransmitter : MonoBehaviour
{
    public JrpgBehaviour receiver;

    private void Start()
    {
        if (receiver == null)
            receiver = transform.parent.gameObject.GetComponent<JrpgBehaviour>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        receiver.TriggerSensor(name, other, "Enter");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        receiver.TriggerSensor(name, other, "Exit");
    }
}
