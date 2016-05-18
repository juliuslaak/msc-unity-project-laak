using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class MainController : MonoBehaviour {

    private Controller controller;

    // Editor variables
    [SerializeField] private GameObject gridPlane;
    [SerializeField] private GameObject m_ExperimentPlane;
    public VRCameraFade VRCameraFade;
    public float spherecastRadius = 0.16F;
    public float forwardRaycastRadius = 1;
    public float bufferRadius = 0.16F;
    public int tutorialTrials = 70;
    public int experimentTrials = 150;
    public float ABSeparationX = 0.6F;

    public bool test = false;
    public bool testTargets;
    [Range(0.0F,0.5F)] public float delayStart;
    [Range(0.0F,0.5F)] public float delayStop;
    public bool debug;

    [HideInInspector] public bool handTriggerDelay;

    [SerializeField] private bool walkthrough;
    [SerializeField] private bool tutorial;
    public int task;  // Task 1: experiment 1, task 2: experiment 2, etc.

    [SerializeField] private UIController m_UIController;
    [SerializeField] private LeapHandVisibilityController m_LeapHandVisibilityontroller;
    [SerializeField] private WarningController m_WarningController;
    [SerializeField] private SubjectDataController m_SubjectDataController;
    [SerializeField] private FixationPointController m_FixationPointController;
    [SerializeField] private GridController m_GridController;
    [SerializeField] private SwipeController m_SwipeController;
    [SerializeField] private HandController m_HandController;

    private Camera cam;
    private Vector3 palmPosition;
    private Vector3 middleFingerTipPosition;
    private int targetStateHash;
    private bool waitingForTarget;
    private GameObject targetObject;
    private AnimatorStateInfo stateInfoPerpendicular;
    private bool timerFinished;
    private bool continueTutorial;
    private bool firstTrigger;
    private string target;
    private List<string> mixedTargets;
    private List<string> mainTutorialTargets;
    private List<string> reactionTutorialTargets;
    private List<string> experimentTargets;
    private List<VibrantBall> balls;
    private List<Vector3> previousHandHitPositions = new List<Vector3>();
    private float targetDistanceFromRaycastHit;
    private Vector3 hitOnPlanePosition;
    //private Vector3 reflectedHitPosition;
    //private bool noCommonHits;
    private KeyCode triggerKey = KeyCode.Mouse0;
    private KeyCode myKey = KeyCode.Space;
    private Vector3 topTurnPalmPosition;
    private Vector3 mockTargetPosition;
    private bool targetShown;
    private bool tooSlow;
    private bool tooLow;
    private bool waitForTopTurn;
    private bool topTurn;
    private bool stopped;
    private bool goingUp;
    private bool rightTurn;
    private bool leftTurn;
    private bool moving;
    private bool goingRight;
    private bool mainTutorialRunning;

    void OnEnable()
    {
        EventManager.StartListening("GoingUp", GoingUp);
        EventManager.StartListening("GoingRight", GoingRight);
        EventManager.StartListening("TopTurn", TopTurn);
        EventManager.StartListening("RightTurn", RightTurn);
        EventManager.StartListening("LeftTurn", LeftTurn);
        EventManager.StartListening("Stopped", Stopped);
        EventManager.StartListening("TooSlow", TooSlow);
        EventManager.StartListening("TooLow", TooLow);
    }
    
    void OnDisable()
    {
        EventManager.StopListening("GoingUp", GoingUp);
        EventManager.StopListening("GoingRight", GoingRight);
        EventManager.StopListening("TopTurn", TopTurn);
        EventManager.StopListening("RightTurn", RightTurn);
        EventManager.StopListening("LeftTurn", LeftTurn);
        EventManager.StopListening("Stopped", Stopped);
        EventManager.StopListening("TooSlow", TooSlow);
        EventManager.StopListening("TooLow", TooLow);
    }

    private IEnumerator Start() {

        // Set variables
        controller = new Controller();
        cam = Camera.main;
        
        if (task == 1)
        {
            if (experimentTrials % 10 != 0)
            {
                Debug.LogError("Trials number not correct!");
            }

            targetStateHash = Animator.StringToHash("VerticalTarget.PlayVerticalTarget");

            IEnumerable<string> noTargets = Enumerable.Repeat("noTarget", experimentTrials / 10);
            IEnumerable<string> randomTargets = Enumerable.Repeat("randomTarget", experimentTrials * 3 / 10);
            IEnumerable<string> handTargets = Enumerable.Repeat("handTarget", experimentTrials * 3 / 10);
            IEnumerable<string> reflectedTargets = Enumerable.Repeat("reflectedTarget", experimentTrials * 3 / 10);

            var concatenatedTargets =
                Enumerable.Concat(noTargets,
                Enumerable.Concat(randomTargets,
                Enumerable.Concat(handTargets, reflectedTargets)));

            if (concatenatedTargets.Count() != experimentTrials)
            {
                Debug.LogError("Target type distribution wrong!");
            }

            experimentTargets = concatenatedTargets.OrderBy(elem => Guid.NewGuid()).ToList();
            mainTutorialTargets = concatenatedTargets.OrderBy(elem => Guid.NewGuid()).ToList();
        }
        else if (task == 2)
        {
            // TASK NOT USED
            Debug.LogError("NO TASK 2 AVAILABLE!!!!!!!!!!!!!!!!");
            //targetStateHash = Animator.StringToHash("HorizontalTarget.PlayHorizontalTarget");

            //IEnumerable<string> noTargets = Enumerable.Repeat("noTarget", experimentTrials / 10);
            //IEnumerable<string> randomTargets = Enumerable.Repeat("randomTarget", experimentTrials * 3 / 10);
            //IEnumerable<string> handTargets = Enumerable.Repeat("handTarget", experimentTrials * 3 / 10);
            //IEnumerable<string> mockTargets = Enumerable.Repeat("mockTarget", experimentTrials * 3 / 10);

            //var concatenatedTargets =
            //    Enumerable.Concat(noTargets,
            //    Enumerable.Concat(randomTargets,
            //    Enumerable.Concat(handTargets, mockTargets)));

            //if (concatenatedTargets.Count() != experimentTrials)
            //{
            //    Debug.LogError("Target type distribution wrong!");
            //}

            //experimentTargets = concatenatedTargets.OrderBy(elem => Guid.NewGuid()).ToList();
            //mainTutorialTargets = concatenatedTargets.OrderBy(elem => Guid.NewGuid()).ToList();
        }
        else if (task == 3)
        {
            targetStateHash = Animator.StringToHash("VerticalTarget.PlayVerticalTarget");

            // FOR TASK 3, THE NR OF TRIALS IS HAND-SET
            IEnumerable<string> noTargets = Enumerable.Repeat("noTarget", 10);
            IEnumerable<string> handTargets = Enumerable.Repeat("handTarget", 70);
            IEnumerable<string> reflectedTargets = Enumerable.Repeat("reflectedTarget", 70);

            var concatenatedTargets =
                Enumerable.Concat(noTargets,
                Enumerable.Concat(handTargets, reflectedTargets));
            
            if (concatenatedTargets.Count() != experimentTrials)
            {
                Debug.LogError("Unmatching trial nr: " + concatenatedTargets.Count() + " vs " + experimentTrials);
            }

            // Add three conditions in the beginning of each experiment
            // A to cover traces, B to get two hand/ref target positions ahead
            List<string> startOfTargetList = new List<string>();
            startOfTargetList.Add("randomTarget"); // A
            startOfTargetList.Add("noTarget"); // B
            startOfTargetList.Add("handTarget"); // A
            startOfTargetList.Add("reflectedTarget"); // B

            IEnumerable<string> startOfTargetListEnum = startOfTargetList;
            
            experimentTargets = Enumerable.Concat(startOfTargetListEnum, concatenatedTargets.OrderBy(elem => Guid.NewGuid())).ToList();
            experimentTrials = experimentTrials + startOfTargetList.Count;
            
            mainTutorialTargets = concatenatedTargets.OrderBy(elem => Guid.NewGuid()).ToList();
            
            // TESTING
            //Debug.LogError("TEST SET");
            //experimentTargets = Enumerable.Repeat("handTarget", 72).ToList();
        }
        else
        {
            Debug.LogError("Only tasks 1-3 are allowed!");
        }

        // FOR TESTING DIFFERENT SETS OF TARGETS
        //mixedTargets = Enumerable.Repeat("mockTarget", experimentTrials).ToList();

        if (test)
        {
            m_LeapHandVisibilityontroller.ShowHands();
            yield return StartCoroutine(TestMode ());
        }
        else
        {
            if (!testTargets)
                m_LeapHandVisibilityontroller.HideHands();

            yield return StartCoroutine(CenterScreen ());
            
            yield return new WaitForSeconds(0.2F);

            if (walkthrough)
            {
                yield return StartCoroutine(ExperimentVocabulary());
            }
            if (tutorial)
            {
                yield return StartCoroutine(ReactionTutorial());
                yield return StartCoroutine(MainTutorialPhase());
            }

            float a = Time.time;
            if (task != 2)
            {
                yield return StartCoroutine(HandUpExperiment());
            }
            else
            {
                yield return StartCoroutine(HandLeftRightExperiment());
            }

            Debug.Log("Time it took to run the experiment: " + (Time.time - a).ToString("F4"));

            Application.Quit();
        }
    }


    void Update()
    {

        if (test)
        {
            waitingForTarget = true;
            handTriggerDelay = false;
            firstTrigger = false;
            target = "noTarget";
        }

        // TODO is this necessary here? : && controller.Frame().Hands.Count > 0
        if (waitingForTarget && controller.Frame().Hands.Count > 0)
        {
            // FIND HAND AND FINGER POSITION
            palmPosition = m_HandController.getPalmPosition();

            Vector3 centerEye = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye);

            // PALM HITS
            Vector3 palmDirection = palmPosition - centerEye;
            RaycastHit[] raycastHits = Physics.SphereCastAll(centerEye, spherecastRadius, palmDirection, 1000, 1 << 9); // Hits only balls


            if (firstTrigger && raycastHits.Count() != 0)
            {
                StartCoroutine(DelayHandTrigger(delayStart, delayStop));
                firstTrigger = false;
            }

            if (!handTriggerDelay)
            {
                // FORWARD HITS
                Vector3 camForwardDirection = cam.transform.TransformDirection(Vector3.forward);
                RaycastHit[] forwardRaycastHits = Physics.SphereCastAll(centerEye, forwardRaycastRadius, camForwardDirection, 1000, 1 << 9);

                // PALM RAYCAST
                RaycastHit hitOnPlane;
                Physics.Raycast(centerEye, palmDirection, out hitOnPlane, 1000, 1 << 8);
                hitOnPlanePosition = hitOnPlane.point;
                
                // REFLECTED HITS
                Vector3 reflectedHitPosition = new Vector3(-hitOnPlanePosition.x, hitOnPlanePosition.y, hitOnPlanePosition.z);
                Vector3 reflectedHitDirection = reflectedHitPosition - centerEye;
                RaycastHit[] reflectedRaycastHits = Physics.SphereCastAll(centerEye, spherecastRadius, reflectedHitDirection, 1000, 1 << 9); // Hits only balls

                // RANDOM AREA
                RaycastHit[] raycastFixationBoxHits = Physics.BoxCastAll(centerEye, new Vector3(Math.Abs(ABSeparationX), 0.25F), m_ExperimentPlane.transform.position + new Vector3(0, 0.25F,0), Quaternion.identity, 1000, 1 << 9);

                IEnumerable<GameObject> forwardRaycastHitObjects = forwardRaycastHits.Select(t => t.transform.gameObject);
                IEnumerable<GameObject> raycastFixationBoxHitObjects = raycastFixationBoxHits.Select(t => t.transform.gameObject);
                IEnumerable<GameObject> raycastHitObjects = raycastHits.Select(t => t.transform.gameObject);
                IEnumerable<GameObject> reflectedRaycastHitObjects = reflectedRaycastHits.Select(t => t.transform.gameObject);

                int count = previousHandHitPositions.Count;

                // By default, the target is behind hand
                if (target == "handTarget" && task == 3)
                {
                    // but if the hand is out of the specificied x-value
                    if (hitOnPlanePosition.x <= ABSeparationX)
                    {
                        if (count > 0)
                        {
                            // a sham condition is chosen
                            target = "handHorizontalTarget";
                        }
                        else
                        {
                            if (debug)
                            {
                                Debug.Log("No previous hand hits, choosing RANDOM");
                            }
                            target = "randomTarget";
                        }
                    }
                }
                // The same for reflected condition
                if (target == "reflectedTarget" && task == 3)
                {
                    if (hitOnPlanePosition.x <= ABSeparationX)
                    {
                        if (count > 0)
                        {
                            target = "reflectedHorizontalTarget";
                        }
                        else
                        {
                            if (debug)
                            {
                                Debug.Log("No previous hand hits, choosing RANDOM");
                            }
                            target = "randomTarget";
                        }
                    }
                }


                if (test || testTargets)
                {
                    Vector3 fingerPosition = m_HandController.getMiddleFingerPosition();
                    Vector3 fingerDirection = fingerPosition - centerEye;

                    //Physics.Raycast(centerEye, fingerDirection, 1000, 1 << 8);
                    //Debug.DrawRay(centerEye, fingerDirection * 100, Color.green);
                    //Debug.DrawRay(centerEye, palmDirection * 100, Color.white);

                    foreach (VibrantBall b in balls)
                    {
                        b.ball.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                    }

                    foreach (GameObject go in forwardRaycastHitObjects)
                    {
                        go.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                    }

                    //foreach (GameObject go in reflectedRaycastHitObjects)
                    //{
                    //    go.GetComponent<Renderer>().material.color = new Color(0, 255, 128);
                    //}

                    foreach (GameObject go in raycastHitObjects)
                    {
                        go.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                    }
                }
                
                if (target == "handHorizontalTarget")
                {
                    // Control x is a previously had hand target raycast hit center
                    // Control y is the current hand height hit position
                    int randPos = UnityEngine.Random.Range(0, count);
                    Vector3 randomHandHit = previousHandHitPositions[randPos];
                    Vector3 controlHitCenter = new Vector3(randomHandHit.x, hitOnPlanePosition.y, randomHandHit.z);
                    Vector3 controlHitDirection = controlHitCenter - centerEye;
                    RaycastHit[] controlRaycastHits = Physics.SphereCastAll(centerEye, spherecastRadius, controlHitDirection, 1000, 1 << 9); // Hits only balls
                    var controlRaycastHitObjects = controlRaycastHits.Select(t => t.transform.gameObject).ToList();

                    GameObject targetObject = ChooseRandomTarget(controlRaycastHitObjects, controlHitCenter);

                    if (targetObject != null)
                    {
                        if (debug)
                            Debug.Log("HAND CONTROL");
                        PlayObjectState(targetObject, targetStateHash);

                        // Use unique hits to cover traces i.e. throw out used positions
                        if (!mainTutorialRunning)
                            previousHandHitPositions.RemoveAt(randPos);
                    }
                }
                else if (target == "reflectedHorizontalTarget")
                {
                    // Control x is a previously had hand target raycast hit center
                    // Control y is the current hand height hit position
                    int randPos = UnityEngine.Random.Range(0, count);
                    Vector3 randomHandHit = previousHandHitPositions[randPos];
                    Vector3 controlReflectedHitCenter = new Vector3(-randomHandHit.x, hitOnPlanePosition.y, randomHandHit.z);
                    Vector3 controlReflectedHitDirection = controlReflectedHitCenter - centerEye;
                    RaycastHit[] controlRaycastHits = Physics.SphereCastAll(centerEye, spherecastRadius, controlReflectedHitDirection, 1000, 1 << 9); // Hits only balls
                    var controlRaycastHitObjects = controlRaycastHits.Select(t => t.transform.gameObject).ToList();

                    GameObject targetObject = ChooseRandomTarget(controlRaycastHitObjects, controlReflectedHitCenter);

                    if (targetObject != null)
                    {
                        if (debug)
                            Debug.Log("REFLECTED CONTROL");
                        PlayObjectState(targetObject, targetStateHash);

                        // Use unique hits to cover traces i.e. throw out used positions
                        if (!mainTutorialRunning)
                            previousHandHitPositions.RemoveAt(randPos);
                    }
                }
                else if (target == "reflectedTarget" || target == "reflectedMockTarget")
                {
                    var raycastHitsVisible = Enumerable.Intersect(reflectedRaycastHitObjects, forwardRaycastHitObjects).ToList();
                    GameObject targetObject = ChooseRandomTarget(raycastHitsVisible, reflectedHitPosition);
                    
                    if (targetObject != null)
                    {
                        if (target == "reflectedMockTarget")
                        {
                            if (debug)
                                Debug.Log("REFLECTED MOCK");
                            StartCoroutine(WaitMockAndPlay(targetObject, targetStateHash));
                        }
                        else
                        {
                            if (debug)
                                Debug.Log("REFLECTED");
                            PlayObjectState(targetObject, targetStateHash);

                            // Save the position of the raycast hit
                            previousHandHitPositions.Add(hitOnPlanePosition);
                        }
                    }

                }
                
                else if (target == "handTarget" || target == "mockTarget")
                {
                    var raycastHitsVisible = Enumerable.Intersect(raycastHitObjects, forwardRaycastHitObjects).ToList();
                    GameObject targetObject = ChooseRandomTarget(raycastHitsVisible, hitOnPlanePosition);

                    if (targetObject != null)
                    {
                        if (target == "mockTarget")
                        {
                            if (debug)
                                Debug.Log("HAND MOCK");
                            StartCoroutine(WaitMockAndPlay(targetObject, targetStateHash));
                        }
                        else
                        {
                            if (debug)
                                Debug.Log("HAND");
                            PlayObjectState(targetObject, targetStateHash);

                            // Save the position of the raycast hit
                            previousHandHitPositions.Add(hitOnPlanePosition);
                        }
                    }

                }
                else if (target == "randomTarget" || target == "randomMockTarget")
                {
                    var fullRandomHits = raycastFixationBoxHitObjects.ToList();
                    GameObject targetObject = ChooseRandomTarget(fullRandomHits, hitOnPlanePosition);
                    
                    if (targetObject != null)
                    {
                        if (target == "randomMockTarget")
                        {
                            if (debug)
                                Debug.Log("RANDOM MOCK");
                            StartCoroutine(WaitMockAndPlay(targetObject, targetStateHash));
                        }
                        else
                        {
                            if (debug)
                                Debug.Log("RANDOM");
                            PlayObjectState(targetObject, targetStateHash);
                        }
                    }
                }
                else if (target == "noTarget")
                {
                    waitingForTarget = false;
                    targetShown = true;

                    if (debug)
                        Debug.Log("NO MOVEMENT");
                }
                else
                {
                    if (debug)
                        Debug.Log("Holy moly, what was that? Target: " + target);
                }
            }
        }
    }

    private IEnumerator TestMode()
    { 
        balls = m_GridController.MakeGrid();
        m_FixationPointController.EnableFixationPoint();
        yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));
        m_GridController.startOscillation();
        //waitingForTarget = false;
        //handTriggerDelay = true;
    }

    private IEnumerator CenterScreen()
    {
        m_GridController.MakeGrid();
        m_FixationPointController.EnableFixationPoint();
        StartCoroutine("RecenterVR");
        m_UIController.ShowText("centerScreen");
        StartCoroutine(VRCameraFade.BeginFadeIn(false));

        yield return StartCoroutine(WaitForKeyDown(myKey));

        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));
        yield return StartCoroutine(m_UIController.HideText("centerScreen"));
        StopCoroutine("RecenterVR");
        m_FixationPointController.DisableFixationPoint();
        m_GridController.DeleteGrid();
    }

    private IEnumerator ExperimentVocabulary()
    {
        StartCoroutine(VRCameraFade.BeginFadeIn(false));
        m_FixationPointController.EnableFixationPoint();

        // Fixation point
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("FixationPoint", myKey));

        // Signal to move hand
        StartCoroutine(PlayRandomCues(1));
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("SignalToMoveHand", myKey));
        m_FixationPointController.DisableFixationPoint();

        // Moving objects
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("MovingObjects", myKey));
        m_FixationPointController.EnableFixationPoint();
        m_GridController.MakeGrid();
        m_GridController.startOscillation();

        yield return StartCoroutine(WaitForKeyDown(myKey));

        // Target object
        yield return StartCoroutine(PlayRandomAfterKeydown());
        
        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));
        m_FixationPointController.DisableFixationPoint();
        m_GridController.stopOscillation();
        m_GridController.DeleteGrid();
    }

    private IEnumerator ReactionTutorial()
    {
        int trialId = 0;

        StartCoroutine(VRCameraFade.BeginFadeIn(false));
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("reactionIntro", myKey));
        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));

        m_FixationPointController.EnableFixationPoint();

        StartCoroutine(WaitForContinue(KeyCode.LeftControl));

        while (!continueTutorial)
        {
            yield return StartCoroutine(PrepareTrialUI());

            if (UnityEngine.Random.Range(0F,1F) < 0.9F)
            {
                if (debug)
                    Debug.Log("Random");

                yield return StartCoroutine(PlayRandomObject(false));
                
                yield return StartCoroutine(WaitForReaction(triggerKey, 2.0F));

                //if (timerFinished)
                //{
                //    yield return StartCoroutine(m_WarningController.ShowNoReactionNoticeUI());
                //}
            }
            else
            {
                if (debug)
                    Debug.Log("No target");

                yield return StartCoroutine(WaitForReaction(triggerKey, 2.0F));
                
                if (!timerFinished)
                {
                    yield return StartCoroutine(m_WarningController.ShowNoTargetNoticeUI());
                }
            }


            yield return StartCoroutine(DestroyTrialUI());

            trialId += 1;
        }

        m_FixationPointController.DisableFixationPoint();
    }
    
    private IEnumerator MainTutorialPhase()
    {
        mainTutorialRunning = true;
        int trialId = 0;

        yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("mainTutorialIntro", myKey));
        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));

        m_FixationPointController.EnableFixationPoint();

        StartCoroutine(WaitForContinue(KeyCode.LeftControl));
        
        while (!continueTutorial && trialId < tutorialTrials)
        {
            Debug.Log("Trial: " + (trialId + 1));

            firstTrigger = true;
            handTriggerDelay = true;
            waitingForTarget = false;

            target = mainTutorialTargets[trialId];

            if (debug)
                Debug.Log("MC: Target chosen in the beginning: " + target);
            
            yield return StartCoroutine(PrepareTrialUI());

            yield return StartCoroutine(WaitForVerticalHandMovement(3.0F));
            
            if (timerFinished)
            {
                if (debug)
                {
                    Debug.Log("MC: Handmovement timer finished");
                }

                m_SwipeController.StopAllCoroutines();

                yield return StartCoroutine(m_WarningController.ShowHandNoticeUI());
            }
            else
            {
                yield return StartCoroutine(WaitForVerticalTarget());

                if (topTurn && target != "noTarget")
                {
                    if (tooLow)
                    {
                        Debug.LogError("Hand too low!");
                    }
                    else
                    {
                        Debug.LogError("Hand TOO FAST");
                    }

                    yield return new WaitForSeconds(1F);
                }
                else
                {
                    yield return StartCoroutine(WaitForTargetAnimation());

                    if (!timerFinished)
                    {
                        yield return StartCoroutine(WaitForReaction(triggerKey, 2.0F));

                        if (!timerFinished && target == "noTarget")
                        {
                            yield return StartCoroutine(handleNoTarget());
                        }
                    }
                }
            }
            
            yield return StartCoroutine(DestroyTrialUI());

            trialId += 1;
        }
        mainTutorialRunning = false;
        m_FixationPointController.DisableFixationPoint();
    }

    private IEnumerator HandUpExperiment()
    {
        int trialId = 0;
        
        yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("experimentIntro", myKey));
        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));

        m_FixationPointController.EnableFixationPoint();
        
        while (trialId < experimentTrials)
        {
            Debug.Log("Trial: " + (trialId + 1));

            firstTrigger = true;
            handTriggerDelay = true;
            waitingForTarget = false;

            float targetAppearTime;
            float targetNoticeTime;

            if (trialId % 40 == 0 && trialId != 0)
            {
                yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));

                StartCoroutine(m_WarningController.ShowPausNoticeUI(3.0F));

                yield return new WaitForSeconds(10);
                yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));
            }

            target = experimentTargets[trialId];

            if (debug)
                Debug.Log("MC: Target chosen in the beginning: " + target);
            
            TrialData trial;

            yield return StartCoroutine(PrepareTrialUI());

            if (debug)
            {
                Debug.Log("MC: Waiting for hand movement");
            }
            yield return StartCoroutine(WaitForVerticalHandMovement(3.0F));
            float handMovementBeginTime = Time.time;

            if (timerFinished)
            {
                if (debug)
                {
                    Debug.Log("MC: Handmovement timer finished");
                }

                m_SwipeController.StopAllCoroutines();

                yield return StartCoroutine(m_WarningController.ShowHandNoticeUI());
                trial = new TrialData(trialId, handMovementBeginTime, 0, 0, 0, target);
            }
            else
            {
                if (debug)
                {
                    Debug.Log("MC: Waiting for hand TARGET");
                }
                yield return StartCoroutine(WaitForVerticalTarget());

                if (topTurn && target != "noTarget")
                {
                    if (tooLow)
                    {
                        Debug.LogError("Hand too low!");

                        // (not recommended)
                        //yield return StartCoroutine(HandleTooLow());
                    }
                    else
                    {
                        Debug.LogError("Hand TOO FAST");

                        // (not recommended)
                        //yield return StartCoroutine(HandleTooFast());
                    }

                    yield return new WaitForSeconds(1F);
                    trial = new TrialData(trialId, handMovementBeginTime, 0, 0, 0, target);
                }
                else
                {
                    yield return StartCoroutine(WaitForTargetAnimation());

                    if (timerFinished)
                    {
                        trial = new TrialData(trialId, handMovementBeginTime, 0, 0, 0, target);
                    }
                    //if (tooSlow)
                    //{
                    //    yield return StartCoroutine(handleTooSlow());
                    //    trial = new TrialData(trialId, handMovementBeginTime, 0, 0, 0, target);
                    //}
                    else
                    {
                        targetAppearTime = Time.time;
                        yield return StartCoroutine(WaitForReaction(triggerKey, 2.0F));
                        targetNoticeTime = Time.time;

                        //if (timerFinished && target != "noTarget")
                        //{
                        //    yield return StartCoroutine(handleNoReaction());
                        //}
                        if (!timerFinished && target == "noTarget")
                        {
                            yield return StartCoroutine(handleNoTarget());
                        }

                        float reactionTime = targetNoticeTime - targetAppearTime;
                        bool reacted = true;

                        if (reactionTime >= 2.0F)
                        {
                            reacted = false;
                        }

                        if (target == "noTarget")
                        {
                            trial = new TrialData(
                                trialId,
                                handMovementBeginTime,
                                targetAppearTime,
                                targetNoticeTime,
                                reactionTime,
                                target,
                                reacted,
                                true
                                );
                        }
                        else
                        {
                            float targetDistanceFromFixation = Mathf.Abs(Vector3.Distance(new Vector3(0, 0, 0), targetObject.transform.position));
                            float targetDistanceFromMiddleFinger = Mathf.Abs(Vector3.Distance(m_HandController.getMiddleFingerHitPoint(), targetObject.transform.position));

                            trial = new TrialData(
                                trialId,
                                handMovementBeginTime,
                                targetAppearTime,
                                targetNoticeTime,
                                reactionTime,
                                target,
                                reacted,
                                true,
                                true,
                                hitOnPlanePosition,
                                targetObject.transform.position,
                                targetDistanceFromFixation,
                                targetDistanceFromRaycastHit,
                                targetDistanceFromMiddleFinger,
                                m_SwipeController.movementBeginTime,
                                m_SwipeController.movementTopTurnTime,
                                m_SwipeController.movementEndTime
                                );
                        }
                    }
                }
            }


            yield return StartCoroutine(DestroyTrialUI());

            trialId += 1;
            SubjectDataController.control.trials.Add(trial);
        }

        m_FixationPointController.DisableFixationPoint();
        m_SubjectDataController.WritePlayerData();

        yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("experimentOutro", triggerKey));
        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));
    }


    private IEnumerator HandLeftRightExperiment()
    {
        int trialId = 0;

        yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("experimentIntro", myKey));
        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));

        m_FixationPointController.EnableFixationPoint();

        while (trialId < experimentTrials)
        {
            Debug.Log("Trial: " + (trialId + 1));

            firstTrigger = true;
            handTriggerDelay = true;
            waitingForTarget = false;

            float targetAppearTime;
            float targetNoticeTime;

            if (trialId % 50 == 0 && trialId != 0)
            {
                yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));

                StartCoroutine(m_WarningController.ShowPausNoticeUI(3.0F));

                yield return new WaitForSeconds(10);
                yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));
            }

            target = experimentTargets[trialId];

            if (debug)
                Debug.Log("MC: Target chosen in the beginning: " + target);

            TrialData trial;

            yield return StartCoroutine(PrepareTrialUI());

            yield return StartCoroutine(WaitForHorizontalHandMovement(3.0F));
            float handMovementBeginTime = Time.time;

            if (timerFinished)
            {
                if (debug)
                {
                    Debug.Log("MC: Handmovement timer finished");
                }

                m_SwipeController.StopAllCoroutines();

                yield return StartCoroutine(m_WarningController.ShowHandNoticeUI());
                trial = new TrialData(trialId, handMovementBeginTime, 0, 0, 0, target);
            }
            //else if (tooLow)
            //{
            //    yield return StartCoroutine(HandleTooLow());
            //    trial = new TrialData(trialId, handMovementBeginTime, 0, 0, 0, target);
            //}
            else
            {
                yield return StartCoroutine(WaitForHorizontalTarget());

                if (controller.Frame().Hands.Count == 0)
                {
                    Debug.Log("Hand dissapeared!");
                    trial = new TrialData(trialId, handMovementBeginTime, 0, 0, 0, target);
                }
                else if (stopped)
                {
                    Debug.Log("Hand moved too fast or too short, could not play target");
                    trial = new TrialData(trialId, handMovementBeginTime, 0, 0, 0, target);
                }
                else
                {
                    yield return StartCoroutine(WaitForTargetAnimation());
                    
                    targetAppearTime = Time.time;
                    yield return StartCoroutine(WaitForReaction(triggerKey, 2.0F));
                    targetNoticeTime = Time.time;
                    
                    if (!timerFinished && target == "noTarget")
                    {
                        yield return StartCoroutine(handleNoTarget());
                    }

                    float reactionTime = targetNoticeTime - targetAppearTime;
                    bool reacted = true;

                    if (reactionTime >= 2.0F)
                    {
                        reacted = false;
                    }

                    if (target == "noTarget")
                    {
                        trial = new TrialData(
                            trialId,
                            handMovementBeginTime,
                            targetAppearTime,
                            targetNoticeTime,
                            reactionTime,
                            target,
                            reacted,
                            true
                            );
                    }
                    else
                    {
                        float targetDistanceFromFixation = Mathf.Abs(Vector3.Distance(new Vector3(0, 0, 0), targetObject.transform.position));
                        float targetDistanceFromMiddleFinger = Mathf.Abs(Vector3.Distance(m_HandController.getMiddleFingerHitPoint(), targetObject.transform.position));

                        trial = new TrialData(
                            trialId,
                            handMovementBeginTime,
                            targetAppearTime,
                            targetNoticeTime,
                            reactionTime,
                            target,
                            reacted,
                            true,
                            true,
                            hitOnPlanePosition,
                            targetObject.transform.position,
                            targetDistanceFromFixation,
                            targetDistanceFromRaycastHit,
                            targetDistanceFromMiddleFinger,
                            m_SwipeController.movementBeginTime,
                            m_SwipeController.movementTopTurnTime,
                            m_SwipeController.movementEndTime
                            );
                    }
                }
            }


            yield return StartCoroutine(DestroyTrialUI());

            trialId += 1;
            SubjectDataController.control.trials.Add(trial);
        }

        m_FixationPointController.DisableFixationPoint();
        m_SubjectDataController.WritePlayerData();

        yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));
        yield return StartCoroutine(ShowInstructionsAndWaitForKeyDown("experimentOutro", triggerKey));
        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));
    }

    private IEnumerator handleNoTarget()
    {
        if (debug)
        {
            Debug.Log("MC: Reacted for no target");
        }

        yield return StartCoroutine(m_WarningController.ShowNoTargetNoticeUI());
    }

    private IEnumerator handleNoReaction()
    {
        if (debug)
        {
            Debug.Log("MC: Reaction not detected within 3 seconds");
        }

        yield return StartCoroutine(m_WarningController.ShowNoReactionNoticeUI());
    }

    private IEnumerator HandleTooLow()
    {
        if (debug)
        {
            Debug.Log("MC: Hand too LOW!");
        }

        yield return StartCoroutine(m_WarningController.ShowHigherNoticeUI());
    }

    private IEnumerator HandleTooFast()
    {
        if (debug)
        {
            Debug.Log("MC: Hand too fast!");
        }

        yield return StartCoroutine(m_WarningController.ShowSlowerNoticeUI());
    }

    private IEnumerator handleTooSlow()
    {
        if (debug)
        {
            Debug.Log("MC: Hand too SLOW!");
        }

        yield return StartCoroutine(m_WarningController.ShowFasterNoticeUI());
    }

    private IEnumerator PlayRandomAfterKeydown()
    {
        while (!Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(myKey))
            {
                yield return StartCoroutine(m_FixationPointController.ShowCue());
                yield return StartCoroutine(PlayRandomObject(true));
            }

            yield return null;
        }
    }

    private IEnumerator PlayRandomCues(int j)
    {
        yield return new WaitForSeconds(2.0F);

        for (int i = 1; i <= j; i++)
        {
            yield return new WaitForSeconds(1.0F);
            yield return StartCoroutine(m_FixationPointController.ShowCue());
        }
    }

    private IEnumerator PrepareTrialUI()
    {
        balls = m_GridController.MakeGrid();

        yield return StartCoroutine(VRCameraFade.BeginFadeIn(false));

        yield return new WaitForSeconds(1);
        m_GridController.startOscillation();

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(m_FixationPointController.ShowCue());
    }

    private IEnumerator DestroyTrialUI()
    {
        yield return StartCoroutine(VRCameraFade.BeginFadeOut(false));
        m_GridController.DeleteGrid();
    }


    private IEnumerator WaitForTopTurn(float time)
    {
        timerFinished = false;
        topTurn = false;
        tooLow = false;

        m_SwipeController.StartUpHandMotionCycle();

        float start = Time.time;
        while (!topTurn && !tooLow && Time.time < start + time)
        {
            yield return null;
        }

        if (Time.time >= start + time)
        {
            timerFinished = true;
        }
    }
    private IEnumerator WaitForVerticalHandMovement(float time)
    {
        timerFinished = false;
        goingUp = false;

        m_SwipeController.StartUpHandMotionCycle();

        float start = Time.time;
        while (!goingUp && Time.time < start + time)
        {
            yield return null;
        }

        if (Time.time >= start + time)
        {
            timerFinished = true;
        }
    }
    private IEnumerator WaitForHorizontalHandMovement(float time)
    {
        timerFinished = false;
        rightTurn = false;

        m_SwipeController.StartLeftRightHandMotionCycle();

        float start = Time.time;
        while (!rightTurn && Time.time < start + time)
        {
            yield return null;
        }

        if (Time.time >= start + time)
        {
            timerFinished = true;
        }
    }

    private IEnumerator WaitForVerticalTarget()
    {
        waitingForTarget = true;
        targetShown = false;
        topTurn = false;
        tooLow = false;
        
        while (waitingForTarget && !topTurn)
        {
            yield return null;
        }

        // Make sure no target is played after hand is stopped or too slow
        waitingForTarget = false;
    }

    private IEnumerator WaitForHorizontalTarget()
    {
        waitingForTarget = true;
        targetShown = false;
        stopped = false;
        
        while (waitingForTarget && !stopped && controller.Frame().Hands.Count > 0)
        {
            yield return null;
        }
        
        waitingForTarget = false;
    }

    private IEnumerator WaitForTargetAnimation()
    {
        float start = Time.time;
        timerFinished = false;

        while (!targetShown && Time.time < start + 2.0F)
        {
            yield return null;
        }

        if (Time.time >= start + 2.0F)
        {
            Debug.LogError("Hand dissapeared before delay end!");
            timerFinished = true;
        }
    }


    private IEnumerator WaitForReaction(KeyCode key, float time)
    {
        float start = Time.time;
        timerFinished = false;

        if (debug)
            Debug.Log("Waiting for reaction"); 

        while (!Input.GetKeyDown(key) && Time.time < start + time)
            yield return null;

        if (Time.time >= start + time)
        {
            timerFinished = true;
        }
    }
    private IEnumerator WaitForKeyDown(KeyCode key)
    {
        timerFinished = false;
        while (!Input.GetKeyDown(key))
            yield return null;
        timerFinished = true;
    }


    private IEnumerator WaitForContinue(KeyCode key)
    {
        continueTutorial = false;
        while (!Input.GetKeyDown(key))
            yield return null;
        continueTutorial = true;
    }

    private IEnumerator DelayHandTrigger(float fromTime, float toTime = 0)
    {
        // Pick random delay period between fromTime and toTime
        // If no toTime is provided, wait exactly fromTime seconds

        float delay;
        if (toTime == 0)
        {
            delay = fromTime;
        }
        else
        {
            delay = UnityEngine.Random.Range(fromTime, toTime);
        }

        yield return new WaitForSeconds(delay);
        
        if (debug)
        {
            Debug.Log("Delay END");
        }

        handTriggerDelay = false;
    }

    private IEnumerator ShowInstructionsAndWaitForKeyDown(string instructions, KeyCode key)
    {
        m_UIController.ShowText(instructions);

        while (!Input.GetKeyDown(key))
        {
            yield return null;
        }

        yield return StartCoroutine(m_UIController.HideText(instructions));
    }

    private IEnumerator PlayRandomObject(bool showColor)
    {
        Vector3 centerEye = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye);
        RaycastHit[] raycastFixationBoxHits = Physics.BoxCastAll(centerEye, new Vector3(Math.Abs(ABSeparationX), 0.5F), m_ExperimentPlane.transform.position, Quaternion.identity, 1000, 1 << 9);

        RaycastHit randomHit = raycastFixationBoxHits[UnityEngine.Random.Range(0, raycastFixationBoxHits.Length)];
        
        if (showColor)
        {
            Color initCol = randomHit.transform.gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
            randomHit.transform.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0, 0, 255));
            yield return new WaitForSeconds(0.1F);
            randomHit.transform.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", initCol);
        }

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5F, 1F));

        PlayObjectState(randomHit.transform.gameObject, targetStateHash);
    }


    private IEnumerator RecenterVR()
    {
        while(true)
        {
            if (Input.GetKeyDown(triggerKey))
            {
                UnityEngine.VR.InputTracking.Recenter();
            }

            yield return null;
        }
    }

    private IEnumerator WaitMockAndPlay(GameObject targetObject, int stateHash)
    {
        if (debug)
        {
            Debug.Log("Waiting mock buffer");
        }

        if (task == 1)
        {
            while (m_HandController.getMiddleFingerHitPoint().y + bufferRadius > targetObject.transform.position.y)
            {
                yield return null;
            }
        }
        else if (task == 2)
        {
            while (m_HandController.getThumbHitPoint().x + bufferRadius > targetObject.transform.position.x)
            {
                yield return null;
            }
        }

        PlayObjectState(targetObject, stateHash);
    }

    private IEnumerator WaitandPlay(GameObject targetObject, float s)
    {
        yield return new WaitForSeconds(s);

        PlayObjectState(targetObject, targetStateHash);
    }

    private void PlayObjectState(GameObject go, int stateHash)
    {
        if (debug)
        {
            Debug.Log("Playing animation");
        }

        go.GetComponent<Animator>().Play(stateHash);
        targetShown = true;
    }


    private GameObject ChooseTarget(List<GameObject> raycastHitsVisible, Vector3 centerHitPosition)
    {
        if (raycastHitsVisible.Count > 0)
        {
            waitingForTarget = false;

            targetDistanceFromRaycastHit = Mathf.Infinity;
            foreach (GameObject go in raycastHitsVisible)
            {
                float currentDistance = Mathf.Abs(Vector3.Distance(go.transform.position, centerHitPosition));

                if (currentDistance < targetDistanceFromRaycastHit)
                {
                    targetDistanceFromRaycastHit = currentDistance;
                    targetObject = go;
                }
            }

            return targetObject;
        }
        else
        {
            return null;
        }
    }

    private GameObject ChooseRandomTarget(List<GameObject> raycastHitsVisible, Vector3 centerHitPosition)
    {
        if (raycastHitsVisible.Count > 0)
        {
            waitingForTarget = false;

            targetObject = raycastHitsVisible[UnityEngine.Random.Range(0, raycastHitsVisible.Count)];
            targetDistanceFromRaycastHit = Mathf.Abs(Vector3.Distance(targetObject.transform.position, centerHitPosition));

            return targetObject;
        }
        else
        {
            return null;
        }
    }


    private void GoingUp()
    {
        goingUp = true;
    }
    
    private void GoingRight()
    {
        goingRight = true;
    }

    private void TopTurn()
    {
        topTurn = true;
    }

    private void RightTurn()
    {
        rightTurn = true;
    }

    private void LeftTurn()
    {
        leftTurn = true;
    }

    private void Stopped()
    {
        stopped = true;
    }

    private void TooSlow()
    {
        tooSlow = true;
    }

    private void TooLow()
    {
        tooLow = true;
    }
}
