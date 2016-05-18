using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BallStatisticsController : MonoBehaviour {

    public static BallStatisticsController control;

    public List<float> times;
    public List<float> positions;

    void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        EventManager.StartListening("showTimesPositions", ShowTimesPositions);
    }

    void OnDisable()
    {
        EventManager.StopListening("showTimesPositions", ShowTimesPositions);
    }

    void Start()
    {
        times = new List<float>();
        positions = new List<float>();
    }
    

    public void AddToTimesPositions(float t, float p)
    {
        times.Add(t);
        positions.Add(p);
    }


    private void ShowTimesPositions()
    {
        Debug.Log("BALL mean pos: " + positions.Average().ToString("F4") + " time: " + times.Average().ToString("F4"));

        times = new List<float>();
        positions = new List<float>();
    }

}
