using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using server.Hubs;
using server.Services;
using server.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();
builder.Services.AddScoped<ChatAuthenticationService>();
builder.Services.AddScoped<DbControllerService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddControllers();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(30); // 30 days (temp)
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy",
        policy => policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://localhost:4173", "http://127.0.0.1:4173")
            .WithOrigins("http://localhost:45002", "http://127.0.0.1:45002")
            .WithOrigins("https://localhost:45002", "https://127.0.0.1:45002")
            .WithOrigins($"https://${IPInfo.GetIPv4OfWiFiInterface()}:5173")
            .WithOrigins($"https://${IPInfo.GetIPv4OfWiFiInterface()}:5173")
            .WithOrigins($"https://${IPInfo.GetIPv4OfWiFiInterface()}:45002")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

    options.AddPolicy("ProdCorsPolicy",
        policy => policy
            .WithOrigins($"https://${IPInfo.GetIPv4OfWiFiInterface()}:45002") // Change this before production
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseCors("ProdCorsPolicy");
}
else
{
    app.UseCors("DevCorsPolicy");
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToPage("/_Host");
app.MapControllers();

app.Run();
