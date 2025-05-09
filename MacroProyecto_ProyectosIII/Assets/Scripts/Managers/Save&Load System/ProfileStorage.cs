using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using UnityEditorInternal;

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

    public static void LoadProfile(string filename)
    {
        string path = Path.Combine(s_profilesDir, filename);
        s_currentProfile = LoadFile<ProfileData>(path);
    }

    public static void StorePlayerProfile(GameObject player)
    {
        if (!Directory.Exists(s_profilesDir))
        {
            Directory.CreateDirectory(s_profilesDir);
        }

        s_currentProfile.x = player.transform.position.x;
        s_currentProfile.y = player.transform.position.y;
        s_currentProfile.newGame = false;

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
