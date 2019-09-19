using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlerHUD : MonoBehaviour
{
    public Slider HP;
    public Slider SP;
    public Transform hudHook;
    public Battler owner;

	// Use this for initialization
	public void Setup ()
    {
        HP = transform.Find("HP").GetComponent<Slider>();
        SP = transform.Find("SP").GetComponent<Slider>();

        HP.maxValue = owner.maxHP.value;
    }

    // Update is called once per frame
    void Update ()
    {
        Refresh();

        if (hudHook != null)
            transform.position = Camera.main.WorldToScreenPoint(hudHook.position);
        else
            if (owner != null)
                Camera.main.WorldToScreenPoint(new Vector3(owner.transform.position.x, transform.position.y + owner.spr.bounds.size.y / 2, transform.position.z));
    }

    void Refresh()
    {
        HP.value = owner.hitPoints;
        SP.value = owner.skillPoints;
    }

    public IEnumerator Disappear(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
