using AssetRipper.Export.Modules.Textures;
using AssetRipper.SourceGenerated.Classes.ClassID_21;
using AssetRipper.SourceGenerated.Classes.ClassID_28;
using AssetRipper.SourceGenerated.Extensions;
using AssetRipper.SourceGenerated.Subclasses.UnityTexEnv;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;

namespace AssetRipper.Export.Modules.Models;

public static class GlbMaterialBuilder
{
	public static SceneBuilder Build(IMaterial material)
	{
		SceneBuilder sceneBuilder = new();
		
		// Create material builder
		MaterialBuilder materialBuilder = CreateMaterialBuilder(material);
		
		// Create a sphere mesh to display the material
		var mesh = CreateSphere(materialBuilder);
		
		// Create the scene with the sphere
		var node = sceneBuilder.AddNode("MaterialPreview");
		sceneBuilder.AddRigidMesh(mesh, node);
		
		return sceneBuilder;
	}

	private static MaterialBuilder CreateMaterialBuilder(IMaterial material)
	{
		MaterialBuilder materialBuilder = new MaterialBuilder(material.Name);
		
		// Extract textures from material
		GetTextures(material, out ITexture2D? mainTexture, out ITexture2D? normalTexture);
		
		if (mainTexture is not null && TryGetMemoryImage(mainTexture, out MemoryImage mainImage))
		{
			materialBuilder.WithBaseColor(mainImage);
		}
		
		if (normalTexture is not null && TryGetMemoryImage(normalTexture, out MemoryImage normalImage))
		{
			materialBuilder.WithNormal(normalImage);
		}
		
		return materialBuilder;
	}

	private static bool TryGetMemoryImage(ITexture2D texture, out MemoryImage image)
	{
		if (TextureConverter.TryConvertToBitmap(texture, out DirectBitmap bitmap))
		{
			using MemoryStream memoryStream = new();
			bitmap.SaveAsPng(memoryStream);
			image = new MemoryImage(memoryStream.ToArray());
			return true;
		}
		else
		{
			image = default;
			return false;
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
			if (IsMainTexture(name))
			{
				mainTexture ??= textureParameter.Texture.TryGetAsset(material.Collection) as ITexture2D;
			}
			else if (IsNormalTexture(name))
			{
				normalTexture ??= textureParameter.Texture.TryGetAsset(material.Collection) as ITexture2D;
			}
			else
			{
				mainReplacement ??= textureParameter.Texture.TryGetAsset(material.Collection) as ITexture2D;
			}
		}
		mainTexture ??= mainReplacement;
	}

	private static bool IsMainTexture(string textureName)
	{
		return textureName is "_MainTex" or "texture" or "Texture" or "_Texture" or "_BaseMap" or "_BaseColorMap";
	}

	private static bool IsNormalTexture(string textureName)
	{
		return textureName is "_Normal" or "Normal" or "normal" or "_BumpMap" or "_NormalMap";
	}

	private static IMeshBuilder<MaterialBuilder> CreateSphere(MaterialBuilder material, float radius = 1.0f, int segments = 24)
	{
		var mesh = new MeshBuilder<VertexPosition, VertexTexture1, VertexEmpty>("MaterialSphere");
		var primitive = mesh.UsePrimitive(material);

		// Generate sphere vertices
		for (int lat = 0; lat <= segments; lat++)
		{
			float theta = lat * MathF.PI / segments;
			float sinTheta = MathF.Sin(theta);
			float cosTheta = MathF.Cos(theta);

			for (int lon = 0; lon <= segments; lon++)
			{
				float phi = lon * 2 * MathF.PI / segments;
				float sinPhi = MathF.Sin(phi);
				float cosPhi = MathF.Cos(phi);

				float x = cosPhi * sinTheta;
				float y = cosTheta;
				float z = sinPhi * sinTheta;

				var position = new VertexPosition(x * radius, y * radius, z * radius);
				var texCoord = new VertexTexture1((float)lon / segments, (float)lat / segments);

				primitive.AddPoint(position, texCoord);
			}
		}

		// Generate sphere indices
		for (int lat = 0; lat < segments; lat++)
		{
			for (int lon = 0; lon < segments; lon++)
			{
				int first = lat * (segments + 1) + lon;
				int second = first + segments + 1;

				primitive.AddTriangle(first, second, first + 1);
				primitive.AddTriangle(second, second + 1, first + 1);
			}
		}

		return mesh;
	}
}