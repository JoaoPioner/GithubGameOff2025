using System;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
  public event Action<Piece> OnDeath; 

  [SerializeField]
  public float HP = 100f;

  [SerializeField]
  public float currentHP;

  [SerializeField]
  public float attackPower = 10f;

  public Image healthBar;

  void Start()
  {
    currentHP = HP;
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

      Debug.Log(gameObject.name + " foi destruído.");
      OnDeath?.Invoke(this);
      Destroy(gameObject);
    }
    else
    {
      currentHP = Mathf.Clamp(currentHP, 0, HP);
      healthBar.fillAmount = (float)currentHP / HP;
    }
  }

  public void Attack(Piece targetPiece)
  {
    targetPiece.Damage(attackPower);
  }
}
