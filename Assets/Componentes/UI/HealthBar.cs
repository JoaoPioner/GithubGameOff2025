using UnityEngine;

public class HealthBar : MonoBehaviour
{
    // Deixar a barra de vida sempre virada para a câmera
    private void LateUpdate()
    {
        transform.forward = -Camera.main.transform.forward;
    }
}
