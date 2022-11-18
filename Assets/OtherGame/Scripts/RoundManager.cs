using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager _instance;
    public int roundCount = 0;
    public int LowestScoreRecord;
    public int thisHoldSum = 0;
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            LowestScoreRecord = 1000;
            _instance = this;
        }
        DontDestroyOnLoad(this);
    }
}
