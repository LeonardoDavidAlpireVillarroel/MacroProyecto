using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using UnityEditorInternal;
using DG.Tweening.Core.Easing;

public static class ProfileStorage
{
    public static ProfileData s_currentProfile;
    private static string s_profilesDir = Application.persistentDataPath + "/Profiles";
    private static string s_indexPath = s_profilesDir + "/__ProfileIndex__.xml";
    private static string s_currentProfilePath = s_profilesDir + "/CurrentProfile.txt";

    public static void CreateNewGame(string profileName)
    {
        if (!Directory.Exists(s_profilesDir))
        {
            Directory.CreateDirectory(s_profilesDir);
        }

        string uniqueName = profileName.Replace(" ", "_") + "_" + System.DateTime.Now.Ticks;
        string filename = uniqueName + ".xml";
        string path = s_profilesDir + "/" + filename;

        s_currentProfile = new ProfileData(profileName, true, 0, 0);
        s_currentProfile.filename = filename;

        SaveFile<ProfileData>(path, s_currentProfile);

        var index = GetProfileIndex();
        if (!index.ProfileFileNames.Contains(filename))
        {
            index.ProfileFileNames.Add(filename);
            SaveFile<ProfileIndex>(s_indexPath, index);
        }

        File.WriteAllText(s_currentProfilePath, filename);
    }

    public static ProfileIndex GetProfileIndex()
    {
        if (File.Exists(s_indexPath) == false)
        {
            return new ProfileIndex();
        }

        return LoadFile<ProfileIndex>(s_indexPath);
    }

    public static void LoadProfile(string filename, GameManager gameManager)
    {
        string path = Path.Combine(s_profilesDir, filename);
        if (File.Exists(path))
        {
            s_currentProfile = LoadFile<ProfileData>(path);
        }

        if (Inventory.Instance != null)
        {
            Inventory.Instance.SetInventoryFromString(s_currentProfile.inventoryJson);
        }

        if (gameManager != null)
        {
            gameManager.points = s_currentProfile.points;
            gameManager.fuerza = s_currentProfile.fuerza;
            gameManager.health = s_currentProfile.playerHealth;
        }
    }

    public static void StorePlayerProfile(GameObject player, GameManager gameManager)
    {
        if (!Directory.Exists(s_profilesDir))
        {
            Directory.CreateDirectory(s_profilesDir);
        }

        s_currentProfile.x = player.transform.position.x;
        s_currentProfile.y = player.transform.position.y;
        s_currentProfile.newGame = false;

        s_currentProfile.points = gameManager.points;
        s_currentProfile.fuerza = gameManager.fuerza;
        s_currentProfile.playerHealth = gameManager.health;

        s_currentProfile.inventoryJson = Inventory.Instance.GetInventoryAsString();

        string path = Path.Combine(s_profilesDir, s_currentProfile.filename);
        SaveFile<ProfileData>(path, s_currentProfile);
    }

    static void SaveFile<T>(string path, T data)
    {
        using (var profileWriter = new StreamWriter(path))
        {
            var profileSerializer = new XmlSerializer(typeof(T));
            profileSerializer.Serialize(profileWriter, data);
        }
    }

    public static void DeleteProfile(string filename)
    {
        string path = Path.Combine(s_profilesDir, filename);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var index = LoadFile<ProfileIndex>(s_indexPath);
        index.ProfileFileNames.Remove(filename);
        SaveFile<ProfileIndex>(s_indexPath, index);

        if (File.Exists(s_currentProfilePath))
        {
            string current = File.ReadAllText(s_currentProfilePath);
            if (current == filename)
            {
                File.Delete(s_currentProfilePath);
                s_currentProfile = null;
            }
        }
    }

    static T LoadFile<T>(string path)
    {
        using (var profileReader = new StreamReader(path))
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(profileReader);
        }
    }
}
