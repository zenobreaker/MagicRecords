using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LanguageData
{
    public string key;
    public string value;
}


public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    private string defaultLanguageCode = "ko-KR"; // �⺻ ��� ����

    public TextAsset[] languageFiles; // ��� ���ҽ� ����

    // key ���� ��ȯ�� ��� ������ ������ ��ųʸ�
    private Dictionary<string, string> localizedTextDict = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        string currentLanguageCode = GetUserSelectedLanguage();

        LoadLanguageFile(currentLanguageCode);

        if (localizedTextDict.Count == 0)
        {
            LoadLanguageFile(defaultLanguageCode);
        }
    }

    private void LoadLanguageFile(string languageCode)
    {
        string languageFileName = languageCode + ".json";
        TextAsset languageFile = Resources.Load<TextAsset>("Languages/" + languageFileName);

        if (languageFile != null)
        {
            string jsonString = languageFile.text;
            LanguageData[] languageData = JsonUtility.FromJson<LanguageData[]>(jsonString);

            foreach (LanguageData data in languageData)
            {
                localizedTextDict[data.key] = data.value;
            }
        }
        else
        {
            Debug.LogError("Language file not found: " + languageFileName);
        }
    }


    public string GetLocalizedText(string key)
    {
       if (localizedTextDict.ContainsKey(key))
        {
            return localizedTextDict[key];
        }
        else
        {
            Debug.LogWarning("Text key not found: " + key);
            return string.Empty;
        }
    }

    private string GetUserSelectedLanguage()
    {
        // ����ڰ� ������ ��� �ڵ带 �������� ����
        // ����: �÷��̾� ������, ���� ���� ��� ��� ������ ������
        return "ko-KR"; // �ӽ÷� "ko-KR"�� ��ȯ�ϴ� ����
    }
}
