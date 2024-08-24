using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Agit.FortressCraft {

    public class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> executeOnMainThread = new Queue<Action>();
        private static MainThreadDispatcher instance;

        public static MainThreadDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject("MainThreadDispatcher");
                    instance = obj.AddComponent<MainThreadDispatcher>();
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }

        public void Update()
        {
            while (executeOnMainThread.Count > 0)
            {
                executeOnMainThread.Dequeue().Invoke();
            }
        }

        public static void Enqueue(Action action)
        {
            if (action == null) return;

            executeOnMainThread.Enqueue(action);
        }
    }
}

