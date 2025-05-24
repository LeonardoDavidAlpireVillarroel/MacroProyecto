using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableKeyValue
{
    public string key;
    public string value;
}

public class ProfileData
{
    public string filename;
    public string name;
    public bool newGame;

    //Tutoriales
    public bool dashTutorialSeen = false;

    //Posicion jugador
    public float x;
    public float y;

    //Progreso jugador
    public int playerHealth;
    public int fuerza;
    public int points;
    //Inventario
    public string inventoryJson;
    public int unlockedLevelCount;

    // PlayerPrefs personalizados
    public List<SerializableKeyValue> customPrefs;

    [System.Serializable]
    public class LevelStats
    {
        public string levelName;
        public float timeCompleted;
        public int fruitsCollected;
        public int enemiesDefeated;
    }

    public List<LevelStats> levelsStats = new List<LevelStats>();

    public LevelStats GetLevelStats(string levelName)
    {
        var stats = levelsStats.Find(l => l.levelName == levelName);
        if (stats == null)
        {
            stats = new LevelStats
            {
                levelName = levelName,
                timeCompleted = 0f,
                fruitsCollected = 0,
                enemiesDefeated = 0
            };
            levelsStats.Add(stats);
        }
        return stats;
    }

    public ProfileData()
    {
        this.filename = "None.xml";
        this.name = "None";
        this.newGame = false;
        this.x = this.y = 0f;

        this.playerHealth = 3;
        this.fuerza = 10;
        this.points = 50;
        this.inventoryJson = string.Empty;
        this.unlockedLevelCount = 2;

        this.customPrefs = new List<SerializableKeyValue>();
        this.levelsStats = new List<LevelStats>();
    }

    public ProfileData(string name, bool newGame, float x, float y)
    {
        this.filename = name.Replace(" ", "_") + ".xml";
        this.name = name;
        this.newGame = newGame;
        this.x = x;
        this.y = y;

        this.playerHealth = 3;
        this.fuerza = 10;
        this.points = 50;
        this.inventoryJson = string.Empty;
        this.unlockedLevelCount = 2;

        this.customPrefs = new List<SerializableKeyValue>();
        this.levelsStats = new List<LevelStats>();
    }

    public void SetPref(string key, string value)
    {
        var existing = customPrefs.Find(p => p.key == key);
        if (existing != null)
            existing.value = value;
        else
            customPrefs.Add(new SerializableKeyValue { key = key, value = value });
    }

    public string GetPref(string key, string defaultValue = "")
    {
        var existing = customPrefs.Find(p => p.key == key);
        return existing != null ? existing.value : defaultValue;
    }

    public int GetPrefInt(string key, int defaultValue = 0)
    {
        string val = GetPref(key);
        return int.TryParse(val, out int result) ? result : defaultValue;
    }

    public void SetPrefInt(string key, int value)
    {
        SetPref(key, value.ToString());
    }

    public float GetPrefFloat(string key, float defaultValue = 0f)
    {
        string val = GetPref(key);
        return float.TryParse(val, out float result) ? result : defaultValue;
    }

    public void SetPrefFloat(string key, float value)
    {
        SetPref(key, value.ToString());
    }
}
