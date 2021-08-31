using System;
using System.Collections.Generic;
using UnityEngine;

public class ClickStateSwitcher : MonoBehaviour, IStateSwitcher
{
    public List<ImageState> States;

    private ImageState currentState;

    // экшн выполняемый при переходе с одной стадии на другую (например смена спрайта, замена награды в этой стадии и т.п.)
    private Action<ImageState> betweenStatesAction;
    // экшн на последней стадии. Тут можно переопределить награду, или удалить этот компонент (например у здания после постройки)
    private Action<ImageState> lastStateAction;
    // экшн для растений который выполнится когда завершится последняя стадия (например удалится с грядки или вернется к состоянию без плодов)
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
