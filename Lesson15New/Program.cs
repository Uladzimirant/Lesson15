namespace Lesson15New
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<MeetupSettings>(builder.Configuration.GetSection("MeetupSettings"));
            builder.Services.AddSingleton<MeetupService>();

            var app = builder.Build();

            app.UseRouting();
            // Configure the HTTP request pipeline.
            MeetupService s = app.Services.GetService<MeetupService>() ?? throw new ArgumentNullException("MeetupService was null at main"); ;

            app.MapGet("/", async context =>
            {
                await context.Response.WriteAsync(s.ToString());
            });

            //Лучше POST, но с браузера нормально только GET отправить можно, а всякие curl-ы и т.д. лень использовать
            //чтобы добавить что-то, пишите /add?id=<параметр>
            app.MapGet("/add", async context =>
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

            app.Run();
        }
    }
}