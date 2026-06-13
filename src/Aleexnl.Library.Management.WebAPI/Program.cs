using Aleexnl.Library.Management.WebAPI.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi(builder.Configuration);

WebApplication app = builder.Build();

await app.UseWebApiAsync();

app.Run();
