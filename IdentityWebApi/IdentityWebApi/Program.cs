using EntityORM.DatabaseEntity;
using IdentityWebApi.ApiFilters;
using IdentityWebApi.Repositories;
using IdentityWebApi.Services;
using IdentityWebApiCommon.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace IdentityWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           
            builder.Services.AddDbContext<IdentityPMContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionDB")!));
            builder.Services.AddScoped<IEmailRepository,EmailRepository>();
            builder.Services.AddScoped<IIdentityService,IdentityService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddHttpClient("InternalApi", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration.GetSection("BaseUrl").Value!);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelFilter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            }); ;

            
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