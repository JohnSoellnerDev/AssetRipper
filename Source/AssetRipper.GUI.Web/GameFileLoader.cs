using AssetRipper.Assets.Bundles;
using AssetRipper.Export.PrimaryContent;
using AssetRipper.Export.UnityProjects;
using AssetRipper.Export.UnityProjects.Configuration;
using AssetRipper.Import.Logging;
using AssetRipper.Import.Structure.Assembly.Managers;
using AssetRipper.IO.Files;
using AssetRipper.Processing;

namespace AssetRipper.GUI.Web;

public static class GameFileLoader
{
	private static GameData? GameData { get; set; }
	[MemberNotNullWhen(true, nameof(GameData))]
	public static bool IsLoaded => GameData is not null;
	public static GameBundle GameBundle => GameData!.GameBundle;
	public static IAssemblyManager AssemblyManager => GameData!.AssemblyManager;
	public static LibraryConfiguration Settings { get; } = LoadSettings();
	private static ExportHandler exportHandler = new(Settings);
	public static ExportHandler ExportHandler
	{
		private get
		{
			return exportHandler;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			value.ThrowIfSettingsDontMatch(Settings);
			exportHandler = value;
		}
	}
	public static bool Premium => ExportHandler.GetType() != typeof(ExportHandler);
	
	private static IProgressService? _progressService;
	public static void SetProgressService(IProgressService progressService)
	{
		_progressService = progressService;
	}

	public static void Reset()
	{
		if (GameData is not null)
		{
			GameData = null;
			Logger.Info(LogCategory.General, "Data was reset.");
		}
	}

	public static async Task LoadAndProcess(IReadOnlyList<string> paths)
	{
		string operationId = $"load_{Guid.NewGuid():N}";
		
		try
		{
			if (_progressService != null)
			{
				await _progressService.StartOperation(operationId, "Loading", $"Loading game files from {paths.Count} path(s)");
			}
			
			Reset();
			Settings.LogConfigurationValues();
			
			if (_progressService != null)
			{
				await _progressService.UpdateProgress(operationId, 1, 3, "Loading game files...");
			}
			
			GameData = ExportHandler.LoadAndProcess(paths);
			
			if (_progressService != null)
			{
				await _progressService.UpdateProgress(operationId, 3, 3, "Loading completed");
				await _progressService.CompleteOperation(operationId, true, "Game files loaded successfully");
			}
		}
		catch (Exception ex)
		{
			if (_progressService != null)
			{
				await _progressService.CompleteOperation(operationId, false, $"Loading failed: {ex.Message}");
			}
			throw;
		}
	}

	public static async Task ExportUnityProject(string path)
	{
		if (!IsLoaded || !IsValidExportDirectory(path))
		{
			return;
		}
		
		string operationId = $"export_unity_{Guid.NewGuid():N}";
		
		try
		{
			if (_progressService != null)
			{
				await _progressService.StartOperation(operationId, "Export", $"Exporting Unity project to {path}");
			}

			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			
			Directory.CreateDirectory(path);
			
			if (_progressService != null)
			{
				await _progressService.UpdateProgress(operationId, 1, 100, "Starting export...");
			}
			
			// Create a wrapped export handler that reports progress
			var wrappedHandler = new ProgressTrackingExportHandler(ExportHandler, _progressService, operationId);
			wrappedHandler.Export(GameData, path);
			
			if (_progressService != null)
			{
				await _progressService.CompleteOperation(operationId, true, "Unity project exported successfully");
			}
		}
		catch (Exception ex)
		{
			if (_progressService != null)
			{
				await _progressService.CompleteOperation(operationId, false, $"Export failed: {ex.Message}");
			}
			throw;
		}
	}

	public static async Task ExportPrimaryContent(string path)
	{
		if (!IsLoaded || !IsValidExportDirectory(path))
		{
			return;
		}
		
		string operationId = $"export_primary_{Guid.NewGuid():N}";
		
		try
		{
			if (_progressService != null)
			{
				await _progressService.StartOperation(operationId, "Export", $"Exporting primary content to {path}");
			}

			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			
			Directory.CreateDirectory(path);
			Logger.Info(LogCategory.Export, "Starting primary content export");
			Logger.Info(LogCategory.Export, $"Attempting to export assets to {path}...");
			
			if (_progressService != null)
			{
				await _progressService.UpdateProgress(operationId, 1, 100, "Starting primary content export...");
			}
			
			Settings.ExportRootPath = path;
			PrimaryContentExporter.CreateDefault(GameData).Export(GameBundle, Settings, LocalFileSystem.Instance);
			
			if (_progressService != null)
			{
				await _progressService.UpdateProgress(operationId, 100, 100, "Export completed");
				await _progressService.CompleteOperation(operationId, true, "Primary content exported successfully");
			}
			
			Logger.Info(LogCategory.Export, "Finished exporting primary content.");
		}
		catch (Exception ex)
		{
			if (_progressService != null)
			{
				await _progressService.CompleteOperation(operationId, false, $"Export failed: {ex.Message}");
			}
			throw;
		}
	}

	private static LibraryConfiguration LoadSettings()
	{
		LibraryConfiguration settings = new();
		settings.LoadFromDefaultPath();
		return settings;
	}

	private static bool IsValidExportDirectory(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			Logger.Error(LogCategory.Export, "Export path is empty");
			return false;
		}
		string directoryName = Path.GetFileName(path);
		if (directoryName is "Desktop" or "Documents" or "Downloads")
		{
			Logger.Error(LogCategory.Export, $"Export path '{path}' is a system directory");
			return false;
		}
		return true;
	}
}
