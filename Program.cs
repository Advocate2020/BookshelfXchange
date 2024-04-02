using Blazored.LocalStorage;
using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BookshelfXchange.Components;
using BookshelfXchange.Constants;
using BookshelfXchange.Maps;
using BookshelfXchange.Middleware;
using BookshelfXchange.Repository;
using BookshelfXchange.Services;
using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddBlazoredToast();
// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();
// Register your custom AuthenticationStateProvider
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetService<CustomAuthenticationStateProvider>());
// Configure HttpClient
builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddScoped<Constants>();

builder.Services.AddScoped<ICookie, Cookie>();

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

// Register FirebaseAuthService
builder.Services.AddTransient<FirebaseAuthService>();
builder.Services.AddScoped<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(Constants.ApiBaseUrl) });
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "devAuth";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/access-denied";
        options.LoginPath = "/sign-in";

    });

builder.Services.AddAuthorization();

//Configure Firebase
#region Firebase Secrets

try
{

    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(Constants.FirebaseSecret)
    });
}
catch (Exception e)
{
    Console.WriteLine($"# Firebase setup failed : {e.Message}");
    throw;
}
#endregion
// Specify API base URL
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
builder.Services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();