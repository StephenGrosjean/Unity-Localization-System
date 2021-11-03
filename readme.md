

# Unity Localization System
### Features

- Localization system using csv file
- Mobile ready

## Requirements
- Unity Engine (Any version should work) [Tested on 2020.3.14f1]
- TextMeshPro package
- Something to edit CSV files

### How to use

#### Change language
```c#
SetText(Lang lang); or SetText(int index);
```
To change language defined in the enum **Lang**.

#### Add Language
```c#
    //Language enum
    public enum Lang
    {
        EN = 1,
        FR
    }

    //Languages
    private Dictionary<string, string> FR = new Dictionary<string, string>();
    private Dictionary<string, string> EN = new Dictionary<string, string>();
```

Add your new language in the enum, and create a new Dictionary corresponding to the language.

Do not forget to add the translations in the CSV file too.

### Download
Last Unity package: [Here](https://github.com/StephenGrosjean/Unity-Localization-System/raw/master/Builds/LocalizationSystem.unitypackage)
