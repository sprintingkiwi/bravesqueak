using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTick : MonoBehaviour
{
    public GameObject tick;

    [Header("System")]
    public PartyMenu partyMenu;
    public PartyHero partyHero;

    GameController gc;

    public void Setup()
    {
        partyMenu = transform.parent.parent.parent.GetComponent<PartyMenu>();
        partyHero = transform.parent.GetComponent<PartyHero>();

        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void OnMouseDown()
    {
        Select();
    }

    public void Select()
    {       

        if (tick == null)
        {
            if (partyMenu.ticks > 2)
                return;

            tick = Instantiate(Resources.Load("Menu/Tick") as GameObject, transform);
            partyMenu.ticks += 1;

            //// Add Hero to Party
            //partyMenu.gc.partyPrefabs[partyMenu.ticks - 1] = partyMenu.gc.unlockedHeroes[transform.parent.GetSiblingIndex()];

            // Add hero to cache list of selected heroes
            foreach (Battler h in gc.heroes)
            {
                if (h.name == partyMenu.availables[partyHero.heroIndex].name)
                    gc.selectionCache.Add(h);
            }

        }
        else
        {
            Destroy(tick);
            tick = null;
            partyMenu.ticks -= 1;

            // Remove hero from cache list of selected heroes
            foreach (Battler h in gc.heroes)
            {
                if (h.name == partyMenu.availables[partyHero.heroIndex].name)
                    gc.selectionCache.Remove(h);
            }
        }
    }

}
