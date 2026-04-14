/*
 * public class G3_GameManager : MonoBehaviour
{
    public int itemsParaGanar = 2;
    public TextMeshProUGUI textoUI;
    public GameObject panelVictoria;

    [Header("Botones de Victoria")]
    public GameObject botonContinuar;
    public GameObject botonSalir;

    private int itemsActuales = 0;
    private int puntosTotales = 0;

    public void ItemRecogido()
    {
        itemsActuales++;
        puntosTotales += 5;

        if (textoUI != null)
        {
            textoUI.text = "Puntos: " + puntosTotales;
        }

        if (MainManager.Instance != null)
        {
            MainManager.Instance.SumarPuntoTemporal(5);
        }

        if (itemsActuales >= itemsParaGanar)
        {
            GanarMinijuego();
        }
    }

    private void GanarMinijuego()
    {
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (MainManager.Instance != null)
            {
                bool historia = MainManager.Instance.modoHistoriaActivo;
                botonContinuar.SetActive(historia);
                botonSalir.SetActive(true);
            }
        }
    }
}
 */
