using System;
using TalentHub.Admin.Data;
using TalentHub.Admin.Repositories;
using TalentHub.Admin.Repositories.Interfaces;
using TalentHub.Admin.Services;
using TalentHub.Admin.Services.Interfaces;
using TalentHub.Admin.Strategies.Interfaces;
using TalentHub.Admin.Strategies;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// MVC + API Controllers
// ===============================
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// ===============================
// Inyección de dependencias
// ===============================

// Empleados
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

// Recomendaciones 
builder.Services.AddScoped<IRecomendacionRepository, RecomendacionRepository>();
builder.Services.AddScoped<IRecomendacionService, RecomendacionService>();

// Vacantes
builder.Services.AddScoped<IVacanteRepository, VacanteRepository>();

// Recomendaciones
builder.Services.AddScoped<IRecomendacionRepository, RecomendacionRepository>();
builder.Services.AddScoped<IRecomendacionService, RecomendacionService>();

// Strategies (Strategy Pattern)
builder.Services.AddScoped<ICandidatoScoringStrategy, AreaStrategy>();
builder.Services.AddScoped<ICandidatoScoringStrategy, AntiguedadStrategy>();
builder.Services.AddScoped<ICandidatoScoringStrategy, EvaluacionStrategy>();


// ===============================
// Sesión (login manual)
// ===============================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===============================
// Inicializar helper SQL
// ===============================
SqlHelper.Initialize(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy
                .WithOrigins("https://talenthub-recomendaciones.vercel.app")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

// ===============================
// Pipeline HTTP
// ===============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("ReactPolicy");
app.UseCors("AllowReact");

// ⚠️ Session SIEMPRE antes de endpoints
app.UseSession();

// Preparado para auth futura
app.UseAuthorization();

// ===============================
// Endpoints
// ===============================

// API
app.MapControllers();

// MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
