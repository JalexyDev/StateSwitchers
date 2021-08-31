using System;
using System.Collections.Generic;
using UnityEngine;

public class ClickStateSwitcher : MonoBehaviour, IStateSwitcher
{
    public List<ImageState> States;

    private ImageState currentState;

    // ���� ����������� ��� �������� � ����� ������ �� ������ (�������� ����� �������, ������ ������� � ���� ������ � �.�.)
    private Action<ImageState> betweenStatesAction;
    // ���� �� ��������� ������. ��� ����� �������������� �������, ��� ������� ���� ��������� (�������� � ������ ����� ���������)
    private Action<ImageState> lastStateAction;
    // ���� ��� �������� ������� ���������� ����� ���������� ��������� ������ (�������� �������� � ������ ��� �������� � ��������� ��� ������)
    private Action<ImageState> finishAction;

    private bool hasNext;

    public void SetStateActions(
    Action<ImageState> betweenStatesAction,
    Action<ImageState> lastStateAction,
    Action<ImageState> finishAction
    )
    {
        this.betweenStatesAction = betweenStatesAction;
        this.lastStateAction = lastStateAction;
        this.finishAction = finishAction;
    }

    public void StartSwitching(int fromIndex = 0)
    {
        if (currentState == null && States.Count > 0)
        {
            hasNext = true;
            SetCurrent(States[fromIndex]);
            betweenStatesAction(currentState);
        }
    }

    public void SelectState(int stateIndex)
    {
        hasNext = true;
        var state = States[stateIndex];
        SetCurrent(state);
        betweenStatesAction(state);
    }

    private void SetCurrent(IState state)
    {
        currentState = state as ImageState;
    }

    public void OnClick()
    {
        if (hasNext)
        {
            int index = States.IndexOf(currentState) + 1;

            if (index >= States.Count && finishAction != null)
            {
                hasNext = false;
                // ��������� ��������, ������� ����� ��������� ����� ��������� ������ (�������� ������� ���� ���������)
                finishAction(currentState);

                return;
            }

            SetCurrent(States[index]);

            if (index < States.Count - 1 && betweenStatesAction != null)
            {
                // ��������� ��������, ������� ����� ��������� ��� �������� � ����� ������ �� ������
                betweenStatesAction(currentState);
            }
            else if (index == States.Count - 1 && lastStateAction != null)
            {
                // ��������� ��������, ������� ����������� �� ��������� ������ (�������������� ������� � ��������, ��������)
                lastStateAction(currentState);
            }
        }
        else
        {
            ClearData();
        }
    }

    private void ClearData()
    {
        currentState = null;
    }
}
