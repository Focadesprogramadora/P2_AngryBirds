using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Pelota : MonoBehaviour
{
    private Controles misControles; // Variables de los controles Input System
    private Rigidbody2D rb; // Obtener el rigidbody 
    private Vector2 posicionInicial; // Variable para recordar de donde se lanzó la pelota

    // variables bool - f o t
    private bool estaArrastrando = false; // Para cuando la pelota le estés haciendo click
    private bool yaFueLanzada = false; // para evitar que la pelota pueda vovler a ser estiada cuando sigue en movimiento
    private bool juegoTerminado = false; // Para cuando ganes o pierdes se queda bloqueado 

    [Header("Configuración Lanzamiento")]
    public float fuerzaLanzamiento = 15f;       // Potencia de lanzamiento
    public float maxDistanciaArrastre = 3f;     // Distancia máxima para tirar

    [Header("UI y Paneles")]
    public int disparosRestantes = 3;          // Número de tiros disponibles
    public TextMeshProUGUI textoDisparos;      // Texto que muestra los tiros restantes
    public GameObject panelGameOver;           // Panel de "perdiste" cuando te quedas sin tiros
    public GameObject panelVictoria;           // Panel de victoria 




    private void Awake() // met para iniciar y preparar sistemas de controles 
    {
        misControles = new Controles();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        posicionInicial = transform.position;       // Donde se coloca la pelota, guardar xy
        rb.simulated = false;       // Apagar las físicas temporalmente

        misControles.Gameplay.Click.performed += ctx => EmpezarArrastre(); // cuando se ejecute la funcion
        misControles.Gameplay.Click.canceled += ctx => SoltarYDisparar();  // cuando se cancele la funcion

        ActualizarUI();

        // Ocultar paneles al iniciar el juego
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);
    }

    // para act y desc de controles
    private void OnEnable() => misControles.Enable();
    private void OnDisable() => misControles.Disable();

    void EmpezarArrastre() //  método para estirar la pelota
    {
        if (juegoTerminado || disparosRestantes <= 0 || yaFueLanzada) return; // ||= O ; revisa 3 condiciones; si ya termino si ye qiedas soin pelotas y su ya fue lanzada

        Vector2 posicionMousePantalla = misControles.Gameplay.Posicion.ReadValue<Vector2>(); // Preguntar a los controles la posición del cursor
        Vector3 mousePos = new Vector3(posicionMousePantalla.x, posicionMousePantalla.y, 0f); // V3 se va a guardar. Crear un nuevo vector acorde a las corrdenadas del mouse en pantalla y se le asgina o po la profundidad 
        mousePos.z = -Camera.main.transform.position.z;// Variable tri , busca la camara, revisa su posicion y ajusta ;
        Vector2 posicionMouseMundo = Camera.main.ScreenToWorldPoint(mousePos); // Toma la pisicion del mouse donde hacemos click y los convierte enc oordenadas para saber donde esTÁN LAS coordenadas dentro del mapa del juego

        if (Vector2.Distance(posicionMouseMundo, transform.position) < 2f) // Calcular la distancia entre posicion del mouse y donde se ubica la foca; si es menos 1 2, signficia que diste clic a la pelota 
        {
            estaArrastrando = true; // si le diste 
            rb.simulated = false; // para que la foca no caiga al suelo meintras la sostenemos
            rb.linearVelocity = Vector2.zero; // frenar movimiento
        }
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Arrastrar la pelota -- Seguir movimientos mientra la estiramos.
        if (estaArrastrando)
        {
            Vector2 posicionMousePantalla = misControles.Gameplay.Posicion.ReadValue<Vector2>(); // leer posicion del mouse en pantalla
            Vector3 mousePos = new Vector3(posicionMousePantalla.x, posicionMousePantalla.y, 0f); // Ajustar profundidad de la cmara
            mousePos.z = -Camera.main.transform.position.z; // ajustar prof
            Vector2 posicionMouseMundo = Camera.main.ScreenToWorldPoint(mousePos); // coordenadas realedes dentro de la pantalla

            Vector2 direccion = posicionMouseMundo - posicionInicial; // distancia entre el centro y lugar del mouse

            if (direccion.magnitude > maxDistanciaArrastre) // rev magnitud y límite de estiramiento
            {
                direccion = direccion.normalized * maxDistanciaArrastre; // para mantener el estiramiento en el límite 
            }

            transform.position = posicionInicial + direccion; // Mueve la pelota para que siga el mouse
        }

        if (yaFueLanzada && rb.linearVelocity.magnitude < 0.1f && rb.simulated) // && = Y ; Si ya fue lanzada, la veloxidad y sus fisicas se encienden. Si ua fue lanza y la vel es menor a p.1 las ficas encendidad, 
        {
            VerificarReinicioDisparo();
        }

        VerificarVictoriaPorEnemigos();
    }

    void SoltarYDisparar() // Calcular la direccion a donde estiramos y dar glpe para disparar
    {
        if (estaArrastrando)
        {
            estaArrastrando = false; // apagar por que ya solto el mouse
            yaFueLanzada = true; // pelota en vuelo
            rb.simulated = true; // encender fisicas

            disparosRestantes--; // restar pelotas del contador
            ActualizarUI();

            Vector2 fuerzaDireccion = posicionInicial - (Vector2)transform.position;
            rb.AddForce(fuerzaDireccion * fuerzaLanzamiento, ForceMode2D.Impulse);

            // calcular direccion del tiro, se le da un golpe de fuerza y se multiplca por la vab fuerza de lanzamineto
        }
    }

    void VerificarReinicioDisparo()
    {
        if (disparosRestantes > 0) // Si todavía hay pelotas
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
            transform.position = posicionInicial;
            yaFueLanzada = false;
        }
        else
        {
            // si te quedaste sin pelotas
            if (panelGameOver != null)
            {
                panelGameOver.SetActive(true);
            }
            juegoTerminado = true;
        }
    }

    void VerificarVictoriaPorEnemigos()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemigo"); // buscar objeto con el tag Enemigos
        if (enemigos.Length == 0 && !juegoTerminado) // si la cantidad de enemigos es iual a 0 y tmb si el juego no ha terminado
        {
            juegoTerminado = true;
            if (panelVictoria != null)
            {
                panelVictoria.SetActive(true); // activar pantalla de victoria si ganamos
            }
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            // apagar físicas de la pelota para que se quede quieta
        }
    }

    void ActualizarUI()
    {
        if (textoDisparos != null)
        {
            textoDisparos.text = "Focas Disponibles " + disparosRestantes; // Aquí se 
        }
    }
}