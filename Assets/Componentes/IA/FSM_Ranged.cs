using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Piece))]
public class FSM_Ranged : FSM
{
    public Transform firePoint; // Ponto de onde os projéteis são disparados
    public GameObject projectilePrefab; // Prefab do projétil

    [SerializeField]
    public bool isWizard = false;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        { animator.SetBool("IsWizard", isWizard); }
    }

    protected override void Attacking()
    {
        if (CheckLostTarget())
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > range)
        {
            ChangeState(AIState.MOVING);
            return;
        }

        agent.isStopped = true;
        transform.LookAt(target.position);

        if (Time.time > cooldown + (1f / attackSpd))
        {
            cooldown = Time.time;

            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            if (gameObject.tag == "HUMANS") 
            {
                AudioManager.instance.PlaySFX("ArrowShot2");
            } else
            {
                AudioManager.instance.PlaySFX("ArrowShot");
            }

            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (animator != null)
            { animator.SetTrigger("Attack"); }
            if (projectile != null)
            {
                projectile.Seek(target);
                projectile.damage = myPiece.attackPower;
            }
        }

    }
    void OnDrawGizmosSelected() // Desenha o alcance da peça na cena
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

