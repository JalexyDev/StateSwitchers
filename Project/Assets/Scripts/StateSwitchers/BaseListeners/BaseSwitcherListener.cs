using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateSwitcher))]
public abstract class BaseSwitcherListener : MonoBehaviour
{
    protected StateSwitcher stateSwitcher;

    private void Awake()
    {
        stateSwitcher = GetComponent<StateSwitcher>();
        OnInit();
    }

    protected virtual void OnInit() { }

    void Start()
    {
        if (stateSwitcher != null)
        {
            stateSwitcher.SetStateActions(
                betweenStatesAction: GetBetweenStateAction(),
                lastStateAction: GetLastStateAction(),
                finishAction: GetFinishStateAction()
            );
            stateSwitcher.SetOnStartSwitchingAction(GetOnStartSwitchingAction());
            stateSwitcher.SetOnSetCurrentStateAction(GetOnSetCurrentStateAction());
            stateSwitcher.SetOnStopSwitchingAction(GetOnStopSwitchingAction());

            stateSwitcher.SetStates(GetIStatesFromStates());
            OnStart();
        }
    }

    protected virtual void OnStart()
    {

    }

    protected abstract Action<IState> GetBetweenStateAction();

    protected abstract List<IState> GetIStatesFromStates();

    protected abstract Action<IState> GetLastStateAction();

    protected abstract Action<IState> GetFinishStateAction();

    protected virtual Action GetOnStartSwitchingAction()
    {
        return null;
    }

    protected virtual Action<IState> GetOnSetCurrentStateAction()
    {
        return null;
    }

    protected virtual Action GetOnStopSwitchingAction()
    {
        return null;
    }

    protected void SelectState(int stateNumber)
    {
        if (stateSwitcher != null)
        {
            stateSwitcher.SelectState(stateNumber);
        }
    }
}
