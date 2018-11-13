using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptDelCanvas : MonoBehaviour {

    public Toggle suelo;
    public Toggle paredes;

    public Button miBoton;
    public Button botonSalir;
    public Button menu;

    public GameObject AdministradorDeJuego;

    private Tablero miTablero;

    public Text losPuntos;
    public Text elNivel;

    bool gameOver = false;

    public GameObject cartelGameOver;

    public GameObject instrucciones;
    public GameObject creditos;

	// Use this for initialization
	void Start () {
        miTablero = AdministradorDeJuego.GetComponent<Tablero>();

        miBoton.onClick.AddListener(iniciarJuego);
        botonSalir.onClick.AddListener(salir);
        menu.onClick.AddListener(volverMenu);

    }
	
	// Update is called once per frame
	void Update () {
        losPuntos.text = "Puntos: " + miTablero.getPuntos();
        elNivel.text = "Nivel: " + miTablero.getLevel();

        if (!miTablero.getJugando() && !gameOver)
        {
            gameOver = true;
            AdministradorDeJuego.SetActive(false);
            cartelGameOver.SetActive(true);
            botonSalir.gameObject.SetActive(true);
            menu.gameObject.SetActive(true);
        }

        if (miTablero.getSalir())
        {
            volverMenu();
        }
    }

    void volverMenu()
    {
        SceneManager.LoadScene("Principal");
    }

    void iniciarJuego()
    {
        miTablero.setLados(paredes.isOn);
        miTablero.setSuelo(suelo.isOn);

        AdministradorDeJuego.SetActive(true);

        suelo.gameObject.SetActive(false);
        paredes.gameObject.SetActive(false);
        miBoton.gameObject.SetActive(false);
        botonSalir.gameObject.SetActive(false);

        instrucciones.SetActive(false);
        creditos.SetActive(false);
    }

    void salir()
    {
        Application.Quit();
    }


}
