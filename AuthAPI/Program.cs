using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Obtenemos la configuraci�n del JWTSettings de appsettings
var JWTSettings = builder.Configuration.GetSection("JWTSetting");

//Obtenemos la cadena de conexi�n
var connectionString = builder.Configuration.GetConnectionString("Connection");
//Agregamos la configuraci�n para utilizar SQLServer
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));


//Agregamos la configuraci�n para ASP -Net Core Identity
builder.Services.AddIdentity<AppUser,
IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.SaveToken = true;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidAudience = JWTSettings["ValidAudience"],
        ValidIssuer = JWTSettings["ValidIssuer"],
        IssuerSigningKey = new
   SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSettings.GetSection("securityKey").Value!))
    };
});



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//A�adiendo el generador Swagger a la colecci�n de servicios.
builder.Services.AddEndpointsApiExplorer();


//Agregando la Definici�n de Seguridad
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization Example : 'Bearer eyeleieieekeieieie",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
     {
         {
             new OpenApiSecurityScheme
             {
                Reference = new OpenApiReference
                 {
                 Type = ReferenceType.SecurityScheme,
                 Id = "Bearer"
                },
                Scheme = "outh2",
                Name = "Bearer",
                In = ParameterLocation.Header,
             },
             new List<string>()
         }
     });
});



var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    //A�adiendo el Middleware de Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
