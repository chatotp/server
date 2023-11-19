using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using server.Hubs;
using server.Services;

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
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://localhost:4173", "http://127.0.0.1:4173")
            .WithOrigins("http://localhost:45002", "http://127.0.0.1:45002")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); ;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseRouting();

app.MapBlazorHub();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToPage("/_Host");
app.MapControllers();

app.Run();
