using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioClip[] sounds;

    private AudioSource AudioSrc => GetComponent<AudioSource>();

    public void PlaySound(AudioClip clip, float volume = 1f, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    {
        AudioSrc.pitch = Random.Range(p1, p2);
        if (destroyed)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
        AudioSrc.PlayOneShot(clip, volume);
    }
}
