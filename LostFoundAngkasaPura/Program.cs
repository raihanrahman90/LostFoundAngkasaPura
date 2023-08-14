using LostFoundAngkasaPura.DAL;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DAL.Seeder;
using LostFoundAngkasaPura.DTO.Error;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.Service.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using LostFoundAngkasaPura.Service.Admin;
using LostFoundAngkasaPura.Service.Mailer;
using LostFoundAngkasaPura.Service.ItemFound;
using LostFoundAngkasaPura.Service.ItemCategory;
using LostFoundAngkasaPura.Utils;
using LostFoundAngkasaPura.Service.ItemClaim;
using LostFoundAngkasaPura.Service.ItemComment;
using LostFoundAngkasaPura.Service.Dashboard;
using LostFoundAngkasaPura.Service.AdminNotification;
using LostFoundAngkasaPura.Service.UserNotification;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

if (env.IsDevelopment())
{
    LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.local.config"));
}
else
{
    LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
}
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
ConfigurationManager configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<LostFoundDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(0),
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});
string mySqlConnectionStr = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LostFoundDbContext>(p => p.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer Scheme.
                        Enter 'Bearer' [Space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement(){
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        },
                        new List<string>()
                }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
            //.AllowAnyOrigin()
            .WithOrigins("http://localhost:3000", "https://loclahost:3000", "http://103.150.92.47:3000")
            //            .WithMethods("PUT", "POST", "GET","OPTIONS", "DELETE")
            .AllowAnyMethod()
            .WithHeaders("Authorization", "Content-Type", "Cookies")
            .WithExposedHeaders("Content-Disposition")
            .AllowCredentials();
        });
});
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<JwtSecurityTokenHandler>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAdminNotificationService, AdminNotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IItemCategoryService, ItemCategoryService>();
builder.Services.AddScoped<IItemClaimService, ItemClaimService>();
builder.Services.AddScoped<IItemCommentService, ItemCommentService>();
builder.Services.AddScoped<IItemFoundService, ItemFoundService>();
builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMailerService, MailerService>();
builder.Services.AddSingleton<UploadLocation>();


var app = builder.Build();
var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
using (var scope = scopedFactory.CreateScope())
{
    var serviceLain = scope.ServiceProvider.GetService<MailerService>();
    var service = scope.ServiceProvider.GetService<DataSeeder>();
    service.Seed();
}
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(configuration.GetValue<string>("Base:Location")),
    RequestPath = "/static"
});

app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
    DefaultResponse<String> response = new DefaultResponse<String>();
    response.Success = false;
    if (exception is DataMessageError)
    {
        response.Data = exception.Message;
        response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
    else if (exception is NotFoundError)
    {
        response.Data = "Data not found" ;
        response.StatusCode = (int)HttpStatusCode.NotFound;
    }
    else if (exception is NotAuthorizeError)
    {
        response.Data = "Access Denied";
        response.StatusCode = (int)HttpStatusCode.Forbidden;
    }
    else
    {
        response.Data = exception.Message;
        response.StatusCode = (int)HttpStatusCode.BadRequest;
    }


    await context.Response.WriteAsJsonAsync(response);

}));
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
