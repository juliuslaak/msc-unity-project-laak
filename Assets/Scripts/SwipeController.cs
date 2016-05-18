using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using System;
using System.Linq;
    
public class SwipeController : MonoBehaviour {
    
    [SerializeField] MainController mainController;
    [SerializeField] WarningController m_WarningController;
    [SerializeField] HandController m_HandController;
    [SerializeField] float minDeltaPosition; // NB! MUST BE THE SAME AS THE TIME IT TAKES VERTICAL MOTION TO RUN A CYCLE
    [SerializeField] float minLength;
    [SerializeField] float minVelocity;
    [SerializeField] bool debugVelocity;

    [HideInInspector] public float movementBeginTime;
    [HideInInspector] public float movementTopTurnTime;
    [HideInInspector] public float movementEndTime;
    [HideInInspector] public bool running;
    
    private List<float> positions = new List<float>();
    private List<float> times = new List<float>();
    private bool minLenghtExceeded;
    private bool minVelocityExceeded;
    private bool testMode;
    private bool debug;

    void Start ()
    {
        testMode = mainController.test;
        debug = mainController.debug;
    }

    void Update()
    {
        if (debugVelocity)
        {
            Vector3 p = m_HandController.getPalmVelocity();

            if (p != default(Vector3))
            {
                Debug.Log(p);
            }
        }
        if (testMode && !running)
        {
            Debug.Log("Starting Motion Begin");
            StartUpHandMotionCycle();
        }
    }

    public void StartUpHandMotionCycle()
    {
        running = true;
        StartCoroutine(DetectVerticalMotion());
    }

    public void StartLeftRightHandMotionCycle()
    {
        running = true;
        StartCoroutine(DetectHorizontalMotion());
    }

    private IEnumerator DetectVerticalMotion()
    {
        while (m_HandController.getPalmVelocity().y < 0.1)
        {
            yield return null;
        }

        if (debug)
            Debug.Log("GOING UP");

        EventManager.TriggerEvent("GoingUp");

        movementBeginTime = Time.time;

        yield return StartCoroutine(MinLengthSpeed("up"));
        yield return StartCoroutine(TopTurn());
    }
    private IEnumerator TopTurn()
    {
        while (m_HandController.getPalmVelocity().y > 0)
        {
            yield return null;
        }

        if (testMode)
        {
            StartCoroutine("measureDeltaPosition");
        }

        if (debug)
            Debug.Log("TOP TURN");

        if (m_HandController.getPalmHitPoint().y < 0.2)
        {
            EventManager.TriggerEvent("TooLow");
            StopAllCoroutines();
        }
        //else
        //{
        //    EventManager.TriggerEvent("TopTurn");

        //    movementTopTurnTime = Time.time;

        //    yield return StartCoroutine(MinLengthSpeedTimed("down"));
        //    yield return StartCoroutine(MotionEnd());
        //}

        EventManager.TriggerEvent("TopTurn");

        movementTopTurnTime = Time.time;

        yield return StartCoroutine(MinLengthSpeedTimed("down"));
        yield return StartCoroutine(VerticalMotionEnd());
    }
    private IEnumerator VerticalMotionEnd()
    {
        while (m_HandController.getPalmVelocity().y < -0.2)
        {
            yield return null;
        }

        //if (mainController.handTriggerDelay)
        //{
        //    StopAllCoroutines();
        //    yield return StartCoroutine(m_WarningController.ShowSlowerNoticeUI());
        //}
        if (debug)
            Debug.Log("STOPPED");

        EventManager.TriggerEvent("Stopped");

        movementEndTime = Time.time;
        running = false;

        if (testMode)
        {
            StopCoroutine("measureDeltaPosition");
            Debug.Log("HAND mean pos: " + positions.Average().ToString("F4") + " time: " + times.Average().ToString("F4"));

            if (BallStatisticsController.control.times.Count() != 0)
            {
                EventManager.TriggerEvent("showTimesPositions");
            }
            else
            {
                Debug.LogError("You moved hand too fast: Ball times/positions null");
            }

            Debug.Log("Turn to Stop time: " + (Time.time - movementTopTurnTime));

            StartUpHandMotionCycle();
        }
    }

    
    private IEnumerator DetectHorizontalMotion()
    {
        while (m_HandController.getPalmVelocity().x < 0.1)
        {
            yield return null;
        }

        if (debug)
            Debug.Log("GOING RIGHT");

        EventManager.TriggerEvent("GoingRight");

        movementBeginTime = Time.time;

        yield return StartCoroutine(MinLengthSpeed("right"));
        yield return StartCoroutine(RightTurn());
    }
    private IEnumerator RightTurn()
    {
        while (m_HandController.getPalmVelocity().x > 0)
        {
            yield return null;
        }

        if (debug)
            Debug.Log("RIGHT TURN");

        EventManager.TriggerEvent("RightTurn");

        movementTopTurnTime = Time.time;

        yield return StartCoroutine(MinLengthSpeedTimed("left"));
        yield return StartCoroutine(HorizontalMotionEnd());
    }
    private IEnumerator HorizontalMotionEnd()
    {
        while (m_HandController.getPalmVelocity().x < -0.2)
        {
            yield return null;
        }
        
        if (debug)
            Debug.Log("STOPPED");

        EventManager.TriggerEvent("Stopped");

        movementEndTime = Time.time;
        running = false;

        if (testMode)
        {
            StartLeftRightHandMotionCycle();
        }
    }



