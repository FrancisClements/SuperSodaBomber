/*
Void Event
    **Concrete Game Listener, from BaseGameListener

    An event listener (receiver) that returns an integer.
    This is used for setting events using the inspector.
*/

namespace SuperSodaBomber.Events{
    public class SceneListener : BaseGameListener
        <SceneIndex, SceneEvent, UnitySceneEvent>{}
}
