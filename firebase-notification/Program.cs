using FirebaseNotification.Features.Notification;
using FirebaseNotification.Services;
using FirebaseNotification.Services.Firebase;

var builder = WebApplication.CreateBuilder(args);

// GBank Controller setup
builder.Services.AddControllers();

// GBank Dependency Injection
builder.Services.AddSingleton<IFirebaseService, FirebaseService>();
builder.Services.AddSingleton<NotificationStore>();
builder.Services.AddScoped<NotificationSetUpService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

app.Run();
