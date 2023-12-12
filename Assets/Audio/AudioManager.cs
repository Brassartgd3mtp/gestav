using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioScriptable audioScriptable; // Référence de la base de donnée

    private void Awake() // Initialisation du Singleton
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(int clipIndex, AudioSource audioSource)
    {
        if (audioScriptable != null)
        {
            AudioClip clip = audioScriptable.GetAudioClip(clipIndex);

            if (audioSource != null && clip != null)
            {
                audioSource.clip = clip;
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning("N'as pas pu jouer le son, Checker l'AudioScriptable et l'AudioSource");
            }
        }
    }
}




