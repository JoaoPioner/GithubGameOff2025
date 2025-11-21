using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Piece))]
public class FSM_BuildilgFocus : FSM 
{
    protected override void Searching()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(targetTag);
        if (buildings.Length == 0)
        {
            // Nenhum edifício encontrado, permanecer no estado de busca
            return;
        }
        // Encontrar o edifício mais próximo
        Transform closestBuilding = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject building in buildings)
        {

            if (building.GetComponent<IBuilding>() == null)
                continue;

            float distance = Vector3.Distance(transform.position, building.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBuilding = building.transform;
            }
        }
        if (closestBuilding != null)
        {
            target = closestBuilding;
            ChangeState(AIState.MOVING);
        }
    }
}
    



