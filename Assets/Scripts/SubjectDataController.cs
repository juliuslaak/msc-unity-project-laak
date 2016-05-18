using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;
using System;

public class SubjectDataController : MonoBehaviour {

    public static SubjectDataController control;

    public List<TrialData> trials = new List<TrialData>();
    public string fullName = "Joe Doe";
    public int age = 25;
    public string sex = "M/N";
    public string handedness = "Right/Left";
    public string vision = "No/Glasses/Yes_NoGlasses";

    private DateTime date = System.DateTime.Now; // This isn't shown in Editor, don't bother with getter
    private int subjectId;
    private string savePath;

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

    void Start()
    {
        savePath = Application.dataPath + "/ExperimentData";
        subjectId = IdControl.subjectControl.id;


        Debug.Log("Subject id: " + subjectId);
    }

    // Saves data to .dat format for future reading from scene
    void SavePlayerData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath + "/subject_" + subjectId + "_data.dat");

        PlayerData data = new PlayerData();
        data.date = date;
        data.subjectId = subjectId;
        data.fullName = fullName;
        data.age = age;
        data.sex = sex;
        data.trials = trials;

        bf.Serialize(file, data);
        file.Close();
    }    

    // Writes data into readable text file
    public void WritePlayerData()
    {
        print("Writing results to file...");

        // Subject header and data
        string data = System.String.Format("Subject_ID;date;Full_Name;Age;Sex;Handedness;Vision\n");
        data += System.String.Format("{0};{1};{2};{3};{4};{5};{6}", subjectId, date, fullName, age, sex, handedness, vision);
        
        File.AppendAllText(savePath + "/subject_" + subjectId + "_personal_data.txt", data);

        // Trial data header and data
        data = System.String.Format("TRIAL_ID;H_0;MOVED;TARGET_SHOWN;REACT;TARGET_TIME_0;TARGET_TIME_1;REACTION_TIME;TARGET;PLANE_HIT_X;PLANE_HIT_Y;PLANE_HIT_Z;CLOSEST_HIT_X;CLOSEST_HIT_Y;CLOSEST_HIT_Z;DIS_FIX;DIS_HIT;DIS_FINGER;H_1;H_2;H_3\n");
        foreach (TrialData trial in trials)
        {
            // TODO Include all the trial properties
            data += trial.trialId + ";";
            data += trial.waitBeginTime.ToString("F4") + ";";
            data += trial.moved.ToString() + ";";
            data += trial.targetShown.ToString() + ";";
            data += trial.reacted.ToString() + ";";
            data += trial.targetAppearTime.ToString("F4") + ";";
            data += trial.targetNoticeTime.ToString("F4") + ";";
            data += trial.reactionTime.ToString("F4") + ";";
            data += trial.target + ";";
            data += trial.hitOnPlane_X.ToString("F4") + ";";
            data += trial.hitOnPlane_Y.ToString("F4") + ";";
            data += trial.hitOnPlane_Z.ToString("F4") + ";";
            data += trial.closestObjectPosition_X.ToString("F4") + ";";
            data += trial.closestObjectPosition_Y.ToString("F4") + ";";
            data += trial.closestObjectPosition_Z.ToString("F4") + ";";
            data += trial.closestHitDistanceFromFixation.ToString("F4") + ";";
            data += trial.closestHitDistanceFromRaycastHit.ToString("F4") + ";";
            data += trial.closestHitDistanceFromMiddleFinger.ToString("F4") + ";";
            data += trial.handMovementBegin.ToString("F4") + ";";
            data += trial.handMovementTopTurn.ToString("F4") + ";";
            data += trial.handMovementEnd.ToString("F4");
            data += "\n";
        }
        
        File.AppendAllText(savePath + "/subject_" + subjectId + "_trial_data.csv", data);

        print("Done!");
    }

}

// TODO Create constructor including all the TrialData parameters
public class TrialData
{
    public int trialId;
    public float waitBeginTime;
    public bool reacted;
    public float targetAppearTime;
    public float targetNoticeTime;
    public float reactionTime;
    public string target;
    public bool moved;
    public bool targetShown;
    public float hitOnPlane_X;
    public float hitOnPlane_Y;
    public float hitOnPlane_Z;
    public float closestObjectPosition_X;
    public float closestObjectPosition_Y;
    public float closestObjectPosition_Z;
    public float closestHitDistanceFromFixation;
    public float closestHitDistanceFromRaycastHit;
    public float closestHitDistanceFromMiddleFinger;
    public float handMovementBegin;
    public float handMovementTopTurn;
    public float handMovementEnd;
    
    public TrialData(
        int trialId,
        float waitBeginTime,
        float targetAppearTime,
        float targetNoticeTime,
        float reactionTime,
        string target,
        bool reacted = false,
        bool moved = false,
        bool targetShown = false,
        Vector3 hitOnPlane = default(Vector3),
        Vector3 closestObjectPosition = default(Vector3),
        float closestHitDistanceFromFixation = 0F,
        float closestHitDistanceFromRaycastHit = 0F,
        float closestHitDistanceFromMiddleFinger = 0F,
        float handMovementBegin = 0F,
        float handMovementTopTurn = 0F,
        float handMovementEnd = 0F)
    {
        this.trialId = trialId;
        this.waitBeginTime = waitBeginTime;
        this.targetAppearTime = targetAppearTime;
        this.targetNoticeTime = targetNoticeTime;
        this.reactionTime = reactionTime;
        this.target = target;
        this.reacted = reacted;
        this.moved = moved;
        this.targetShown = targetShown;
        hitOnPlane_X = hitOnPlane.x;
        hitOnPlane_Y = hitOnPlane.y;
        hitOnPlane_Z = hitOnPlane.z;
        closestObjectPosition_X = closestObjectPosition.x;
        closestObjectPosition_Y = closestObjectPosition.y;
        closestObjectPosition_Z = closestObjectPosition.z;
        this.closestHitDistanceFromFixation = closestHitDistanceFromFixation;
        this.closestHitDistanceFromRaycastHit = closestHitDistanceFromRaycastHit;
        this.closestHitDistanceFromMiddleFinger = closestHitDistanceFromMiddleFinger;
        this.handMovementBegin = handMovementBegin;
        this.handMovementTopTurn = handMovementTopTurn;
        this.handMovementEnd = handMovementEnd;
    }
}

[Serializable]
class PlayerData
{
    public DateTime date;
    public int subjectId;
    public string fullName;
    public int age;
    public string sex;
    public List<TrialData> trials;
}