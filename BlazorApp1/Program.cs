using BlazorApp1.Components;
using BlazorApp1.Components.Account;
using BlazorApp1.Data;
using BlazorApp1.Handlers;
using BlazorApp1.Hubs;
using BlazorApp1.Services;
using BlazorApp1.Services.Server;
using BlazorStrap;
using BlazorStrap.V5;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorStrap();


builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<HttpClient>(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

    var client = new HttpClient
    {
        BaseAddress = new Uri(navigationManager.BaseUri)
    };
    var httpContext = httpContextAccessor.HttpContext;
    if (httpContext?.User?.Identity?.IsAuthenticated == true)
    {
    }

    return client;
});
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<DiceService>();
builder.Services.AddScoped<CharacterPdfService>();
builder.Services.AddScoped<DndBeyondImportService>();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:7282")
});


// Add HttpContextAccessor and CookieHandler
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CookieHandler>();

// Register HttpClients with CookieHandler
builder.Services.AddHttpClient<CampaignService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7282/");
})
.AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient<CharacterService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7282/");
})
.AddHttpMessageHandler<CookieHandler>();
;
builder.Services.AddHttpClient<ChatService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7282/");
})
.AddHttpMessageHandler<CookieHandler>();
builder.Services.AddHttpClient<DndBeyondImportService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7282/");
})
.AddHttpMessageHandler<CookieHandler>();
builder.Services.AddHttpClient<InventoryService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7282/");
})
.AddHttpMessageHandler<CookieHandler>();
builder.Services.AddHttpClient<JournalService>(client =>
{ 
    client.BaseAddress = new Uri("https://localhost:7282/");
})
.AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient<PlayerEventService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7282/");
})
.AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient<ResourceService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7282/");
})
.AddHttpMessageHandler<CookieHandler>();



builder.Services.AddControllers();
/*builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});*/
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services
    .AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    })
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    });

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("StreamDeckLocal", policy =>
    {
        policy
            .WithOrigins("http://localhost", "http://127.0.0.1", "https://localhost", "https://localhost:7282")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("StreamDeckLocal");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Request.Headers["X-Forwarded-Proto"] = "https";
    await next();
});

//app.UseAntiforgery();
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/api")
    && !context.Request.Path.StartsWithSegments("/resourcehub"),
    appBuilder => appBuilder.UseAntiforgery()
);
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapHub<RollHub>("/rollhub");
app.MapHub<ResourceHub>("/resourcehub");

app.Run();