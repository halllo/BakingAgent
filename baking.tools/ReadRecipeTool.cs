using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using OllamaSharp;

namespace baking.tools;

[McpServerToolType]
public class ReadRecipeTool(ILogger<ReadRecipeTool> logger)
{
	[McpServerTool]
	[Description("Read the recipe from an image file using OCR.")]
	public async Task<string> ReadRecipe(string filepath)
	{
		if (!File.Exists(filepath))
		{
			logger.LogError("File {Filepath} does not exist.", filepath);
			throw new McpException($"File {filepath} does not exist.");
		}
		else
		{
			logger.LogInformation("Reading recipe from {Filepath}...", filepath);
		}

		using IChatClient ollamaChatClient = new OllamaApiClient(
			uri: new Uri("http://localhost:11434"),
			defaultModel: "deepseek-ocr:latest");

		var chatResponse = await ollamaChatClient.GetResponseAsync([
			new ChatMessage(ChatRole.User, [
					new TextContent("<|grounding|>Convert the document to markdown."),
					new DataContent(File.ReadAllBytes(filepath), "image/jpeg")
				])
		]);

		var ocrResponse = chatResponse.Messages.Single().Text;
		logger.LogInformation("Finished reading recipe from {Filepath}.", filepath);
		return ocrResponse;
	}
}