﻿
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System;

using Avokates_CRM;

using Avokates_CRM.Models.DB;
using WebSite.Helpers;
using WebSite.DataLayer;

namespace WebSite
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            HelperSecurity.Init(configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // получаем строку подключения из файла конфигурации
            string connection = Configuration.GetConnectionString("DefaultConnection");
            // добавляем контекст AdvokatesContext в качестве сервиса в приложение
            services.AddDbContext<LawyerCRMContext>(options =>
                options.UseSqlServer(connection));

            services.AddScoped<ISecuryty, Security>();
            services.AddScoped<IDataLayer, DataLayerDB>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;                                              // ОБЯЗАТЕЛЬНО ПОМЕНЯТЬ на false!!!!
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // добавление кэширования
            services.AddMemoryCache();
           // services.AddDistributedMemoryCache();

            services.AddSession(
                //options => {                options.IdleTimeout = TimeSpan.FromMinutes(60);            }
                );

            services.AddAuthentication
                (options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,                // валидация ключа безопасности
                        ValidateIssuer = false,                         // укзывает, будет ли валидироваться издатель при валидации токена 
                        ValidateAudience = false,                       // будет ли валидироваться потребитель токена   
                        //ValidIssuer = AuthHelper.ISSUER,               // строка, представляющая издателя
                        //ValidAudience = AuthHelper.AUDIENCE,           // установка потребителя токена
                                                                        // TODO: изменить алгоритм шифрования на асимметричный. В админке использовать публичный ключ, в апи - приватный
                        //IssuerSigningKey = AuthHelper.GetSymmetricSecurityKey(Configuration),                   // установка ключа безопасности
                        ValidateLifetime = false,   // будет ли валидироваться время существования
                        ClockSkew = System.TimeSpan.FromMinutes(30000)
                    };
                });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("EnableCors", builder =>
            //    {
            //        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials().Build();
            //    });
            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();               // конфигурировать сессии, чтоб можно было получать из них токен

            // внедряем токен в хидер поступающих запросов
            app.Use(async (context, next) =>
            {
                //string[] s = context.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
                //if (!(s.Length>0 && s[0]=="Data"))
                //{
                    var token = context.Session.GetString("token");
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Request.Headers.Add("Authorization", "Bearer " + token);
                    }
                    else
                    {       // перехват запроса и возврат на авторизацию
                        context.Request.Path = "/Auth/Authorization/";
                    }
                //}
                await next();
            });

            app.UseAuthentication();

            app.UseCors("EnableCors");

            //BaseHelper.Init(Configuration);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Auth}/{action=Authorization}/{id?}");
            });
        }
    }
}