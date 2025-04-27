using UnityEngine;
using UnityEngine.UI;

public class ToggleCanvas : MonoBehaviour
{
    // Referencias a los Canvas
    public Canvas currentCanvas; // El Canvas que quieres ocultar
    public Canvas targetCanvas;  // El Canvas que quieres activar

    // Referencia al botón
    public Button toggleButton;

    void Start()
    {
        // Asegúrate de que el botón esté configurado
        if (toggleButton != null)
        {
            // Agregar un listener para el evento onClick del botón
            toggleButton.onClick.AddListener(SwitchCanvas);
        }
    }

    void SwitchCanvas()
    {
        if (currentCanvas != null && targetCanvas != null)
        {
            // Desactiva el Canvas actual
            currentCanvas.gameObject.SetActive(false);

            // Activa el Canvas objetivo
            targetCanvas.gameObject.SetActive(true);
        }
    }
}
