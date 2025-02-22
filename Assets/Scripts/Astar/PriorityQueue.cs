using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> _elements = new();

    public int Count => _elements.Count;

    public void Enqueue(T element)
    {
        _elements.Add(element);
        _elements.Sort();
    }

    public T Dequeue()
    {
        T item = _elements[0];
        _elements.RemoveAt(0);
        return item;
    }
}