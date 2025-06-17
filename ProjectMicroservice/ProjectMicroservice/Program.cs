using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectMicroservice;
using ProjectMicroservice.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using StackExchange.Profiling.Storage;
using System.Collections.ObjectModel;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting a web application.");

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("log.txt")
        .WriteTo.Seq("http://localhost:5341/")  // admin password - no need apiKey
        .WriteTo.MSSqlServer(
            connectionString: "Server=localhost\\SQLEXPRESS;Database=Northwind;Integrated Security=True;TrustServerCertificate=True;",
            sinkOptions: new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                SchemaName = "dbo",
                AutoCreateSqlTable = true
            },
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            columnOptions: new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn("UserName", System.Data.SqlDbType.NVarChar) // extra column
                }
            }
        )
        .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);

    Log.Information("BEFORE builder.Host.UseSerilog");

    builder.Host.UseSerilog();

    Log.Information("AFTER builder.Host.UseSerilog");

    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "API v1",
            Description = "Web API Version 1"
        });

        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Version = "v2",
            Title = "API v2",
            Description = "Web API Version 2"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Token'inizi 'Bearer {token}' formatinda girin."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
    });

    #region JWT Token Configuration
    var jwtSettings = builder.Configuration.GetSection("JWT");
    var secretKey = jwtSettings["SecretKey"];
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey!))
        };
    });

    builder.Services.AddDbContext<NorthwindContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
    });
    #endregion

    #region Rate limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.AddPolicy("TokenPolicy", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 1,
                    Window = TimeSpan.FromSeconds(10)
                }));

        options.AddPolicy("DataPolicy", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromSeconds(20)
                }));

        //// Global rate limit
        //options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        //    RateLimitPartition.GetFixedWindowLimiter(
        //        partitionKey: context.Request.Headers.Host.ToString(),
        //        factory: partition => new FixedWindowRateLimiterOptions
        //        {
        //            PermitLimit = 1,
        //            Window = TimeSpan.FromSeconds(10) // Limit: 1 request per 10 seconds
        //        }));
    });
    #endregion

    #region miniprofiler-benchmark
    // https://miniprofiler.com/dotnet/AspDotNetCore

    builder.Services.AddMiniProfiler(options => 
    {
        options.RouteBasePath = "/profiler"; // https://localhost:7219/profiler/results
        (options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);
        options.TrackConnectionOpenClose = true;
    });
    #endregion

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("User", httpContext.User?.Identity?.Name ?? "anonymous");
        };
    });

    app.UseAuthentication();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseRateLimiter(); // Rate limiting middleware

    app.UseMiniProfiler();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
