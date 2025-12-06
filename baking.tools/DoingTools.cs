using System.ComponentModel;
using ModelContextProtocol.Server;

namespace baking.tools;

[McpServerToolType]
public class DoingTools(ILogger<DoingTools> logger)
{
	[McpServerTool]
	[Description("Take a specified amount of an ingredient in a given unit. Always check availability before you invoke this!")]
	public async Task<string> Take(string ingredient, string amount, string unit)
	{
		logger.LogInformation("Taking {Amount} {Unit} of {Ingredient}", amount, unit, ingredient);
		return $"Took {amount} {unit} of {ingredient}.";
	}

	[McpServerTool]
	[Description("Mix ingredients using a specified method (e.g., stir, whisk, knead).")]
	public async Task<string> Mix(string[] ingredients, string method)
	{
		logger.LogInformation("Mixing ingredients: {Ingredients} using {Method}", string.Join(", ", ingredients), method);
		return $"Mixed {string.Join(", ", ingredients)} using {method}.";
	}

	[McpServerTool]
	[Description("Set a timer for a specified number of minutes.")]
	public async Task<string> SetTimer(string minutes)
	{
		var minutesValue = double.TryParse(minutes, out var minutesParsed) ? minutesParsed : 2;
		logger.LogInformation("Setting timer for {Minutes} minutes", minutesValue);
		await Task.Delay(TimeSpan.FromMinutes(minutesValue));
		logger.LogInformation("Timer finished after {Minutes} minutes", minutesValue);
		return $"Waited for {minutesValue} minutes.";
	}

	[McpServerTool]
	[Description("Cool down ingredients after heating.")]
	public async Task<string> Cool(string[] ingredients)
	{
		logger.LogInformation("Cooling ingredients: {Ingredients}", string.Join(", ", ingredients));
		return $"Cooled {string.Join(", ", ingredients)}.";
	}

	[McpServerTool]
	[Description("Serve or plate the finished dish.")]
	public async Task<string> Serve(string dish)
	{
		logger.LogInformation("Serving dish: {Dish}", dish);
		return $"Served {dish}. Enjoy!";
	}

	[McpServerTool]
	[Description("Clean up the workspace after baking.")]
	public async Task<string> CleanUp()
	{
		logger.LogInformation("Cleaning up the workspace");
		return "Cleaned up the workspace.";
	}
}