using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly UnityEvent<Figure> OnObjectSelected = new UnityEvent<Figure>();
    public static readonly UnityEvent OnGameWin = new UnityEvent();
    public static readonly UnityEvent OnGameLose = new UnityEvent();

    public static void SetObjectSelected(Figure value)
    {
        OnObjectSelected.Invoke(value);
    }

    public static void SetGameWin()
    {
        OnGameWin.Invoke();
    }
    public static void SetGameLose()
    {
        OnGameLose.Invoke();
    }
}
