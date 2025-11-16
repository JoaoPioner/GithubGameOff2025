using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] public float HP = 100f;
    [SerializeField] public float attackPower = 10f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Damage(float hits)
    {
        HP -= hits;
    }

    public void Attack(Piece targetPiece)
    {
        targetPiece.Damage(attackPower);
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            Debug.Log(gameObject.name + " foi destruído.");
            Destroy(gameObject);
        }

    }
}
