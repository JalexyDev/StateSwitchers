using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwitcherTimeListener : BaseSpriteSwitcherListener, ITimeReceiver
{
    public List<TimeImageState> States;

    private TimeController timeController;
    private DateTime nextStateDate;

    private TimeController GetTimeController()
    {
        if (timeController == null)
        {
            timeController = TimeController.Instance;
        }

        return timeController;
    }

    protected override List<IState> GetIStatesFromStates()
    {
        return new List<IState>(States);
    }

    public void ReceiveTime(DateTime currentTime)
    {
        if (currentTime >= nextStateDate)
        {
            stateSwitcher.Switch();
            ReceiveTime(currentTime);
        }
    }

    protected override Action GetOnStartSwitchingAction()
    {
        return () =>
        {
            GetTimeController().SubscribeOnTimeUpdates(this);
        };
    }

    protected override Action<IState> GetOnSetCurrentStateAction()
    {
        return (state) =>
        {
            UpdateNextStateDate(state);
        };
    }

    private void UpdateNextStateDate(IState state)
    {
        if (nextStateDate == DateTime.MinValue)
        {
            nextStateDate = GetTimeController().GetCurrentTime().AddSeconds((state as TimeImageState).DurationSeconds);
        }
        else
        {
            nextStateDate = nextStateDate.AddSeconds((state as TimeImageState).DurationSeconds);
        }
    }

    protected override Action GetOnStopSwitchingAction()
    {
        return () =>
        {
            GetTimeController().Unsubscribe(this);
            nextStateDate = DateTime.MinValue;
        };
    }

    private void OnDestroy()
    {
        GetTimeController().Unsubscribe(this);
    }
}
