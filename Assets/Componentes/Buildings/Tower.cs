using UnityEngine;

public class Tower : MonoBehaviour, IBuilding
{
  [Header("Tower config")]
  public float attackRange = 10f;
  public float attackDamage = 20f;
  public float fireRate = 1f;
  public Transform firePoint;
  public GameObject projectilePrefab;

  private float fireCooldown = 0f;
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
    if (targetEnemy == null) return;

    GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    AudioManager.instance.PlaySFX("ArrowShot2");
    Projectile projectile = projectileGO.GetComponent<Projectile>();
    if (projectile != null)
    {
      projectile.Seek(targetEnemy);
      projectile.damage = attackDamage;
    }
  }

  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
  }
}
