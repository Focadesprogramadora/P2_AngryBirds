using UnityEngine;
using TMPro;

public class Resultados : MonoBehaviour
{
    public TextMeshProUGUI TextoPuntajeAnterior;

    void Start()
    {
        // Recuperar puntaje anterior
        int PuntSalvado = PlayerPrefs.GetInt("PuntajeFinal", 0);

        // Mostrar en pantalla
        if (TextoPuntajeAnterior != null)
        {
            TextoPuntajeAnterior.text = "Puntaje del nivel: " + PuntSalvado;
        }
    }
}
