using Hangfire;
using Hangfire.SqlServer;
using Loloca_BE.Business.BackgroundServices.Implements;
using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Business.Models.Mapper;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Implements;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add connection string
builder.Services.AddDbContext<LolocaDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true
    }));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program), typeof(MappingProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

//Add services to the container
builder.Services.AddScoped<IRefreshTokenBackgroundService, RefreshTokenBackgroundService>();
builder.Services.AddScoped<IPaymentRequestBackgroundService, PaymentRequestBackgroundService>();
builder.Services.AddScoped<ITourGuideBackgroundService, TourGuideBackgroundService>();
builder.Services.AddScoped<ITourBackgroundService, TourBackgroundService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITourGuideService, TourGuideService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICitiesService, CitiesService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();
builder.Services.AddScoped<IPaymentRequestService, PaymentRequestService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IBookingTourGuideRequestService, BookingTourGuideRequestService>();
builder.Services.AddScoped<IBookingTourRequestService, BookingTourRequestService>();
builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();
//builder.Services.AddScoped<IBookingTourRequestService, BookingTourRequestService>();

// Register in-memory caching
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// Set policy permission for roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim("Role", "1"));
    options.AddPolicy("RequireTourGuideRole", policy => policy.RequireClaim("Role", "2"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireClaim("Role", "3"));
    options.AddPolicy("RequireAdminOrTourGuideRole", policy => policy.RequireClaim("Role", "1", "2"));
    options.AddPolicy("RequireAdminOrCustomerRole", policy => policy.RequireClaim("Role", "1", "3"));
    options.AddPolicy("RequireTourGuideOrCustomerRole", policy => policy.RequireClaim("Role", "2", "3"));
    options.AddPolicy("RequireAllRoles", policy => policy.RequireClaim("Role", "1", "2", "3"));
});

// Load .env file
DotNetEnv.Env.Load();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

// Schedule the recurring job
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate(
    "RemoveExpiredRefreshToken",
    () => app.Services.CreateScope().ServiceProvider.GetRequiredService<IRefreshTokenBackgroundService>().RemoveExpiredRefreshToken(),
    Cron.Minutely
    );
recurringJobManager.AddOrUpdate(
    "RefreshTourGuideCache", 
    () => app.Services.CreateScope().ServiceProvider.GetRequiredService<ITourGuideBackgroundService>().RefreshTourGuideCache(), 
    Cron.Minutely
    );
recurringJobManager.AddOrUpdate(
    "RefreshTourGuideInCityCache",
    () => app.Services.CreateScope().ServiceProvider.GetRequiredService<ITourGuideBackgroundService>().RefreshTourGuideInCityCache(),
    Cron.Minutely
    );
recurringJobManager.AddOrUpdate(
    "RefreshTourCache",
    () => app.Services.CreateScope().ServiceProvider.GetRequiredService<ITourBackgroundService>().RefreshTourCache(),
    Cron.Minutely
    );
recurringJobManager.AddOrUpdate(
    "RefreshTourInCityCache",
    () => app.Services.CreateScope().ServiceProvider.GetRequiredService<ITourBackgroundService>().RefreshTourInCityCache(),
    Cron.Minutely
    );
recurringJobManager.AddOrUpdate(
    "RejectExpiredPaymentRequest",
    () => app.Services.CreateScope().ServiceProvider.GetRequiredService<IPaymentRequestBackgroundService>().RejectExpiredPaymentRequest(),
    Cron.Minutely
    );

app.UseCors();

app.UseHttpsRedirection();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
