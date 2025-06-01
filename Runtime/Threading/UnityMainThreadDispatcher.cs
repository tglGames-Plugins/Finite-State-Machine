using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace TGL.FSM.Threads
{
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher _instance;
        private readonly ConcurrentQueue<Action> _actions = new();

        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<UnityMainThreadDispatcher>(FindObjectsInactive.Include);
                    if (_instance == null)
                    {
                        _instance = new GameObject(nameof(UnityMainThreadDispatcher)).AddComponent<UnityMainThreadDispatcher>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }

                return _instance;
            }
        }

        void Update()
        {
            if (_actions.IsEmpty)
            {
                return;
            }
            
            while (_actions.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }

        public void Enqueue(Action action)
        {
            _actions.Enqueue(action);
        }
    }
}