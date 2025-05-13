using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TextInputManager : MonoBehaviour
{
    private static bool Quizgame, MainMenu;
    public static string inputLead, inputPLesson, oldNote;

    private string currentLang;
    private string currentLesson;
    [SerializeField] private TMP_InputField noteInputField;
    [SerializeField] private DatabaseScript dbScript;


    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "QuizGame")
        {
            Quizgame = true;
            MainMenu = false;
        }
        else
        {
            MainMenu = true;
            Quizgame = false;

        }
    }
    public void grabText(string input)
    {
        Debug.Log($"Written: {input}");
        if (Quizgame)
        {
            inputLead = input;
        }
        else if (MainMenu)
        {
            inputPLesson = input;
        }

    }

    public void LoadNote(string lang, string lesson)
    {
        currentLang = lang;
        currentLesson = lesson;

        string query = $"SELECT notes FROM pinnedlessons WHERE languagedata_id = (SELECT id FROM languagedata WHERE lang = '{lang}' AND question = '{lesson}');";
        StartCoroutine(LoadNoteFromDB(query));
    }

    private IEnumerator LoadNoteFromDB(string query)
    {
        WWWForm form = new WWWForm();
        form.AddField("sqlPost", query);
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/lingonomicon/ReadData.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string rawJson = www.downloadHandler.text;
            if (!string.IsNullOrWhiteSpace(rawJson) && rawJson != "[]")
            {
                string wrappedJson = "{\"entries\":" + rawJson + "}";
                NoteEntryList result = JsonUtility.FromJson<NoteEntryList>(wrappedJson);
                noteInputField.text = result.entries[0].notes;
            }
            else
            {
                noteInputField.text = ""; // or show a placeholder
            }
        }
        else
        {
            Debug.LogError("Failed to load note: " + www.error);
        }
    }

    [System.Serializable]
    public class NoteEntry
    {
        public string notes;
    }

    [System.Serializable]
    public class NoteEntryList
    {
        public NoteEntry[] entries;
    }

    public void SaveNote()
    {
        string newNote = noteInputField.text;
        string query = $"UPDATE pinnedlessons SET notes = '{newNote}' WHERE languagedata_id = (SELECT id FROM languagedata WHERE lang = '{currentLang}' AND question = '{currentLesson}');";
        dbScript.StartCoroutine("SendData", query);
        Debug.Log($"New Note for '{currentLesson}' ({currentLang}): {newNote}");

    }

}





