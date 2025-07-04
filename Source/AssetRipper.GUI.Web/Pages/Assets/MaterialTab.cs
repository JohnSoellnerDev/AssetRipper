using AssetRipper.Assets;
using AssetRipper.GUI.Web.Paths;

namespace AssetRipper.GUI.Web.Pages.Assets;

internal sealed class MaterialTab : AssetHtmlTab
{
	public string Source { get; }

	public override string DisplayName => Localization.AssetTabMaterial;

	public override string HtmlName => "material";

	public override bool Enabled => AssetAPI.HasMaterialData(Asset);

	public MaterialTab(IUnityObjectBase asset, AssetPath path) : base(asset)
	{
		Source = AssetAPI.GetMaterialUrl(path);
	}

	public override void Write(TextWriter writer)
	{
		using (new Table(writer).WithCustomAttribute("width", "100%").WithCustomAttribute("height", "100%").End())
		{
			using (new Tbody(writer).End())
			{
				using (new Tr(writer).End())
				{
					using (new Td(writer).WithAlign("center").WithCustomAttribute("valign", "middle").End())
					{
						new Canvas(writer)
							.WithId("babylonMaterialCanvas")
							.WithStyle("width: 100%; height: 100vh;")
							.WithCustomAttribute("material-data-path", Source)
							.Close();
					}
				}
			}
		}
	}
}