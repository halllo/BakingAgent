using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello there! This is the agent.");

app.MapAGUI("/agui", await CreateLMStudioAgent(builder));

app.Run();

static async Task<AIAgent> CreateLMStudioAgent(WebApplicationBuilder builder)
{
    var httpClientTransport = new HttpClientTransport(new()
    {
        Name = "Baking MCP Server",
        Endpoint = new Uri("http://localhost:5116/mcp"),
        TransportMode = HttpTransportMode.StreamableHttp,
    });

    var mcpClient = await McpClient.CreateAsync(httpClientTransport);
    var mcpTools = await mcpClient.ListToolsAsync();

    var agent = new OpenAIClient(new ApiKeyCredential("_"), new() { Endpoint = new Uri("http://127.0.0.1:1234/v1") })
        .GetChatClient("qwen/qwen3-vl-4b")
        .AsIChatClient()
        .CreateAIAgent(
            name: "AGUIAssistant",
            tools: [.. mcpTools.Cast<AITool>()]);

    return agent;
}