    private IEnumerator measureDeltaPosition()
    {
        Vector3 startPosition = m_HandController.getPalmPosition();
        Vector3 curPos;
        float deltaPosition;

        float a = Time.time;

        while (true)
        {
            curPos = m_HandController.getPalmPosition();
            deltaPosition = Math.Abs(curPos.y - startPosition.y);

            if (deltaPosition >= minDeltaPosition)
            {
                positions.Add(deltaPosition);
                times.Add(Time.time - a);

                a = Time.time;
                startPosition = m_HandController.getPalmPosition();
            }

            yield return null;
        }

    }

    private IEnumerator MinLengthSpeedTimed(string direction)
    {
        StartCoroutine(SwipeMinLength());
        StartCoroutine(SwipeMinVelocity(direction));

        float start = Time.time;
        float t = 0.8F;

        while (!(minLenghtExceeded && minVelocityExceeded) && (t > Time.time - start))
        {
            yield return null;
        }

        if (t < Time.time - start)
        {
            if (debug)
            {
                if (!minVelocityExceeded)
                    Debug.Log("Min velocity length not exceeded!");
                else
                    Debug.Log("Min swipe length not exceeded!");
            }

            EventManager.TriggerEvent("TooSlow");
        }
    }

    private IEnumerator MinLengthSpeed(string direction)
    {
        StartCoroutine(SwipeMinLength());
        StartCoroutine(SwipeMinVelocity(direction));
        
        while (!(minLenghtExceeded && minVelocityExceeded))
        {
            yield return null;
        }
    }

    private IEnumerator SwipeMinLength()
    {
        if (mainController.task != 2)
        {
            float startPosition = m_HandController.getPalmPosition().y;

            minLenghtExceeded = false;
            while (Math.Abs(m_HandController.getPalmPosition().y - startPosition) * 1000 < minLength)
            {
                yield return null;
            }
            minLenghtExceeded = true;
        }
        else
        {
            float startPosition = m_HandController.getPalmPosition().x;

            minLenghtExceeded = false;
            while (Math.Abs(m_HandController.getPalmPosition().x - startPosition) * 1000 < minLength)
            {
                yield return null;
            }
            minLenghtExceeded = true;
        }

        //Debug.Log("Length exceeded!");

    }

    private IEnumerator SwipeMinVelocity(string direction)
    {
        minVelocityExceeded = false;

        if (direction == "up")
        {
            while (m_HandController.getPalmVelocity().y > 0 && Math.Abs(m_HandController.getPalmVelocity().y) * 1000 < minVelocity)
            {
                yield return null;
            }
        }
        else if (direction == "down")
        {
            while (m_HandController.getPalmVelocity().y <= 0 && Math.Abs(m_HandController.getPalmVelocity().y) * 1000 < minVelocity)
            {
                yield return null;
            }
        }
        else if (direction == "right")
        {
            while (m_HandController.getPalmVelocity().x > 0 && Math.Abs(m_HandController.getPalmVelocity().x) * 1000 < minVelocity / 2)
            {
                yield return null;
            }
        }
        else if (direction == "left")
        {
            while (m_HandController.getPalmVelocity().x <= 0 && Math.Abs(m_HandController.getPalmVelocity().x) * 1000 < minVelocity / 2)
            {
                yield return null;
            }
        }
        else
        {
            while (Math.Abs(m_HandController.getPalmVelocity().y) * 1000 < minVelocity)
            {
                yield return null;
            }
        }

        minVelocityExceeded = true;

        //Debug.Log("Velocity exceeded!");

    }
}
