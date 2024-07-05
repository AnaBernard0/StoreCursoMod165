using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NToastNotify;
using StoreCursoMod165;
using StoreCursoMod165.Data;
using StoreCursoMod165.Data.SeedDatabase;
using StoreCursoMod165.Services;
using System.Globalization;
using static StoreCursoMod165.StoreCursoMod165Constants;
using static StoreCursoMod165.StoreCursoMod165Constants.POLICIES;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 4;

}
    )
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(POLICIES.APP_POLICY_VENDEDORES.NAME, policy => policy.RequireRole(POLICIES.APP_POLICY_VENDEDORES.APP_POLICY_ROLES));
    options.AddPolicy(POLICIES.APP_POLICY_LOGISTICA.NAME, policy => policy.RequireRole(POLICIES.APP_POLICY_LOGISTICA.APP_POLICY_ROLES));
    options.AddPolicy(POLICIES.APP_POLICY_ADMINISTRADOR.NAME, policy => policy.RequireRole(POLICIES.APP_POLICY_ADMINISTRADOR.APP_POLICY_ROLES));

});

//Add Translations
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
const string defaultCulture = "pt";

CultureInfo ptCI = new CultureInfo(defaultCulture);

var supportedCultures = new[]
{
    ptCI,
    new CultureInfo("en")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services
        .AddMvc()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Resource));
        });


//Notification
builder.Services
    .AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Resource));
    })
    .AddNToastNotifyToastr(new ToastrOptions()
    {
        ProgressBar = true,
        PositionClass = ToastPositions.TopRight
    });
builder.Services
    .AddTransient<IEmailSender, EmailSender>(); // para envio de emails

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//Aply Translations
app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value

);

app.UseNToastNotify();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

SeedDB();
app.Run();



void SeedDB()
{ 
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    SeedDatabase.Seed(dbContext, userManager, roleManager);
}