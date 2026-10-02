using UnityEngine;

public class DragonSoundController : MonoBehaviour
{
    [SerializeField] private DragonSoundsSO dragonSoundsSO;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBasicAttack()
    {
        audioSource.PlayOneShot(dragonSoundsSO.basicAttack);
    }
    public void PlayClawAttack()
    {
        audioSource.PlayOneShot(dragonSoundsSO.clawAttack);
    }
    public void PlayFlameAttack()
    {
        audioSource.PlayOneShot(dragonSoundsSO.flameAttack);
    }
    public void PlayFlyingFlameAttack()
    {
        audioSource.PlayOneShot(dragonSoundsSO.flyFlameAttack);
    }
    public void PlayRoar()
    {
        audioSource.PlayOneShot(dragonSoundsSO.roar);
    }
    public void PlayGrowl()
    {
        audioSource.PlayOneShot(dragonSoundsSO.growl);
    }
    public void PlayHurt()
    {
        // Debug.Log("Sound Hurt");
        audioSource.PlayOneShot(dragonSoundsSO.hurt);
    }
    public void PlayFly()
    {
        audioSource.PlayOneShot(dragonSoundsSO.fly);
    }
    public void PlayLand()
    {
        audioSource.PlayOneShot(dragonSoundsSO.land);
    }
    public void PlayDie()
    {
        audioSource.PlayOneShot(dragonSoundsSO.die);
    }

    public void StopSound()
    {
        audioSource.Stop();
    }
}
