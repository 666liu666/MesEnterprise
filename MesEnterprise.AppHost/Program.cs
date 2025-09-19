var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MesEnterprise>("mesenterprise");

builder.Build().Run();
