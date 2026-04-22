using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.IoC;

namespace Content.Client.Crocaine;

public sealed class ColoredBlurStripesOverlay : Overlay
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override OverlaySpace Space => OverlaySpace.ScreenSpace;
    private readonly ShaderInstance _shader;

    // Эту переменную будет плавно менять Система
    public float CurrentIntensity = 0f;

    public ColoredBlurStripesOverlay()
    {
        IoCManager.InjectDependencies(this);
        _shader = _prototypeManager.Index<ShaderPrototype>("ColoredBlurStripes").InstanceUnique();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var screenTexture = args.Viewport.RenderTarget.Texture;
        var handle = args.ScreenHandle;

        _shader.SetParameter("SCREEN_TEXTURE", screenTexture);
        _shader.SetParameter("intensity", CurrentIntensity);

        handle.UseShader(_shader);
        handle.DrawRect(args.ViewportBounds, Color.White);
        handle.UseShader(null);
    }
}
