using UnityEngine;
using UnityEngine.Audio;

public class Carrot : MonoBehaviour
{
    [SerializeField] AudioSource m_audioSource;
    [SerializeField] ParticleSystem m_particleSystem;

    void OnTriggerEnter2D(Collider2D _collision)
    {
        //Check whether it is touching the player
        if (!_collision.GetComponent<Player>()) return;

        //Play Sound Effect and Particle Animation
        m_audioSource.gameObject.SetActive(true);
        m_audioSource.Play();
        m_particleSystem.Play();
        m_audioSource.transform.parent = transform.parent;
        Destroy(m_particleSystem.gameObject, Mathf.Max(m_audioSource.clip.length, m_particleSystem.main.duration));

        //Add to collected carrots and destroy asset
        FindObjectOfType<GameManager>().AddCarrot(1U);
        Destroy(gameObject);
    }
}
