using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          //Cantidad de salud con la que empeiza el tanque
    public Slider m_Slider;//Slider que representa la salud del tanque
    public Image m_FillImage; //COmponente de imagen del slider
    public Color m_FullHealthColor = Color.green;//Color del slider con salud completa(verde)
    public Color m_ZeroHealthColor = Color.red; //Color del slider con salud vacía(rojo)
    public GameObject m_ExplosionPrefab;//Prefab que instanciamos al inicio y usamos cuando el tanque se muere


    private AudioSource m_ExplosionAudio; //La fuenta de audio a reporudcir cuando el tanque explota
    private ParticleSystem m_ExplosionParticles; //Sistema de partículas que se reproducen al destruir el tanque
    private float m_CurrentHealth; //Variable para almacenar la salud del tanque
    private bool m_Dead; //Variable para comprobar si el tanque tiene salud


    private void Awake()
    {
        //Instanciamos el prefab de la explosión
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        //Refrencia de la fuente de audio para la explosión
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        //Deshabilitimas el sistema de partículas de la explosión (para activarlo cuando explote)
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        //Al habilitar el tanque, reseteamos la salud y el booleano de si está muerto o no
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        //Actualizamos el slider de salud (valor y color)
        SetHealthUI();
    }


    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        // Reducimos la salud según la cantidad de daño recibida.
        m_CurrentHealth -= amount;

        // Actualizamos el slider de salud con esos valores
        SetHealthUI();

        //Si la salud es menor que 0 y aún no lo he explotado, llamo al método OnDeath (al morir).
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = m_CurrentHealth;

        // Creo un color para el slider entre verde y rojo en función del porcentaje de salud
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        // Configuro el booleano a true para asegurarme de que explota solo una vez
        m_Dead = true;
        //Coloco el prefab de explosión en la posición actual del tanque y lo activo.
        m_ExplosionParticles.transform.position = transform.position;

        m_ExplosionParticles.gameObject.SetActive(true);
        //Reproduzco el sistema de partículas del tanque explotando.
        m_ExplosionParticles.Play();
        //Reproduzco el audio del tanque explotando.
        m_ExplosionAudio.Play();
        //Desactivo el tanque.
        gameObject.SetActive(false);
    }
}