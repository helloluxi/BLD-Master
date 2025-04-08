using System;
using System.Collections.Generic;
namespace CS;

public class PriorityQueue<T>(int capacity, IComparer<T> comparer)
{
    private readonly IComparer<T> comparator = comparer ?? Comparer<T>.Default;
    private T[] queue = new T[capacity];
    public int Count = 0, ModCount = 0;

    public void Push(T t)
    {
        ModCount++;
        int i = Count;
        if (i >= queue.Length)
        {
            var copy = queue;
            queue = new T[queue.Length + (queue.Length >> 1)];
            for (int j = 0; j < queue.Length; j++)
                queue[j] = copy[j];
        }
        Count = i + 1;
        if (i == 0)
            queue[0] = t;
        else
            SiftUp(i, t);
    }
    public T Pop()
    {
        if (Count == 0)
            throw new InvalidOperationException();
        int s = --Count;
        ModCount++;
        T result = queue[0];
        T x = queue[s];
        queue[s] = default;
        if (s != 0)
            SiftDown(0, x);
        return result;
    }
    public void Clear()
    {
        ModCount++;
        for (int i = 0; i < Count; i++)
            queue[i] = default;
        Count = 0;
    }
    public T[] ToArray()
    {
        var copy = new T[Count];
        for (int i = 0; i < Count; i++)
            copy[i] = queue[i];
        return copy;
    }
    private void SiftUp(int k, T x)
    {
        while (k > 0)
        {
            int parent = (k - 1) >> 1;
            T t = queue[parent];
            if (comparator.Compare(x, t) >= 0)
                break;
            queue[k] = t;
            k = parent;
        }
        queue[k] = x;
    }
    private void SiftDown(int k, T x)
    {
        int half = Count >> 1;
        while (k < half)
        {
            int child = (k << 1) + 1;
            T t = queue[child];
            int right = child + 1;
            if (right < Count &&
                comparator.Compare(t, queue[right]) > 0)
                t = queue[child = right];
            if (comparator.Compare(x, t) <= 0)
                break;
            queue[k] = t;
            k = child;
        }
        queue[k] = x;
    }
}
