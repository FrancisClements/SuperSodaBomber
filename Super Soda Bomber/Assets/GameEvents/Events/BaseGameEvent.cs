using System.Collections.Generic;
using UnityEngine;

/*
BaseGameEvent
    The base class for Game Events (sender).

    A ScriptableObject that is responsible for adding, removing and calling the
    listeners (receivers) inside the list.
*/

namespace SuperSodaBomber.Events{
    public abstract class BaseGameEvent<T> : ScriptableObject
    {
        public List<IGameEventListener<T>> listeners = new List<IGameEventListener<T>>();

        public void Raise(T item){
            for (int i = listeners.Count - 1; i >= 0 ; i--)
            {
                //loops the listener
                listeners[i].OnEventRaised(item);
            }
        }

        public void RegisterListener(IGameEventListener<T> l){
            if(!listeners.Contains(l))
                listeners.Add(l);
        }

        public void UnregisterListener(IGameEventListener<T> l){
            if(listeners.Contains(l))
                listeners.Remove(l);
        }
    }
}
