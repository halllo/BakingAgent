using System.ComponentModel;
using ModelContextProtocol.Server;

namespace baking.tools;

[McpServerToolType]
public class PlanningTools(ILogger<PlanningTools> logger)
{
	[McpServerTool]
	[Description("Check if an ingredient is available.")]
	public async Task<string> CheckAvailability(string ingredient, string amount, string unit)
	{
		logger.LogInformation("Checking availability of {Amount} {Unit} of {Ingredient}...", amount, unit, ingredient);
		var rnd = new Random();
		if (rnd.NextDouble() < 0.3)
		{
			logger.LogInformation("{Ingredient} is unavailable.", ingredient);
			return $"{ingredient} is unavailable. You need to buy it.";
		}
		else
		{
			logger.LogInformation("{Ingredient} is available.", ingredient);
			return $"{ingredient} is available.";
		}
	}

	[McpServerTool]
	[Description("Buy ingredient or utility in case it is not available but needed.")]
	public async Task<string> Buy(string ingredient)
	{
		logger.LogInformation("Buying {Ingredient}", ingredient);
		return $"Bought {ingredient}. It is now available for use.";
	}
}