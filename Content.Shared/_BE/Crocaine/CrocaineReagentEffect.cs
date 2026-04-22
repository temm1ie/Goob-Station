using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Crocaine;

[DataDefinition]
public sealed partial class CrocaineReagentEffect : EntityEffect
{
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => "It causes powerful visual hallucinations and blurred vision.";

    public override void Effect(EntityEffectBaseArgs args)
    {
        var comp = args.EntityManager.EnsureComponent<CrocaineVisualsComponent>(args.TargetEntity);
        comp.TimeLeft = 2f;
    }
}
