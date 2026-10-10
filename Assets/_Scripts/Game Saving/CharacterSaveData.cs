using System;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData
{
    [Header("Character Name")]
    public string characterName;

    [Header("Time Played")]
    public float secondsPlayed;
    
    // we can only save data from 'basic' variables types (float, int, bool,... )
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;
}
