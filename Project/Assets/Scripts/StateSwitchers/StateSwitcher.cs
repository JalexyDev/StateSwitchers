using System;
using System.Collections.Generic;
using UnityEngine;

public class StateSwitcher : MonoBehaviour
{
    private List<IState> states;
    private IState currentState;

    // экшн выполняемый при переходе с одной стадии на другую (например смена спрайта, замена награды в этой стадии и т.п.)
    private Action<IState> betweenStatesAction;
    // экшн на последней стадии. Тут можно переопределить награду, или удалить этот компонент (например у здания после постройки)
    private Action<IState> lastStateAction;
    // экшн для растений который выполнится когда завершится последняя стадия (например удалится с грядки или вернется к состоянию без плодов)
    private Action<IState> finishAction;
    // экшн который вызывается перед началом переключения состояний
    private Action onStartSwitchingAction;
    // экшн который вызываем при выборе любого нового состояния
    private Action<IState> onSetCurrentStateAction;
    // экшн который вызывается при окончании переключения состояний
    private Action onStopSwitchingAction;

    private bool hasNext;

    public void SetStates(List<IState> states)
    {
        this.states = states;
    }

    public void SetStateActions(
        Action<IState> betweenStatesAction,
        Action<IState> lastStateAction,
        Action<IState> finishAction
    )
    {
        this.betweenStatesAction = betweenStatesAction;
        this.lastStateAction = lastStateAction;
        this.finishAction = finishAction;
    }

    public void SetOnStartSwitchingAction(Action action)
    {
        onStartSwitchingAction = action;
    }

    public void SetOnSetCurrentStateAction(Action<IState> action)
    {
        onSetCurrentStateAction = action;
    }

    public void SetOnStopSwitchingAction(Action action)
    {
        onStopSwitchingAction = action;
    }

    public void StartSwitching(int stateIndex = 0)
    {
        onStartSwitchingAction?.Invoke();
        SelectState(stateIndex);
    }

    public void SelectState(int stateIndex)
    {
        if (states.Count > stateIndex)
        {
            hasNext = true;
            var state = states[stateIndex];
            SetCurrent(states[stateIndex]);
            betweenStatesAction(state);
        }
    }

    private void SetCurrent(IState state)
    {
        currentState = state;
        onSetCurrentStateAction?.Invoke(state);
    }

    public void Switch()
    {
        if (hasNext)
        {
            int index = states.IndexOf(currentState) + 1;

            if (index >= states.Count && finishAction != null)
            {
                hasNext = false;
                // выполняем действия, которые нужно выполнить после последней стадии (например удалить этот компонент)
                finishAction(currentState);

                return;
            }

            SetCurrent(states[index]);

            if (index < states.Count - 1 && betweenStatesAction != null)
            {
                // выполняем действия, которые нужно выполнить при переходе с одной стадии на другую
                betweenStatesAction(currentState);
            }
            else if (index == states.Count - 1 && lastStateAction != null)
            {
                // выполняем действие, которое выполняется на последней стадии (переопределить награду у растений, например)
                lastStateAction(currentState);
            }
        }
        else
        {
            StopSwitching();
        }
    }

    public void StopSwitching()
    {
        currentState = null;
        hasNext = false;
        onStopSwitchingAction?.Invoke();
    }
}
