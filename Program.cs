using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BookshelfXchange.Maps;
using BookShelfXChange.Components;
using BookShelfXChange.Components.Account;
using BookShelfXChange.Constants;
using BookShelfXChange.Data;
using BookShelfXChange.Repository;
using BookShelfXChange.Services;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddHubOptions(opt =>
{
    opt.DisableImplicitFromServicesParameters = true;
});
builder.Services.AddMudServices();

builder.Services.AddHttpClient("https://localhost:7040", client =>
{
    // Configure the default request timeout (e.g., 30 seconds)
    client.Timeout = TimeSpan.FromSeconds(180);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddBlazoredToast();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("devAuth").AddCookie("devAuth", options =>
{
    options.Cookie.Name = "devAuth";
    options.LoginPath = "/sign-in";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddScoped<Constants>();

// Register FirebaseAuthClient
var config = builder.Configuration;
var apiKey = config["Firebase:ApiKey"];
var authDomain = config["Firebase:AuthDomain"];
var firebaseConfig = new FirebaseAuthConfig
{
    ApiKey = apiKey,
    AuthDomain = authDomain,
    Providers = new FirebaseAuthProvider[]
    {
        // Add and configure individual providers        
        new EmailProvider(),
    },

};

var firebaseAuth = new FirebaseAuthClient(firebaseConfig);

// Register FirebaseAuthClient as a service
builder.Services.AddSingleton(firebaseAuth);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(Constants.ApiBaseUrl) });
builder.Services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
// Register FirebaseAuthService
builder.Services.AddTransient<FirebaseAuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseAuthentication();
app.UseAntiforgery();
app.UseAuthorization();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
