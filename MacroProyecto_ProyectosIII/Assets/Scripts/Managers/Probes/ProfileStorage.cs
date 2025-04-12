using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using UnityEditorInternal;

public static class ProfileStorage
{
    public static ProfileData s_currentProfile;
    public static void CreateNewGame(string profileName)
    {
        s_currentProfile = new ProfileData(profileName, true, 0, 0);

        string path = Application.streamingAssetsPath + "/Profiles/" + s_currentProfile.fileName;
        SaveFile<ProfileData>(path, s_currentProfile);
    }

    static void SaveFile<T>(string path, T data)
    {
        var profileWriter = new StreamWriter(path);
        var profileSerializer = new XmlSerializer(typeof(T));
        profileSerializer.Serialize(profileWriter, data);
        profileWriter.Dispose();
    }

    static T LoadFile<T>(string path)
    {
        var profileReader = new StreamReader(path);
        var serializer = new XmlSerializer(typeof(T));
        var obj = (T)serializer.Deserialize(profileReader);
        profileReader.Dispose();

        return obj;
    }
}
