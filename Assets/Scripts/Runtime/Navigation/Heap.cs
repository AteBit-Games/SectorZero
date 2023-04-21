using System;

namespace Runtime.Navigation
{

    public interface IHeapItem<in T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }

    public class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] _elements;
        public int Count { get; private set; }

        public Heap(int maxHeapSize) => _elements = new T[maxHeapSize];
        public bool Contains(T item) => Equals(_elements[item.HeapIndex], item);
        
        public void Add(T item) {
            item.HeapIndex = Count;
            _elements[Count] = item;
            SortUp(item);
            Count++;
        }
        
        public T RemoveFirst() {
            T firstItem = _elements[0];
            Count--;
            _elements[0] = _elements[Count];
            _elements[0].HeapIndex = 0;
            SortDown(_elements[0]);
            return firstItem;
        }

        private void SortUp(T item) {
            int parentIndex = ( item.HeapIndex - 1 ) / 2;
            while ( true ) {
                T parentItem = _elements[parentIndex];
                if ( item.CompareTo(parentItem) > 0 ) {
                    Swap(item, parentItem);
                } else { break; }
                parentIndex = ( item.HeapIndex - 1 ) / 2;
            }
        }

        private void SortDown(T item) {
            while ( true ) {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                if ( childIndexLeft < Count ) {
                    var swapIndex = childIndexLeft;
                    if ( childIndexRight < Count ) {
                        if ( _elements[childIndexLeft].CompareTo(_elements[childIndexRight]) < 0 ) {
                            swapIndex = childIndexRight;
                        }
                    }
                    if ( item.CompareTo(_elements[swapIndex]) < 0 ) {
                        Swap(item, _elements[swapIndex]);
                    } else { return; }
                } else { return; }
            }
        }

        private void Swap(T itemA, T itemB) {
            _elements[itemA.HeapIndex] = itemB;
            _elements[itemB.HeapIndex] = itemA;
            (itemA.HeapIndex, itemB.HeapIndex) = (itemB.HeapIndex, itemA.HeapIndex);
        }
    }
}