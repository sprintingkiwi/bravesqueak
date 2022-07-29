using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;


public class EULA : MonoBehaviour
{
    [Header("Options")]
    public string consentFileName;

    [Header("Drag and Drop")]
    public GameObject scrollView;

    [System.Serializable]
    class EULAConsent
    {
        public string date;
        public string answer;
    }
    EULAConsent eulaConsent;
    FileStream saveFile;
    BinaryFormatter bf;
    string consentPath;

    // Start is called before the first frame update
    void Start()
    {
        consentPath = Application.persistentDataPath + "/" + consentFileName + ".dat";
        Debug.Log(consentPath);
        bf = new BinaryFormatter();

        if (!File.Exists(consentPath))
        {
            eulaConsent = new EULAConsent
            {
                date = Time.time.ToString(),
                answer = "Not Accepted"
            };
            SaveEULAConsent();
            scrollView.SetActive(true);
        }
        else
        {
            eulaConsent = GetEULAConsent();
            if (eulaConsent != null)
            {
                if (eulaConsent.answer == "Accepted") GoOn();
                else scrollView.SetActive(true);
            }
        }
    }

    EULAConsent GetEULAConsent()
    {
        saveFile = File.Open(consentPath, FileMode.Open);
        if (saveFile.Length > 0)
        {
            eulaConsent = (EULAConsent)bf.Deserialize(saveFile);
            saveFile.Close();
            Debug.Log("DATE: " + eulaConsent.date + "; ANSWER: " + eulaConsent.answer);
            return eulaConsent;
        }
        else
        {
            Debug.LogWarning("EULA save file is empty. This should not happen. Returning null.");
            return null;
        }
    }

    void SaveEULAConsent()
    {
        saveFile = File.Open(consentPath, FileMode.OpenOrCreate);
        bf.Serialize(saveFile, eulaConsent);
        saveFile.Close();
    }

    public void Accepted()
    {
        eulaConsent.date = Time.time.ToString();
        eulaConsent.answer = "Accepted";
        SaveEULAConsent();
        GoOn();
    }

    public void NotAccepted()
    {
        Application.Quit();
    }

    void GoOn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
