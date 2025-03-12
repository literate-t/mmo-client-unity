using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    List<T> _heap = new List<T>();

    // O(logN)
    public void Push(T data)
    {
        // 힙의 맨 끝에 새로운 데이터를 삽입한다
        _heap.Add(data);

        int now = _heap.Count - 1;
        // up-heap : 자식에서 시작해 부모 노드와 비교한다
        while (now > 0)
        {
            // 도장깨기를 시도
            int next = (now - 1) / 2;
            if (_heap[now].CompareTo(_heap[next]) < 0)
                break;

            // 두 값을 교체한다
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;

            // 검사 위치를 이동한다
            now = next;
        }
    }

    // O(logN)
    public T Pop()
    {
        // 반환할 데이터를 따로 저장
        T ret = _heap[0];

        // 마지막 데이터를 루트로 이동한다
        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        lastIndex--;

        // down-heap : 새로운 루트로 시작해 자식 노드들과 비교한다
        int now = 0;
        while (true)
        {
            int left = 2 * now + 1;
            int right = 2 * now + 2;

            int next = now;
            // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동
            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                next = left;
            // 오른값이 현재값(왼쪽 이동 포함)보다 크면, 오른쪽으로 이동
            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                next = right;

            // 값 변화가 없다면 더 이상 정렬이 필요 없음
            if (next == now)
                break;

            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;

            // 새 루트 지정
            now = next;
        }

        return ret;
    }

    public int Count { get { return _heap.Count; } }
}
