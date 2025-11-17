using UnityEngine;

public class Piece : MonoBehaviour
{
  [SerializeField] public float HP = 100f;
  [SerializeField] public float attackPower = 10f;

  public void Damage(float hits)
  {
    HP -= hits;
    if (HP <= 0)
    {
      var tower = GetComponent<Tower>();
      if (tower != null)
      {
        GameStateManager.Instance.VPDestroyed();
      }
      Debug.Log(gameObject.name + " foi destruído.");
      Destroy(gameObject);
    }
  }

  public void Attack(Piece targetPiece)
  {
    targetPiece.Damage(attackPower);
  }
}
