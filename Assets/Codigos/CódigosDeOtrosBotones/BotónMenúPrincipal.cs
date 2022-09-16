using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotónMenúPrincipal : MonoBehaviour
{
    public int numeroEscena;

    public void MenuPrincipal()
    {
        SceneManager.LoadScene(numeroEscena);
    }
}
