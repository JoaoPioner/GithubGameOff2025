using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower config")]
    public float attackRange = 10f;
    public float attackDamage = 20f;
    public float fireRate = 1f; // Tiros por segundo
    public Transform firePoint; // Ponto de onde os projéteis são disparados
    public GameObject projectilePrefab; // Prefab do projétil

    private float fireCooldown = 0f; // Tempo restante para o próximo tiro
    private Transform targetEnemy;

    public void Update()
    {
        UpdateTarget();

        if (targetEnemy == null)
            return;

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }

        fireCooldown -= Time.deltaTime;
    }

    void UpdateTarget()
    {
        // Verifica se o alvo atual ainda está dentro do alcance
        if (targetEnemy != null)
        {
            if (targetEnemy == null)
            {
                targetEnemy = null;
                return;
            }

            float distanceToTarget = Vector3.Distance(transform.position, targetEnemy.position);

            if (distanceToTarget <= attackRange)
                return;

            targetEnemy = null;
        }

        // Encontra o inimigo mais próximo dentro do alcance
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("GOBLINS");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            targetEnemy = nearestEnemy.transform;
        }
        else
        {
            targetEnemy = null;
        }
    }

    void Shoot()
    {
        // Instancia o projétil e configura seu alvo
        if (targetEnemy == null) return;
      
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Seek(targetEnemy);
            projectile.damage = attackDamage;
        }
    }

    void OnDrawGizmosSelected() // Desenha o alcance da torre na cena
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
