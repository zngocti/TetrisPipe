using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDePieza : MonoBehaviour {

    bool estaOcupado = false;
    bool esUnaBomba = false;

    bool abiertoArriba = false;
    bool abiertoAbajo = false;
    bool abiertoIzqueirda = false;
    bool abiertoDerecha = false;

    bool revisadoArriba = false;
    bool revisadoAbajo = false;
    bool revisadoIzquierda = false;
    bool revisadoDerecha = false;

    bool revisadoTodo = false;
    bool piezaUtil = false;

    public GameObject codo;
    public GameObject horizontal;
    public GameObject t;
    public GameObject cruz;
    public GameObject final;
    public GameObject bomba;
    public GameObject efectoBlanco;
    public GameObject efectoRojo;

    int quePiezaSoy = -1;
    int cuantosGradosTengo = 0;

    static public float tiempoDeDestruccion = 0.5f;
    float timerDestruccion = 0;
    bool destruyendo = false;

    int miGrupo = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (destruyendo)
        {
            timerDestruccion = timerDestruccion - Time.deltaTime;
            if (timerDestruccion <= 0)
            {
                limpiar();
            }
        }
	}

    public void setGrupo(int grupo)
    {
        miGrupo = grupo;
    }

    public int getGrupo()
    {
        return miGrupo;
    }

    public static float getTiempoDeDestrucción()
    {
        return tiempoDeDestruccion;
    }

    public void resetearEstadoRevision()
    {
        revisadoArriba = false;
        revisadoAbajo = false;
        revisadoIzquierda = false;
        revisadoDerecha = false;

        revisadoTodo = false;
        piezaUtil = false;

        miGrupo = 0;
    }

    public void destruyendoPieza(bool esEfectoBlanco)
    {
        if (esEfectoBlanco)
        {
            efectoBlanco.SetActive(true);
        }
        else
        {
            efectoRojo.SetActive(true);
        }
        timerDestruccion = tiempoDeDestruccion;
        destruyendo = true;
    }

    public void setPieza(int forma, int grados)
    {
        if (forma != -1)
        {
            quePiezaSoy = forma;
            estaOcupado = true;

            switch (forma)
            {
                case 0:
                    codo.SetActive(true);
                    abiertoAbajo = true;
                    abiertoIzqueirda = true;
                    break;
                case 1:
                    horizontal.SetActive(true);
                    abiertoIzqueirda = true;
                    abiertoDerecha = true; ;
                    break;
                case 2:
                    t.SetActive(true);
                    abiertoIzqueirda = true;
                    abiertoDerecha = true;
                    abiertoAbajo = true;
                    break;
                case 3:
                    cruz.SetActive(true);
                    abiertoIzqueirda = true;
                    abiertoDerecha = true;
                    abiertoArriba = true;
                    abiertoAbajo = true;
                    break;
                case 4:
                    final.SetActive(true);
                    abiertoDerecha = true;
                    break;
                case 5:
                    bomba.SetActive(true);
                    esUnaBomba = true;
                    break;
                default:
                    break;
            }

            int num = grados / 90;

            for (int i = 0; i < num; i++)
            {
                unGiro();
            }
        }
    }

    public int getQuePiezaSoy()
    {
        return quePiezaSoy;
    }

    public int getGrados()
    {
        return cuantosGradosTengo;
    }

    public void limpiar()
    {
        switch (quePiezaSoy)
        {
            case 0:
                codo.transform.rotation = Quaternion.identity;
                codo.SetActive(false);
                break;
            case 1:
                horizontal.transform.rotation = Quaternion.identity;
                horizontal.SetActive(false);
                break;
            case 2:
                t.transform.rotation = Quaternion.identity;
                t.SetActive(false);
                break;
            case 3:
                cruz.transform.rotation = Quaternion.identity;
                cruz.SetActive(false);
                break;
            case 4:
                final.transform.rotation = Quaternion.identity;
                final.SetActive(false);
                break;
            case 5:
                bomba.transform.rotation = Quaternion.identity;
                bomba.SetActive(false);
                break;
            default:
                break;
        }

        efectoBlanco.SetActive(false);
        efectoRojo.SetActive(false);

        esUnaBomba = false;

        abiertoArriba = false;
        abiertoAbajo = false;
        abiertoIzqueirda = false;
        abiertoDerecha = false;

        revisadoArriba = false;
        revisadoAbajo = false;
        revisadoIzquierda = false;
        revisadoDerecha = false;

        quePiezaSoy = -1;
        cuantosGradosTengo = 0;
        estaOcupado = false;

        revisadoTodo = false;
        piezaUtil = false;
        destruyendo = false;

        timerDestruccion = 0;
        miGrupo = 0;
}

    public bool getPiezaUtil()
    {
        return piezaUtil;
    }

    public void setPiezaUtil(bool esUtil)
    {
        piezaUtil = esUtil;
        if (!revisadoTodo)
        {
            forceRevisadoTodo();
        }
    }

    public bool getRevisadoTodo()
    {
        return revisadoTodo;
    }

    public void setRevisadoTodo()
    {
        if (revisadoAbajo && revisadoArriba && revisadoIzquierda && revisadoDerecha)
        {
            revisadoTodo = true;
        }
    }

    void forceRevisadoTodo()
    {
        revisadoAbajo = true;
        revisadoArriba = true;
        revisadoIzquierda = true;
        revisadoDerecha = true;

        revisadoTodo = true;
    }

    //revisados o no

    public bool getRArriba()
    {
        return revisadoArriba;
    }

    public void setRArriba()
    {
        revisadoArriba = true;
        setRevisadoTodo();
    }

    public bool getRAbajo()
    {
        return revisadoAbajo;
    }

    public void setRAbajo()
    {
        revisadoAbajo = true;
        setRevisadoTodo();
    }

    public bool getRIzquierda()
    {
        return revisadoIzquierda;
    }

    public void setRIzquierda()
    {
        revisadoIzquierda = true;
        setRevisadoTodo();
    }

    public bool getRDerecha()
    {
        return revisadoDerecha;
    }

    public void setRDerecha()
    {
        revisadoDerecha = true;
        setRevisadoTodo();
    }

    //abierto o cerrado

    public bool getAArriba()
    {
        return abiertoArriba;
    }

    public bool getAAbajo()
    {
        return abiertoAbajo;
    }

    public bool getADerecha()
    {
        return abiertoDerecha;
    }

    public bool getAIzquierda()
    {
        return abiertoIzqueirda;
    }

    public void unGiro()
    {
        switch (quePiezaSoy)
        {
            case 0:
                codo.transform.Rotate(0, 0, 90);
                break;
            case 1:
                horizontal.transform.Rotate(0, 0, 90);
                break;
            case 2:
                t.transform.Rotate(0, 0, 90);
                break;
            case 3:
                cruz.transform.Rotate(0, 0, 90);
                break;
            case 4:
                final.transform.Rotate(0, 0, 90);
                break;
            case 5:
                bomba.transform.Rotate(0, 0, 90);
                break;
            default:
                break;
        }

        cuantosGradosTengo += 90;

        bool temporal = abiertoAbajo;
        abiertoAbajo = abiertoIzqueirda;
        abiertoIzqueirda = abiertoArriba;
        abiertoArriba = abiertoDerecha;
        abiertoDerecha = temporal;
    }

    public bool getEstaOcupado()
    {
        return estaOcupado;
    }

    public bool getEsUnaBomba()
    {
        return esUnaBomba;
    }
}
