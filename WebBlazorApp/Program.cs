using Abc.AuthorLibrary;
using ABC.BooksLibrary;
using ABC.BusinessBase;
using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using WebBlazorApp.Data;
using WebBlazorApp.Filters;
using Microsoft.AspNetCore.ResponseCompression;
using WebBlazorApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, BlazorAuthenticationStateProvider>();


builder.Services.AddSingleton<IUserInfoService, UserInfoService>();

builder.Services.AddDbContext<DbContext, AbcContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("sql"),
                                                    x => x.MigrationsAssembly("ABC.BusinessBase")),
                                                    contextLifetime: ServiceLifetime.Singleton);

builder.Services.AddTransient<DbConnectionProvider>();

builder.Services.AddSingleton<IAuthorRepository, AuthorRepository>();
builder.Services.AddSingleton<IBooksRepository, BookRepository>();
builder.Services.AddSingleton<IPersonRepository, PersonRepository>();

builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IGenericTransactionService, GenericTransactionService>();

builder.Services.AddTransient<IAuthorService, AuthorService>();
builder.Services.AddTransient<IBookService, BookService>();

builder.Services.AddTransient<IPersonService, PersonService>();


builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new [] { "application/octet-stream" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    
}

app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = new CustomExceptionHandler().Invoke });

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthorization();
app.UseRouting();

app.UseResponseCompression();

app.MapHub<ChatHub>("/chathub");
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


app.Run();
