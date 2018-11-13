using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Musica : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Musica");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
