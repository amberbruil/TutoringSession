using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TutoringSession.Data;
using TutoringSession.Models;  
using TutoringSession.Dtos;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// MVC (for Razor Views)
// ------------------------------
builder.Services.AddControllersWithViews()
    .ConfigureApiBehaviorOptions(options =>
    {
        // We'll validate explicitly in the minimal API
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
    });

// ------------------------------
// EF Core (SQL Server)
// ------------------------------
builder.Services.AddDbContext<TutoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------------------
// Swagger / OpenAPI
// ------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tutoring Session Micro API",
        Version = "v1",
        Description = "Minimal API that calculates tutoring session earnings and saves to the database."
    });
});

// ------------------------------
// HttpClient Factory
// ------------------------------
builder.Services.AddHttpClient();

var app = builder.Build();

// ------------------------------
// Middleware
// ------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tutoring Session Micro API v1");
    c.RoutePrefix = "swagger"; // UI at /swagger
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ------------------------------
// MVC Route
// ------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Session}/{action=Index}/{id?}");
    
// ------------------------------
// Minimal API endpoint
// POST /api/session/calculate
// ------------------------------
app.MapPost("/api/session/calculate", async ([FromBody] SessionCreateDto dto, TutoringDbContext db) =>
{
    // Manual validation (kept explicit so it matches jQuery mapping)
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(dto.LecturerName))
        errors["LecturerName"] = new[] { "Lecturer name is required." };

    if (string.IsNullOrWhiteSpace(dto.StudentName))
        errors["StudentName"] = new[] { "Student name is required." };

    if (dto.HoursTutored <= 0)
        errors["HoursTutored"] = new[] { "Hours must be greater than 0." };

    if (dto.SessionDate == default)
        errors["SessionDate"] = new[] { "Session date is required." };

    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    // --- BUSINESS RULE (edit here if rate changes)
    const decimal HourlyRate = 200m;
    var fee = (decimal) dto.HoursTutored * HourlyRate;

    // Persist to DB (maps to Sessions table)
    var entity = new Session
    {
        LecturerName = dto.LecturerName,
        StudentName = dto.StudentName,
        SessionDate = dto.SessionDate,
        HoursTutored = dto.HoursTutored,
        HourlyRate = HourlyRate,
        FeeAmount = fee
    };

    db.Sessions.Add(entity);
    await db.SaveChangesAsync();

    // Return calculated payload
    var read = new SessionReadDto(
        entity.Id,
        entity.LecturerName,
        entity.StudentName,
        entity.SessionDate,
        entity.HoursTutored,
        entity.HourlyRate,
        entity.FeeAmount
    );

    return Results.Ok(read);
})
.WithName("CalculateSessionEarnings")
.WithTags("Sessions")
.Produces<SessionReadDto>(StatusCodes.Status200OK)
.ProducesValidationProblem(StatusCodes.Status400BadRequest);

app.Run();
