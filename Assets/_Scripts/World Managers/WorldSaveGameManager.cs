using System;
using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    [SerializeField] private PlayerManager player;

    [Header("SAVE/ LOAD")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("Current Character Data")]
    public CharacterSlot currentCharacterSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;

    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;
    public CharacterSaveData characterSlot04;
    public CharacterSaveData characterSlot05;
    public CharacterSaveData characterSlot06;
    public CharacterSaveData characterSlot07;
    public CharacterSaveData characterSlot08;


    private void Awake()
    {
        // there can be only one instance at one time
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }

    }

    private void DecideCharacterFileNameBasedOnCharacterSlotBeingUsed()
    {
        switch (currentCharacterSlotBeingUsed)
        {
            case CharacterSlot.CharacterSlot_01:
                saveFileName = "characterSlot_01";
                break;
            case CharacterSlot.CharacterSlot_02:
                saveFileName = "characterSlot_02";
                break;
            case CharacterSlot.CharacterSlot_03:
                saveFileName = "characterSlot_03";
                break;
            case CharacterSlot.CharacterSlot_04:
                saveFileName = "characterSlot_04";
                break;
            case CharacterSlot.CharacterSlot_05:
                saveFileName = "characterSlot_05";
                break;
            case CharacterSlot.CharacterSlot_06:
                saveFileName = "characterSlot_06";
                break;
            case CharacterSlot.CharacterSlot_07:
                saveFileName = "characterSlot_07";
                break;
            case CharacterSlot.CharacterSlot_08:
                saveFileName = "characterSlot_08";
                break;
            default:
                break;
        }
    }

    public void CreateNewGame()
    {
        // Create a new file with a file name depending on which slot we are using
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();

        currentCharacterData = new CharacterSaveData();
    }

    public void LoadGame()
    {
        // load a previous file with a file name depending on which slot we are using 
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();

        saveFileDataWriter = new SaveFileDataWriter();

        // generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());

    }

    public void SaveGame()
    {
        // save the current file under a file name depending on which slot we're using 
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();
        
        saveFileDataWriter = new SaveFileDataWriter();
        // generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;


        // pass the player info from game to their save file
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);
        
        // write that info onto a json file, saved to this machine
        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
        
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }

}

