using CommentsAPI;
using CommentsAPI.Data;
using CommentsAPI.Middlewares;
using CommentsAPI.Services.LoggerService;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string connectionsString = builder.Configuration.GetConnectionString("SQLConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionsString);
});
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddHangfire(config => 
    {
        config.UseSqlServerStorage(connectionsString).WithJobExpirationTimeout(TimeSpan.FromSeconds(120));
    }
);
builder.Services.AddHangfireServer(options => 
    {
        options.Queues = new[] { "default" };
        options.WorkerCount = 1;
    }
);

LogManager.Configuration = new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionsHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();

app.Run();
