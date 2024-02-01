using Hangfire;
using Hangfire.Dashboard;
using Videos.Application;
using Videos.Persistence;
using Videos.WebApi.Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();


DBInit(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

//app.UseAuthorization();


app.UseHangfireDashboard("/jobs", new DashboardOptions()
{
    Authorization = new[] { new CustomAuthorizeFilter() }
});

app.UseHttpsRedirection();

app.MapHangfireDashboard();
app.MapControllers();

app.Run();

void DBInit(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        try
        {
            var context = serviceProvider.GetRequiredService<VideoDbContext>();
            DbInitializer.Initialize(context);
        }
        catch (Exception)
        {
            throw;
        }
    }
}