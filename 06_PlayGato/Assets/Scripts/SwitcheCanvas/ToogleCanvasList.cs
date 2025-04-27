using UnityEngine;
using UnityEngine.UI;

public class ToogleCanvasList : MonoBehaviour
{
    // Referencias a los Canvas: el actual y el siguiente
    public Canvas currentCanvas;
    public Canvas nextCanvas;

    // Este método será asignado a cualquier botón desde el Inspector
    public void SwitchToNextCanvas()
    {
        // Verifica que los Canvas estén configurados
        if (currentCanvas != null && nextCanvas != null)
        {
            // Desactiva el Canvas actual
            currentCanvas.gameObject.SetActive(false);

            // Activa el Canvas siguiente
            nextCanvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Asegúrate de asignar los Canvas en el script.");
        }
    }
}