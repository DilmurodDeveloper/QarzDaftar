using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Seeders;
using QarzDaftar.Server.Api.Services.Foundations.Customers;
using QarzDaftar.Server.Api.Services.Foundations.Debts;
using QarzDaftar.Server.Api.Services.Foundations.Payments;
using QarzDaftar.Server.Api.Services.Foundations.Registrations;
using QarzDaftar.Server.Api.Services.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Services.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Services.Foundations.UserNotes;
using QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Services.Foundations.Users;
using QarzDaftar.Server.Api.Services.Processings.Authentications;
using QarzDaftar.Server.Api.Services.Processings.Customers;
using QarzDaftar.Server.Api.Services.Processings.Debts;
using QarzDaftar.Server.Api.Services.Processings.Payments;
using QarzDaftar.Server.Api.Services.Processings.Tokens;
using QarzDaftar.Server.Api.Services.Processings.UserNotes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StorageBroker>();
builder.Services.AddTransient<IStorageBroker, StorageBroker>();
builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
builder.Services.AddTransient<IDateTimeBroker, DateTimeBroker>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IDebtService, DebtService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IUserNoteService, UserNoteService>();
builder.Services.AddTransient<ISubscriptionHistoryService, SubscriptionHistoryService>();
builder.Services.AddTransient<IUserPaymentLogService, UserPaymentLogService>();
builder.Services.AddTransient<ISuperAdminService, SuperAdminService>();
builder.Services.AddTransient<IRegistrationService, RegistrationService>();

builder.Services.AddScoped<IPasswordHasher<SuperAdmin>, PasswordHasher<SuperAdmin>>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenProcessingService, TokenProcessingService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICustomerProcessingService, CustomerProcessingService>();
builder.Services.AddScoped<IDebtProcessingService, DebtProcessingService>();
builder.Services.AddScoped<IPaymentProcessingService, PaymentProcessingService>();
builder.Services.AddScoped<IUserNoteProcessingService, UserNoteProcessingService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient",
        policy => policy
            .WithOrigins(
                "http://localhost:5173",
                "https://qarz-daftar-898gh0gtn-dilmurodmadirimovs-projects.vercel.app",
                "https://qarz-daftar.vercel.app"
            )
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var storageBroker = services.GetRequiredService<StorageBroker>();
    var configuration = services.GetRequiredService<IConfiguration>();
    var passwordHasher = services.GetRequiredService<IPasswordHasher<SuperAdmin>>();

    await SuperAdminSeeder.SeedAsync(storageBroker, configuration, passwordHasher);
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => "QarzDaftar API is running.");
app.UseHttpsRedirection();
app.UseCors("AllowClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
