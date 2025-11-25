using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
  public static SpawnManager Instance { get; private set; }

  [Header("Configurações")]
  public LayerMask groundLayer;

  private GameObject currentGhost;
  private UnitData currentUnitData;

  void Awake()
  {
    Instance = this;
  }

  void Update()
  {
    if (currentGhost == null) return;
    if (Mouse.current == null) return;

    MoveGhost();

    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
      PositionPiece();
    }

    if (Mouse.current.rightButton.wasPressedThisFrame)
    {
      CancelGhost();
    }
  }

  public void SelectPiece(UnitData unitData)
  {
    if (currentGhost != null) Destroy(currentGhost);

    currentUnitData = unitData;
    currentGhost = Instantiate(unitData.prefab.gameObject);

    var agent = currentGhost.GetComponent<NavMeshAgent>();
    if (agent != null) agent.enabled = false;

    var col = currentGhost.GetComponent<Collider>();
    if (col != null) col.enabled = false;

    var aiScript = currentGhost.GetComponent<FSM>();
    if (aiScript == null)
    {
      aiScript = currentGhost.GetComponent<FSM_Ranged>();
    }
    if (aiScript != null) aiScript.enabled = false;

    // (Opcional) Desativar script de ataque se tiver
    // var attack = currentGhost.GetComponent<AttackScript>();
    // if (attack != null) attack.enabled = false;
  }

  void MoveGhost()
  {
    Vector2 mousePos = Mouse.current.position.ReadValue();

    Ray ray = Camera.main.ScreenPointToRay(mousePos);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
    {
      currentGhost.transform.position = hit.point;
    }
  }
  void PositionPiece()
  {
    if (GameStateManager.Instance.SpendGold(-currentUnitData.cost) == false)
    {
      Debug.Log("Ouro insuficiente na hora H!");
      CancelGhost();
      return;
    }

    var agent = currentGhost.GetComponent<NavMeshAgent>();
    if (agent != null) agent.enabled = true;

    var col = currentGhost.GetComponent<Collider>();
    if (col != null) col.enabled = true;

    var aiScript = currentGhost.GetComponent<FSM>();
    if (aiScript == null)
    {
      aiScript = currentGhost.GetComponent<FSM_Ranged>();
    }
    if (aiScript != null) aiScript.enabled = true;

    currentGhost = null;
    currentUnitData = null;

    Debug.Log("Unidade construída com sucesso!");
  }

  void CancelGhost()
  {
    if (currentGhost != null)
    {
      Destroy(currentGhost);
    }
    currentGhost = null;
    currentUnitData = null;
  }
}
