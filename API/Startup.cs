using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            /*services.AddScoped<ITokenService, TokenService>();
            //SingleTon - share same object for till app shutdown
            //Scoped -- share same object for within particular http call - will use this all time
            //Transient - new object for every Interface called or used
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddDbContext<DataContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")
                ));
            */

            //above code will in extension methods
            services.AddControllers();
            services.AddCors();
            services.AddApplicationServices(Configuration);//Extension methods
            services.AddIdentityServices(Configuration);
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
            //            ValidateIssuer = false,
            //            ValidateAudience = false
            //        };
            //    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /* if (env.IsDevelopment())
             {
                 app.UseDeveloperExceptionPage(); //if we wont use exception, then we wont get exact errror occured in response, but in terminal will get some error , but servor wont crashes, and continue to send responses,to handle we have to use try-catch in controller
                 //app.UseSwagger();
                 //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
             }*/

            app.UseMiddleware<ExceptionMiddleware>();//use custom middleware

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:4200"));
            app.UseAuthentication();//ordering is important, after cores and b4 authorization
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
