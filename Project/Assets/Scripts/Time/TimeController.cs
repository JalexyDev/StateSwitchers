using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private const double MIN_TIME_UPDATE_PERIOD = 1;

    public static TimeController Instance;

    //можно запрашивать с сервера
    private DateTime lastCheckedTime;

    // список подписчиков на время
    private List<ITimeReceiver> receivers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        InitTime();
    }

    private void InitTime()
    {
        if (receivers == null)
        {
            receivers = new List<ITimeReceiver>();
        }

        lastCheckedTime = DateTime.Now;
    }

    private void Update()
    {
        if ((DateTime.Now - lastCheckedTime).TotalSeconds > MIN_TIME_UPDATE_PERIOD)
        {
            lastCheckedTime = DateTime.Now;

            if (receivers != null && receivers.Count > 0)
            {
                // в процессе игры в receivers могут добавляться элементы. Нельзя изменять список во время перебора
                // так что этот список будет включаться в себя всех подписчиков на момент начала перебора
                List<ITimeReceiver> enumeratableReceivers = new List<ITimeReceiver>(receivers);

                foreach (ITimeReceiver receiver in enumeratableReceivers)
                {
                    if (receiver != null)
                    {
                        receiver.ReceiveTime(lastCheckedTime);
                    }
                }
            }
        }
    }

    public void SubscribeOnTimeUpdates(ITimeReceiver receiver)
    {
        if (!receivers.Contains(receiver))
        {
            receivers.Add(receiver);
        }
    }

    public void Unsubscribe(ITimeReceiver receiver)
    {
        if (receivers.Contains(receiver))
        {
            receivers.Remove(receiver);
        }
    }

    public DateTime GetCurrentTime()
    {
        return lastCheckedTime;
    }
}

