using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public List<Menu> subMenus = new List<Menu>();
    public Menu father;

    // Use this for initialization
    public virtual void Setup ()
    {
        Jrpg.Log("Starting menu: " + name);
        GameController.Instance.player.joy.transform.parent.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	public virtual void Update ()
    {
        if (InputManager.Instance.ButtonBDown())
        {
            MenuDestruction();
        }
    }

    public virtual void MenuDestruction()
    {        
        // Delete menu-related stuff under canvas
        foreach (Transform t in GameObject.Find("Canvas").transform)
            if (t.name.Contains(name))
                Destroy(t.gameObject);

        GameController.Instance.currentMap.gameObject.SetActive(true);
        if (subMenus.Count == 0)
        {
            if (father != null)
            {
                father.subMenus.Remove(this);
                father.gameObject.SetActive(true);
            }            
            Destroy(this.gameObject);
        }
        //GameController.instance.player.canMove = true;
        GameController.Instance.player.joy.transform.parent.gameObject.SetActive(true);
    }
}
