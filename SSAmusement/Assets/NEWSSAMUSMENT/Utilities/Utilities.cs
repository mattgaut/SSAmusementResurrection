using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities {
    public class Pair<T, U> {
        public Pair() {
        }

        public Pair(T first, U second) {
            this.first = first;
            this.second = second;
        }

        public T first { get; set; }
        public U second { get; set; }
    };

    public class Lock {
        HashSet<int> locks;

        public Lock() {
            locks = new HashSet<int>();
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
            int lock_value = Random.Range(int.MinValue, int.MaxValue);
            while (locks.Contains(lock_value)) {
                lock_value = Random.Range(int.MinValue, int.MaxValue);
            }
            locks.Add(lock_value);
            return lock_value;
        }

        public bool RemoveLock(int lock_value) {
            return locks.Remove(lock_value);
        }
    }
}