namespace Content.Shared.Crocaine;

public sealed class CrocaineVisualsSystem : EntitySystem
{
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<CrocaineVisualsComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            comp.TimeLeft -= frameTime;
            if (comp.TimeLeft <= 0)
            {
                RemComp<CrocaineVisualsComponent>(uid);
            }
        }
    }
}
