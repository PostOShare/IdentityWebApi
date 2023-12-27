using EntityORM.DatabaseEntity;
using Microsoft.EntityFrameworkCore;

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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
