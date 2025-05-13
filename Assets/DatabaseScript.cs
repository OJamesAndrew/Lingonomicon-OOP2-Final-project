using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;



public class DatabaseScript : MonoBehaviour
{
    //string InsertphpURL = "http://localhost/lingonomicon/InsertData.php";
    public GameObject SaveScorebtn;
    public static string phpEcho = "Test";
    public static string[] LBnames, LBscores, LBlangs;
    public static string[] PLlessons, PLlangs, PLtranslations, PLnotes;
    public static bool LBempty, PLempty;
    public TMP_InputField textbox;
    public static string currentNoteLang, currentNoteLesson;
    IEnumerator SendData(string query)
    {
        Debug.Log("Saving...");
        WWWForm form = new WWWForm();
        form.AddField("sqlPost", query);
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/lingonomicon/InsertData.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error sending data: " + www.error);
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text);
        }
    }
    
    public void addLeaderBoard()
    {
        string name = TextInputManager.inputLead;        
        string score = QuizManager.score.ToString();
        string lang = MenuManager.selectedlang;

        if(name.Trim()=="" || name == null) { name = "nameless"; }
        string sqlQuery = $"INSERT INTO leaderboards (name, score, lang) VALUES ('{name}', '{score}', '{lang}')";

        StartCoroutine(SendData(sqlQuery)); 

        SaveScorebtn.SetActive(false);
        Debug.Log("Saved to Database: " + score);
    }

    [SerializeField] private TMP_Text questionText;
    public void pinLesson()
    {
        string question = questionText.text;
        string lang = MenuManager.selectedlang;
        string sqlQuery = $"INSERT INTO pinnedlessons (languagedata_id) SELECT id FROM languagedata WHERE question = '{question}' AND lang = '{lang}';";

        StartCoroutine(SendData(sqlQuery)); 
    }

    public void unpinLesson()
    {
        string lang = currentNoteLang;
        string lesson = currentNoteLesson;
        string sqlQuery = $"DELETE FROM pinnedlessons WHERE languagedata_id = (SELECT id FROM languagedata WHERE question = '{lesson}' AND lang = '{lang}' LIMIT 1)";
        StartCoroutine(SendData(sqlQuery));
        Debug.Log($"Deleted pinned lesson: {lesson} (Spanish)");
    }

    public void SaveNotes()
    {
        string note = TextInputManager.inputPLesson;
        string lang = currentNoteLang;
        string lesson = currentNoteLesson;
        string sqlQuery = $"UPDATE pinnedlessons SET notes = '{note}' WHERE languagedata_id = ( SELECT id FROM languagedata WHERE lang = '{lang}' AND question = '{lesson}');";

        StartCoroutine(SendData(sqlQuery));
        Debug.Log($"New Note for '{lesson}' ({lang}): {note}");

    }

    [System.Serializable]
    public class PlayerEntryList
    {
        public PlayerEntry[] entries;
    }
    [System.Serializable]
    public class PlayerEntry
    {
        public string name;
        public string score;
        public string lang;
    }

    public IEnumerator GetLeaderboardData(System.Action onComplete = null)
    {
        Debug.Log("Fetching Data 2");
        WWWForm form = new WWWForm();
        form.AddField("sqlPost", "SELECT * FROM leaderboards ORDER BY score DESC");
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/lingonomicon/ReadData.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            
        }
        else
        {
            string rawJson = www.downloadHandler.text;

            if (rawJson == "[]" || string.IsNullOrWhiteSpace(rawJson))
            {
                Debug.Log("Leaderboard is empty.");
                LBempty = true;
            }
            else
            {
                LBempty = false;
                string wrappedJson = "{\"entries\":" + rawJson + "}";
                Debug.Log(wrappedJson);
                PlayerEntryList list = JsonUtility.FromJson<PlayerEntryList>(wrappedJson);

                // Convert to string arrays
                string[] names = new string[list.entries.Length];
                string[] scores = new string[list.entries.Length];
                string[] langs = new string[list.entries.Length];

                for (int i = 0; i < list.entries.Length; i++)
                {
                    names[i] = list.entries[i].name;
                    Debug.Log(names[i]);
                    scores[i] = list.entries[i].score;
                    langs[i] = list.entries[i].lang;
                }

                LBnames = names;
                LBscores = scores;
                LBlangs = langs;
                string testt = (names.Length - 1).ToString();
                Debug.Log(LBnames[names.Length - 1] + " " + testt);
            }
            
        }
        onComplete?.Invoke();
    }
    //string q = "SELECT l.lang, l.question, l.eng_ans FROM pinnedlessons p INNER JOIN languagedata l ON p.languagedata_id = l.id";


    [System.Serializable]
    public class PinnedLessonList
    {
        public LessonEntry[] entries;
    }
    [System.Serializable]
    public class LessonEntry
    {
        public string question;
        public string eng_ans;
        public string lang;
        public string notes;
    }
    public IEnumerator GetPLessonsData(System.Action onComplete = null)
    {
        Debug.Log("Fetching Data 2");
        WWWForm form = new WWWForm();
        string qry = "SELECT l.lang, l.question, l.eng_ans, p.notes FROM pinnedlessons p INNER JOIN languagedata l ON p.languagedata_id = l.id";
        form.AddField("sqlPost", qry);
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/lingonomicon/ReadData.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);

        }
        else
        {
            string rawJson = www.downloadHandler.text;
            Debug.Log(rawJson);
            if (rawJson == "[]" || string.IsNullOrWhiteSpace(rawJson))
            {
                Debug.Log("Leaderboard is empty.");
                PLempty = true;
            }
            else
            {
                PLempty = false;
                string wrappedJson = "{\"entries\":" + rawJson + "}";

                Debug.Log(wrappedJson);
                PinnedLessonList list = JsonUtility.FromJson<PinnedLessonList>(wrappedJson);

                // Convert to string arrays
                string[] lessons = new string[list.entries.Length];
                string[] translations = new string[list.entries.Length];
                string[] langs = new string[list.entries.Length];
                string[] notes = new string[list.entries.Length];

                for (int i = 0; i < list.entries.Length; i++)
                {
                    lessons[i] = list.entries[i].question;
                    translations[i] = list.entries[i].eng_ans;
                    langs[i] = list.entries[i].lang;
                    notes[i] = list.entries[i].notes;
                    Debug.Log("Loaded: " + lessons[i]);
                }

                PLlangs = langs;
                PLlessons = lessons;
                PLtranslations = translations;
                PLnotes = notes;
                
            }

        }
        onComplete?.Invoke();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
