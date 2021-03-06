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
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.SceneManagement;

public class LocalizerEditor : EditorWindow
{
    [MenuItem("Tools/Localizer")]
    public static void ShowWindow() {
        GetWindow(typeof(LocalizerEditor));
    }

    private void OnGUI() {
        if (GUILayout.Button("Add Localizable Component")) {
            if (Selection.activeTransform != null) {
                if(Selection.activeTransform.gameObject.GetComponent<Localizable>() == null) {
                    if(Selection.activeTransform.gameObject.GetComponent<TextMeshProUGUI>() != null) {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        Selection.activeTransform.gameObject.AddComponent(typeof(Localizable));
                    }
                    else {
                        Debug.Log("Can't add Localizable component if TextMeshProUGUI is not present");
                    }
                }
                else {
                    Debug.Log("Can't add another Localizable Component");
                }
            }
        }
        GUILayout.Space(20);
        GUILayout.Label("Create a manager in the Init scene");
        if (GUILayout.Button("Create Manager")) {
            GameObject manager = new GameObject("LocalizationManager");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Undo.RegisterCreatedObjectUndo(manager, "Undo instantiated " + manager.name);
            manager.AddComponent(typeof(LocalizationManager));
        }
    }

}


