using Robust.Shared.GameStates;

namespace Content.Shared.Crocaine;

[RegisterComponent, NetworkedComponent]
public sealed partial class CrocaineVisualsComponent : Component
{
    [DataField]
    public float TimeLeft = 2f;
}
