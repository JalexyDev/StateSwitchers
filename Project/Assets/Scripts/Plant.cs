using System;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private TimeStateSwitcher stateSwitcher;
    private SpriteRenderer spriteRenderer;
    
    // ����� ������ "�������� ��� ������"
    public int stateIndexWithoutProduct;

    private void Awake()
    {
        stateSwitcher = GetComponent<TimeStateSwitcher>();
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

    private Action<TimeImageState> GetBetweenStateAction()
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

    private Action<TimeImageState> GetLastStateAction()
    {
        return (TimeState) =>
        {
            SetSprite(TimeState.Sprite);
        };
    }

    private Action<TimeImageState> GetFinishStateAction()
    {
        return (TimeState) =>
        {
            //todo ������� ����� �� �������� �������� ��� ������
            SelectState(stateIndexWithoutProduct);
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
