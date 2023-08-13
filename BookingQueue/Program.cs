using System.Data;
using System.Globalization;
using System.Net;
using System.Reflection;
using AspNetCore.ReCaptcha;
using BookingQueue.BLL.Resources;
using BookingQueue.BLL.Services;
using BookingQueue.BLL.Services.Interfaces;
using BookingQueue.DAL.GenericRepository;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache(); // Use a distributed cache for session state in a production scenario
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connectionFactory = sp.GetRequiredService<IDbConnectionFactory>();
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

    // Check if the HttpContext is available
    var session = httpContextAccessor.HttpContext?.Session;

    // Check if the session is available and get the selected database
    if (session != null)
    {
        var selectedDatabase = session.GetString("SelectedDatabase");
        if (!string.IsNullOrEmpty(selectedDatabase))
        {
            var connectionString = builder.Configuration[$"ConnectionStrings:{selectedDatabase}"];
            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionFactory.CreateConnection(connectionString);
            }
        }
    }

    httpContextAccessor.HttpContext?.Response.Redirect("/");
    return null;
});

builder.Services.AddReCaptcha(builder.Configuration.GetSection("GoogleReCaptcha"));
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<LocService>();

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IServicesService, ServicesService>();
builder.Services.AddTransient<IAdvanceService, AdvanceService>();

builder.Services.AddRazorPages();
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
            return factory.Create("SharedResource", assemblyName.Name);
        };
    });

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

        if (exceptionHandlerPathFeature?.Error is NullReferenceException)
        {
            context.Response.Redirect("/");
            return;
        }

        await context.Response.WriteAsync(
            exception?.Message ?? "Произошла ошибка. Пожалуйста, повторите попытку позже.");
    });
});

var supportedCultures = new[]
{
    new CultureInfo("ru"),
    new CultureInfo("uk"),
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ru", "ru"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Branch}/{action=Index}/{id?}");

app.Run();