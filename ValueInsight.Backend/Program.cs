using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Services;

// JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

// 🔥 NECESARIO
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Prevent automatic inbound claim mapping (keep claim names as emitted in the token)
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Controllers
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<ValueInsightDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// HttpClient
builder.Services.AddHttpClient();

// Services
builder.Services.AddScoped<AiCoachService>();
builder.Services.AddScoped<CulturalFitService>();
builder.Services.AddScoped<TeamCultureService>();
builder.Services.AddScoped<AssessmentHistoryService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "ValueInsight.Backend", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// ------------------------------
// JWT AUTHENTICATION
// ------------------------------
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Jwt:Key is not configured in configuration.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        ),

        // Use the plain "role" claim name (matches token emitted below)
        RoleClaimType = "role"
    };
});

var app = builder.Build();


// ------------------------------
// AUTO APPLY MIGRATIONS (guarded)
// ------------------------------
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ValueInsightDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log and continue — avoids crashing the entire app silently on startup
        logger.LogError(ex, "An error occurred while applying database migrations.");
        // Re-throw if you prefer to stop startup:
        // throw;
    }
}


// Swagger
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ValueInsight.Backend v1");
        c.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// JWT middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();