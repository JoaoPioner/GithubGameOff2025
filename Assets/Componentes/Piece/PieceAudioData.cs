using UnityEngine;

[CreateAssetMenu(fileName = "PieceAudio", menuName = "Audio/Piece Audio Data")]
public class PieceAudioData : ScriptableObject
{
    [Header("SFX Names")]
    public string attackSound = "Attack";
    public string deathSound = "Death";
    public string hitSound = "Hit";
    public string moveSound = "Move";
    public string spawnSound = "Spawn";
}