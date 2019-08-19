using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    // Define a class for storing enemy prefab-place association
    [System.Serializable]
    public class Enemy : System.Object
    {
        public EnemyBattler recipe;
        public Transform place;
    }

    [System.Serializable]
    public class CameraAdjust : System.Object
    {
        public Vector3 delta;
        public float speed;
    }

    public GameObject battleback;
    public Transform partyPosAdjust;
    public CameraAdjust cameraAdjust;
    public AudioClip battleMusic;
    public Enemy[] enemies;
    public enum Type { Common, Miniboss, Boss };
    public Type type;
}
