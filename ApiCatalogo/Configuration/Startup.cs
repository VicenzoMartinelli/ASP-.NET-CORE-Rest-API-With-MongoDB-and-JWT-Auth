using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Api.Repositórios;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddTransient<DBContext>();
      services.AddTransient<IRepositoryUser, RepositoryUser>();


      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "MybeApi",
                ValidAudience = "MybeApi",
                IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
              };

              options.Events = new JwtBearerEvents
              {
                OnAuthenticationFailed = context =>
                {
                  Debug.WriteLine("Token inválido..:. " + context.Exception.Message);
                  return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                  Debug.WriteLine($"Toekn válido...: {context.SecurityToken} - Data: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
                  return Task.CompletedTask;
                }
              };
            });

      services.AddAuthorization(options =>
      {
        options.AddPolicy("Over18", policy =>
        {
          policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
          policy.RequireAuthenticatedUser();
        });
      });

      services.AddMvc();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info { Title = "Buble", Version = "v1" });
      });

      services.AddAutoMapper();
      
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Buble v1");
        c.DocumentTitle = "BuBle API";
        c.DefaultModelsExpandDepth(-1);
        c.RoutePrefix = "";
      });

      app.UseMvc(routes =>
      {
        //routes.MapRoute(
        //  name: "Home",
        //  template: "{Page = Home}"
        //  );
      });

    }
  }
}
