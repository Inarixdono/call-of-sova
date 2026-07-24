using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class ArmsAnimationController : MonoBehaviour
{
    private static readonly int FireHash = Animator.StringToHash("Fire");
    private static readonly int FireAltHash = Animator.StringToHash("FireAlt");
    private static readonly int ReloadHash = Animator.StringToHash("Reload");
    private static readonly int MeleeHash = Animator.StringToHash("Melee");
    private static readonly int DrawHash = Animator.StringToHash("Draw");
    private static readonly int HideWeaponHash = Animator.StringToHash("HideWeapon");

    private Animator animator;
    private bool isHidden;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            animator.SetTrigger(FireAltHash);
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            animator.SetTrigger(MeleeHash);
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleWeapon();
        }
    }

    private void ToggleWeapon()
    {
        if (isHidden)
        {
            animator.SetTrigger(DrawHash);
        }
        else
        {
            animator.SetTrigger(HideWeaponHash);
        }

        isHidden = !isHidden;
    }
}
