using EducationPlatform.BLL.IServices;
using EducationPlatform.BLL.Services;
using Microsoft.EntityFrameworkCore;
using EducationPlatform.DAL.Data;
using EducationPlatform.DAL.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//  Entity Framework DbContext
builder.Services.AddDbContext<EducationDbContext>(options =>
    options.UseSqlServer(connectionString));

//  Identity Services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<EducationDbContext>()
    .AddDefaultTokenProviders();

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("InstructorOnly", policy => policy.RequireRole("Instructor"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Repository Pattern Registration - THIS WAS MISSING!
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add DAL repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// generic repository 
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// BLL Services Registration
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();

// AutoMapper configuration
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
});

builder.Services.AddLogging();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();