using BookLending.API.Middlewares;
using BookLending.Application.Interfaces;
using BookLending.Application.Jobs;
using BookLending.Application.Mapping;
using BookLending.Application.Services;
using BookLending.Common.Errors;
using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using BookLending.Infrastructure.Configuration;
using BookLending.Infrastructure.Persistence;
using BookLending.Infrastructure.Persistence.Seeding;
using BookLending.Infrastructure.Repositories;
using BookLending.Infrastructure.UnitOfWork;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace BookLending.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog from appsettings.json
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            // Replace default logging with Serilog
            builder.Host.UseSerilog();

            // Config
            builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
            builder.Services.Configure<SystemAdminCredentialsConfig>(builder.Configuration.GetSection("SystemAdminCredentials"));
            builder.Services.Configure<BorrowSettings>(builder.Configuration.GetSection("BorrowSettings"));

            // Add services to the container.
            builder.Services.AddDbContext<BookLendingDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<BookLendingDbContext>();

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBorrowService, BorrowService>();

            builder.Services.AddHangfire(configuration =>
                configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHangfireServer();
            builder.Services.AddScoped<OverdueBookChecker>();


            // Add Swagger Service
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BookLending API",
                    Version = "v1"
                });
            });

            // Add custom error handling
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(p => p.Value.Errors.Count > 0) 
                        .SelectMany(p => p.Value.Errors) 
                        .Select(e => e.ErrorMessage) 
                        .ToArray();

                    var validationErrorResponse = new ApiValidationErrorResponse { Errors = errors };
                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();
            app.UseRouting(); 

            #region database migration & seeding 
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<BookLendingDbContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var adminConfig = services.GetRequiredService<IOptions<SystemAdminCredentialsConfig>>();

                await context.Database.MigrateAsync(); 
                await IdentitySeedData.Initialize(roleManager, userManager, adminConfig);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration or seeding.");
            }
            #endregion

            app.UseMiddleware<ExceptionHandlerMiddleware>(); // Use custom error handling middleware
            app.UseStatusCodePagesWithRedirects("/errors/{0}"); // Redirect to error pages for status codes (4xx, 5xx)


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookLending API V1");
                    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard();
            RecurringJob.AddOrUpdate<OverdueBookChecker>(
                "check-overdue-books",
                job => job.CheckOverdueBooksAsync(),
                Cron.Minutely); 

            app.MapControllers(); 

            app.Run();
        }
    }
}
