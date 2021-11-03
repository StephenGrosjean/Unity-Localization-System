/*
 MIT License

Copyright (c) 2021 Stephen Grosjean

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;

    //Language enum
    public enum Lang
    {
        EN = 1,
        FR
    }

    //Languages
    private Dictionary<string, string> FR = new Dictionary<string, string>();
    private Dictionary<string, string> EN = new Dictionary<string, string>();

    //Current selected language
    public Lang currentLanguage = Lang.EN;

    private bool isInit;
    private string assetPath;

    
    private void Awake() {
        //Create Singleton
        instance = this;
        DontDestroyOnLoad(this);

        //Add OnLoaded function to scene manager callbacks
        SceneManager.sceneLoaded += OnLoaded;
    }

    private void Init() {
        isInit = false;
        string fileContent = "";

        //Read keys.csv depending on the platform
#if UNITY_EDITOR
        fileContent = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/Lang/keys.csv");
#elif UNITY_ANDROID
    var _path = Application.streamingAssetsPath + "/Lang/keys.csv";
    UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(_path);
    www.SendWebRequest();
    while (!www.isDone) { }

    fileContent = www.downloadHandler.text;
#elif UNITY_IOS
    fileContent = System.IO.File.ReadAllText(Application.dataPath + "/Raw/Lang/keys.csv");

#else
    fileContent = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/Lang/keys.csv");
#endif
        //Split file by lines
        string[] lines = fileContent.Split("\n"[0]);

        //Split keys and store into keys list
        List<string[]> keys = new List<string[]>();
        for (int i = 1; i < lines.Length - 1; i++) {
            string[] key = lines[i].Split(";"[0]);
            keys.Add(key);
        }

        //Populate each language dictionary
        for (int i = 0; i < keys.Count; i++) {
            EN.Add(keys[i][0], keys[i][(int)Lang.EN]);
            FR.Add(keys[i][0], keys[i][(int)Lang.FR]);
        }

        isInit = true;
    }

    void Start() {
        //Save language pref in PlayerPrefs
        if (PlayerPrefs.HasKey("Language")) {
            currentLanguage = (Lang)PlayerPrefs.GetInt("Language");
        }
        else {
            currentLanguage = Lang.EN;
            PlayerPrefs.SetInt("Language", 1);
        }

        //Init languages and set scene Texts
        ReloadLocalization();
    }

    void OnLoaded(Scene scene, LoadSceneMode mode) {
        ReloadLocalization();
    }

    /// <summary>
    /// Find the Key for the selected language
    /// </summary>
    string FindKey(string key, Lang lang) {
        switch (lang) {
            case Lang.EN:
                if (EN.ContainsKey(key))
                    return EN[key];
                else Debug.LogError("No keys for : " + key);
                break;
            case Lang.FR:
                if (FR.ContainsKey(key))
                    return FR[key];
                else Debug.LogError("No keys for : " + key);
                break;
        }
        return "";
    }

    /// <summary>
    /// Find the Key for the current selected language
    /// </summary>
    public string FindKey(string key) {
        return FindKey(key, currentLanguage);
    }

    /// <summary>
    /// Set the text of each Localizable component depending on the language
    /// </summary>
    void SetText(Lang lang) {
        Localizable[] toLocalize = Resources.FindObjectsOfTypeAll(typeof(Localizable)) as Localizable[];

        foreach (Localizable l in toLocalize) {
            l.Init();
            l.textComponent.text = FindKey(l.baseKey, lang);
        }
    }

    /// <summary>
    /// Set the current selected language
    /// </summary>
    public void SetLanguage(Lang language) {
        currentLanguage = language;
        PlayerPrefs.SetInt("Language", (int)language);
        ReloadLocalization();
    }

    /// <summary>
    /// Set Language based on index (Used in UIButtons)
    /// </summary>
    public void SetLanguage(int index) {
        SetLanguage((Lang)index);
    }

    /// <summary>
    /// Reload the localization
    /// </summary>
    public void ReloadLocalization() {
        FR.Clear();
        EN.Clear();
        Init();
        SetText(currentLanguage);
    }
}
