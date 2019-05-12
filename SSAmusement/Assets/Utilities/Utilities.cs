using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities {

    public class Pair<T, U> {
        public Pair() {
        }

        [SerializeField] T _first;
        [SerializeField] U _second;

        public Pair(T first, U second) {
           _first = first;
           _second = second;
        }

        public T first { get { return _first; } set { _first = value; } }
        public U second { get { return _second; } set { _second = value; } }
    };

    public class Lock {
        HashSet<int> locks;

        int next_lock;

        public Lock() {
            locks = new HashSet<int>();
            next_lock = int.MinValue;
        }

        public bool locked {
            get { return locks.Count > 0; }
        }

        public bool unlocked {
            get { return locks.Count == 0; }
        }

        public void Clear() {
            locks.Clear();
        }

        public int AddLock() {
            int lock_value = next_lock++;
            locks.Add(lock_value);
            return lock_value;
        }

        public bool RemoveLock(int lock_value) {
            return locks.Remove(lock_value);
        }
    }
}