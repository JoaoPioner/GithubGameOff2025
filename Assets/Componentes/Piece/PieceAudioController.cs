using UnityEngine;

public class PieceAudioController : MonoBehaviour
{
    [Header("Audio Configuration")]
    public PieceAudioData audioData;

    [Header("Set directly")]
    public string attackSoundName = "Attack";
    public string deathSoundName = "Death";
    public string hitSoundName = "Hit";
    public string moveSoundName = "Move";
    public string spawnSoundName = "Spawn";

    void Start()
    {
        PlaySpawnSound();
    }

    public void PieceAudio_PlayAttack()
    {
        PlayAttackSound();
    }

    public void PieceAudio_PlayDeath()
    {
        PlayDeathSound();
    }

    public void PlayAttackSound()
    {
        Debug.Log($"[{gameObject.name}] PlayAttackSound() foi chamado!");
        string soundName = audioData != null ? audioData.attackSound : attackSoundName;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(soundName);
        }
    }

    public void PlayDeathSound()
    {
        string soundName = audioData != null ? audioData.deathSound : deathSoundName;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(soundName);
        }
    }

    /*
    public void PlayHitSound()
    {
        string soundName = audioData != null ? audioData.hitSound : hitSoundName;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(soundName);
        }
    }
    

    public void PlayMoveSound()
    {
        string soundName = audioData != null ? audioData.moveSound : moveSoundName;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(soundName);
        }
    }
    */

    public void PlaySpawnSound()
    {
        string soundName = audioData != null ? audioData.spawnSound : spawnSoundName;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(soundName);
        }
    }

    // Método genérico para tocar qualquer som
    public void PlayCustomSound(string soundName)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(soundName);
        }
    }
}
