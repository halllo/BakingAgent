using System.ComponentModel;
using System.Diagnostics;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace baking.tools;

[McpServerToolType]
public class OvenTool(ILogger<OvenTool> logger)
{
	[McpServerTool]
	[Description("Bake ingredients in the oven at a specified temperature and duration.")]
	public async Task<string> UseOven(
		[Description("Name of the dish or the baked good.")] string recipeName,
		[Description("How does the dish look visually.")] string visualDescription,
		[Description("Temperature in Celsius at which to bake the dish.")] string temperatureCelsius,
		[Description("Duration in minutes for which to bake the dish.")] string minutes)
	{
		logger.LogInformation("Baking {RecipeName} ({VisualDescription}) at {Temperature}°C for {Minutes} minutes", recipeName, visualDescription, temperatureCelsius, minutes);

		var imagePrompt = $"Delicious {recipeName} freshly baked.";
		var processStartInfo = new ProcessStartInfo
		{
			FileName = "mflux-generate",
			Arguments = $"--model schnell --prompt \"{imagePrompt}\" --steps 2 --seed 2 -q 8",
			UseShellExecute = false,
			CreateNoWindow = true
		};

		using var process = new Process { StartInfo = processStartInfo };
		process.Start();
		logger.LogInformation("Started image generation process for {RecipeName} with PID {PID}", recipeName, process.Id);
		process.WaitForExit();
		logger.LogInformation("Image generation process for {RecipeName} completed with exit code {ExitCode}", recipeName, process.ExitCode);

		var image = new FileInfo("./image.png");
		if (image.Exists)
		{
			var timestampedImagePath = $"./image_{DateTime.Now:yyyyMMddHHmmss}.png";
			image.MoveTo(timestampedImagePath, overwrite: true);
			logger.LogInformation("Generated image for {RecipeName} is available at {ImagePath}", recipeName, timestampedImagePath);
			return $"""
			Successfully baked {recipeName} in the oven at {temperatureCelsius}°C. 
			You can now let it cool down and then serve it. 
			Take a look at {new FileInfo(timestampedImagePath).FullName} for a preview of the final product.
			""";
		}
		else
		{
			logger.LogWarning("Generated image for {RecipeName} was not found at ./image.jpg", recipeName);
			throw new McpException("Backing failed. Result image not found.");
		}
	}
}