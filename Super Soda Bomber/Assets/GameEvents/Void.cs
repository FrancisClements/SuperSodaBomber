/*
Void
    A class that contains nothing. 

    It's required because of the GameListener (receiver) and GameEvents (sender)
    are both requiring types.

    Void.cs is both nothing and something.
*/

namespace SuperSodaBomber.Events{
    [System.Serializable]
    public struct Void{}
}