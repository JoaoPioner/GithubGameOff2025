using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
  public event Action<Piece> OnDeath;

  private Animator animator;
  private NavMeshAgent navMeshAgent;

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

  public Image healthBar;

  void Start()
  {
    currentHP = HP;
    animator = GetComponent<Animator>();
    navMeshAgent = GetComponent<NavMeshAgent>();
    if (GameStateManager.Instance != null)
    {
      GameStateManager.Instance.OnGameEnded += EndGameHandler;
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

    if (currentHP <= 0)
    {
      var tower = GetComponent<Tower>();
      if (tower != null)
      {
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
}
