using System.Data;
using System.Net;
using AspNetCore.ReCaptcha;
using BookingQueue.BLL.Services;
using BookingQueue.BLL.Services.Interfaces;
using BookingQueue.DAL.GenericRepository;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Diagnostics;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Configure your database connection
builder.Services.AddScoped<IDbConnection>(c => {
    var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
    return new MySqlConnection(connectionString);
});

builder.Services.AddReCaptcha(builder.Configuration.GetSection("GoogleReCaptcha"));

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IServicesService, ServicesService>();
builder.Services.AddTransient<IAdvanceService, AdvanceService>();

builder.Services.AddRazorPages();
builder.Services.AddMvc();
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure log4net
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// Add global exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "text/html";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Log the exception
        var logger = LogManager.GetLogger(exceptionHandlerPathFeature?.Path);
        logger.Error("An error occurred.", exception);

        await context.Response.WriteAsync("An error occurred. Please try again later.");
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();