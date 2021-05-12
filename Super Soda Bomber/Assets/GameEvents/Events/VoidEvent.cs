using UnityEngine;

/*
Void Event
    **Concrete Game Event, from BaseGameEvent
    A sender that returns nothing in return.

    This will be used for creating ScriptableObjects at the Editor
    and triggers for scripts.
*/

namespace SuperSodaBomber.Events{
    [CreateAssetMenu(fileName = "New Void Event", menuName = "GameEvents/VoidEvent")]
    public class VoidEvent : BaseGameEvent<Void>
    {
        public void Raise() => Raise(new Void());
    }
}
