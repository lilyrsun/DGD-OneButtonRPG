using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    Animator anim;

    void Awake() => anim = GetComponent<Animator>();

    public void PlayAttack() => anim.SetTrigger("Attack");
}
