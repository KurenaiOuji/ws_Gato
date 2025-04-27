using UnityEngine;
using UnityEngine.UI;

public class ToggleCanvas : MonoBehaviour
{
    // Referencias a los Canvas
    public Canvas currentCanvas; // El Canvas que quieres ocultar
    public Canvas targetCanvas;  // El Canvas que quieres activar

    // Referencia al bot�n
    public Button toggleButton;

    void Start()
    {
        // Aseg�rate de que el bot�n est� configurado
        if (toggleButton != null)
        {
            // Agregar un listener para el evento onClick del bot�n
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
