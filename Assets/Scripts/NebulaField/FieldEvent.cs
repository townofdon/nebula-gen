using System;

public static class FieldEvent
{
    public static Action OnReinitializeFields;

    public static void Init()
    {
        OnReinitializeFields = null;
    }
}
