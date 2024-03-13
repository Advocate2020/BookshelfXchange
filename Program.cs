using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BookshelfXchange.Components;
using BookshelfXchange.Constants;
using BookshelfXchange.Maps;
using BookshelfXchange.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazoredToast();


// Configure HttpClient
builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddScoped<ApiUrl>();
builder.Services.AddScoped<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(ApiUrl.ApiBaseUrl) });

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

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();