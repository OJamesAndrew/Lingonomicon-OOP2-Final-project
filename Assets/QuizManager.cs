using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using static qtsData;
using System.Collections.Generic;
using UnityEngine.Networking;

public class QuizManager : MonoBehaviour
{
    public HealthBar playerHealthBar;
    public HealthBar enemyHealthBar;

    public TMP_Text questionText;
    public TMP_Text scoreText;
    public Button[] choices = new Button[4];
    public qtsData qtsdata;
    
    public SpriteRenderer EnemyBlack;
    public SpriteRenderer PlayerBlack;
    public Animator PlayerIdle;
    public Animator EnemyIdle;

    public AttackScript Attack;

    private static Player player1;
    private static Enemy enemy1;


    public static bool GameFinished = false;
    public static bool IsMyTurn = true;
    public static bool correct;

    private int currentQues = 0;
    public static int score = 0;

    [System.Serializable]
    public struct QuizEntry
    {
        public string question;
        public string eng_ans;
        public string fake_ans1;
        public string fake_ans2;
        public string fake_ans3;
    }
    [System.Serializable]
    public class QuizEntryList
    {
        public QuizEntry[] entries;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        //HEALTH SYSTEM SETUP
        InitializeStats(MenuManager.selecteddiff);
        //SQL SETUP
        string sqlquery;
        questionText.text = "localhost is off";
        GameFinished = false;
        
        score = 0;
        if (MenuManager.selecteddiff != "Arcade")
        {
            sqlquery = $"SELECT question, eng_ans, fake_ans1, fake_ans2, fake_ans3" +
                        $" FROM languagedata WHERE lang = '{MenuManager.selectedlang}' AND category = '{MenuManager.selecteddiff}'" +
                        $"ORDER BY RAND() LIMIT 20;";
        }
        else
        {
            sqlquery = $"SELECT question, eng_ans, fake_ans1, fake_ans2, fake_ans3" +
                       $" FROM languagedata WHERE lang = '{MenuManager.selectedlang}' ORDER BY RAND();";
        }

        WWWForm form = new WWWForm();
        form.AddField("sqlPost", sqlquery);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/lingonomicon/ReadData.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            string json = "{\"entries\":" + www.downloadHandler.text + "}";
            Debug.Log("Raw JSON: " + json);
            QuizEntryList entryList = JsonUtility.FromJson<QuizEntryList>(json);
            if (entryList == null || entryList.entries == null)
            {
                Debug.LogError("Parsed JSON is null or missing 'entries' array.");
                yield break;
            }
            Question[] result = new Question[entryList.entries.Length];

