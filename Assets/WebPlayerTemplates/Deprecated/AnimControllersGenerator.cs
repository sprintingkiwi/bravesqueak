using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.IO;

public class AnimControllersGenerator : MonoBehaviour
{
    public Dictionary<string, string> controllerPaths = new Dictionary<string, string>();
    //public string testPathBattler;
    //public string testPathController;

	// Use this for initialization
	void Awake ()
    {
        CreateAnimControllers();
    }

    void Start()
    {
        //CreateAnimControllers();

        //Debug.Log("NOW LOADING CONTROLLERS");
        //foreach (KeyValuePair<string, string> p in controllerPaths)
        //{
        //    Debug.Log("loading controller at path: " + p.Value);
        //    testPathBattler = p.Key;
        //    testPathController = p.Value;
        //    GameObject b = Resources.Load(p.Key) as GameObject;
        //    RuntimeAnimatorController r = Resources.Load(p.Value) as RuntimeAnimatorController;
        //    Debug.Log(b.name);
        //    b.GetComponent<Animator>().runtimeAnimatorController = r;
        //}
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public virtual void CreateAnimControllers()
    {
        string[] categoryFolder = { "Heroes/", "Enemies/" };
        for (int i = 0; i < 2; i++)
        {
            DirectoryInfo battlersDir = new DirectoryInfo("Assets/Resources/Battlers/" + categoryFolder[i]);
            foreach (DirectoryInfo battlerFolder in battlersDir.GetDirectories())
            {
                string battlerPath = "Assets/Resources/Battlers/" + categoryFolder[i] + battlerFolder.Name + "/";
                //GameObject battlerPrefab = AssetDatabase.LoadAssetAtPath(battlerPath + battlerFolder.Name + ".prefab", typeof(GameObject)) as GameObject;
                Debug.Log("Creating State Machines for " + battlerPath);
                GameObject battler = Resources.Load("Battlers/" + categoryFolder[i] + battlerFolder.Name + "/" + battlerFolder.Name) as GameObject;
                DirectoryInfo animsDir = new DirectoryInfo(battlerPath + "Animations/");
                string controllerPath = battlerPath + battlerFolder.Name + "Controller.controller";

                //AssetDatabase.DeleteAsset(controllerPath);
                var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                //RuntimeAnimatorController controller = battler.GetComponent<Animator>().runtimeAnimatorController;

                //foreach (DirectoryInfo animFolder in animsDir.GetDirectories())
                DirectoryInfo[] animsFolders = animsDir.GetDirectories();
                //var rootStateMachine = controller.layers[0].stateMachine;
                for (int a = 0; a < animsFolders.Length; a++)
                {
                    DirectoryInfo animFolder = animsFolders[a];

                    // Creates the controller
                    Debug.Log("Getting controller for " + animFolder.FullName);
                    string animPath = battlerPath + "Animations/" + animFolder.Name + "/";

                    // Add StateMachines
                    if (a > 0)
                        controller.AddLayer((animFolder.Name).ToString());
                    //else
                        //controller.layers[0].name = (animFolder.Name).ToString();
                    var rootStateMachine = controller.layers[a].stateMachine;

                    // Find animation files
                    FileInfo[] anims = new DirectoryInfo(animPath).GetFiles();

                    // Idle state
                    UnityEditor.Animations.AnimatorState idleState = rootStateMachine.AddState("Idle");
                    // Entry
                    rootStateMachine.defaultState = idleState;
                    // Any states
                    var resetTransition = rootStateMachine.AddAnyStateTransition(idleState);
                    resetTransition.hasExitTime = true;
                    resetTransition.exitTime = 1.0f;
                    resetTransition.duration = 0;
                    Debug.Log("Adding motion: Idle");
                    idleState.motion = Resources.Load("Battlers/" + categoryFolder[i] + battlerFolder.Name + "/" + "Animations/" + animFolder.Name + "/" + "idle") as Motion;
                    idleState.speed = 0.3f;

                    List<UnityEditor.Animations.AnimatorState> animStates = new List<UnityEditor.Animations.AnimatorState>();
                    foreach (FileInfo anim in anims)
                    {
                        string fileName = anim.Name.Split('.')[0];
                        string fileExt = anim.Name.Split('.')[1];
                        if (fileExt == "anim" && anim.Name.Split('.').Length == 2)
                        {
                            if (fileName != "Idle")
                            {
                                Debug.Log("Animation " + fileName + " of " + battlerFolder.Name);
                                // Add trigger parameters
                                string triggerName = fileName.ToLower();
                                controller.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);

                                // Add States
                                UnityEditor.Animations.AnimatorState state = rootStateMachine.AddState(fileName);
                                animStates.Add(state);

                                // All animations
                                string motionPath = "Battlers/" + categoryFolder[i] + battlerFolder.Name + "/" + "Animations/" + animFolder.Name + "/" + fileName;
                                Debug.Log("Adding motion: " + motionPath);
                                Motion actualMotion = Resources.Load(motionPath) as Motion;
                                state.motion = actualMotion;
                                state.speed = 0.5f;

                                // Add Transitions
                                var t = idleState.AddTransition(state);
                                t.duration = 0;
                                t.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, triggerName);
                            }
                        }
                    }                    
                }

                // Attach the controller? 
                Debug.Log("loading controller for " + battler.name);
                battler.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Battlers/" + categoryFolder[i] + battlerFolder.Name + "/" + battlerFolder.Name + "Controller") as RuntimeAnimatorController;

                //string resourceBattlerPath = "Battlers/" + categoryFolder[i] + battlerFolder.Name + "/" + battlerFolder.Name;
                //string resourceControllerPath = "Battlers/" + categoryFolder[i] + battlerFolder.Name + "/" + battlerFolder.Name + "Controller";
                //controllerPaths.Add(resourceBattlerPath, resourceControllerPath);

            }
        }
    }
}
