using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello there! This is the agent.");

app.MapAGUI("/agui", await CreateAgent((await ConnectMcpTools()).Cast<AITool>()));

app.Run();

static async Task<AIAgent> CreateAgent(IEnumerable<AITool> tools)
{
    var ollamaClient = new OpenAIClient(
        credential: new ApiKeyCredential("_"),
        options: new() { Endpoint = new Uri("http://localhost:11434/v1") })
        .GetChatClient("qwen3-vl:4b-instruct");

    var lmStudioClient = new OpenAIClient(
        credential: new ApiKeyCredential("_"),
        options: new() { Endpoint = new Uri("http://127.0.0.1:1234/v1") })
        .GetChatClient("qwen/qwen3-vl-4b");

    var agent = ollamaClient.CreateAIAgent(
        name: "Baker",
        instructions: """
        You are Baker, a very skilled and effective baking agent.
        You have access to various tools to help you with planning, shopping, and baking.
        Use the tools when needed to complete your tasks successfully.
        Dont ask the user many questions, try to be as autonomous as possible.
        """,
        tools: [.. tools]);
    
    return agent;
}

static async Task<IList<McpClientTool>> ConnectMcpTools()
{
    var httpClient = new HttpClient()
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    var httpClientTransport = new HttpClientTransport(new()
    {
        Name = "Baking Tools MCP Server",
        Endpoint = new Uri("http://localhost:5116/mcp"),
        TransportMode = HttpTransportMode.StreamableHttp,
    }, httpClient);

    var mcpClient = await McpClient.CreateAsync(httpClientTransport);
    var mcpTools = await mcpClient.ListToolsAsync();
    return mcpTools;
}
