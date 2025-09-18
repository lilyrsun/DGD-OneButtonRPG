using UnityEngine;
using UnityEngine.SceneManagement;
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource source;
    [Range(0f, 1f)] public float musicVolume = 0.6f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (!source) source = GetComponent<AudioSource>();
        source.volume = musicVolume;
        source.loop = true;
        source.Play();
    }

    public void SetVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        if (source) source.volume = musicVolume;
    }

    public void FadeTo(float targetVol, float duration = 1f) =>
        StartCoroutine(FadeRoutine(targetVol, duration));

    System.Collections.IEnumerator FadeRoutine(float target, float dur)
    {
        float start = source.volume, t = 0f;
        while (t < dur) { t += Time.deltaTime; source.volume = Mathf.Lerp(start, target, t / dur); yield return null; }
        source.volume = target;
    }
}
