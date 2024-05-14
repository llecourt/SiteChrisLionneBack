using SiteChrisLionneBack;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = "https://securetoken.google.com/sitechrislionne";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/sitechrislionne",
            ValidateAudience = true,
            ValidAudience = "sitechrislionne",
            ValidateLifetime = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context => {
                var firebaseToken = context.SecurityToken as JwtSecurityToken;
                if (firebaseToken == null)
                {
                    context.Fail("Invalid token");
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context => {
                context.Fail("Invalid token");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Database.init();
ImageStore.init();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();
