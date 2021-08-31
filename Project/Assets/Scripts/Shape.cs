using System;
using UnityEngine;

public class Shape : MonoBehaviour
{
    private ClickStateSwitcher stateSwitcher;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        stateSwitcher = GetComponent<ClickStateSwitcher>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        if (stateSwitcher != null)
        {
            stateSwitcher.SetStateActions(
                betweenStatesAction: GetBetweenStateAction(),
                lastStateAction: GetLastStateAction(),
                finishAction: GetFinishStateAction()
            );

            stateSwitcher.StartSwitching();
        }
    }

    private void OnMouseDown()
    {
        stateSwitcher.OnClick();
    }

    private Action<ImageState> GetBetweenStateAction()
    {
        return (TimeState) =>
        {
            SetSprite(TimeState.Sprite);
        };
    }

    private void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    private Action<ImageState> GetLastStateAction()
    {
        return (TimeState) =>
        {
            SetSprite(TimeState.Sprite);
        };
    }

    private Action<ImageState> GetFinishStateAction()
    {
        return (TimeState) =>
        {
            SelectState(0);
        };
    }

    private void SelectState(int stateNumber)
    {
        if (stateSwitcher != null)
        {
            stateSwitcher.SelectState(stateNumber);
        }
    }
}
