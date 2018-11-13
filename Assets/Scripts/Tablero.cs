using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablero : MonoBehaviour {

    bool jugando = true;

    public int alto;
    public int ancho;

    private int maxAlto;
    private int maxAncho;

    public Transform miTablero;
    public GameObject tileFondo;
    public GameObject tileFinalRojo;
    private GameObject[,] misTilesDelFondo;
    private InfoDePieza[,] misPiezas;

    private int xDelSpawn;

    private int xActual;
    private int yActual;

    private float timer = 0;
    private float timerParaTecla = 0;
    private float timerParaFicha = 0;

    public float tiempoEntreTeclas = 0.2f;
    public float tiempoEntreFicha = 1;

    private float timerBajandoFicha = 0;
    public float tiempoParaBajar = 0.5f;

    private bool usandoUnaFicha = false;
    private bool tengoQueCrearFicha = true;
    private bool hayQueVerificarPipe = false;

    private float timerParaDestruccion = 0;
    private bool destruyendo = false;

    private int huecos = 0;

    private int grupoActual = 0;

    public Transform miCamara;

    private bool lados = true;
    private bool suelo = true;

    private GameObject tileDelShift;
    private GameObject tileDelNext;
    private InfoDePieza miShift;
    private InfoDePieza miNext;

    private int lejos = 4;

    private bool shiftDisponible = true;

    private int level = 0;

    private int puntosTotales = 0;
    private int nivelDeCadena = 1;
    private int contadorDeNivel = 0;

    public int cantidadParaMultiplicador = 10;
    public int cantidadParaSubirNivel = 15;
    public float reduccionEnElTimer = 0.1f;

    bool salir = false;

    // Use this for initialization
    void Start () {

        misTilesDelFondo = new GameObject[ancho, alto];
        misPiezas = new InfoDePieza[ancho, alto];

        maxAlto = alto - 1;
        maxAncho = ancho - 1;

        xDelSpawn = ancho / 2;

        iniciarFondo();
        agregarFinales();

        miCamara.position = new Vector3(xDelSpawn, (alto / 2) - 1, -10);
	}
	
	// Update is called once per frame
	void Update () {
        if (jugando)
        {            
            timer = timer + Time.deltaTime;

            if (tengoQueCrearFicha)
            {
                nivelDeCadena = 1;
                shiftDisponible = true;

                iniciarFicha();
            }

            if (timer - timerParaFicha >= tiempoEntreFicha && usandoUnaFicha)
            {
                bajarFicha();
                timerParaFicha = timer;
            }
            
            if (hayQueVerificarPipe)
            {                
                if (verificadorTableroPipe())
                {
                    verificadorTableroBorrar();
                }
                else
                {
                    tengoQueCrearFicha = true;
                }

                grupoActual = 0;
                hayQueVerificarPipe = false;
                resetearRevisionDeFichas();
            }

            if (destruyendo)
            {
                if (timer - timerParaDestruccion > InfoDePieza.getTiempoDeDestrucción())
                {
                    huecos = verificarHuecos();
                    destruyendo = false;
                    if (huecos > 0)
                    {
                        bajarTodo();
                        huecos--;

                        if (huecos == 0)
                        {
                            hayQueVerificarPipe = true;
                        }
                        else
                        {
                            timerBajandoFicha = timer;
                        }
                    }
                    else
                    {
                        tengoQueCrearFicha = true;
                    }
                }
            }

            if (huecos > 0 && timer - timerBajandoFicha > tiempoParaBajar)
            {
                bajarTodo();
                huecos--;

                if (huecos == 0)
                {
                    hayQueVerificarPipe = true;
                }
                else
                {
                    timerBajandoFicha = timer;
                }
            }

            verificarTeclas();
        }

        verificarEscape();
    }

    public bool getJugando()
    {
        return jugando;
    }

    void verificarEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            salir = true;
        }
    }

    public bool getSalir()
    {
        return salir;
    }

    int verificarHuecos()
    {
        int espacios = 0;
        int actual = 0;
        bool contar = false;

        for (int i = 0; i < ancho; i++)
        {
            for (int c = maxAlto; c >= 0; c--)
            {
                if (misPiezas[i,c].getEstaOcupado())
                {
                    if (!contar)
                    {
                        contar = true;
                    }
                }
                else
                {
                    if (contar)
                    {
                        actual++;
                    }
                }

                if (c == 0 && contar)
                {
                    if (actual > espacios)
                    {
                        espacios = actual;
                    }

                    actual = 0;
                    contar = false;
                }
            }
        }

        return espacios;
    }

    void bajarTodo()
    {
        for (int i = 1; i < alto; i++)
        {
            for (int c = 0; c < ancho; c++)
            {
                yActual = i;
                xActual = c;

                bajarFicha();
            }
        }
    }

    void resetearRevisionDeFichas()
    {
        for (int i = 0; i < alto; i++)
        {
            for (int c = 0; c < ancho; c++)
            {
                misPiezas[c, i].resetearEstadoRevision();
            }
        }
    }

    void verificadorTableroBorrar()
    {
        int losPuntos = verificarGrupo();

        if (losPuntos > 0)
        {
            calcularPuntos(losPuntos);
        }

        for (int i = 0; i < alto; i++)
        {
            for (int c = 0; c < ancho; c++)
            {
                if (misPiezas[c, i].getEstaOcupado())
                {
                    if (misPiezas[c, i].getPiezaUtil())
                    {
                        misPiezas[c, i].destruyendoPieza(true);
                        timerParaDestruccion = timer;
                        destruyendo = true;
                    }
                }
            }
        }
    }

    void calcularPuntos(int num)
    {
        contadorDeNivel = contadorDeNivel + num;

        int nuevoNum = num;
        int multiplicador = 1;

        while (nuevoNum > cantidadParaMultiplicador)
        {
            nuevoNum = nuevoNum - cantidadParaMultiplicador;
            multiplicador++;
        }

        num = num * multiplicador * nivelDeCadena * level;

        nivelDeCadena++;

        while (contadorDeNivel >= cantidadParaSubirNivel)
        {
            contadorDeNivel = contadorDeNivel - cantidadParaSubirNivel;
            level++;
            tiempoParaBajar = tiempoParaBajar - reduccionEnElTimer;
        }

        puntosTotales = puntosTotales + num;
    }

    public int getLevel()
    {
        return level;
    }

    public int getPuntos()
    {
        return puntosTotales;
    }

    int verificarGrupo()
    {
        int total = 0;

        for (int j = grupoActual; j > 0; j--)
        {
            int esteGrupo = 0;

            for (int i = 0; i < alto; i++)
            {
                for (int c = 0; c < ancho; c++)
                {
                    if (misPiezas[c, i].getGrupo() == j)
                    {
                        esteGrupo++;

                        if (!misPiezas[c, i].getPiezaUtil())
                        {
                            esteGrupo = 0;
                            cancelarGrupo(j);

                            i = alto;
                            c = ancho;
                        }
                    }
                }
            }

            total = total + esteGrupo;
        }

        return total;
    }

    void cancelarGrupo(int num)
    {
        for (int i = 0; i < alto; i++)
        {
            for (int c = 0; c < ancho; c++)
            {
                if (misPiezas[c, i].getGrupo() == num)
                {
                    misPiezas[c, i].setPiezaUtil(false);
                }
            }
        }
    }

    bool verificadorTableroPipe()
    {
        bool hayPipeFormada = false;

        for (int i = 0; i < alto; i++)
        {
            for (int c = 0; c < ancho; c++)
            {
                if (misPiezas[c, i].getEstaOcupado())
                {
                    if (!misPiezas[c, i].getRevisadoTodo())
                    {
                        grupoActual++;

                        if (verificarPipe(c, i, 0, grupoActual))
                        {
                            hayPipeFormada = true;
                        }
                    }
                }
            }
        }

        return hayPipeFormada;
    }

    bool verificarPipe(int verifX, int verifY, int deDonde, int grupo)
    {
        //si la pieza esta ocupada y por lo tanto tiene sentido verificar
        if (misPiezas[verifX, verifY].getEstaOcupado())
        {
            if (misPiezas[verifX, verifY].getRevisadoTodo())
            {
                if ((deDonde == 1 && !misPiezas[verifX, verifY].getAAbajo()) ||
                (deDonde == 2 && !misPiezas[verifX, verifY].getAIzquierda()) ||
                (deDonde == 3 && !misPiezas[verifX, verifY].getAArriba()) ||
                (deDonde == 4 && !misPiezas[verifX, verifY].getADerecha()))
                {
                    return false;
                }

                return misPiezas[verifX, verifY].getPiezaUtil();
            }
            //veo si estoy entrando por un lado que no tiene respuesta en este cubo
            if ((deDonde == 1 && !misPiezas[verifX, verifY].getAAbajo()) ||
                (deDonde == 2 && !misPiezas[verifX, verifY].getAIzquierda()) ||
                (deDonde == 3 && !misPiezas[verifX, verifY].getAArriba()) ||
                (deDonde == 4 && !misPiezas[verifX, verifY].getADerecha()))
            {
                //esta ficha ni siquiera tiene algo que ver, por eso no la afecto
                return false;
            }
            else
            {
                switch (deDonde)
                {
                    case 1:
                        misPiezas[verifX, verifY].setRAbajo();
                        break;
                    case 2:
                        misPiezas[verifX, verifY].setRIzquierda();
                        break;
                    case 3:
                        misPiezas[verifX, verifY].setRArriba();
                        break;
                    case 4:
                        misPiezas[verifX, verifY].setRDerecha();
                        break;
                    default:
                        break;
                }
            }

            misPiezas[verifX, verifY].setGrupo(grupo);

            //veo si está en una de las dos paredes
            if (verifX == 0)
            {
                if (!lados && misPiezas[verifX, verifY].getAIzquierda())
                {
                    misPiezas[verifX, verifY].setPiezaUtil(false);
                    return false;
                }

                misPiezas[verifX, verifY].setRIzquierda();
            }
            //la otra pared
            if (verifX == maxAncho)
            {
                if (!lados && misPiezas[verifX, verifY].getADerecha())
                {
                    misPiezas[verifX, verifY].setPiezaUtil(false);
                    return false;
                }

                misPiezas[verifX, verifY].setRDerecha();
            }
            //veo si está en el suelo
            if (verifY == 0)
            {
                if (!suelo && misPiezas[verifX, verifY].getAAbajo())
                {
                    misPiezas[verifX, verifY].setPiezaUtil(false);
                    return false;
                }

                misPiezas[verifX, verifY].setRAbajo();
            }

            //veo si tiene salida para arriba
            if (!misPiezas[verifX, verifY].getRArriba())
            {
                misPiezas[verifX, verifY].setRArriba();

                if (misPiezas[verifX, verifY].getAArriba())
                {
                    //veo si puede haber una pieza arriba
                    if (verifY < maxAlto)
                    {
                        if (!verificarPipe(verifX, verifY + 1, 1, grupo))
                        {
                            misPiezas[verifX, verifY].setPiezaUtil(false);
                            return false;
                        }
                    }
                    //si hay salida para arriba, pero arriba solo puede haber techo
                    else
                    {
                        misPiezas[verifX, verifY].setPiezaUtil(false);
                        return false;
                    }
                }
            }

            //veo si tiene salida para la derecha
            if (!misPiezas[verifX, verifY].getRDerecha())
            {
                misPiezas[verifX, verifY].setRDerecha();

                if (misPiezas[verifX, verifY].getADerecha())
                {
                    //veo si puede haber una pieza a la derecha
                    if (verifX < maxAncho)
                    {
                        if (!verificarPipe(verifX + 1, verifY, 2, grupo))
                        {
                            misPiezas[verifX, verifY].setPiezaUtil(false);
                            return false;
                        }
                    }
                }
            }

            //veo si tiene salida para abajo
            if (!misPiezas[verifX, verifY].getRAbajo())
            {
                misPiezas[verifX, verifY].setRAbajo();

                if (misPiezas[verifX, verifY].getAAbajo())
                {
                    //veo si puede haber una pieza abajo
                    if (verifY > 0)
                    {
                        if (!verificarPipe(verifX, verifY - 1, 3, grupo))
                        {
                            misPiezas[verifX, verifY].setPiezaUtil(false);
                            return false;
                        }
                    }
                }
            }

            //veo si tiene salida para la izquierda
            if (!misPiezas[verifX, verifY].getRIzquierda())
            {
                misPiezas[verifX, verifY].setRIzquierda();

                if (misPiezas[verifX, verifY].getAIzquierda())
                {
                    //veo si puede haber una pieza a la izquierda
                    if (verifX > 0)
                    {
                        if (!verificarPipe(verifX - 1, verifY, 4, grupo))
                        {
                            misPiezas[verifX, verifY].setPiezaUtil(false);
                            return false;
                        }
                    }
                }
            }
        }
        else
        {
            misPiezas[verifX, verifY].setPiezaUtil(false);
            return false;
        }
        
        misPiezas[verifX, verifY].setPiezaUtil(true);
        return true;
    }

    void bajarFicha()
    {
        if (yActual > 0)
        {
            if (!misPiezas[xActual, yActual - 1].getEstaOcupado())
            {
                misPiezas[xActual, yActual - 1].setPieza(misPiezas[xActual, yActual].getQuePiezaSoy(), misPiezas[xActual, yActual].getGrados());
                misPiezas[xActual, yActual].limpiar();
                yActual--;
            }
            else
            {
                if (usandoUnaFicha)
                {
                    if (!misPiezas[xActual, yActual].getEsUnaBomba())
                    {
                        hayQueVerificarPipe = true;
                    }
                    else
                    {
                        explotarBomba();
                    }
                }

                usandoUnaFicha = false;
            }
        }
        else
        {
            if (usandoUnaFicha)
            {
                if (!misPiezas[xActual, yActual].getEsUnaBomba())
                {
                    hayQueVerificarPipe = true;
                }
                else
                {
                    explotarBomba();
                }
            }

            usandoUnaFicha = false;
        }
    }

    void explotarBomba()
    {
        misPiezas[xActual, yActual].destruyendoPieza(false);

        if (xActual > 0)
        {
            misPiezas[xActual - 1, yActual].destruyendoPieza(false);
            if (yActual > 0)
            {
                misPiezas[xActual, yActual - 1].destruyendoPieza(false);
                misPiezas[xActual - 1, yActual - 1].destruyendoPieza(false);
            }
            if (yActual < maxAlto)
            {
                misPiezas[xActual, yActual + 1].destruyendoPieza(false);
                misPiezas[xActual - 1, yActual + 1].destruyendoPieza(false);
            }
        }
        if (xActual < maxAncho)
        {
            misPiezas[xActual + 1, yActual].destruyendoPieza(false);
            if (yActual > 0)
            {
                misPiezas[xActual, yActual - 1].destruyendoPieza(false);
                misPiezas[xActual + 1, yActual - 1].destruyendoPieza(false);
            }
            if (yActual < maxAlto)
            {
                misPiezas[xActual, yActual + 1].destruyendoPieza(false);
                misPiezas[xActual + 1, yActual + 1].destruyendoPieza(false);
            }

        }

        timerParaDestruccion = timer;
        destruyendo = true;
    }

    void verificarTeclas()
    {
        if (usandoUnaFicha)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && xActual > 0 && !misPiezas[xActual - 1, yActual].getEstaOcupado())
            {
                misPiezas[xActual - 1, yActual].setPieza(misPiezas[xActual, yActual].getQuePiezaSoy(), misPiezas[xActual, yActual].getGrados());
                misPiezas[xActual, yActual].limpiar();
                xActual--;
                timerParaTecla = timer;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) && xActual < maxAncho && !misPiezas[xActual + 1, yActual].getEstaOcupado())
            {
                misPiezas[xActual + 1, yActual].setPieza(misPiezas[xActual, yActual].getQuePiezaSoy(), misPiezas[xActual, yActual].getGrados());
                misPiezas[xActual, yActual].limpiar();
                xActual++;
                timerBajandoFicha = timer;
                timerParaTecla = timer;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                misPiezas[xActual, yActual].unGiro();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                bajarFicha();
                timerParaTecla = timer;
            }

            if (timer - timerParaTecla >= tiempoEntreTeclas)
            {
                timerParaTecla = timer;

                if (Input.GetKey(KeyCode.LeftArrow) && xActual > 0 && !misPiezas[xActual - 1, yActual].getEstaOcupado())
                {
                    misPiezas[xActual - 1, yActual].setPieza(misPiezas[xActual, yActual].getQuePiezaSoy(), misPiezas[xActual, yActual].getGrados());
                    misPiezas[xActual, yActual].limpiar();
                    xActual--;
                    timerParaTecla = timer;
                }
                if (Input.GetKey(KeyCode.RightArrow) && xActual < maxAncho && !misPiezas[xActual + 1, yActual].getEstaOcupado())
                {
                    misPiezas[xActual + 1, yActual].setPieza(misPiezas[xActual, yActual].getQuePiezaSoy(), misPiezas[xActual, yActual].getGrados());
                    misPiezas[xActual, yActual].limpiar();
                    xActual++;
                    timerBajandoFicha = timer;
                    timerParaTecla = timer;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    bajarFicha();
                    timerParaTecla = timer;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                while (usandoUnaFicha)
                {
                    bajarFicha();
                }

                timerBajandoFicha = timer;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                cambioShift();
            }
        }
    }

    void cambioShift()
    {
        if (shiftDisponible)
        {
            if (!miShift.getEstaOcupado())
            {
                tengoQueCrearFicha = true;
            }
            shiftDisponible = false;

            int laPieza = misPiezas[xActual, yActual].getQuePiezaSoy();
            int losGrados = misPiezas[xActual, yActual].getGrados();

            misPiezas[xActual, yActual].limpiar();

            misPiezas[xDelSpawn, maxAlto].setPieza(miShift.getQuePiezaSoy(), miShift.getGrados());

            miShift.limpiar();
            miShift.setPieza(laPieza, losGrados);

            xActual = xDelSpawn;
            yActual = maxAlto;
        }
    }

    void iniciarFicha()
    {
        if (level == 0)
        {
            setFichaNext();
            level++;
        }
        if (misPiezas[xDelSpawn, maxAlto].getEstaOcupado())
        {
            jugando = false;
        }
        else
        {
            misPiezas[xDelSpawn, maxAlto].setPieza(miNext.getQuePiezaSoy(), miNext.getGrados());
            miNext.limpiar();

            setFichaNext();

            xActual = xDelSpawn;
            yActual = maxAlto;

            tengoQueCrearFicha = false;
            usandoUnaFicha = true;
        }
    }

    void setFichaNext()
    {
        int num = Random.Range(0, 19);

        switch (num)
        {
            case 0:
                miNext.setPieza(0, 0);
                break;
            case 1:
                miNext.setPieza(0, 90);
                break;
            case 2:
                miNext.setPieza(0, 180);
                break;
            case 3:
                miNext.setPieza(0, 270);
                break;
            case 4:
            case 5:
                miNext.setPieza(1, 0);
                break;
            case 6:
            case 7:
                miNext.setPieza(1, 90);
                break;
            case 8:
                miNext.setPieza(2, 0);
                break;
            case 9:
                miNext.setPieza(2, 90);
                break;
            case 10:
                miNext.setPieza(2, 180);
                break;
            case 11:
                miNext.setPieza(2, 270);
                break;
            case 12:
            case 13:
                miNext.setPieza(3, 0);
                break;
            case 14:
                miNext.setPieza(4, 0);
                break;
            case 15:
                miNext.setPieza(4, 90);
                break;
            case 16:
                miNext.setPieza(4, 180);
                break;
            case 17:
                miNext.setPieza(4, 270);
                break;
            default:
                miNext.setPieza(5, 0);
                break;
        }
    }

    void iniciarFondo()
    {
        for (int i = 0; i < ancho; i++)
        {
            for (int c = 0; c < alto; c++)
            {
                Vector2 posicion = new Vector2(i, c);
                misTilesDelFondo[i, c] = Instantiate(tileFondo, posicion, Quaternion.identity);
                misTilesDelFondo[i, c].transform.parent = miTablero;
                misPiezas[i, c] = misTilesDelFondo[i, c].GetComponent<InfoDePieza>();
            }
        }

        Vector2 nuevoVector = new Vector2(ancho + lejos, alto - lejos / 2);
        tileDelNext = Instantiate(tileFondo, nuevoVector, Quaternion.identity);
        tileDelNext.transform.parent = miTablero;
        miNext = tileDelNext.GetComponent<InfoDePieza>();

        nuevoVector = new Vector2(- lejos, alto - lejos / 2);
        tileDelShift = Instantiate(tileFondo, nuevoVector, Quaternion.identity);
        tileDelShift.transform.parent = miTablero;
        miShift = tileDelShift.GetComponent<InfoDePieza>();
    }

    void agregarFinales()
    {
        if (lados)
        {
            for (int i = 0; i < alto; i++)
            {
                Vector2 posicion = new Vector2(-1, i);
                GameObject unGameObject = Instantiate(tileFinalRojo, posicion, Quaternion.identity);
                unGameObject.transform.parent = miTablero;
            }
            for (int i = 0; i < alto; i++)
            {
                Vector2 posicion = new Vector2(ancho, i);
                GameObject unGameObject = Instantiate(tileFinalRojo, posicion, Quaternion.Euler(0, 0, 180));
                unGameObject.transform.parent = miTablero;
            }
        }

        if (suelo)
        {
            for (int i = 0; i < ancho; i++)
            {
                Vector2 posicion = new Vector2(i, -1);
                GameObject unGameObject = Instantiate(tileFinalRojo, posicion, Quaternion.Euler(0, 0, 90));
                unGameObject.transform.parent = miTablero;
            }
        }
    }

    public void setLados(bool hayLados)
    {
        lados = hayLados;
    }

    public void setSuelo(bool haySuelo)
    {
        suelo = haySuelo;
    }
}
