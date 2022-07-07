using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask; //Usado para filtrar a qué afecta la explosión de la bomba.Debería ajustarse a "Players"
    public ParticleSystem m_ExplosionParticles; //Referencia a las partículas que se reproducirán en la explosión
    public AudioSource m_ExplosionAudio;//Referencia al audio que se reproducirá en la explosión
    public float m_MaxDamage = 100f; //Cantidad de daño si la explosiónestá centrada en el tanque
    public float m_ExplosionForce = 1000f; //Cantidad de fuerza añadida al tanque en el centro de la explosión
    public float m_MaxLifeTime = 2f; //Tiempo de vida en segundos de la bomba
    public float m_ExplosionRadius = 5f; //Radio máximo desde la explosión para calcular los tanques que se verán afectados


    private void Start()
    {
        //Si no se ha destruido aún, destruir la bomba después de su tiempo de vida
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        // Recoge los colliders en una esfera desde la posición de la bomba con el radio máximo.

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        //Recorro los colliders
        for (int i = 0; i < colliders.Length; i++)
        {
            // Selecciono su Rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            //SI no tienen, paso al siguiente.
            if (!targetRigidbody) continue;
            // Añado la fiuerza de la explosión.
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
            // Busco el script TankHealth asociado con el Rigidbody.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            //SI no hay script TankHealth, paso al siguiente.
            if (!targetHealth) continue;
            //Calculo el daño a aplicar en función de la distancia a la bomba.
            float damage = CalculateDamage(targetRigidbody.position);
            //Aplico el daño al tanque.
            targetHealth.TakeDamage(damage);
        }
        //Desacnlo el sistema de aprticulas de la bomba.
        m_ExplosionParticles.transform.parent = null;

        // Reproduczco el sistema de pertículas.
        m_ExplosionParticles.Play();
        //Reproduzco el audio.
        m_ExplosionAudio.Play();
        //Cuando las partículas han terminado, destruyo su objeto asociado.
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        //Destruyo la bomba.
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        //Creo un vector desde la bomba al objetivo.
        Vector3 explosionToTarget = targetPosition - transform.position;
        // Calculo la distancia desde la bomba al objetivo.
        float explosionDistance = explosionToTarget.magnitude;
        //Calculo la proporción de máxima distancia (radio máximo) desde la explosión al tanque.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        //Calculo el daño a esa proporción.
        float damage = relativeDistance * m_MaxDamage;
        // Me aseguro de que el mínimo daño siempre es 0.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}