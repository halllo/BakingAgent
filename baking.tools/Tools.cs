using System.ComponentModel;
using ModelContextProtocol.Server;

namespace baking.tools;

[McpServerToolType]
public class Tools(ILogger<Tools> logger)
{
	[McpServerTool]
	[Description("Check if an ingredient is available.")]
	public async Task<string> CheckAvailability(string ingredient, int amount, string unit)
	{
		logger.LogInformation("Checking availability of {Amount} {Unit} of {Ingredient}", amount, unit, ingredient);
		return $"{ingredient} is available.";
	}

	[McpServerTool]
	[Description("Buy ingredient or utility in case it is not available but needed.")]
	public async Task<string> Buy(string ingredient)
	{
		logger.LogInformation("Buying {Ingredient}", ingredient);
		return $"Bought {ingredient}. It is now available for use.";
	}

	[McpServerTool]
	[Description("Take a specified amount of an ingredient in a given unit.")]
	public async Task<string> Take(string ingredient, int amount, string unit)
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
	[Description("Bake ingredients in the oven at a specified temperature and duration.")]
	public async Task<string> UseOven(string[] ingredients, int temperatureCelsius, int minutes)
	{
		logger.LogInformation("Baking ingredients: {Ingredients} at {Temperature}°C for {Minutes} minutes", string.Join(", ", ingredients), temperatureCelsius, minutes);
		return $"Baked {string.Join(", ", ingredients)} in the oven at {temperatureCelsius}°C for {minutes} minutes.";

		//todo: use flux
	}

	[McpServerTool]
	[Description("Set a timer for a specified number of minutes.")]
	public async Task<string> SetTimer(int minutes)
	{
		logger.LogInformation("Setting timer for {Minutes} minutes", minutes);
		return $"Timer set for {minutes} minutes.";
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