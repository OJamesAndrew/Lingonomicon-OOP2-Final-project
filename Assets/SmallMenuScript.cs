using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SmallMenuScript : MonoBehaviour
{
    private static bool Quizgame, aysLB, aysPL, unpin, MainMenu;
    [SerializeField] private GameObject SmallMenu;
    [SerializeField] private GameObject ShowButton;
    [SerializeField] private GameObject HideButton;
    [SerializeField] private GameObject ScoreSaver;
    [SerializeField] private TMP_Text Label;
    [SerializeField] private GameObject aysMenu;
    [SerializeField] private TMP_Text aysFlavText;
    [SerializeField] public TMP_InputField currentNote;
    public static string MenuLabel;

    void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "QuizGame")
        {
            Quizgame = true;
            MainMenu = false;
        }
        else
        {
            Quizgame= false;
            MainMenu = true;
        }
    }

    private void Start()
    {
        if(Quizgame)
        {
            SmallMenu.SetActive(true);
            SmallMenu.SetActive(false);
            SmallMenu.SetActive(true);
        }
        if (MainMenu) 
        {
            SmallMenu.SetActive(false);
        }
    }
    public void ShowBtn()
    {
        SmallMenu.SetActive(true);
        if (Quizgame) 
        {
            ShowButton.SetActive(false); //Pause Button

            if (QuizManager.GameFinished== false)  //if game is ongoing
            {
                print("Paused");
                MenuLabel = "Paused";
                ScoreSaver.SetActive(false);
                HideButton.SetActive(true);
            }
            else if (MenuManager.selecteddiff == "Arcade") //game finished and in arcade
            {
                print("Endless Finished");
                MenuLabel = $"Score: {QuizManager.score}";
                HideButton.SetActive(false);
                ScoreSaver.SetActive(true);
            }
            else                //game finished (any difficulty)
            {
                ScoreSaver.SetActive(false);
                HideButton.SetActive(false);
            }
            Label.text = MenuLabel;
            SmallMenu.SetActive(false);
        }
        else if(MainMenu)
        {
            SmallMenu.SetActive(true);
        }
        SmallMenu.SetActive(true);

    }
    public void HideBtn() //Resume and cancel button
    {
        if (Quizgame)
        {
            ShowButton.SetActive(true);
        }
        SmallMenu.SetActive(false);
        Debug.Log("Menu Hidden");

    }

    public void aysHide() //No button. Also called after all functions of Yes button are done
    {
        aysMenu.SetActive(false);
        aysLB = false;
        aysPL = false;
        unpin = false;
    }

    public void aysResetLB()
    {
        aysMenu.SetActive(true);
        aysFlavText.text = "This action cannot be undone!";
        aysLB = true;
    }

    public void aysNoteCancel()
    {
        Debug.Log("Cancel clicked");
        SmallMenu.SetActive(true);
        aysMenu.SetActive(true);
        aysFlavText.text = "Unsaved changes will be discarded!";
        aysPL = true;
        SmallMenu.SetActive(true);
    }

    public void aysUnpin()
    {
        aysMenu.SetActive(true);
        aysFlavText.text = "Unpinning a note cannot be undone!";
        unpin = true;
    }

    public DatabaseScript unpinner;
    public void aysYesBtnClick()
    {
        if (aysLB) 
        { 
            ConfirmResetLB();
            menuManager.Leaderboardsbtn();
            aysHide();
        }
        if (aysPL) 
        { 
            ConfirmDiscardNote();
            aysHide();
            SmallMenu.SetActive(false);
        }
        if (unpin)
        {
            unpinner.unpinLesson();
            menuManager.PLessonsbtn();
            aysHide();
        }
        aysLB=false;
        aysPL=false;
        unpin=false;
    }
    public MenuManager menuManager;
    private void ConfirmResetLB()
    {
        menuManager.ResetLBbtn();
        aysHide();
    }
    private void ConfirmDiscardNote()
    {
        TextInputManager.inputPLesson = TextInputManager.oldNote;
    }

}
