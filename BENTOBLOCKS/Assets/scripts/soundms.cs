using UnityEngine;

public class Soundms : MonoBehaviour
{
 
    public static Soundms Instance { get; private set; }

    [SerializeField] public AudioSource sounddef;
    [SerializeField] public AudioClip transitionwhoosh;

    private void Awake()
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

    public void playsfx(AudioClip clip)
    {
        if (sounddef != null && clip != null)
        {
            sounddef.PlayOneShot(clip);
        }
    }
}