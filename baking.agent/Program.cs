using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapAGUI("/agui",
    await CreateLMStudioAgent(builder)
);

app.Run();

static async Task<AIAgent> CreateLMStudioAgent(WebApplicationBuilder builder)
{
    var agent = new OpenAIClient(new ApiKeyCredential("_"), new() { Endpoint = new Uri("http://127.0.0.1:1234/v1") })
        .GetChatClient("qwen/qwen3-vl-4b")
        .AsIChatClient()
        .CreateAIAgent(
            name: "AGUIAssistant",
            tools: [
                AIFunctionFactory.Create(
                    method: () => DateTimeOffset.UtcNow,
                    name: "get_current_time",
                    description: "Get the current UTC time."
                )
            ]);

    return agent;
}