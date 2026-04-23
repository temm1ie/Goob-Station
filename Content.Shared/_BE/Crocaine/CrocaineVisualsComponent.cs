using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Crocaine;

[RegisterComponent, NetworkedComponent]
public sealed partial class CrocaineVisualsComponent : Component
{
    [DataField] public float TimeLeft = 2f;
}

[Serializable, NetSerializable]
public enum CrocaineVisuals : byte
{
    IsInvisible
}
