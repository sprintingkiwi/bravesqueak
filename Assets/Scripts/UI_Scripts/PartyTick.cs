using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTick : MonoBehaviour
{
    public GameObject tick;

    [Header("System")]
    PartyMenu partyMenu;

    void Start()
    {
        partyMenu = transform.parent.parent.parent.GetComponent<PartyMenu>();
    }

    void OnMouseDown()
    {
        if (tick == null)
        {
            if (partyMenu.ticks < 3)
            {
                tick = Instantiate(Resources.Load("Menu/Tick") as GameObject, transform);
                partyMenu.ticks += 1;

                // Add Hero to Party
                partyMenu.gc.partyPrefabs[partyMenu.ticks - 1] = partyMenu.gc.heroes[transform.parent.GetSiblingIndex()];
            }
        }
        else
        {
            Destroy(tick);
            tick = null;
            partyMenu.ticks -= 1;
        }
    }

}
