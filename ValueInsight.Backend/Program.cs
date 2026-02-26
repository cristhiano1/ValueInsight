using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Services; // ⭐ IMPORTANTE

var builder = WebApplication.CreateBuilder(args);

// 🔹 Controllers
builder.Services.AddControllers();

// 🔹 DbContext (SQL Server)
builder.Services.AddDbContext<ValueInsightDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 CORS (frontend podrá llamar al API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 🔹 HttpClient (necesario para la IA / Ollama)
builder.Services.AddHttpClient();

// 🔥 REGISTRO DE SERVICES (LO QUE FALTABA)
builder.Services.AddScoped<AiCoachService>();
builder.Services.AddScoped<CulturalFitService>();
builder.Services.AddScoped<TeamCultureService>();

// 🔹 Swagger (ya viene pero lo dejamos claro)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Swagger solo en development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Middlewares
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();