var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer(o => o.ServerInfo = new()
    {
        Name = "Baking Tools MCP Server",
        Description = "A set of tools for baking-related tasks.",
        Version = "1.0.0",
    })
    .WithHttpTransport(o => o.Stateless = true)
    .WithToolsFromAssembly(typeof(Program).Assembly)
    ;

var app = builder.Build();

app.MapGet("/", () => "Hello there! These are the tools.");

app.MapMcp("/mcp");

app.Run();
