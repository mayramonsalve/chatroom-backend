using Jobsity.Chatroom.DTOs;
using Jobsity.Chatroom.Hubs;
using Jobsity.Chatroom.Models;
using Jobsity.Chatroom.RabbitMQ;
using Jobsity.Chatroom.Services;
using Jobsity.Common.Extensions;
using Jobsity.Common.Middlewares;
using Microsoft.EntityFrameworkCore;

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
                            .WithOrigins("http://localhost:3000/", "http://localhost:3001/", "http://localhost:3002/")
                            .SetIsOriginAllowed((host) => true);
                      });
});
builder.Services.AddDbContext<DataContext>();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());
builder.Services.AddSingleton<IList<string>>(opts => new List<string>() { "Jobsity", "TestUser", "Mayra" });
builder.Services.SetUpRabbitMQ(builder.Configuration);
builder.Services.AddHostedService<Receiver>();
builder.Services.AddScoped<IMessageService, MessageService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}
app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();
