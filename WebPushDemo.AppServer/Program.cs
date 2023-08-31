using WebPushDemo.AppServer.Options;
using WebPushDemo.AppServer.Services;
using WebPushDemo.Shared.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<PushNotificationService>();
builder.Services.AddOptions().Configure<VapidInfoOptions>(builder.Configuration.GetSection(VapidInfoOptions.SectionKey));

builder.Services.AddWebPushServerServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("subscribe", async(WebPushSubscription subscription, PushNotificationService service)=>
{
    await service.Subscribe(subscription);
    return Results.Ok();
});

app.MapGet("request-example-notification", (PushNotificationService service)=>
{
    service.SendExampleNotification();
    return Results.Ok();
});

app.UseCors("AllowAll");

app.Run();
