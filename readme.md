

# Unity Localization System
### Features

- Localization system using JSON file
- JSON editor inside Unity 

## Requirements
- Unity Engine (Any version should work) [Tested on 2021.3.1f1]
- TextMeshPro package
- Newtonsoft Json (com.unity.nuget.newtonsoft-json) >= V 2.0.2
- Something to edit JSON Files (Optional)

## How to use

### Change language
```c#
Localizer.Instance.SetLanguage(Language language);
or
Localizer.Instance.SetLanguage(int index);
```
To change language defined in the enum **Language**.

### Add Language
Insert your new language before the **Count**
```c#
    //Language enum
    public enum Language {
        EN,
        ES,
        IT,
        FR,
        DE,
        Count //Must be last
    }
```
Then duplicate the current keys inside the JSON file (Found at StreamingAssets/Lang/.lang)

```json
  "FR": {
    "Lang_name": "Français",
    "Test_Text": "Bonjour"
  },
```

### Editor
1. Open the editor window at Tools/Localizer
2. Press "Load"
3. Edit your translations
    - Navigate though the pages with "Previous" and "Next"
    - Add a new key with "Add" button
    - Delete a key with the "Trash" button
4. Press "Save" if the button is shown
