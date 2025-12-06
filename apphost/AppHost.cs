var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.baking_agent>("Baking-Agent");
builder.AddProject<Projects.baking_tools>("Baking-Tools");

builder.Build().Run();