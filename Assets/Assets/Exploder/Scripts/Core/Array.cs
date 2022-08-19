// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

namespace Exploder
{
    /// <summary>
    /// array list with
    /// </summary>
    /// <typeparam name="T">item of array</typeparam>
    public class Array<T>
    {
        private T[] array;

        private int size;
        private int index;

        public Array(int size)
        {
            this.array = new T[size];
            this.size = size;
            this.index = 0;
        }

        public void Initialize(int newSize)
        {
            if (newSize > size)
            {
                ExploderUtils.Log("Initialize, resising: " + size + " => " + newSize);
                array = new T[newSize];
                size = newSize;
            }

            Clear();
        }

        public int Count
        {
            get
            {
                return index;
            }
        }

        public T this[int key]
        {
            get
            {
                ExploderUtils.Assert(key < size, "Key index out of range! " + key + " maxSize: " + size);
                return array[key];
            }
        }

        public void Clear()
        {
            for (int i = 0; i < index; i++)
            {
                array[i] = default(T);
            }

            index = 0;
        }

        public void Add(T data)
        {
            array[index++] = data;

            if (index >= size)
            {
                ExploderUtils.Log("Resizing Array: " + size + " => " + size*2);

                var array2 = new T[size*2];
                for (int i = 0; i < size; i++)
                {
                    array2[i] = array[i];
                }

                this.array = array2;
            }
        }

        public void Reverse()
        {
            for (var i = 0; i < index / 2; i++)
            {
                var tmp = array[i];
                array[i] = array[index - i - 1];
                array[index - i - 1] = tmp;
            }
        }
    }
}
