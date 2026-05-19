using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class ControladorProgresoCircular : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private Image imagenRellenoAzul;
    [SerializeField] private TextMeshProUGUI textoPorcentaje; 

    /// <summary>
    /// Actualiza visualmente el progreso del escaneo.
    /// </summary>
    /// <param name="progreso">Valor entre 0.0f y 1.0f</param>
    public void ActualizarProgreso(float progreso)
    {
        
        progreso = Mathf.Clamp01(progreso);

        if (imagenRellenoAzul != null)
        {
            imagenRellenoAzul.fillAmount = progreso;
        }

        if (textoPorcentaje != null)
        {
            int porcentajeEntero = Mathf.RoundToInt(progreso * 100f);
            textoPorcentaje.text = porcentajeEntero + "%";
        }
    }
}