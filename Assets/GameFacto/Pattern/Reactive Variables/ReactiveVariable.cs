
using System;
using System.Collections.Generic;

public class ReactiveVariable<T> 
{
    public event Action<T,T> Changed;
    private T m_Value;
    private IEqualityComparer<T> m_Comparer;
    public ReactiveVariable(): this (default(T))
    {
        
    }
    public ReactiveVariable(T value): this (value,EqualityComparer<T>.Default)
    {
        
    }

    public ReactiveVariable(T value,IEqualityComparer<T> comparer){
        m_Value = value;
        m_Comparer = comparer;
    }
   

    

    public T Value
    {
        get=>m_Value;
        set
        {
            T oldValue = m_Value;
            m_Value = value;

            if (m_Comparer.Equals(m_Value, oldValue) == false)
            {
                Changed?.Invoke(oldValue,m_Value);
            }
        }

    }

}   

public class ReactiveList<T>
{
    public event Action<T> Added;
    public event Action<T> Removed;

    private List<T> m_elements = new();

    public IReadOnlyList<T> Elements =>m_elements;

    public virtual void Add(T element)
    {
        m_elements.Add(element);
        Added?.Invoke(element);
    }

    public virtual void Remove( T element)
    {
        m_elements.Remove(element);
        Removed?.Invoke(element);
    }

}
public abstract class EqualityComparer<T> : IEqualityComparer<T>
{



    public static EqualityComparer<T> Default{get;}


    public abstract bool Equals(T x, T y);

    public abstract int GetHashCode(T obj);


}