using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterTMP : MonoBehaviour
{
    [TextArea] public string fullText;       // the text to type
    public float charsPerSecond = 30f;       // speed
    public bool autoStart = true;            // type on Start
    public bool preserveRichText = true;     // keep <b>, <color>, etc.
    public AudioSource sfxSource;            // optional
    public AudioClip typeTick;               // optional per-char sound

    TMP_Text tmp;
    Coroutine typing;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        if (string.IsNullOrEmpty(fullText)) fullText = tmp.text; // use current text if field empty
        tmp.text = fullText;
        tmp.maxVisibleCharacters = 0;
    }


    void Start()
    {
        if (autoStart) Play();
    }

    public void Play()
    {
        StopTyping();
        typing = StartCoroutine(TypeRoutine());
    }

    public void SkipToEnd()
    {
        StopTyping();
        tmp.maxVisibleCharacters = tmp.textInfo.characterCount;
    }

    void StopTyping()
    {
        if (typing != null) StopCoroutine(typing);
        typing = null;
    }

    IEnumerator TypeRoutine()
    {
        // Force layout so textInfo is up-to-date
        tmp.ForceMeshUpdate();
        int total = tmp.textInfo.characterCount;
        int visible = 0;

        float secPerChar = 1f / Mathf.Max(1f, charsPerSecond);

        while (visible < total)
        {
            visible++;
            tmp.maxVisibleCharacters = visible;

            if (typeTick && sfxSource) sfxSource.PlayOneShot(typeTick);

            // Optional: slower on punctuation
            char c = tmp.textInfo.characterInfo[Mathf.Clamp(visible - 1, 0, total - 1)].character;
            float delay = secPerChar * ((c == '.' || c == '!' || c == '?') ? 3f :
                                        (c == ',' || c == ';' || c == ':') ? 1.5f : 1f);

            yield return new WaitForSeconds(delay);
        }
    }
}
