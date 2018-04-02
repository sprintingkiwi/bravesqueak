using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusChance
{
    public Status status;

    [Range(1, 100)]
    public int chance;
}
