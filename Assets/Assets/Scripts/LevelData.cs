using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] public int maxGuests;
    [SerializeField] public int minTime;
    [SerializeField] public int maxTime;
    [SerializeField] public int difficultyLevel;
    [SerializeField] public int maxMadGuests;

    public void Start(){
        StartLevel();
    }
    public void StartLevel(){
        LevelManager.Instance.StartLevel(this);
    }
}
