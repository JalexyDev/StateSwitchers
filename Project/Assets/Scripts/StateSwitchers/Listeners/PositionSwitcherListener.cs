using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSwitcherListener : BaseSwitcherListener
{
    public Transform selectedObject;
    public List<PositionState> positionStates;

    private Vector3 startPosition;
    private bool isObjectOnStart;

    protected override void OnInit()
    {
        startPosition = selectedObject.position;
        isObjectOnStart = true;
    }

    protected override List<IState> GetIStatesFromStates()
    {
        return new List<IState>(positionStates);
    }

    protected override Action<IState> GetBetweenStateAction()
    {
        return (state) =>
        {
            selectedObject.position = (state as PositionState).position;
        };
    }

    protected override Action<IState> GetLastStateAction()
    {
        return (state) =>
        {
            selectedObject.position = (state as PositionState).position;
            selectedObject.localScale *= 2;
        };
    }

    protected override Action<IState> GetFinishStateAction()
    {
        return (state) =>
        {
            selectedObject.position = startPosition;
            selectedObject.localScale /= 2;
            isObjectOnStart = true;
        };
    }

    public void SwitchPos()
    {
        if (isObjectOnStart)
        {
            isObjectOnStart = false;
            SelectState(0);
        }
        else
        {
            stateSwitcher.Switch();
        }
    }
}
