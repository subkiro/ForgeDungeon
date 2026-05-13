
using System;
using System.Collections.Generic;
using UnityEngine;
public class ReactiveVariable<T>
{
    public event Action<T, T> Changed;

    private T m_Value;

    public T Value
    {
        get => m_Value;
        set
        {
            if (m_Value is IEquatable<T> equatable)
            {
                if (equatable.Equals(value)) return;
            }
            else
            {
                if (Equals(m_Value, value)) return;
            }

            T oldValue = m_Value;
            m_Value = value;
            Changed?.Invoke(oldValue, m_Value);
        }
    }

    public ReactiveVariable() { }

    public ReactiveVariable(T value)
    {
        m_Value = value;
    }

    public void SetWithoutNotify(T value)
    {
        m_Value = value;
    }

    public static implicit operator T(ReactiveVariable<T> variable)
        => variable != null ? variable.Value : default;
}
public class ReactiveList<T>
{
    public event Action<T> OnAdded;
    public event Action<T> OnRemoved;

    private List<T> m_elements = new();

    public IReadOnlyList<T> Elements => m_elements;

    public virtual void Add(T element)
    {
        m_elements.Add(element);
        OnAdded?.Invoke(element);
    }

    public virtual void Remove(T element)
    {
        m_elements.Remove(element);
        OnRemoved?.Invoke(element);
    }

}

public class ReactiveStackableDictionary<T>
{
    public event Action<KeyValuePair<T, int>, int> OnAdded;
    public event Action<KeyValuePair<T, int>, int> OnRemoved;
    private Dictionary<T, int> m_elements = new();

    public IReadOnlyDictionary<T, int> Elements => m_elements;

    public virtual void Add(T key, int value = 1)
    {

        if (m_elements.ContainsKey(key))
        {
            m_elements[key] += value;
        }
        else
        {
            m_elements.Add(key, value);
        }

        var valuePair = new KeyValuePair<T, int>(key, m_elements[key]);
        OnAdded?.Invoke(valuePair, value);
    }

    public virtual void Remove(T key, int amount = 1)
    {
        if (amount == 0) return;

        if (m_elements.ContainsKey(key))
        {
            int totalValue = m_elements[key];
            if (totalValue > amount)
            {

                totalValue -= amount;
                m_elements[key] = totalValue;
            }
            else
            {
                m_elements.Remove(key);
                totalValue = 0;

            }
            var valuePair = new KeyValuePair<T, int>(key, totalValue);

            OnRemoved?.Invoke(valuePair, amount);
        }

    }


}



public abstract class EqualityComparer<T> : IEqualityComparer<T>
{



    public static EqualityComparer<T> Default { get; }


    public abstract bool Equals(T x, T y);

    public abstract int GetHashCode(T obj);


}