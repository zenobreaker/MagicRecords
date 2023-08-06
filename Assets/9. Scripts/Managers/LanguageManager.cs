using System.Collections;
using System.Collections.Generic;
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

    private string defaultLanguageCode = "ko-KR"; // 기본 언어 설정

    public TextAsset[] languageFiles; // 언어 리소스 파일

    // key 값과 변환된 언어 내용을 가지는 딕셔너리
    private Dictionary<string, string> localizedTextDict = new Dictionary<string, string>();
    private bool isChanging;

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
        // 사용자가 선택한 언어 코드를 가져오는 로직
        // 예시: 플레이어 프로필, 게임 설정 등에서 언어 설정을 가져옴
        return "ko-KR"; // 임시로 "ko-KR"을 반환하는 예시
    }

    // 키값을 받으면 로컬관련한 테이블에서 찾아서 해당 언어로 변환된 값을 반환
    public string GetLocaliztionValue(string key)
    {
        // 현재 선택한 언어 
        Locale currentLocale = LocalizationSettings.SelectedLocale;

        var value = LocalizationSettings.StringDatabase.GetLocalizedString(
            "MyTable", key, currentLocale);

        return value;
    }
    
    // 언어 변경 이벤트 

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

    // 게임 플레잉 중 변경
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

            // 변환될 이벤트 정리 
        }
    }
}
