using Jobsity.Bot.Hubs;
using Jobsity.Bot.RabbitMQ;
using Jobsity.Common.Extensions;
using Jobsity.Common.Middlewares;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .AddCommandLine(args)
    .Build();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithOrigins("http://localhost:3000/") //", http://www.contoso.com"); ;
                            .SetIsOriginAllowed((host) => true);
                      });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.SetUpRabbitMQ(builder.Configuration);
builder.Services.AddSingleton<Sender>();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.MapHub<BotHub>("/bot");

app.Run();
