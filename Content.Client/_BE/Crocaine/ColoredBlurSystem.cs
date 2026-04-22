using System;
using Content.Shared.Crocaine;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Crocaine;

public sealed class ColoredBlurSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private float _targetIntensity = 0f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CrocaineVisualsComponent, ComponentInit>(OnEffectInit);
        SubscribeLocalEvent<CrocaineVisualsComponent, ComponentRemove>(OnEffectRemove);
        SubscribeLocalEvent<CrocaineVisualsComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<CrocaineVisualsComponent, PlayerDetachedEvent>(OnPlayerDetached);
    }

    private void OnEffectInit(EntityUid uid, CrocaineVisualsComponent component, ComponentInit args) => CheckAndEnable(uid);
    private void OnEffectRemove(EntityUid uid, CrocaineVisualsComponent component, ComponentRemove args) => CheckAndDisable(uid);
    private void OnPlayerAttached(EntityUid uid, CrocaineVisualsComponent component, PlayerAttachedEvent args) => CheckAndEnable(uid);
    private void OnPlayerDetached(EntityUid uid, CrocaineVisualsComponent component, PlayerDetachedEvent args) => CheckAndDisable(uid);

    private void CheckAndEnable(EntityUid uid)
    {
        if (_playerManager.LocalEntity == uid)
        {
            _targetIntensity = 1f;

            if (!_overlayManager.HasOverlay<ColoredBlurStripesOverlay>())
                _overlayManager.AddOverlay(new ColoredBlurStripesOverlay());
        }
    }

    private void CheckAndDisable(EntityUid uid)
    {
        if (_playerManager.LocalEntity == uid)
        {
            _targetIntensity = 0f;
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_overlayManager.TryGetOverlay<ColoredBlurStripesOverlay>(out var overlay))
        {
            var fadeSpeed = 0.5f * frameTime;

            // Используем MathF вместо MathHelper!
            if (overlay.CurrentIntensity < _targetIntensity)
                overlay.CurrentIntensity = MathF.Min(overlay.CurrentIntensity + fadeSpeed, _targetIntensity);
            else if (overlay.CurrentIntensity > _targetIntensity)
                overlay.CurrentIntensity = MathF.Max(overlay.CurrentIntensity - fadeSpeed, _targetIntensity);

            if (_targetIntensity == 0f && overlay.CurrentIntensity <= 0f)
            {
                _overlayManager.RemoveOverlay<ColoredBlurStripesOverlay>();
            }
        }
    }
}
