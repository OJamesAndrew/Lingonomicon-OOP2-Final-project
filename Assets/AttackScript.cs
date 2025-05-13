using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    private Animator AttackAnim;
    public GameObject PlayerAttack;
    public GameObject EnemyAttack;
    public GameObject PlayerShield;
    [SerializeField] Animator ZapAttack;
    [SerializeField] Animator MobAttack;
    [SerializeField] Animator ForceField;
    public SpriteRenderer Enemy1;
    public SpriteRenderer Player1;
    private static bool IsMyTurn=true;
    private static bool correct;
    public static float AnimTime;
    public AudioScript SFXplayer;
    public void TriggerEnemyDamage() //called in Lightning Animator
    {
        if(IsMyTurn==true)
        {
            if(correct ==true){ StartCoroutine(EnemyHurt()); Debug.Log("red"); }
            else { StartCoroutine(EnemyDefend()); Debug.Log("blue"); }
        }
        
    }
    IEnumerator EnemyHurt()
    {
        Color originalColor = Enemy1.color;
        Enemy1.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        Enemy1.color = originalColor;
    }
    IEnumerator EnemyDefend()
    {
        Color originalColor = Enemy1.color;
        Enemy1.color = Color.cyan;
        yield return new WaitForSeconds(0.5f);
        Enemy1.color = originalColor;
    }
    
    public void ShowAttacks()  //called when a button is pressed
    {
        IsMyTurn = QuizManager.IsMyTurn;
        correct = QuizManager.correct;
        Debug.Log($"IsMyTurn: {IsMyTurn.ToString()} | correct: {correct.ToString()}");
        string animName;

        if (IsMyTurn)           //Player turn: Use Lightning 
        {
            PlayerAttack.SetActive(true);
            Debug.Log("Activated");
            AttackAnim = ZapAttack;
            SFXplayer.playZap();
            animName="WizAttack";
        }
        else                    // Enemy Turn: Use Fire
        {
            EnemyAttack.SetActive(true);
            AttackAnim = MobAttack;
            SFXplayer.playFire();
            animName = "firebreath";
            if(correct) 
            { 
                PlayerShield.SetActive(true);       //correct = Show forcefield
                //ForceField.Rebind();
                ForceField.Play("ForceField", 0, 0f);
                Debug.Log("Shielding");
            }
            else
            {
                StartCoroutine(PlayerHurt());
            }
        }
        Debug.Log("Animating...");
        Debug.Log("Current state: " + AttackAnim.GetCurrentAnimatorStateInfo(0).IsName("WizAttack"));
        //AttackAnim.Rebind();
        AttackAnim.Play(animName, 0, 0f);
        Debug.Log($"Animating... {animName}");
        AnimTime = (float)AttackAnim.GetCurrentAnimatorStateInfo(0).length;
        
        StartCoroutine(ToggleAttacks());
    }
    public void TriggerPlayerDamage()
    {
        
    }
    private IEnumerator PlayerHurt()
    {
        yield return new WaitForSeconds(0.2f);
        Color originalColor = Player1.color;
        Player1.color = new Color(1, 0.2f, 0);
        yield return new WaitForSeconds(1.2f);
        Player1.color = originalColor;
    }

    private IEnumerator ToggleAttacks()
    {
        yield return null; // One frame delay
        AnimTime = AttackAnim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(AnimTime);
        PlayerAttack.SetActive(false);
        EnemyAttack.SetActive(false);
        if(!IsMyTurn && correct)
        {
            yield return new WaitForSeconds(ForceField.GetCurrentAnimatorStateInfo(0).length);
            PlayerShield.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerAttack.SetActive(true);
        EnemyAttack.SetActive(true);
        PlayerShield.SetActive(true);

        PlayerAttack.GetComponent<SpriteRenderer>().enabled = false;
        EnemyAttack.GetComponent<SpriteRenderer>().enabled = false;
        PlayerShield.GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(DisableAnims());
    }
    IEnumerator DisableAnims()
    {
        yield return null;
        PlayerAttack.GetComponent<SpriteRenderer>().enabled = true;
        EnemyAttack.GetComponent<SpriteRenderer>().enabled = true;
        PlayerShield.GetComponent<SpriteRenderer>().enabled = true;

        PlayerAttack.SetActive(false);
        EnemyAttack.SetActive(false);
        PlayerShield.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
