using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : MonoBehaviour
{
    public string name;
    public int type; // -1 = portal; 0 = spikes; 1 = keyBlock; 2 = key; 3 = stroller 
    public bool isFatal;
    public Vector2 startingPosition;
    public int startingDirection; // 0 = right, 1 = left, 2 = up, 3 = down;
    public float timeBeforeTurn;
    public float timeOffset;
}
