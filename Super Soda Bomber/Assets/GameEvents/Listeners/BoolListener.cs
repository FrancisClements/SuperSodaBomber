/*
Void Event
    **Concrete Game Listener, from BaseGameListener

    An event listener (receiver) that returns a boolean.
    This is used for setting events using the inspector.
*/

namespace SuperSodaBomber.Events{
    public class BoolListener : BaseGameListener
        <bool, BoolEvent, UnityBoolEvent>{}
}
