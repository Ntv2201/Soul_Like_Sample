using UnityEngine;
using System.IO;
using System;

public class SaveFileDataWriter : MonoBehaviour
{
    public string saveDataDirectoryPath = "";
    public string saveFileName = "";

    // before we create a new file save, we must check to see if one of this character slot already exists
    public bool CheckToSeeIfFileExists()
    {
        if(File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // used to delete character save files
    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    // used to create a save file upon startin a new game
    public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
    {
        // make a path to save file
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try
        {
            // create the directory the file will be written to
            // if it does not already exists
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Creating save file, at save path" + savePath);

            // serialize the c# game data object ino json
            string dataToStore = JsonUtility.ToJson(characterData, true);

            // write the file to our system
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(dataToStore);
                }
            }   
        }
        catch (Exception ex)
        {
            Debug.LogError("Error whilst trying to save data, game not saved" + savePath + "\n" + ex);
        }
    }

    // used to load a save file
    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterData = null;

        // male a path to load the file (A location on the machine)
        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                // Deserialize the data from json back to Unity
                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch (Exception ex)
            {
                // 
            }
        }
        return characterData;
    }

}


