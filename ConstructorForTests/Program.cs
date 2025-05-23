using ConstructorForTests.API;
using ConstructorForTests.Database;
using ConstructorForTests.Handlers;
using ConstructorForTests.Quartz;
using ConstructorForTests.Repositories;
using ConstructorForTests.UserSolutionHandler;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		//Cors
		builder.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(builder =>
			{
				builder.SetIsOriginAllowed(origin => true)
					   .AllowAnyMethod()
					   .AllowAnyHeader()
					   .AllowCredentials();
			});
		});

		//Documentation
		builder.Services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo
			{
				Version = "v1",
				Title = "Constructor for test API",
				Description = "управление тестами ссылка на сайт - https://constructor-dev-ed2c.onrender.com",
			});
		});

		// Add services to the container.
		builder.Services.AddScoped<IAuthenticationRepo, AuthenticationRepo>();
		builder.Services.AddScoped<ITestRepo, TestRepo>();
		builder.Services.AddScoped<IUserRepo, UserRepo>();
		builder.Services.AddScoped<ISolutionHandler, SolutionHandler>();
		builder.Services.AddScoped<IEmailSender, EmailSender>();

		//Connect Db
		builder.Services.AddDbContext<AppDbContext>(opt =>
		{
			opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
		});
		builder.Services.AddDistributedMemoryCache();

		//Add Sesion options
		builder.Services.AddSession(options =>
		{
			options.Cookie.Name = "Session";
			options.IdleTimeout = TimeSpan.FromDays(1);
			options.Cookie.HttpOnly = true;
			options.Cookie.IsEssential = true;
		});

		//Add quartz
		builder.Services.AddQuartz(q =>
		{
			var jobKey = new JobKey("CheckDeadlineJob");
			q.AddJob<CheckDeadlineJob>(opts => opts.WithIdentity(jobKey));

			q.AddTrigger(opts => opts
			.ForJob(jobKey)
			.WithIdentity("CheckDeadlineJob-trigger")
			//Every minutes
			//.WithCronSchedule("0 * * ? * *")
			.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(00, 00))
			);
		});
		builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

		builder.Services.AddControllers();

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(options =>
		{
			var basePath = AppContext.BaseDirectory;

			var xmlPath = Path.Combine(basePath, "ConstructorForTests.xml");
			options.IncludeXmlComments(xmlPath);
		});

		builder.Services.AddHttpContextAccessor(); 

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.yaml", "v1");
			});
		}

		app.UseCors();

		app.UseSession();

		app.UseHttpsRedirection();

		app.UseAuthentication();

		app.MapControllers();

		app.Run();
	}
}