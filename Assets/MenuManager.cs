using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;


public class MenuManager : MonoBehaviour
{
    public Animator BookAnims;
    public GameObject magiCircle;
    public TMP_Text Language;
    public TMP_Text Difficulty;
    public TMP_Text FlavorText;
    public GameObject LevelSelector;
    public GameObject LeaderboardMenu;
    public GameObject PinLessonsMenu;
    public GameObject LBRecord;
    public GameObject LBParent;
    public GameObject LBEmptyMSG, PLEmptyMSG;
    public DatabaseScript DBs;
    public GameObject PLRecord;
    public GameObject PLParent;
    public GameObject Analytics;



    private string[] Languages = {"Spanish", "Portuguese", "Japanese" };
    private int langIndex=0;
    private string[] Difficulties = { "Vocabulary", "Phrases", "Sayings", "Arcade" };
    private string[] FlavText = { "Easy", "Medium", "Hard", "Endless" };
    private int diffIndex=0;
    public static string selectedlang = "Spanish";
    public static string selecteddiff = "Vocabulary";
 
    [SerializeField] private Camera cam;
    private bool zooming =false;

    public void BookOpen()
    {
        Debug.Log("open");
        BookAnims.SetTrigger("opendabuk");
        Invoke("ShowMenu", 2.3f);
        StartCoroutine(Zoom());        
    }
    IEnumerator Zoom()
    {
        yield return new WaitForSeconds(1.5f);
        zooming = true;
    }

    public void ShowAnalytics()
    {
        Analytics.SetActive(true);
        LevelSelector.SetActive(false);
        PinLessonsMenu.SetActive(false);
        LeaderboardMenu.SetActive(false);
    }

    public void ShowMenu()
    {
        Debug.Log("Exiting to main menu...");
        SceneManager.LoadSceneAsync("Level Selection");

    }
    public void Playbutton() // shows level selection menu
    {
        mcd = -1;
        LevelSelector.SetActive(true);
        PinLessonsMenu.SetActive(false);
        LeaderboardMenu.SetActive(false);
        Analytics.SetActive(false);
        selectedlang = Languages[langIndex];
        selecteddiff = Difficulties[diffIndex];
    }
    List<GameObject> Lessonz = new List<GameObject>();
    public void PLessonsbtn() // shows the pinned lessons
    {
        LevelSelector.SetActive(false);
        PinLessonsMenu.SetActive(true);
        LeaderboardMenu.SetActive(false);
        Analytics.SetActive(false);
        PLEmptyMSG.SetActive(false);

        Transform ggg = PLParent.transform; // changes the size of the box that will be scrolled up and down
        if (ggg.childCount > 1)
        {
            Debug.Log(ggg.childCount.ToString());
            foreach (GameObject go in Lessonz) { Destroy(go); Debug.Log("deleted clone"); }
            Lessonz.Clear();
            Lessonz.TrimExcess();
        }
        RectTransform contTF = PLParent.GetComponent(typeof(RectTransform)) as RectTransform;
        contTF.sizeDelta = new Vector2(contTF.sizeDelta.x, 90);
        float lessonSize = contTF.sizeDelta.y;
        PLRecord.SetActive(false);
        StartCoroutine(DBs.GetPLessonsData(() =>
        {
            if (DatabaseScript.PLempty == false)
            {
                string[] lessons = DatabaseScript.PLlessons;
                string[] translations = DatabaseScript.PLtranslations;
                string[] langs = DatabaseScript.PLlangs;
               
                for (int i = 0; i < lessons.Length; i++)
                {

                    GameObject recs = Instantiate(PLRecord, PLParent.transform);
                    recs.SetActive(true);
                    Lessonz.Add(recs);
                    recs.SetActive(true);
                    string lesson = $"{langs[i]}: {lessons[i]} \r\nEnglish: {translations[i]}";

                    Lessonz[i].GetComponentInChildren<Transform>().Find("LessonText").GetComponent<TMP_Text>().text = lesson; 

                    contTF.sizeDelta = new Vector2(contTF.sizeDelta.x, contTF.sizeDelta.y + lessonSize);
                    Debug.Log("Pinned Lessons loaded successfully!");
                }
            }
            else
            {
                PLEmptyMSG.SetActive(true);
            }

        }));
    }
    public SmallMenuScript SM;
    public TextInputManager textInputManager;

    public void OnLessonClicked(string lang, string question)
    {
        textInputManager.LoadNote(lang, question);
    }
    public void ViewNotesBtn()
    {
        string[] notes = DatabaseScript.PLnotes;
        string[] lang = DatabaseScript.PLlangs;
        string[] lesson = DatabaseScript.PLlessons;
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Debug.Log("Viewing Notes");
        
        Transform lessonItem = clickedButton.transform.parent;

        TMP_Text lessonText = lessonItem.GetComponentInChildren<TMP_Text>();
        for (int i = 0; i < notes.Length; i++) 
        {
            if (lessonText. text.Contains(lesson[i]) && lessonText.text.Contains(lang[i])) 
            {
                //SmallMenuScript.thisNote = notes[i];
                DatabaseScript.currentNoteLesson= lesson[i];
                DatabaseScript.currentNoteLang = lang[i];
                SM.ShowBtn();
                OnLessonClicked(lang[i], lesson[i]);
                Debug.Log($"{lang[i]}: {lesson[i]} ");   
                
                break;
            }
        }
    }
    public void LoaderPL()
    {
        string[] lang = DatabaseScript.PLlangs;
        string[] lesson = DatabaseScript.PLlessons;
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        Transform lessonItem = clickedButton.transform.parent;

        TMP_Text lessonText = lessonItem.GetComponentInChildren<TMP_Text>();
        for (int i = 0; i < lesson.Length; i++)
        {
            if (lessonText.text.Contains(lesson[i]) && lessonText.text.Contains(lang[i]))
            {
                DatabaseScript.currentNoteLesson = lesson[i];
                DatabaseScript.currentNoteLang = lang[i];
                break;
            }
        }
    }

