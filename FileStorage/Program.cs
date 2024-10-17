using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FileStorage.Database;
using FileStorage.Repositories;
using FileStorage.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS configuration for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Configure MongoDbSettings from appsettings.json
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Register Repository and Service layers
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add JWT Bearer authentication for Google
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Google:ClientId"], // Google Client ID from appsettings
            ValidIssuer = "https://accounts.google.com", // Google Issuer
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                // Use Google's public keys to validate the token
                var client = new HttpClient();
                var keys = client.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs").Result;
                var jsonWebKeySet = new JsonWebKeySet(keys);
                return jsonWebKeySet.Keys;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication(); // Ensure this is added before UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();
