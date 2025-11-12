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
    private NavMeshAgent agent; // O componente que move a tropa (o "GPS")
    
    [Header("Configuração de Alvo")]
    public Transform target;
    public string targetTag = "Enemy";

    [Header("Configurações de Ataque")]
    public float range = 2f;
    public float attackSpd = 1f;
    public float cooldown = 2f;
    
    private Piece myPiece;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myPiece = GetComponent<Piece>();
        actualState = AIState.SEARCHING;
    }


    
    void Update()
    {
        switch (actualState)
        {
            case AIState.SEARCHING:
                RunSearching();
                break;
            case AIState.MOVING:
                RunMoving();
                break;
            case AIState.ATTACKING:
                RunAttack();
                break;
        }
    }
    
    private void ChangeState(AIState novoEstado)
    {
        // Debug.Log(name + " mudou do estado " + estadoAtual + " para " + novoEstado);
        actualState = novoEstado;

        // Se começamos a nos mover, precisamos reativar o agente
        if (novoEstado == AIState.MOVING)
        {
            agent.Resume();
        }
    }
    
    private bool FoundNextEnemy()
    {
        // Esta é uma forma simples de encontrar alvos.
        // Pode ser otimizada depois, mas funciona bem.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);
        
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

        // Se encontramos alguém, definimos como alvo "lockado"
        if (closestEnemy is not null)
        {
            target = closestEnemy.transform;
            return true;
        }
        else
        {
            // Se não, limpamos o alvo (caso ele tenha morrido)
            target = null;
            return false;
        }
    }

    private void RunSearching()
    {
        if (target is not null)
        {
            ChangeState(AIState.MOVING);
            return;
        }

        if (FoundNextEnemy())
        {
            ChangeState(AIState.MOVING);
        }
        
    }
    
    private void RunMoving()
    {
        // Se o alvo morreu ou sumiu (ficou inativo) enquanto íamos até ele...
        if (target is null || !target.gameObject.activeInHierarchy)
        {
            ChangeState(AIState.SEARCHING);
            return;
        }

        // Calcula a distância até o alvo
        float distnceToTarget = Vector3.Distance(transform.position, target.position);

        // Se já estamos dentro do alcance de ataque...
        if (distnceToTarget <= range)
        {
            ChangeState(AIState.ATTACKING);
        }
        else
        {
            // Se não, continuamos nos movendo.
            // O NavMeshAgent cuida de desviar dos obstáculos.
            agent.SetDestination(target.position);
        }
    }
    
    private void RunAttack()
    {
        // Se o alvo morreu ou sumiu...
        if (target is null || !target.gameObject.activeInHierarchy)
        {
            ChangeState(AIState.SEARCHING);
            return;
        }

        // Verifica se o alvo fugiu e saiu do alcance
        float distnceToTarget = Vector3.Distance(transform.position, target.position);
        if (distnceToTarget > range)
        {
            ChangeState(AIState.MOVING);
            return;
        }

        // --- Lógica de Ataque ---
        // Para o movimento
        agent.Stop(); 
        // Vira para o inimigo
        transform.LookAt(target.position);

        // Controla a cadência (velocidade) do ataque
        if (Time.time > cooldown + (1f / attackSpd))
        {
            // Atualiza o tempo do cooldown
            cooldown = Time.time;

            // Chama a função de atacar
            AttackTarget();
        }
    }
    
    private void AttackTarget()
    {
        // Aqui é onde a mágica acontece.
        // Você pode tocar uma animação, criar um projétil, etc.
        Debug.Log(name + " atacou " + target.name);

        // Exemplo: Pegar o script de Vida do inimigo e causar dano
        Piece targetRef = target.GetComponent<Piece>();
        if (targetRef is not null)
        {
            targetRef.Damage(10f); // Causa 10 de dano (exemplo)
        }
    }
}
