using UnityEngine;
using DG.Tweening;

public class ControllerAnim : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Transform controller;
    [SerializeField] private Transform targetControllerPos;
    [Space]
    [SerializeField] private Transform upButton;
    [SerializeField] private Transform downButton;
    [SerializeField] private Transform submitButton;
    [Space]
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSFX;

    private void OnEnable()
    {
        MainMenuManager.onMoveUp += PressUp;
        MainMenuManager.onMoveDown += PressDown;
        MainMenuManager.onSubmit += PressSubmit;

        MainMenuManager.onPlay += MoveController;
    }

    private void OnDisable()
    {
        MainMenuManager.onMoveUp -= PressUp;
        MainMenuManager.onMoveDown -= PressDown;
        MainMenuManager.onSubmit -= PressSubmit;

        MainMenuManager.onPlay -= MoveController;
    }

    void PressUp() => Animate(upButton);
    void PressDown() => Animate(downButton);
    void PressSubmit() => Animate(submitButton);

    private void Animate(Transform _button)
    {
        _button.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(_button.DOScale(0.85f, 0.08f));
        seq.Append(_button.DOScale(1f, 0.12f));

        if (clickSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSFX);
        }
    }

    private void MoveController()
    {
        controller.DOKill();

        Vector3 targetRotation = controller.localEulerAngles;
        targetRotation.z = 0f;

        Sequence seq = DOTween.Sequence();

        seq.Join(controller.DOLocalMove(targetControllerPos.transform.position, 1f).SetEase(Ease.OutCubic));

        seq.Join(controller.DOLocalRotate(targetRotation, 1f).SetEase(Ease.OutCubic));
    }
}
