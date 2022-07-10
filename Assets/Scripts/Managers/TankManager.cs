using System;
using UnityEngine;

[Serializable] //Hace que los atributos aparezcan en el inspector (si no los escondemos)
public class TankManager
{
    //Esta clase gestiona la configuración del tanque junto con el GameManager
    //gestiona el comportamiento de los tanques y si los jugadores tienen cont
    //  rol sobre el tanque
    //en los distintos momentos del juego
    public Color m_PlayerColor;
    public Transform m_SpawnPoint; //Posición y direción en la que se generará el tanque
    [HideInInspector] public int m_PlayerNumber; //Especifica con qué jugador está actuando el Game Manager 
    [HideInInspector] public string m_ColoredPlayerText; ///String que reprsenta el color del tanque
    [HideInInspector] public GameObject m_Instance; //Refernecia a la instancia del tanque cuando se crea
    [HideInInspector] public int m_Wins; ///Número de victorias del jugador 


    private TankMovement m_Movement; ///Referencia al script de movimiento deltanque. Utilizado para deshabilitar y habilitar el control
    private TankShooting m_Shooting; //Referencia al script de disparo del tanque. Utilizado para deshabilitar y habilitar el contro
    private GameObject m_CanvasGameObject; //Utilizado para deshabilitar el UI del mundo durante als fases de inicio y fin de cada ronda


    public void Setup()
    {
        //Cojo referencias de los componentes
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
        // /Ajusto los número de jugadores para que sean iguales en todos los scripts

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        ///Creo un string usando el color del tanque que diga PLAYER 1, etc.
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
        //Cojo todos los renderers del tanque
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }

    ///Usado durante la fases del juego en las que el jugador no debe poder controlar el tanque
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    //Usado durante la fases del juego en las que el jugador debe poder controlar el tanque
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    ///Usado al inicio de cada ronda para poner el tanque en su estado inicial
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
