using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public event Action<Piece> OnDeath;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private PieceAudioController audioController;

    [Header("Stats")]
    [SerializeField]
    public float HP = 100f;
    [SerializeField]
    public float currentHP;
    [SerializeField]
    public bool isUnarmed = false;
    [SerializeField]
    public bool isRanged = false;
    [SerializeField]
    public float attackPower = 10f;

    [Header("UI")]
    public Image healthBar;

    void Awake()
    {
        audioController = GetComponent<PieceAudioController>();
    }

    void Start()
    {
        currentHP = HP;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameEnded += EndGameHandler;
        }

        if (audioController != null)
        {
            audioController.PlaySpawnSound();
        }
    }

    void Update()
    {
        if (animator == null || navMeshAgent == null)
            return;

        float speedRatio = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
        animator.SetFloat("Speed", speedRatio);
    }

    public void Damage(float hits)
    {
        currentHP -= hits;
        AudioManager.instance.PlaySFX("Hit", 0.15f);
        if (currentHP <= 0)
        {
            var tower = GetComponent<Tower>();
            if (tower != null)
            {
                AudioManager.instance.PlaySFX("TowerDestroyed");
                GameStateManager.Instance.VPDestroyed();
            }
            else
            {

                if (animator != null)
                {
                    
                    animator.SetInteger("DeathIndex", UnityEngine.Random.Range(0, 2));
                    animator.SetTrigger("Die");
                }
            }

            if (gameObject.tag == "HUMANS")
            {
                AudioManager.instance.PlaySFX("HumanDeath");
            }
            else
            {
                AudioManager.instance.PlaySFX("EnemyDeath");
            }

            Debug.Log(gameObject.name + " foi destruído.");
            OnDeath?.Invoke(this);

            Destroy(gameObject);
        }
        else
        {

            if (animator != null)
            {
                animator.SetInteger("HitIndex", UnityEngine.Random.Range(0, 2));
                animator.SetTrigger("GetHit");
            }

            currentHP = Mathf.Clamp(currentHP, 0, HP);
            healthBar.fillAmount = (float)currentHP / HP;
        }
    }

    public void Attack(Piece targetPiece)
    {

        if (animator != null)
        {
            animator.SetInteger("AttackIdx", isUnarmed ? UnityEngine.Random.Range(5, 7) : UnityEngine.Random.Range(0, 5));
            animator.SetBool("IsUnarmed", isUnarmed);
            animator.SetTrigger("Attack");
        }

        targetPiece.Damage(attackPower);
    }

    private void EndGameHandler(bool playerWon)
    {
        if (animator != null)
        {
            if (playerWon)
                animator.SetTrigger("Victory");
            else
                animator.SetTrigger("Defeat");
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }
    }

    void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameEnded -= EndGameHandler;
        }
    }
}