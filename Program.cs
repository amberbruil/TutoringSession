using Microsoft.EntityFrameworkCore;
using TutoringSession.Data;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<TutoringDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("TutoringDb")));

// MVC + Views
builder.Services.AddControllersWithViews();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient for MVC -> API calls
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Conventional MVC route (for SessionController)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Session}/{action=Index}/{id?}");

// Attribute-routed API controllers
app.MapControllers();

// Optional: root redirect to the form
app.MapGet("/", () => Results.Redirect("/Session/Index"));

app.Run();
