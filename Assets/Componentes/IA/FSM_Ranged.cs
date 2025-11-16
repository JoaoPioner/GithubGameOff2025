using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Piece))]
public class FSM_Ranged : MonoBehaviour
{
    public enum AIState
    {
        SEARCHING,
        MOVING,
        ATTACKING
    }

    [Header("Configurações de Estado")]
    public AIState actualState;

    [Header("Configurações de Movimento")]
    private NavMeshAgent agent; // O componente que move a tropa (o "GPS")

    [Header("Configuração de Alvo")]
    public Transform target = null;
    public string targetTag = "Enemy";

    [Header("Configurações de Ataque")]
    public float range = 2f;
    public float attackSpd = 1f;
    public float cooldown = 2f;
    public float attackDamage = 10f;
    public Transform firePoint; // Ponto de onde os projéteis são disparados
    public GameObject projectilePrefab; // Prefab do projétil

    private Piece myPiece;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myPiece = GetComponent<Piece>();
        actualState = AIState.SEARCHING;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(name + " está no estado " + actualState);
        // Debug.Log("Alvo atual: " + (target == null ? "Nenhum" : target.name));
        switch (actualState)
        {
            case AIState.SEARCHING:
                Searching();
                break;
            case AIState.MOVING:
                Moving();
                break;
            case AIState.ATTACKING:
                Attacking();
                break;
        }
    }
    private void ChangeState(AIState newState)
    {
        // Debug.Log(name + " mudou do estado " + actualState + " para " + newState);
        actualState = newState;

        if (newState == AIState.MOVING)
        {
            agent.isStopped = false;
        }
    }

    private void Searching()
    {
        if (target != null)
        {
            ChangeState(AIState.MOVING);
            return;
        }
        else
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);
            // Debug.Log(enemies.Length + " inimigos encontrados.");

            float menorDistancia = Mathf.Infinity;
            GameObject closestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                // Ignora enemys que já estão mortos/inativos
                if (!enemy.activeInHierarchy) continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                // Se este enemy está mais perto que o último que checamos...
                if (distance < menorDistancia)
                {
                    menorDistancia = distance;
                    closestEnemy = enemy;
                }
            }

            if (closestEnemy != null)
            {
                target = closestEnemy.transform;
                ChangeState(AIState.MOVING);
                return;
            }
            else
            {
                target = null;
                return;
            }

        }
    }

    private void Moving()
    {
        if (target == null)
        {
            ChangeState(AIState.SEARCHING);
            return;
        }
        else if (!target.gameObject.activeInHierarchy)
        {
            target = null;
            ChangeState(AIState.SEARCHING);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= range)
        {
            agent.isStopped = true;
            ChangeState(AIState.ATTACKING);
            return;
        }
        else
        {
            agent.SetDestination(target.position);
        }
    }

    private void Attacking()
    {
        if (target == null)
        {
            ChangeState(AIState.SEARCHING);
            return;
        }

        if (!target.gameObject.activeInHierarchy)
        {
            target = null;
            ChangeState(AIState.SEARCHING);
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
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Seek(target);
                projectile.damage = attackDamage;
            }
        }

    }
    void OnDrawGizmosSelected() // Desenha o alcance da peça na cena
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

