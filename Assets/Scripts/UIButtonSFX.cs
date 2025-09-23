using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource sfxSource;   // drag in an AudioSource (2D, not looping)
    public AudioClip hoverClip;
    public AudioClip clickClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sfxSource && hoverClip)
            sfxSource.PlayOneShot(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sfxSource && clickClip)
            sfxSource.PlayOneShot(clickClip);
    }
}
