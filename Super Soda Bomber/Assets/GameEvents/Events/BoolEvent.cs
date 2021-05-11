using UnityEngine;

/*
Void Event
    **Concrete Game Event, from BaseGameEvent
    A sender that returns a boolean.

    This will be used for creating ScriptableObjects at the Editor
    and triggers for scripts.
*/

namespace SuperSodaBomber.Events{
    [CreateAssetMenu(fileName = "New Bool Event", menuName = "GameEvents/BoolEvent")]
    public class BoolEvent : BaseGameEvent<bool>{}
}
