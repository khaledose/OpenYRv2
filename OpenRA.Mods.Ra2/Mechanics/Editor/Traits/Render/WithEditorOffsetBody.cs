using OpenRA.Mods.Common.Traits.Render;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;
using OpenRA.Mods.Common.Graphics;
using OpenRA.Graphics;

namespace OpenRA.Mods.Ra2.Mechanics.Editor.Traits.Render;
public class WithEditorOffsetBodyInfo : TraitInfo, IRenderActorPreviewSpritesInfo, IEditorActorOptions
{
	public override object Create(ActorInitializer init)
	{
		return new WithEditorOffsetBody();
	}

	public IEnumerable<EditorActorOption> ActorOptions(ActorInfo ai, World world)
	{
		yield return new EditorActorSlider("X Offset", 10, -2048, 2048, 2,
			actor =>
			{
				var init = actor.GetInitOrDefault<XOffsetInit>();
				return init?.Value ?? 0;
			},
			(actor, value) => actor.ReplaceInit(new XOffsetInit((int)value)));

		yield return new EditorActorSlider("Y Offset", 11, -2048, 2048, 2,
			actor =>
			{
				var init = actor.GetInitOrDefault<YOffsetInit>();
				return init?.Value ?? 0;
			},
			(actor, value) => actor.ReplaceInit(new YOffsetInit((int)value)));

		yield return new EditorActorSlider("Z Offset", 12, -2048, 2048, 2,
			actor =>
			{
				var init = actor.GetInitOrDefault<ZOffsetInit>();
				return init?.Value ?? 0;
			},
			(actor, value) => actor.ReplaceInit(new ZOffsetInit((int)value)));
	}

	public IEnumerable<IActorPreview> RenderPreviewSprites(ActorPreviewInitializer init, string image, int facings, PaletteReference p)
	{
		var anim = new Animation(init.World, image);
		var body = init.Actor.TraitInfoOrDefault<WithSpriteBodyInfo>();
		anim.PlayRepeating(RenderSprites.NormalizeSequence(anim, init.GetDamageState(), body?.Sequence ?? "idle"));

		var offset = () =>
		{
			var x = init.GetOrDefault<XOffsetInit>();
			var y = init.GetOrDefault<YOffsetInit>();
			var z = init.GetOrDefault<ZOffsetInit>();

			return new WVec(x?.Value ?? 0, y?.Value ?? 0, z?.Value ?? 0);
		};

		yield return new SpriteActorPreview(anim, offset, () => 0, p);
	}
}

public class WithEditorOffsetBody { }

public class XOffsetInit : ValueActorInit<int>, ISingleInstanceInit
{
	public XOffsetInit(int value)
		: base(value) { }
}

public class YOffsetInit : ValueActorInit<int>, ISingleInstanceInit
{
	public YOffsetInit(int value)
		: base(value) { }
}

public class ZOffsetInit : ValueActorInit<int>, ISingleInstanceInit
{
	public ZOffsetInit(int value)
		: base(value) { }
}
