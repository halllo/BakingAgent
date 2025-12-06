var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer()
    .WithHttpTransport(o => o.Stateless = true)
    .WithToolsFromAssembly(typeof(Program).Assembly)
    ;

var app = builder.Build();

app.MapGet("/", () => "Hello there! These are the tools.");

app.MapMcp("/mcp");

app.Run();
