using System;
using System.Collections;

public class TimeKeeper
{
    private float WaitTime;
    private float value;
    private Func<IEnumerator> FunctionToExecute;

    public TimeKeeper(float seconds, Func<IEnumerator> func)
    {
        WaitTime = seconds;
        FunctionToExecute = func;
        value = 0;
    }

    public IEnumerator ReturnOrIncrease(float increaseValue)
    {
        if ((value += increaseValue) > WaitTime)
        {
            value = 0;
            yield return FunctionToExecute();
        }
        yield return null;
    }
}