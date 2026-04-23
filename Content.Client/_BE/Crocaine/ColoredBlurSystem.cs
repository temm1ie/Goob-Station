using System;
using Content.Shared.Crocaine;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Crocaine;

public sealed class ColoredBlurSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private float _currentIntensity = 0f;
    private float _targetIntensity = 0f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CrocaineVisualsComponent, ComponentInit>(OnClientInit);
        SubscribeLocalEvent<CrocaineVisualsComponent, ComponentShutdown>(OnClientShutdown);
        SubscribeLocalEvent<PlayerAttachedEvent>(OnPlayerAttached);
    }

    private void OnClientInit(EntityUid uid, CrocaineVisualsComponent component, ComponentInit args) => CheckAndEnable(uid);
    private void OnClientShutdown(EntityUid uid, CrocaineVisualsComponent component, ComponentShutdown args) => CheckAndDisable(uid);
    private void OnPlayerAttached(PlayerAttachedEvent args) { if (HasComp<CrocaineVisualsComponent>(args.Entity)) CheckAndEnable(args.Entity); }

    private void CheckAndEnable(EntityUid uid)
    {
        if (_playerManager.LocalEntity == uid)
        {
            _targetIntensity = 1f;
            if (!_overlayManager.HasOverlay<ColoredBlurStripesOverlay>())
                _overlayManager.AddOverlay(new ColoredBlurStripesOverlay());
        }
    }

    private void CheckAndDisable(EntityUid uid) { if (_playerManager.LocalEntity == uid) _targetIntensity = 0f; }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var fadeSpeed = 0.5f * frameTime;
        if (_currentIntensity < _targetIntensity)
            _currentIntensity = MathF.Min(_currentIntensity + fadeSpeed, 1f);
        else if (_currentIntensity > _targetIntensity)
            _currentIntensity = MathF.Max(_currentIntensity - fadeSpeed, 0f);

        if (_overlayManager.TryGetOverlay<ColoredBlurStripesOverlay>(out var overlay))
        {
            overlay.CurrentIntensity = _currentIntensity;
            if (_targetIntensity == 0f && _currentIntensity <= 0f)
                _overlayManager.RemoveOverlay<ColoredBlurStripesOverlay>();
        }

        var query = EntityQueryEnumerator<HumanoidAppearanceComponent, SpriteComponent>();
        while (query.MoveNext(out var uid, out var humanoid, out var sprite))
        {

            bool shouldHide = HasComp<CrocaineVisualsComponent>(uid) || (uid == _playerManager.LocalEntity && _currentIntensity > 0.001f);

            if (!shouldHide)
                continue;

            foreach (HumanoidVisualLayers layer in Enum.GetValues(typeof(HumanoidVisualLayers)))
            {

                if (sprite.LayerMapTryGet(layer, out int idx))
                    sprite.LayerSetVisible(idx, false);

                if (sprite.LayerMapTryGet($"marking-{layer}", out int mIdx))
                    sprite.LayerSetVisible(mIdx, false);
            }
        }
    }
}
