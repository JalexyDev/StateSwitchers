using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeStateSwitcher : MonoBehaviour, IStateSwitcher, ITimeReceiver
{
    public List<TimeImageState> States;

    private TimeImageState currentState;
    private TimeController timeController;
    private DateTime nextStateDate;

    // экшн выполняемый при переходе с одной стадии на другую (например смена спрайта, замена награды в этой стадии и т.п.)
    private Action<TimeImageState> betweenStatesAction;
    // экшн на последней стадии. Тут можно переопределить награду, или удалить этот компонент (например у здания после постройки)
    private Action<TimeImageState> lastStateAction;
    // экшн для растений который выполнится когда завершится последняя стадия (например удалится с грядки или вернется к состоянию без плодов)
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

    public void SelectState(int stateIndex)
    {
        if (States.Count > stateIndex)
        {
            hasNext = true;
            var state = States[stateIndex];
            SetCurrent(state);
            betweenStatesAction(state);
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
                    // выполняем действия, которые нужно выполнить после последней стадии (например удалить этот компонент)
                    finishAction(currentState);

                    return;
                }

                SetCurrent(States[index]);

                if (index < States.Count - 1 && betweenStatesAction != null)
                {
                    // выполняем действия, которые нужно выполнить при переходе с одной стадии на другую
                    betweenStatesAction(currentState);
                }
                else if (index == States.Count - 1 && lastStateAction != null)
                {
                    // выполняем действие, которое выполняется на последней стадии (переопределить награду у растений, например)
                    lastStateAction(currentState);
                }

                // если игрок выходил и пропустил несколько стадий, то с помощью рекурсии пробегаемся по всем пройденным стадиям.
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
