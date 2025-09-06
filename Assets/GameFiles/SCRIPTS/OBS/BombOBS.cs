using UnityEngine;

public class BombOBS : MonoBehaviour
{
    public ParticleSystem collectEffect;
    public Animator DefeatMenu_anim;
    public GameObject Player;
    public GameObject GameOver;
    public GameObject Bomb;
    public AudioSource collectSound;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (collectSound != null)
                collectSound.Play();

            if (collectEffect != null)
                collectEffect.Play();

            Bomb.SetActive(false);
            GameOver.SetActive(true);
            DefeatMenu_anim.SetTrigger("play");
            Player.SetActive(false);
        }
    }
}