using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LocalizerEditor : EditorWindow
{
    private Dictionary<Localizer.Language, Dictionary<string, string>> m_loadedLocalization;
    private Dictionary<Localizer.Language, Dictionary<string, string>> m_copiedLocalization;

    string m_newKey = "";

    private bool IsLoaded = false;
    private bool _EnableNewKey;
    private Vector2 m_scrollPos = Vector2.zero;
    private bool NeedSaving = false;
    private int m_currentPage = 1;
    private int m_maxShown = 100;

    List<string> keys = new List<string>();
    List<List<string>> LangList = new List<List<string>>();
    [MenuItem("Tools/Localizer")]
    public static void ShowWindow() {
        EditorWindow window = CreateWindow<LocalizerEditor>("Localizer");
        window.maxSize = new Vector2(1240, 840);
        window.minSize = new Vector2(430, 200);
    }
    private void OnGUI() {

        if(!IsLoaded) {
            if(GUILayout.Button("Load")) {
                Load();
            }
        }
        else {
            int maxRange = m_maxShown * m_currentPage;

            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
            {
                GUILayout.BeginHorizontal();
                {
                    //Start Keys column
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("Keys", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        if(maxRange >= keys.Count) {
                            maxRange = keys.Count;
                        }

                        for(int i = m_maxShown * (m_currentPage - 1); i < maxRange; i++) {
                            string key = keys[i];
                            GUILayout.BeginHorizontal();
                            {
                                if(GUILayout.Button((Texture)Resources.Load("Localizer/delete"), GUILayout.Width(25), GUILayout.Height(25))) {
                                    DeleteKey(key);
                                }
                                GUILayout.Label(key, EditorStyles.boldLabel);

                            }
                            GUILayout.EndHorizontal();
                            GUILayout.FlexibleSpace();
                        }
                    }
                    GUILayout.EndVertical();
                    //End keys column

                    for(int i = 0; i < (int)Localizer.Language.Count; i++) {
                        //Start Languages column
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(((Localizer.Language)i).ToString(), EditorStyles.boldLabel);
                            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                            if(maxRange >= LangList[i].Count) {
                                maxRange = LangList[i].Count;
                            }
                            for(int j = m_maxShown * (m_currentPage - 1); j < maxRange; j++) {
                                if(m_copiedLocalization[(Localizer.Language)i].ContainsKey(keys[j])) {
                                    string beforeEdit = m_copiedLocalization[(Localizer.Language)i][keys[j]];
                                    m_copiedLocalization[(Localizer.Language)i][keys[j]] = EditorGUILayout.TextField(m_copiedLocalization[(Localizer.Language)i][keys[j]]);
                                    string AfterEdit = m_copiedLocalization[(Localizer.Language)i][keys[j]];
                                    if(beforeEdit != AfterEdit) {
                                        NeedSaving = true;
                                    }
                                }
                                GUILayout.FlexibleSpace();

                            }
                        }
                        GUILayout.EndVertical();
                        //End Languages column
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space(20);

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    if(_EnableNewKey) {
                        m_newKey = EditorGUILayout.TextField(m_newKey);

                        if(GUILayout.Button("Add")) {
                            AddKey(m_newKey);
                            m_newKey = "";
                            _EnableNewKey = false;
                        }
                    }
                    else {
                        if(GUILayout.Button("New Key")) {
                            _EnableNewKey = true;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(20);

                if(NeedSaving) {
                    if(GUILayout.Button("Save")) {
                        Save();
                    }
                }
            }
            GUILayout.EndVertical();

            if(IsLoaded && Mathf.CeilToInt(keys.Count / m_maxShown) > 1) {
                GUILayout.BeginHorizontal();
                {
                    if(GUILayout.Button("Previous")) {
                        PreviousPage();
                    }

                    if(GUILayout.Button("Next")) {
                        NextPage();
                    }
                }
                GUILayout.EndHorizontal();
            }
        }


    }

    private void NextPage() {
        int maxPages = Mathf.CeilToInt((float)keys.Count / (float)m_maxShown);
        Debug.Log(maxPages);
        if(m_currentPage + 1 > maxPages) return;

        m_currentPage++;
    }

    private void PreviousPage() {
        if(m_currentPage - 1 <= 0) return;
        m_currentPage--;
    }

    private void DeleteKey(string key) {
        IsLoaded = false;
        foreach(KeyValuePair<Localizer.Language, Dictionary<string, string>> languages in m_copiedLocalization) {
            languages.Value.Remove(key);
        }
        Refresh();
    }

    private void AddKey(string key) {
        IsLoaded = false;
        foreach(KeyValuePair<Localizer.Language, Dictionary<string, string>> languages in m_copiedLocalization) {
            if(!languages.Value.ContainsKey(key)) {
                languages.Value.Add(key, "");
            }
        }

        Refresh();
    }

    private void Refresh() {
        Save();
        Load();
    }

    private void Load() {
        keys.Clear();
        LangList.Clear();

        m_loadedLocalization = new Dictionary<Localizer.Language, Dictionary<string, string>>();
        m_copiedLocalization = new Dictionary<Localizer.Language, Dictionary<string, string>>();

        m_loadedLocalization = Localizer.Load();
        m_copiedLocalization = CopyDictionary(m_loadedLocalization);
        IsLoaded = true;

        int index = 0;

        foreach(KeyValuePair<Localizer.Language, Dictionary<string, string>> languages in m_loadedLocalization) {
            LangList.Add(new List<string>());
            foreach(KeyValuePair<string, string> words in languages.Value) {
                if(!keys.Contains(words.Key))
                    keys.Add(words.Key);

                LangList[index].Add(words.Value);
            }
            index++;
        }
    }

    private void Save() {
        m_loadedLocalization = CopyDictionary(m_copiedLocalization);
        Localizer.Save(m_loadedLocalization);
        NeedSaving = false;
    }

    private static Dictionary<Localizer.Language, Dictionary<string, string>> CopyDictionary(Dictionary<Localizer.Language, Dictionary<string, string>> toCopy) {
        Dictionary<Localizer.Language, Dictionary<string, string>> dict = new Dictionary<Localizer.Language, Dictionary<string, string>>();

        foreach(KeyValuePair<Localizer.Language, Dictionary<string, string>> languages in toCopy) {
            dict.Add(languages.Key, new Dictionary<string, string>());

            foreach(KeyValuePair<string, string> words in languages.Value) {
                dict[languages.Key].Add(words.Key, words.Value);
            }
        }
        return dict;
    }


}

