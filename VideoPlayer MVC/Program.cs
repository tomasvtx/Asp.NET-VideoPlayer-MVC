using VideoPlayer_MVC.C_SHARP;
using static VideoPlayer_MVC.C_SHARP.MODELS;
using MediaInfo;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;

VideoPlayer_MVC.C_SHARP.MODELS.StartTime.Start();

Random random = new Random();
MODELS.InstanceID = random.Next();
var builder = WebApplication.CreateBuilder(args);
Console.WriteLine(builder.ToString());

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=StartupWait}/{id?}");

MODELS.webRootPath = app.Environment.WebRootPath;

await app.RunAsync();
