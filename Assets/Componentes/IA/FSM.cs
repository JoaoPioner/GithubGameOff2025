using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Piece))]
public class FSM : MonoBehaviour
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
    protected NavMeshAgent agent;

    [Header("Configuração de Alvo")]
    public Transform target = null;
    public string targetTag = "Enemy";

    [Header("Configurações de Busca (Reavaliação)")]
    // NOVO: Define de quanto em quanto tempo ele procura novos alvos enquanto anda
    public float searchInterval = 0.5f;
    private float lastSearchTime = 0f;

    [Header("Configurações de Ataque")]
    public float range = 2f;
    public float attackSpd = 1f;
    public float cooldown = 2f;

    [SerializeField]
    public bool isFoe = false;

    protected Piece myPiece;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myPiece = GetComponent<Piece>();
        actualState = AIState.SEARCHING;

        // Garante que a busca inicial aconteça imediatamente
        lastSearchTime = -searchInterval;
    }

    protected void Update()
    {
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

    protected void ChangeState(AIState newState)
    {
        actualState = newState;

        if (newState == AIState.MOVING)
        {
            agent.isStopped = false;
        }
    }

    // --- MUDANÇA 1: Lógica de encontrar alvo extraída para uma função auxiliar ---
    // Isso permite que a gente reuse essa lógica tanto no SEARCHING quanto no MOVING
    protected Transform FindClosestTarget()
    {
        GameObject[] enemies;

        if (isFoe && targetTag != "TOWER")
        {
            enemies = GameObject.FindGameObjectsWithTag(targetTag).Concat(GameObject.FindGameObjectsWithTag("TOWER")).ToArray();
        }
        else
        {
            enemies = GameObject.FindGameObjectsWithTag(targetTag);
        }

        float menorDistancia = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if (!enemy.activeInHierarchy) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < menorDistancia)
            {
                menorDistancia = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy != null ? closestEnemy.transform : null;
    }

    protected virtual void Searching()
    {
        // Se já temos um alvo válido, movemos
        if (target != null && target.gameObject.activeInHierarchy)
        {
            ChangeState(AIState.MOVING);
            return;
        }

        // Usa a nova função auxiliar
        Transform newTarget = FindClosestTarget();

        if (newTarget != null)
        {
            target = newTarget;
            ChangeState(AIState.MOVING);
        }
        else
        {
            target = null;
        }
    }

    protected virtual void Moving()
    {
        if (CheckLostTarget())
        {
            return;
        }

        // --- MUDANÇA 2: Reavaliação Periódica ---
        // A cada 'searchInterval' segundos, verificamos se existe alguém mais perto
        if (Time.time > lastSearchTime + searchInterval)
        {
            lastSearchTime = Time.time;

            Transform potentialNewTarget = FindClosestTarget();

            // Se achamos alguém E esse alguém é diferente do atual...
            if (potentialNewTarget != null && potentialNewTarget != target)
            {
                // ...trocamos de alvo!
                target = potentialNewTarget;
                // Debug.Log("Troquei de alvo para um mais próximo!");
            }
        }
        // ----------------------------------------

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= range)
        {
            agent.isStopped = true;
            ChangeState(AIState.ATTACKING);
            return;
        }
        else
        {
            // Importante atualizar o destino caso o alvo tenha mudado ou se movido
            agent.SetDestination(target.position);
        }
    }

    protected virtual void Attacking()
    {
        if (CheckLostTarget())
        {
            return;
        }

        // Opcional: Você também pode querer que ele troque de alvo enquanto ataca
        // se alguém passar muito perto, mas no Clash Royale geralmente eles
        // focam até o alvo sair do alcance ou morrer. 
        // Vamos manter simples por enquanto.

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

            Piece targetRef = target.GetComponent<Piece>();
            if (targetRef != null)
            {
                
                myPiece.Attack(targetRef);
                if (gameObject.tag == "HUMANS")
                {
                    AudioManager.instance.PlaySFX("SwordHit");
                }
                else
                {
                    AudioManager.instance.PlaySFX("SwordHit2");
                }

                if (gameObject.name.Contains("FoeBandit"))
                {
                    Debug.Log("Gold -20");
                    GameStateManager.Instance.SpendGold(20);
                }
            }
        }
    }

    protected bool CheckLostTarget()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            target = null;
            ChangeState(AIState.SEARCHING);
            return true;
        }
        return false;
    }
}