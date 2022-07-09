using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1; //Para identificar a los diferentes jugadorzes
    public Rigidbody m_Shell; //Prefab de la bomba
    public Transform m_FireTransform; //Hijo del tanque en el que se generará la bomba (desde donde se lanzará)
    public Slider m_AimSlider; //Hijo del tanque que muestra la fuerza de lanzamiento de la bomba
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f; //Fuerza mínima de dipsaro (si no se mantiene presionado el botón de disparo)
    public float m_MaxLaunchForce = 30f; //Fuerza máxima de dipsaro (si se mantiene presionado el botón de disparo hasta la máxima carga)
    public float m_MaxChargeTime = 0.75f; //Tiempo máximo de carga antes de ser lanzado el disparo con máxima fuerza


    private string m_FireButton; //Eje de disparo utilizado para lanzar las bombas
    private float m_CurrentLaunchForce; //Fuerza dada a la bomba cuando se suelta el botón de disparo
    private float m_ChargeSpeed; //Velocidad de carga, basasda en el máximo tiempo de carga
    private bool m_Fired; //Booleano que comprueba si se ha lanzado la bomba


    private void OnEnable()
    {
        // reset launch force and ui
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        // El eje de disparo basado en el número de jugador
        m_FireButton = "Fire" + m_PlayerNumber;
        //Velocidad de carga, basasda en el máximo tiempo de carga y los valores de carga máximo y mínimo
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }

        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}