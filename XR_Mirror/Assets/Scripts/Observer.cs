using System;
using UnityEngine;

[Serializable]
public class Observer<T>
{
    [SerializeField] public T value = default;
    public event Action<T> OnValueChanged;

    public T Value
    {
        get => value;
        set => Set(value);
    }

    public Observer(T value)
    {
        this.value = value;
        OnValueChanged = default;
    }

    public void Set(T value)
    {
        this.value = value;
        InvokeChanges();
    }

    public void InvokeChanges()
    {
        OnValueChanged?.Invoke(value);
    }

    public void AddListener(Action<T> callback) => OnValueChanged += callback;

    public void RemoveListener(Action<T> callback) => OnValueChanged -= callback;

    public static implicit operator T(Observer<T> observer) => observer.value;
}
