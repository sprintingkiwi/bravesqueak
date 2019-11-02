using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlerHUD : MonoBehaviour
{
    public Slider HP;
    public Slider SP;
    public Slider SPTotal;
    public Slider SPPlus;
    public float oldHP;
    public float oldSP;
    public Transform hudHook;
    public Battler owner;

    // Use this for initialization
    public void Setup()
    {
        HP = transform.Find("HP").GetComponent<Slider>();
        SP = transform.Find("SP").GetComponent<Slider>();
        SPTotal = transform.Find("SP_Total").GetComponent<Slider>();
        SPPlus = transform.Find("SP_Plus").GetComponent<Slider>();

        HP.maxValue = owner.maxHP.value;

        HP.value = owner.hitPoints;
        SP.value = owner.skillPoints;
        SPTotal.value = owner.skillPoints;
        SPPlus.value = owner.skillPoints;

        oldHP = HP.value;
        //oldSP = SP.value;

        HP.gameObject.SetActive(false);
        SP.gameObject.SetActive(false);
        SPTotal.gameObject.SetActive(false);
        SPPlus.gameObject.SetActive(false);
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
            StartCoroutine(ChangeValue(HP, owner.hitPoints, 0.025f, 0.5f));
            oldHP = owner.hitPoints; //update old
        }

        // SP
        //if (oldSP != owner.skillPoints)
        //{

        //    //StartCoroutine(ChangeValue(SP, owner.skillPoints, 0.4f, 1f)); //Doing this in battle menu
        //    oldSP = owner.skillPoints; //update old
        //}
    }

    public IEnumerator ChangeValue(Slider slider, float newValue, float waitChanging, float waitAfter)
    {
        slider.gameObject.SetActive(true);

        float delta = Mathf.Abs(slider.value - newValue);
        if (slider.value < newValue)
        {
            for (int i = 0; i < delta; i++)
            {
                slider.value += 1;
                yield return new WaitForSeconds(waitChanging);
            }
        }
        else
        {
            for (int i = 0; i < delta; i++)
            {
                slider.value -= 1;
                yield return new WaitForSeconds(waitChanging);
            }
        }

        yield return new WaitForSeconds(waitAfter);
        slider.gameObject.SetActive(false);
    }

    public IEnumerator Appear(float time = 0, bool hp = true, bool sp = true)
    {
        yield return new WaitForSeconds(time);

        if (hp)
            HP.gameObject.SetActive(true);
        if (sp)
        {
            SP.gameObject.SetActive(true);
            SPTotal.gameObject.SetActive(true);
            SPPlus.gameObject.SetActive(true);
        }
        yield return null;
    }

    public IEnumerator Disappear(float time = 0, bool hp = true, bool sp = true)
    {
        if (hp)
            HP.gameObject.SetActive(false);
        if (sp)
        {
            SP.gameObject.SetActive(false);
            SPTotal.gameObject.SetActive(false);
            SPPlus.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(time);
    }
}
