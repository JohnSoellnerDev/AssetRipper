using AssetRipper.Assets;
using AssetRipper.GUI.Web.Paths;
using AssetRipper.SourceGenerated.Classes.ClassID_21;
using AssetRipper.SourceGenerated.Classes.ClassID_28;
using AssetRipper.SourceGenerated.Subclasses.UnityTexEnv;
using AssetRipper.SourceGenerated.Extensions;
using AssetRipper.Primitives;

namespace AssetRipper.GUI.Web.Pages.Assets;

internal sealed class MaterialTab : AssetHtmlTab
{
	public override string DisplayName => Localization.AssetTabMaterial;

	public override string HtmlName => "material";

	public override bool Enabled => Asset is IMaterial;

	private readonly string? _albedoUrl;
	private readonly string? _normalUrl;

	public MaterialTab(IUnityObjectBase asset, AssetPath path) : base(asset)
	{
		if (asset is IMaterial material)
		{
			GetTextures(material, out ITexture2D? mainTexture, out ITexture2D? normalTexture);

			if (mainTexture is not null)
			{
				_albedoUrl = AssetAPI.GetImageUrl(mainTexture.GetPath(), "png");
			}
			if (normalTexture is not null)
			{
				_normalUrl = AssetAPI.GetImageUrl(normalTexture.GetPath(), "png");
			}
		}
	}

	public override void Write(TextWriter writer)
	{
		using (new Table(writer).WithCustomAttribute("width", "100%" ).WithCustomAttribute("height", "100%" ).End())
		{
			using (new Tbody(writer).End())
			{
				using (new Tr(writer).End())
				{
					using (new Td(writer).WithAlign("center").WithCustomAttribute("valign", "middle").End())
					{
						Canvas canvas = new Canvas(writer)
							.WithId("babylonMaterialCanvas")
							.WithStyle("width: 100%; height: 100vh;");
						if (_albedoUrl is not null)
						{
							canvas.WithCustomAttribute("albedo-url", _albedoUrl);
						}
						if (_normalUrl is not null)
						{
							canvas.WithCustomAttribute("normal-url", _normalUrl);
						}
						canvas.Close();
					}
				}
			}
		}
	}

	private static void GetTextures(IMaterial material, out ITexture2D? mainTexture, out ITexture2D? normalTexture)
	{
		mainTexture = null;
		normalTexture = null;
		ITexture2D? mainReplacement = null;
		foreach ((Utf8String utf8Name, IUnityTexEnv textureParameter) in material.GetTextureProperties())
		{
			string name = utf8Name.String;
			ITexture2D? tex = textureParameter.Texture.TryGetAsset(material.Collection) as ITexture2D;
			if (tex is null)
			{
				continue;
			}
			if (IsMainTexture(name))
			{
				mainTexture ??= tex;
			}
			else if (IsNormalTexture(name))
			{
				normalTexture ??= tex;
			}
			else
			{
				mainReplacement ??= tex;
			}
		}
		mainTexture ??= mainReplacement;
	}

	private static bool IsMainTexture(string textureName)
	{
		return textureName is "_MainTex" or "texture" or "Texture" or "_Texture";
	}

	private static bool IsNormalTexture(string textureName)
	{
		return textureName is "_Normal" or "Normal" or "normal" or "_BumpMap";
	}
}