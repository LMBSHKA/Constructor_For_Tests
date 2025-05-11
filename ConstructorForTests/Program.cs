using ConstructorForTests.Database;
using ConstructorForTests.Filters;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddScoped<IAuthenticationRepo, AuthenticationRepo>();
		builder.Services.AddScoped<ITestRepo, TestRepo>();
		builder.Services.AddScoped<IUserSolutionRepo, UserSolutionRepo>();

		builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

		//Connect Db
		//builder.Services.AddDbContext<AppDbContext>(opt =>
		//{
		//	opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
		//});
		builder.Services.AddDistributedMemoryCache();

		//Add Sesion options
		builder.Services.AddSession(options =>
		{
			options.Cookie.Name = "Session";
			options.IdleTimeout = TimeSpan.FromDays(1);
			options.Cookie.HttpOnly = true;
			options.Cookie.IsEssential = true;
		});

		builder.Services.AddControllers();

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddHttpContextAccessor();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		PrepDb.PrepPopulation(app);

		//app.UseRouting();

		app.UseSession();

		app.UseHttpsRedirection();

		app.UseAuthentication();

		app.MapControllers();

		app.Run();
	}
}