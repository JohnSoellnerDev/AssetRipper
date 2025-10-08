FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development

WORKDIR /app

COPY AssetRipper.sln ./
COPY nuget.config ./
COPY Source/Directory.Build.props Source/

COPY Source/AssetRipper.GUI.Free/*.csproj Source/AssetRipper.GUI.Free/
COPY Source/AssetRipper.GUI.Web/*.csproj Source/AssetRipper.GUI.Web/
COPY Source/AssetRipper.GUI.Licensing/*.csproj Source/AssetRipper.GUI.Licensing/
COPY Source/AssetRipper.GUI.Localizations/*.csproj Source/AssetRipper.GUI.Localizations/
COPY Source/AssetRipper.Web/*.csproj Source/AssetRipper.Web/
COPY Source/AssetRipper.Export.PrimaryContent/*.csproj Source/AssetRipper.Export.PrimaryContent/
COPY Source/AssetRipper.Export.UnityProjects/*.csproj Source/AssetRipper.Export.UnityProjects/
COPY Source/AssetRipper.Export/*.csproj Source/AssetRipper.Export/
COPY Source/AssetRipper.Export.Modules.Audio/*.csproj Source/AssetRipper.Export.Modules.Audio/
COPY Source/AssetRipper.Export.Modules.Models/*.csproj Source/AssetRipper.Export.Modules.Models/
COPY Source/AssetRipper.Export.Modules.Shader/*.csproj Source/AssetRipper.Export.Modules.Shader/
COPY Source/AssetRipper.Export.Modules.Textures/*.csproj Source/AssetRipper.Export.Modules.Textures/
COPY Source/AssetRipper.Import/*.csproj Source/AssetRipper.Import/
COPY Source/AssetRipper.Processing/*.csproj Source/AssetRipper.Processing/
COPY Source/AssetRipper.Assets/*.csproj Source/AssetRipper.Assets/
COPY Source/AssetRipper.IO.Files/*.csproj Source/AssetRipper.IO.Files/
COPY Source/AssetRipper.SerializationLogic/*.csproj Source/AssetRipper.SerializationLogic/
COPY Source/AssetRipper.SourceGenerated.Extensions/*.csproj Source/AssetRipper.SourceGenerated.Extensions/
COPY Source/AssetRipper.Numerics/*.csproj Source/AssetRipper.Numerics/
COPY Source/AssetRipper.Yaml/*.csproj Source/AssetRipper.Yaml/
COPY Source/AssetRipper.DocExtraction/*.csproj Source/AssetRipper.DocExtraction/
COPY Source/Smolv/*.csproj Source/Smolv/
COPY Source/SpirV/*.csproj Source/SpirV/
COPY Source/UnityEngine/*.csproj Source/UnityEngine/

COPY Source/AssetRipper.GUI.Licensing.SourceGenerator/*.csproj Source/AssetRipper.GUI.Licensing.SourceGenerator/
COPY Source/AssetRipper.GUI.Localizations.SourceGenerator/*.csproj Source/AssetRipper.GUI.Localizations.SourceGenerator/
COPY Source/AssetRipper.GUI.SourceGenerator/*.csproj Source/AssetRipper.GUI.SourceGenerator/
COPY Source/AssetRipper.IO.Files.SourceGenerator/*.csproj Source/AssetRipper.IO.Files.SourceGenerator/
COPY Source/AssetRipper.Processing.SourceGenerator/*.csproj Source/AssetRipper.Processing.SourceGenerator/
COPY Source/AssetRipper.SourceGenerated.Extensions.SourceGenerator/*.csproj Source/AssetRipper.SourceGenerated.Extensions.SourceGenerator/

RUN dotnet restore Source/AssetRipper.GUI.Free/AssetRipper.GUI.Free.csproj

COPY Localizations/ Localizations/

COPY Source/ Source/
COPY LICENSE.md ./

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_RUNNING_IN_CONTAINER=true

WORKDIR /app/Source/AssetRipper.GUI.Free
ENTRYPOINT ["dotnet", "watch", "run", "--non-interactive", "--no-hot-reload", "--", "--port", "5000", "--host", "0.0.0.0", "--launch-browser=false"]

