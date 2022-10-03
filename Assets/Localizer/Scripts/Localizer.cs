/*
 MIT License

Copyright (c) 2022 Stephen Grosjean

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
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Localizer : MonoBehaviour
{
    public static Localizer Instance;  //Instance of the Localizer

    [HideInInspector] public UnityEvent OnLanguageChange; // Event called at the end of the init

    private Dictionary<Language, Dictionary<string, string>> m_languages = new Dictionary<Language, Dictionary<string, string>>(); //Dictionary of all the keys
    private Language m_currentLanguage; // The current language
    private bool m_isInit;  // Is it loaded?

    // Add your language code here
    public enum Language {
        EN,
        ES,
        IT,
        FR,
        DE,
        Count //Must be last
    }


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        m_languages = Load();
        float end = Time.realtimeSinceStartup;
        m_isInit = true;
        OnLanguageChange.Invoke();
    }

    public void FirstInit() {
        for(int i = 0; i < (int)Language.Count; i++) {
            m_languages.Add((Language)i, new Dictionary<string, string>());

            m_languages[(Language)i].Add("Lang_name", "");
        }     
    }

    public void Add(Language language, string key, string word) {
        if(!m_isInit) {
            Debug.LogError("[Localizer] Error not init");
            return;
        }
        m_languages[language].Add(key, word);
    }

    public static void Save(Dictionary<Language, Dictionary<string, string>> languages) {
        string s = JsonConvert.SerializeObject(languages, Formatting.Indented);
        File.WriteAllText(Application.streamingAssetsPath + "/Lang/" + ".lang",s);
    }

    public static Dictionary<Language, Dictionary<string, string>> Load() {
        return JsonConvert.DeserializeObject<Dictionary<Language, Dictionary<string, string>>>(File.ReadAllText(Application.streamingAssetsPath + "/Lang/" + ".lang"));
    }

    public string GetWord(Language language, string key) {
        return m_languages[language][key];
    }

    public string GetWord(string key) {
        if(m_languages[m_currentLanguage].ContainsKey(key)) {

            return m_languages[m_currentLanguage][key];
        }
        else {
            Debug.LogError("[Localizer] Error, key not found");
            return "";
        }
    }

    // Call this to set the language
    public void SetLanguage(Language language) {
        m_currentLanguage = language;
        OnLanguageChange.Invoke();
    }

    // Call this to set the language
    public void SetLanguage(int index) {
        SetLanguage((Language)index);  
    }
}

