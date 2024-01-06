using EntityORM.DatabaseEntity;
using IdentityWebApi.Configuration;
using IdentityWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace IdentityWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connection = "Data Source=LAPTOP-01RB8N41\\MSSQL2;Initial Catalog=IdentityPM;User ID=sa;Password=Sq1231";
            builder.Services.AddControllers();
            builder.Services.AddDbContext<IdentityPmContext>(options => options.UseSqlServer(connection));
            builder.Services.AddScoped<IEmailService,Email>();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(swagger =>
                {
                    swagger.EnableAnnotations();
                    swagger.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "PostOShare Identity API",
                        Version = "v1",
                        Description = "API documentation for managing and searching users"
                    });
                }) ;

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
