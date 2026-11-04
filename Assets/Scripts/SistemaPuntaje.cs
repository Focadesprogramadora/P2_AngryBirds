using UnityEngine;
using TMPro;

public class SistemaPuntaje : MonoBehaviour
{
    private int puntajeActual = 0; // Variable para guardar el puntaje del jugador

 
    public TextMeshProUGUI textoPuntaje; // Variable para el componente del texto del canvas

    void Start()
    {
        ActualizarTextoPuntaje(); // Cuando inicie el juego, se muestra la puntuación en 0
    }

    // Método para que los enemigos permitan sumar puntos
    public void SumarPuntos(int cantidad)
    {
        puntajeActual += cantidad;
        ActualizarTextoPuntaje();
                        // El valor que guardamos en puntajeActual se le suma
                        // lo que venga en la variable cantidad. Se va actualizando en pantalla.
    }

    void ActualizarTextoPuntaje()
    {
        if (textoPuntaje != null) // Revisar errores en la interfaz 
        {
            textoPuntaje.text = "Puntaje: " + puntajeActual;
                    //Que el texto del canva indique el puntaje y muestre númericamente la variable
                    // donde se guarda el score que lleva el jugador
        }
    }
}