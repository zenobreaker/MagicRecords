using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class LanguageData
{
    public string key;
    public string value;
}


public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

//    private string defaultLanguageCode = "ko-KR"; // �⺻ ��� ����

    public TextAsset[] languageFiles; // ��� ���ҽ� ����

    // key ���� ��ȯ�� ��� ������ ������ ��ųʸ�
    private Dictionary<string, string> localizedTextDict = new Dictionary<string, string>();
    private bool isChanging;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        string currentLanguageCode = GetUserSelectedLanguage();


        //LoadLanguageFile(currentLanguageCode);

        if (localizedTextDict.Count == 0)
        {
          //  LoadLanguageFile(defaultLanguageCode);
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

    public string ReplaceVariables(string text, Dictionary<string, object> variables,
         bool isValuePercentage = false)
    {
        Dictionary<string, bool> variableDict = new Dictionary<string, bool>
        { 
            {"value", true },
            {"OPTION_VALUE_1", true },
            {"OPTION_VALUE_2", true },
            {"OPTION_VALUE_3", true },
            {"OPTION_DURATION_1", false },
        };


        string pattern = @"\{(\w+)\}";
        MatchEvaluator evaluator = (match) =>
        {
            string variableName = match.Groups[1].Value;
            if (variables.TryGetValue(variableName, out object value))
            {
                // if(variableName == "value") 
                if (variableDict.TryGetValue(variableName, out bool result) && isValuePercentage == true)
                {
                    return value.ToString() + "%";
                }
                return value.ToString();
            }
            return match.Value;
        };

        return Regex.Replace(text, pattern, evaluator);
    }


    public string GetLocaliztionWithSpecialOption(string key, SpecialOption option)
    {
        if (option == null) return "" ;

        string result = "";



        return result; 
    }

    // �������� ������ �ش� �������� ġȯ�ؼ� ��ȯ�ϴ� �Լ�
    public string GetLocalizationWithValues(string key, float duration, float value,
         bool isPercentage = false)
    {
        string result = "";

        result = GetLocaliztionValue(key);

        result = ReplaceVariables(result, new Dictionary<string, object>
            {
                { "duration", duration },
                { "value", value }
            }, isPercentage);

        return result; 
    }


    // Ű���� ������ ���ð����� ���̺��� ã�Ƽ� �ش� ���� ��ȯ�� ���� ��ȯ
    public string GetLocaliztionValue(string key)
    {
        // ���� ������ ��� 
        Locale currentLocale = LocalizationSettings.SelectedLocale;

        var value = LocalizationSettings.StringDatabase.GetLocalizedString(
            "MyTable", key, currentLocale);

        return value;
    }

    // ��ų ���� Ű���� ������ ���̺��� ���� ��ȯ�� ���� ��ȯ
    public string GetLocalizationSkillDesc(string skillKeycode)
    {
        // ���� ������ ��� 
        Locale currentLocale = LocalizationSettings.SelectedLocale;

        var value = LocalizationSettings.StringDatabase.GetLocalizedString(
         "SkillDescTable", skillKeycode, currentLocale);

        return value;
    }
    
    // ��� ���� �̺�Ʈ 

    public void ChangeLocale(int index)
    {
        if (isChanging)
            return;

        StartCoroutine(ChangeRoutine(index));
    }

    IEnumerator ChangeRoutine(int index)
    {
        isChanging = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        isChanging = false;
    }

    // ���� �÷��� �� ����
    void OnChangeLocale(Locale locale)
    {
        StartCoroutine(ChangeLocaleRoutine(locale));
    }

    IEnumerator ChangeLocaleRoutine(Locale loacale)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("MyTable");
        yield return loadingOperation;

        if(loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            //string talkerName = table.GetEntry().GetLoacaliztionString(); 

            // ��ȯ�� �̺�Ʈ ���� 
        }
    }
}
