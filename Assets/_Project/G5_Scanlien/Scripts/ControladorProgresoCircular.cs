using UnityEngine;
using UnityEngine.UI;
using TMPro; // Requerido si usas TextMeshPro

public class ControladorProgresoCircular : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private Image imagenRellenoAzul;
    [SerializeField] private TextMeshProUGUI textoPorcentaje; // Cambia a "Text" si no usas TextMeshPro

    /// <summary>
    /// Actualiza visualmente el progreso del escaneo.
    /// </summary>
    /// <param name="progreso">Valor entre 0.0f y 1.0f</param>
    public void ActualizarProgreso(float progreso)
    {
        // Aseguramos que el valor esté entre 0 y 1 para evitar errores
        progreso = Mathf.Clamp01(progreso);

        // 1. Actualizar el relleno del círculo (pide un valor de 0 a 1)
        if (imagenRellenoAzul != null)
        {
            imagenRellenoAzul.fillAmount = progreso;
        }

        // 2. Actualizar el texto del porcentaje (multiplicamos por 100)
        if (textoPorcentaje != null)
        {
            int porcentajeEntero = Mathf.RoundToInt(progreso * 100f);
            textoPorcentaje.text = porcentajeEntero + "%";
        }
    }
}