    List<GameObject> records = new List<GameObject>();
    public void Leaderboardsbtn() // shows the leaderboards
    {
        LevelSelector.SetActive(false);
        PinLessonsMenu.SetActive(false);
        LeaderboardMenu.SetActive(true);
        Analytics.SetActive(false);
        LBEmptyMSG.SetActive(false);

        Transform ggg = LBParent.transform; // changes the size of the box that will be scrolled up and down
        if (ggg.childCount > 1  )
        {
            Debug.Log(ggg.childCount.ToString());
            foreach (GameObject go in records) { Destroy(go); Debug.Log("deleted clone"); }
            records.Clear();
            records.TrimExcess();
        }
        RectTransform contTF = LBParent.GetComponent(typeof (RectTransform)) as RectTransform;
        contTF.sizeDelta = new Vector2(contTF.sizeDelta.x, 80);
        LBRecord.SetActive(false);
        Debug.Log("Fetching Data 1");
        StartCoroutine(DBs.GetLeaderboardData(() =>
        {
            if (DatabaseScript.LBempty == false)
            {
                string[] names = DatabaseScript.LBnames;
                string[] scores = DatabaseScript.LBscores;
                string[] langs = DatabaseScript.LBlangs;
                string test = names[0];
                Debug.Log(test);

                for (int i = 0; i < names.Length; i++)
                {

                    GameObject recs = Instantiate(LBRecord, LBParent.transform);
                    recs.SetActive(true);
                    records.Add(recs);
                    recs.SetActive(true);
                    Debug.Log(names[i].ToString());

                    records[i].GetComponentInChildren<Transform>().Find("rank").GetComponent<TMP_Text>().text = (i + 1).ToString(); //ranking
                    records[i].GetComponentInChildren<Transform>().Find("name").GetComponent<TMP_Text>().text = names[i];
                    records[i].GetComponentInChildren<Transform>().Find("lang").GetComponent<TMP_Text>().text = langs[i];
                    records[i].GetComponentInChildren<Transform>().Find("score").GetComponent<TMP_Text>().text = scores[i].ToString(); ;

                    contTF.sizeDelta = new Vector2(contTF.sizeDelta.x, contTF.sizeDelta.y + 40);
                    Debug.Log("Leaderboards loaded successfully!");
                }
            }
            else
            {
                LBEmptyMSG.SetActive(true);
            }
            
        }));
        
    }

    public void ResetLBbtn()
    {
        StartCoroutine(ResetLeaderboard());
    }

    public IEnumerator ResetLeaderboard(System.Action onComplete = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("sqlPost", "TRUNCATE TABLE leaderboards");

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/lingonomicon/InsertData.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log("Leaderboard reset successfully.");
        }

        onComplete?.Invoke();
        yield return UnityWebRequest.Result.Success;
        Leaderboardsbtn();
    }


    public void StartQuiz() 
    {
        Debug.Log($"Quiz started. Language: {selectedlang}, Type: {selecteddiff}");
        SceneManager.LoadSceneAsync("QuizGame"); 
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            float speed = (float) mcd * 50 * Time.deltaTime;
            magiCircle.transform.Rotate(0, 0, speed);
        }
        catch { }

        if (zooming && cam.orthographicSize > 0.17)
        {
            Debug.Log("zooming");
            float vel = 0f;
            float zoom = 0.001f;
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref vel, 0.25f);
        }

    }

    private static int mcd = -1;
    public void LangLeft()//        LANGUAGES
    {
        langIndex--;
        if (langIndex < 0) { langIndex = Languages.Length-1; }
        Language.text = Languages[langIndex];
        selectedlang = Languages[langIndex];
    }
    public void LangRight() 
    {
        langIndex++;
        if (langIndex > Languages.Length-1) { langIndex = 0; }
        Language.text = Languages[langIndex];
        selectedlang = Languages[langIndex];
    }

    public void DLeft()     // DIFFICULTIES
    {
        diffIndex--;
        if (diffIndex < 0) { diffIndex = Difficulties.Length - 1; }
        Difficulty.text = Difficulties[diffIndex];
        FlavorText.text = FlavText[diffIndex];
        selecteddiff = Difficulties[diffIndex];
        mcd = 1 * diffIndex+1;
    }
    public void DRight() //change to traverse through an array
    {
        diffIndex++;
        if (diffIndex > Difficulties.Length - 1) { diffIndex = 0; }
        Difficulty.text = Difficulties[diffIndex];
        FlavorText.text = FlavText[diffIndex];
        selecteddiff = Difficulties[diffIndex];
        mcd = -1 * diffIndex-1;
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }

}


