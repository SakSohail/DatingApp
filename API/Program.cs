using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Repository pattern - Repository  mediates b/w domain and data mapping layers,acting like an in-memory domain object collection

//what we are doing right now - 
//web server <==> controller <==> DbContext <==> Database



//Repository pattern
//web server <==> controller <==> Repository <==> DbContext <==> Database

//reduce duplicate code, means resusabillty
//testing will be easy
//unit test ==> controller ==> Mock repository

//Dis advantages
//abstraction of abstraction - DBContext is already making abstraction
//each entity has its own repository, so more code 
//need to implement Unit of Work Pattern to control transaction

//image can be uploadde into DB as blob,into server as filesystem,and cloud
//best will be cloud no disk space problem but it will cost
//Cloudinary -- cloud platform

/* flow 
 1) client upload image with JWT to server
2) server uploads the photo to CLoudinary with key given by cloudinary for authentication
3)cloudinary stores photo and send response
4)api stores photo url and public id to DB
5)saved in DB and given auto generated id
6)201 created response sent to client with location header
 */
namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();//creates new IServiceScope that can be used to resolve service scope

            var services = scope.ServiceProvider;
            try //middleware not accesble so we have to handle exception
            {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();//asyncronlusy applies pending migration for context to DB, and create Db if not exists
                //we can restart application for DB recreation
                //if DB drops , then restart application to recreate DB
                //drop Db from CLI - drop-database and restart application

                await Seed.SeedUsers(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }

            await host.RunAsync();//call run method at last after above opreation completes
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
