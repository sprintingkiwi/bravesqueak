using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlerHUD : MonoBehaviour
{
    public Slider HP;
    public Slider SP;
    public float oldHP;
    public float oldSP;
    public Transform hudHook;
    public Battler owner;
    public float waitChanging;

    // Use this for initialization
    public void Setup()
    {
        HP = transform.Find("HP").GetComponent<Slider>();
        SP = transform.Find("SP").GetComponent<Slider>();

        HP.maxValue = owner.maxHP.value;

        HP.value = owner.hitPoints;
        SP.value = owner.skillPoints;
        oldHP = HP.value;
        oldSP = SP.value;

        HP.gameObject.SetActive(false);
        SP.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
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
        // HP
        if (oldHP != owner.hitPoints)
        {
            StartCoroutine(ChangeValue(HP, owner.hitPoints, waitChanging));
            oldHP = owner.hitPoints;
        }
        // SP
        if (oldSP != owner.skillPoints)
        {

            StartCoroutine(ChangeValue(SP, owner.skillPoints, waitChanging));
            oldSP = owner.skillPoints;
        }
    }

    public IEnumerator ChangeValue(Slider slider, float newValue, float wait)
    {
        slider.gameObject.SetActive(true);

        float delta = Mathf.Abs(slider.value - newValue);
        if (slider.value < newValue)
        {
            for (int i = 0; i < delta; i++)
            {
                slider.value += 1;
                yield return new WaitForSeconds(wait);
            }
        }
        else
        {
            for (int i = 0; i < delta; i++)
            {
                slider.value -= 1;
                yield return new WaitForSeconds(wait);
            }
        }

        slider.gameObject.SetActive(false);
    }

    public IEnumerator Appear(float time = 0, bool hp = true, bool sp = true)
    {
        yield return new WaitForSeconds(time);

        if (hp)
            HP.gameObject.SetActive(true);
        if (sp)
            SP.gameObject.SetActive(true);
        yield return null;
    }

    public IEnumerator Disappear(float time = 0, bool hp = true, bool sp = true)
    {
        if (hp)
            HP.gameObject.SetActive(false);
        if (sp)
            SP.gameObject.SetActive(false);

        yield return new WaitForSeconds(time);
    }
}
