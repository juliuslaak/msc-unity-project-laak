using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class IdControl : MonoBehaviour {

    public static IdControl subjectControl;

    private string savePath;
    private int persistentId; // We don't want to change that from the Editor
    public int id
    {
        get
        {
            return persistentId;
        }
    }

    void Awake()
    {
        // Keep between Scenes
        if (subjectControl == null)
        {
            DontDestroyOnLoad(gameObject);
            subjectControl = this;
        }
        else if (subjectControl != this)
        {
            Destroy(gameObject);
        }

        savePath = Application.dataPath + "/ExperimentData";

        if (File.Exists(savePath + "/id_data.dat"))
        {
            LoadNextId();
        }
        else
        {
            persistentId = 1;
        }
    }

    void OnDisable()
    {
        SaveCurrentId();
    }

    public void SaveCurrentId()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Create(savePath + "/id_data.dat"))
        {
            IdData data = new IdData();
            data.persistentId = persistentId;

            bf.Serialize(file, data);
        }
    }

    public void LoadNextId()
    {
        if (File.Exists(savePath + "/id_data.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(savePath + "/id_data.dat", FileMode.Open))
            {
                IdData data = (IdData)bf.Deserialize(file);

                persistentId = data.persistentId + 1;
            }
                
        }
    }
}

[Serializable]
class IdData
{
    public int persistentId;
}
