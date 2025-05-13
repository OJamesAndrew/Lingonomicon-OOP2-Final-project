using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioScript : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip BookSong;  //  "World of Magic" credits to Scott Buckley
    public AudioClip MainSong;  //  "Village Consort" credits to Kevin Macleod
    public AudioClip QuizSong;  // "Achaidh Cheide" credits to Kevin Macleod
    public AudioClip Zapper;    // Electric Sound Effect by freesound_community from Pixabay
    public AudioClip Flamer;    // Fire Sound Effect by floraphonic from Pixabay

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Main Menu")
        {
            musicSource.clip = BookSong;
            musicSource.volume = 0.5f;
            musicSource.loop = true;
            musicSource.Play();
        }
        else if (SceneManager.GetActiveScene().name == "Level Selection")
        {
            musicSource.clip = MainSong;
            musicSource.volume = 0.2f;
            musicSource.loop = true;
            musicSource.Play();
        }
        else if (SceneManager.GetActiveScene().name == "QuizGame")
        {
            musicSource.clip = QuizSong;
            musicSource.volume = 0.5f;
            musicSource.loop = true;
            musicSource.Play();
        }

    }

    public void playZap()
    {
        SFXSource.clip = Zapper;
        SFXSource.PlayOneShot(Zapper);
    }
    public void playFire()
    {
        SFXSource.clip = Flamer;
        SFXSource.PlayOneShot(Flamer);
    }
}
