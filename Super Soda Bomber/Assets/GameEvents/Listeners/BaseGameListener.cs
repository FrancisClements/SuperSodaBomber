using UnityEngine;
using UnityEngine.Events;

/*
BaseGameListener
    The base class for Game Listeners (receiver).

    A script that lets other components of the 
    GameObject to be called by a GameEvent (sender)
    using UnityEvent.
*/

namespace SuperSodaBomber.Events{
    public abstract class BaseGameListener<T, E, UER> : MonoBehaviour, IGameEventListener<T>
        where E: BaseGameEvent<T>
        where UER: UnityEvent<T>
    {
        public E gameEvent;
        public E GameEvent { get; set; }
        public UER unityEventResponse;

        //add to list if the attached scene is on use...
        void OnEnable(){
            if (gameEvent != null)
                gameEvent.RegisterListener(this);
        }

        //...remove it otherwise
        void OnDisable(){
            if (gameEvent != null)
                gameEvent.UnregisterListener(this);
        }

        public void OnEventRaised(T item){
            if (unityEventResponse != null){
                unityEventResponse.Invoke(item);
            }
        }

    }
}
