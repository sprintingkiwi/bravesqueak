using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class EnemyBattler : AIBattler
{  
    // Use this for initialization
    public override void Setup ()
    {
        base.Setup();

        faction = Faction.Enemies;

        SetupPerks();

        // Attach anim controller
        //Debug.Log("Attaching animator controller to " + name);
        //animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath("Assets/Resources/Battlers/Enemies/" + prefabName.ToString() + "/Controllers/Default.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();


    }    
}
