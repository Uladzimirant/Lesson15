using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lesson15Old
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
            services.Configure<MeetupSettings>(Configuration.GetSection("MeetupSettings"));
            services.AddSingleton<MeetupService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MeetupService s)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(s.ToString());
                });
                //Лучше POST, но с браузера нормально только GET отправить можно, а всякие curl-ы и т.д. лень использовать
                //чтобы добавить что-то, пишите /add?id=<параметр>
                endpoints.MapGet("/add", async context =>
                {
                    try
                    {
                        var id = context.Request.Query["id"];
                        if (string.IsNullOrEmpty(id)) throw new HelperException("no id", "Parameter \'id\' is missing");
                        if (s.Add(id))
                        {
                            context.Response.StatusCode = 201;
                            await context.Response.WriteAsync($"Successfully added user with id \'{id}\'");
                        }
                        else throw new HelperException("add fail", "Maximum size for meeting reached. Couldn't add.");
                    }
                    catch (HelperException e)
                    {
                        switch (e.Type)
                        {
                            case "no id":
                                context.Response.StatusCode = 422;
                                await context.Response.WriteAsync(e.Message);
                                break;
                            case "add fail":
                                context.Response.StatusCode = 409;
                                await context.Response.WriteAsync(e.Message);
                                break;
                        }
                    }
                });
            });
        }

        private class HelperException : Exception
        {
            public string Type;
            public HelperException(string type, string message) : base(message)
            {
                Type = type;
            }
        }

    }
}
