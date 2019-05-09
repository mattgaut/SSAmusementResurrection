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

    public class Event {
        public delegate void EventDelegate();
        List<EventDelegate> callbacks;

        public Event() {
            callbacks = new List<EventDelegate>();
        }
        public Event(Event e) {
            callbacks = new List<EventDelegate>(e.callbacks);
        }

        public void Invoke() {
            foreach (EventDelegate ed in callbacks) {
                ed.Invoke();
            }
        }

        public static Event operator +(Event e, EventDelegate callback){
            Event new_event = new Event(e);
            new_event.callbacks.Add(callback);
            return new_event;
        }
        public static Event operator -(Event e, EventDelegate callback) {
            Event new_event = new Event(e);
            new_event.callbacks.Remove(callback);
            return new_event;
        }
    }

    public class Event<T1> {
        public delegate void EventDelegate(T1 arg1);
        List<EventDelegate> callbacks;

        public Event() {
            callbacks = new List<EventDelegate>();
        }
        public Event(Event<T1> e) {
            callbacks = new List<EventDelegate>(e.callbacks);
        }

        public void Invoke(T1 arg1) {
            foreach (EventDelegate ed in callbacks) {
                ed.Invoke(arg1);
            }
        }

        public static Event<T1> operator +(Event<T1> e, EventDelegate callback) {
            Event<T1> new_event = new Event<T1>(e);
            new_event.callbacks.Add(callback);
            return new_event;
        }
        public static Event<T1> operator -(Event<T1> e, EventDelegate callback) {
            Event<T1> new_event = new Event<T1>(e);
            new_event.callbacks.Remove(callback);
            return new_event;
        }
    }

    public class Event<T1, T2> {


    }

    public class Event<T1, T2, T3> {


    }

    public class Event<T1, T2, T3, T4> {


    }

    public class Event<T1, T2, T3, T4, T5>{


    }
}