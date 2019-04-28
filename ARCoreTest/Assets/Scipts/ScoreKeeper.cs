using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int strokes = 0;
    void Awake()
    {
        //Make it so that this script is available in every scene by not destroying the gameobject when a new scene is loaded
        DontDestroyOnLoad(transform.gameObject);
    }

}
