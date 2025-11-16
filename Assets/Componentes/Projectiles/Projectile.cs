using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    public float speed = 20f;
    [HideInInspector] public float damage; // Dano do projétil, baseado no dano do objeto que o disparou
    public GameObject impactEffect; //Efeito visual ao atingir o alvo

    public void Seek(Transform _target) // Define o alvo do projétil
    {
        target = _target;
    }

    void Update()
    {
        // Se o alvo for nulo ou destruído, destrói o projétil
        if (target == null || target.Equals(null))
        {
            Destroy(gameObject);
            return;
        }
        // Calcula a direção para o alvo
        Vector3 dir = target.position - transform.position;

        // Calcula a distância que o projétil pode percorrer neste frame
        float distanceThisFrame = speed * Time.deltaTime;
        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        // Move o projétil em direção ao alvo
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        // Faz o projétil olhar para o alvo
        if (target != null && !target.Equals(null))
            transform.LookAt(target);
    }

    void HitTarget()
    {
        // Instancia o efeito de impacto, se houver
        if (impactEffect != null)
        {
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
        }

        // Aplica dano ao inimigo atingido
        if (target != null && !target.Equals(null))
        {
            Piece enemy = target.GetComponent<Piece>();
            if (enemy != null)
            {
                enemy.Damage(damage);
            }
        }
        // Destrói o projétil após o impacto
        Destroy(gameObject);
    }
}
