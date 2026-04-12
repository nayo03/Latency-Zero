using UnityEngine;
using TMPro;

public class G2_UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI warningText;

    void Start()
    {
        // Limpiamos el texto al empezar la escena
        if (warningText != null) warningText.text = "";
    }

    public void MostrarMensajeMuerteCaida()
    {
        if (warningText != null)
        {
            warningText.text = "You’ve been toasted…";
        }
    }

    public void MostrarMensajeMuerteChoque()
    {
        if (warningText != null) warningText.text = "Don't break the crust!";
    }
}