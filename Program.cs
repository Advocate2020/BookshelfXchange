using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BookshelfXchange.Components;
using BookshelfXchange.Constants;
using BookshelfXchange.Maps;
using BookshelfXchange.Repository;
using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazoredToast();


// Configure HttpClient
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

// Register FirebaseAuthService
builder.Services.AddTransient<FirebaseAuthService>();
builder.Services.AddScoped<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(Constants.ApiBaseUrl) });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


var cookieName = ".MySessionCookie";
builder.Services.AddAuthentication(cookieName).AddCookie(cookieName, options =>
{
    options.Cookie.Name = cookieName;
    options.LoginPath = "/sign-in";
    options.AccessDeniedPath = "/not-found";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});
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

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();