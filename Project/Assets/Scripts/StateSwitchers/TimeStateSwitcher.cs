using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeStateSwitcher : MonoBehaviour, IStateSwitcher, ITimeReceiver
{
    public List<TimeImageState> States;

    private TimeController timeController;
    private TimeImageState currentState;
    private DateTime nextStateDate;

    // ���� ����������� ��� �������� � ����� ������ �� ������ (�������� ����� �������, ������ ������� � ���� ������ � �.�.)
    private Action<TimeImageState> betweenStatesAction;
    // ���� �� ��������� ������. ��� ����� �������������� �������, ��� ������� ���� ��������� (�������� � ������ ����� ���������)
    private Action<TimeImageState> lastStateAction;
    // ���� ��� �������� ������� ���������� ����� ���������� ��������� ������ (�������� �������� � ������ ��� �������� � ��������� ��� ������)
    private Action<TimeImageState> finishAction;

    private bool hasNext;

    public void SetStateActions(
    Action<TimeImageState> betweenStatesAction,
    Action<TimeImageState> lastStateAction,
    Action<TimeImageState> finishAction
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

        GetTimeController().SubscribeOnTimeUpdates(this);
    }

    private TimeController GetTimeController()
    {
        if (timeController == null)
        {
            timeController = TimeController.Instance;
        }

        return timeController;
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
        currentState = state as TimeImageState;
        UpdateNextStateDate();
    }

    private void UpdateNextStateDate()
    {
        if (nextStateDate == DateTime.MinValue)
        {
            nextStateDate = GetTimeController().GetCurrentTime().AddSeconds(currentState.DurationSeconds);
        }
        else
        {
            nextStateDate = nextStateDate.AddSeconds(currentState.DurationSeconds);
        }
    }

    public void ReceiveTime(DateTime currentTime)
    {
        if (hasNext)
        {
            if (currentTime >= nextStateDate)
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

                // ���� ����� ������� � ��������� ��������� ������, �� � ������� �������� ����������� �� ���� ���������� �������.
                ReceiveTime(currentTime);
            }
        }
        else
        {
            GetTimeController().Unsubscribe(this);
            ClearData();
        }
    }

    private void ClearData()
    {
        currentState = null;
        nextStateDate = DateTime.MinValue;
    }

    private void OnDestroy()
    {
        GetTimeController().Unsubscribe(this);
    }
}
