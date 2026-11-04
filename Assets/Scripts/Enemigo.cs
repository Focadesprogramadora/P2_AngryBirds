using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Configuración de Puntos")]
    public int puntosQueDa = 100;   // Variable númerica que contiene los puntos de cada enemigo

    private void OnCollisionEnter2D(Collision2D collision)      // Función se activa cuando dos objecos colisionan entre sí
    {
        if (collision.gameObject.CompareTag("Pelota"))      // Se compara el tag a Pelota
        {
            SistemaPuntaje gestorPuntaje = FindAnyObjectByType<SistemaPuntaje>(); //  Busca al puntaje y lo guarda en una variable llamada gestorPuntaje

            if (gestorPuntaje != null) // Se verifica la variable puntaje
            {
                gestorPuntaje.SumarPuntos(puntosQueDa); // Suma de puntos al puntaje 
            }

          
            Destroy(gameObject);        // Se destruye al enemigo
        }
    }
}