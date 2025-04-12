using Application.Services;
using Carproject;
using Infrustructure.Context;
using Infrustructure.Repository;
using Infrustructure.Repository.IRepository;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Application.Command.User.CommandHandler;
using Microsoft.Extensions.DependencyInjection;
using Application.Command.User.Command;
using Domain.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});





builder.Services.AddDbContext<CARdbcontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CAR"))
);

builder.Services.AddControllers().AddNewtonsoftJson();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = true,
        ValidateAudience = true,
        //
        ValidIssuer = "http://localhost:5260",
        ValidAudience = "http://localhost:5260",

    };
});

// ثبت Generic Repository و IUserInfoRepository
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUserInfoRepository<>), typeof(UserInfoRepository<>));

// ثبت MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UserRegisterCommandHandler<>).Assembly));

//Email 
builder.Services.AddScoped<IEmailService, EmailService>();

//اضافه کردن سرویس SaleNotificationService
builder.Services.AddScoped<ISaleNotificationService, SaleNotificationService>();

// سرویس گزارش محبوب ترین مدل خودرو
builder.Services.AddScoped<IReportService, ReportService>();


// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRequestHandler<UserRegisterCommand<Buyer>, Guid>, UserRegisterCommandHandler<Buyer>>();
builder.Services.AddScoped<IRequestHandler<UserRegisterCommand<Seller>, Guid>, UserRegisterCommandHandler<Seller>>();
builder.Services.AddScoped<IRequestHandler<UserRegisterCommand<Moder>, Guid>, UserRegisterCommandHandler<Moder>>();

builder.Services.AddScoped<IRequestHandler<UserLoginCommand<Buyer>, string>, UserLoginCommandHandler<Buyer>>();
builder.Services.AddScoped<IRequestHandler<UserLoginCommand<Seller>, string>, UserLoginCommandHandler<Seller>>();
builder.Services.AddScoped<IRequestHandler<UserLoginCommand<Moder>, string>, UserLoginCommandHandler<Moder>>();

//builder.Services.AddTransient(typeof(IRequestHandler<>), typeof(RequestPasswordResetCommandHandler<>));
//builder.Services.AddScoped<IRequestHandler<RequestPasswordResetCommand<Buyer>, string>, RequestPasswordResetCommandHandler<Buyer>>();
//builder.Services.AddScoped<IRequestHandler<RequestPasswordResetCommand<Seller>, string>, RequestPasswordResetCommandHandler<Seller>>();
//builder.Services.AddScoped<IRequestHandler<RequestPasswordResetCommand<Moder>, string>, RequestPasswordResetCommandHandler<Moder>>();

builder.Services.AddScoped<IRequestHandler<RequestPasswordResetCommand<Buyer>, string>, RequestPasswordResetCommandHandler<Buyer>>();
builder.Services.AddScoped<IRequestHandler<RequestPasswordResetCommand<Seller>, string>, RequestPasswordResetCommandHandler<Seller>>();
builder.Services.AddScoped<IRequestHandler<RequestPasswordResetCommand<Moder>, string>, RequestPasswordResetCommandHandler<Moder>>();





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

app.Run();