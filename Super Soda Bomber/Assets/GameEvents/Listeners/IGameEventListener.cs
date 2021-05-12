/*
IGameEventListener
    An interface that is used and required on every Game Listeners (receivers)
    with a type T as a filter.

    Ex: T as Void.cs (IGameEventListener<Void>)
*/

namespace SuperSodaBomber.Events{
    public interface IGameEventListener<T>{
        void OnEventRaised(T item);
    }
}