            for(int i = 0; i < entryList.entries.Length; i++)
            {
                QuizEntry entry = entryList.entries[i];
                Question ques;
                string[] choices = new string[] { entry.eng_ans, entry.fake_ans1, entry.fake_ans2, entry.fake_ans3 };
                List<string> shuffled = new List<string>(choices);

                for (int j = 0; j < shuffled.Count; j++)
                {
                    string temp = shuffled[j];
                    int randIndex = Random.Range(j, shuffled.Count);
                    shuffled[j] = shuffled[randIndex];
                    shuffled[randIndex] = temp;
                }
                ques.questionText = entry.question;
                ques.choices = shuffled.ToArray();
                ques.correctChoiceIndex = shuffled.IndexOf(entry.eng_ans);

                result[i] = ques;
            }
            qtsdata.questions = result;
            IsMyTurn = true;
            SetQuestions(currentQues);
            Debug.Log("Loaded Succesfully");
        }
    }

    public static float playerHealth;
    public static float enemyHealth;
    public static float playerDamage = 1.2f;
    public static float enemyDamage;
    public static bool isEndless;
    private float pD, eD;
    public void InitializeStats(string difficulty)
    {
        isEndless = false;

        switch (difficulty)
        {
            case "Vocabulary": // Easy
                playerHealth = 12f;
                enemyHealth = 12f;
                enemyDamage = 1f;
                break;

            case "Phrases": // Medium
                playerHealth = 12f;
                enemyHealth = 15f;
                enemyDamage = 1.2f;
                break;

            case "Sayings": // Hard
                playerHealth = 12f;
                enemyHealth = 18f;
                enemyDamage = 1.4f;
                break;

            case "Arcade": // Endless
                playerHealth = 25f;
                enemyHealth = float.PositiveInfinity;
                enemyDamage = 1f;
                isEndless = true;
                break;

            default:
                Debug.LogWarning("Unknown difficulty. Defaulting to Easy.");
                playerHealth = 12f;
                enemyHealth = 12f;
                enemyDamage = 1f;
                break;
        }
        player1 = new Player(playerHealth);
        enemy1 = new Enemy(enemyHealth);
        playerHealthBar.SetMaxHealth(player1.health);
        enemyHealthBar.SetMaxHealth(enemy1.health);
        pD = playerDamage;
        eD = enemyDamage;
        Debug.Log($"Player HP: {playerHealth}, Enemy HP: {enemyHealth}, Enemy Damage: {enemyDamage}");
    }
    void SetQuestions(int questionIndex)
    {
        questionText.text = qtsdata.questions[questionIndex].questionText;
        ShowQues();

        foreach (Button r in choices) 
        { 
        r.onClick.RemoveAllListeners();
        }

        for (int i =0; i<choices.Length; i++)
        {
            choices[i].GetComponentInChildren<TMP_Text>().text = qtsdata.questions[questionIndex].choices[i];
            int choiceIndex = i;
            choices[i].onClick.AddListener(() => { 
                CheckAns(choiceIndex);
                StartCoroutine(ShowCorrectAns(questionIndex));
            
            });
            
            
        }

    }
    IEnumerator ShowCorrectAns(int questionIndex)
    {   
        TMP_Text correctChoice = choices[qtsdata.questions[questionIndex].correctChoiceIndex].GetComponentInChildren<TMP_Text>();
        Color originalColor = correctChoice.color;
        correctChoice.color = Color.green;
        yield return new WaitForSeconds(AttackScript.AnimTime);
        correctChoice.color = originalColor;
    }

    void CheckAns(int answerIndex)
    {
        
        if (answerIndex == qtsdata.questions[currentQues].correctChoiceIndex)
        {
            score++;
            scoreText.text = "Score: " + score; 
            correct = true;
            foreach (Button r in choices) { r.interactable =  false; }
            if (IsMyTurn)   
            {
                enemy1.TakeDamage(playerDamage);
                Debug.Log($"Enemy Health: {enemy1.health}");
                enemyHealthBar.SetHealth(enemy1.health);
            }
        }
        else
        {
            correct = false;
            foreach (Button r in choices) { r.interactable = false; }
            if (!IsMyTurn)
            { 
                player1.TakeDamage(enemyDamage);
                playerHealthBar.SetHealth(player1.health);
            }
        }
        Attack.ShowAttacks();
        StartCoroutine(Next());
        if (IsMyTurn) { IsMyTurn = false; }
        else { IsMyTurn=true; }

    }

    public SmallMenuScript SmallMenu;

    IEnumerator Next()
    {
        yield return new WaitForSeconds(AttackScript.AnimTime);

        // Check if player or enemy has died
        if (player1.health <= 0f)
        {
            SmallMenuScript.MenuLabel = "Defeat!";
            PlayerBlack.color = Color.black;
            PlayerIdle.speed = 0f;
            GameFinished = true;
            SmallMenu.ShowBtn();
            yield break; 
        }
        else if (enemy1.health <= 0f)
        {
            SmallMenuScript.MenuLabel = "Victory!";
            EnemyBlack.color = Color.black;
            EnemyIdle.speed = 0f;
            GameFinished = true;
            SmallMenu.ShowBtn();
            yield break; 
        }

        // Loop questions
        currentQues++;
        if (currentQues >= qtsdata.questions.Length)
        {
            ;
            ShuffleQuestionsAndAnswers(qtsdata.questions, qtsdata.questions.Length - 1);
            currentQues = 0;
            
        }

        Reset(); // Load next question
    }
    void ShowQues()
    {
        Transform imageChild = questionText.transform.GetChild(0);
        if (imageChild != null && imageChild.TryGetComponent<Image>(out Image _))
        {
            foreach (Transform child in imageChild)
            {
                if (child.GetComponent<TextMeshProUGUI>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            TMP_Text clonedText = Instantiate(questionText, imageChild);
            clonedText.text = questionText.text;
            Destroy(clonedText.GetComponentInChildren<Image>());
            clonedText.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("Child Image component not found.");
        }
    }

    void ShuffleQuestionsAndAnswers(Question[] questions , int lastIndex)
    {
        string lastQ = questions[lastIndex].questionText;

        for (int i = 0; i < questions.Length; i++)
        {
            int randIndex = Random.Range(i, questions.Length);
            (questions[i], questions[randIndex]) = (questions[randIndex], questions[i]);
        }

        if (questions[0].questionText == lastQ)
        {
            (questions[0], questions[1]) = (questions[1], questions[0]);
        }

        for (int q = 0; q < questions.Length; q++)
        {
            var question = questions[q];
            string correctAnswer = question.choices[question.correctChoiceIndex];

            for (int i = 0; i < question.choices.Length; i++)
            {
                int randIndex = Random.Range(i, question.choices.Length);
                (question.choices[i], question.choices[randIndex]) = (question.choices[randIndex], question.choices[i]);
            }

            for (int i = 0; i < question.choices.Length; i++)
            {
                if (question.choices[i] == correctAnswer)
                {
                    question.correctChoiceIndex = i;
                    break;
                }
            }
            questions[q] = question; 
        }

    }

    public void Reset()
    {
        foreach (Button r in choices)
        {
            r.interactable = true;
        }
        SetQuestions(currentQues);
    }
    
    // Update is called once per frame
    void Update()
    {
        
        //playerDamage = enemyDamage = Test;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerDamage = enemyDamage = 100;
            Debug.Log("Do or Die");
            print($"Dmg: {playerDamage} vs {enemyDamage}");
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerDamage = 1.2f;
            enemyDamage = eD;
            Debug.Log("Normal");
            print($"Dmg: {playerDamage} vs {enemyDamage}");
        }
    }
    public float Test = 0;
}
public class Character
{
    public float health;

    public Character(float health)
    {
        this.health = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0.01f) health = 0f; 
    }
}

public class Player : Character
{
    public Player(float health) : base(health) { }
}

public class Enemy : Character
{
    public Enemy(float health) : base(health) { }
}


