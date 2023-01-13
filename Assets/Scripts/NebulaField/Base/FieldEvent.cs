using System;

public static class FieldEvent
{

    // public delegate void OnReinitializeFieldsAction();
    // public static event OnReinitializeFieldsAction OnReinitializeFields;

    public static Action OnReinitializeFields;

    public static void Init()
    {
        // OnReinitializeFields = null;
    }
}
