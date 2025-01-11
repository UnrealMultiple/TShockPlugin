using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LazyAPI.Utility;

public class CircularQueue<T> : IEnumerable<T>
{
    private readonly T[] _array;
    private int _head;
    private int _tail;
    private int _size;
    private readonly int _capacity;

    public CircularQueue(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be positive.", nameof(capacity));
        }

        this._capacity = capacity;
        this._array = new T[this._capacity];
        this._head = 0;
        this._tail = 0;
        this._size = 0;
    }

    public void Enqueue(T item)
    {
        if (this._size == this._capacity)
        {
            this._head = (this._head + 1) % this._capacity;
        }
        else
        {
            this._size++;
        }
        this._array[this._tail] = item;
        this._tail = (this._tail + 1) % this._capacity;
    }

    public bool TryDequeue([MaybeNullWhen(false)] out T item)
    {
        if (this._size == 0)
        {
            item = default;
            return false;
        }
        item = this._array[this._head];
        this._head = (this._head + 1) % this._capacity;
        this._size--;
        return true;
    }

    private void ThrowForEmptyQueue()
    {
        throw new InvalidOperationException("this queue is empty!");
    }

    public T Dequeue()
    {
        if (this._size == 0)
        {
            this.ThrowForEmptyQueue();
        }
        var item = this._array[this._head];
        this._head = (this._head + 1) % this._capacity;
        this._size--;
        return item;
    }

    public bool TryPeek([MaybeNullWhen(false)] out T item)
    {
        if (this._size == 0)
        {
            item = default;
            return false;
        }
        item = this._array[this._head];
        return true;
    }

    public T Peek()
    {
        if (this._size == 0)
        {
            this.ThrowForEmptyQueue();
        }
        return this._array[this._head];
    }

    public bool Contains(T item)
    {
        if (this._size == 0)
        {
            return false;
        }

        if (this._head < this._tail)
        {
            return Array.IndexOf(this._array, item, this._head, this._size) >= 0;
        }

        // We've wrapped around. Check both partitions, the least recently enqueued first.
        return
            Array.IndexOf(this._array, item, this._head, this._array.Length - this._head) >= 0 ||
            Array.IndexOf(this._array, item, 0, this._tail) >= 0;
    }

    public void Clear()
    {
        this._head = 0;
        this._tail = 0;
        this._size = 0;
        Array.Clear(this._array, 0, this._capacity);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < this._size; i++)
        {
            yield return this._array[(this._head + i) % this._capacity];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}