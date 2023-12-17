using AuthorsMicroservice;
using Contracts;
using Contracts.ApiContracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ********** UniversalClient **********
//builder.Services.AddHttpClient<UniversalClient<IProductsMicroservice>>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:5001/");
//});

builder.Services.AddHttpClient("ProductsMicroservice", client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/");
});

// ********** Manual proxy with universal client **********
//builder.Services.AddScoped<IProductsMicroservice, ProductsMicroserviceProxy>(provider =>
//{
//    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("ProductsMicroservice");
//    var universalClient = new UniversalClient<IProductsMicroservice>(httpClient);
//    return new ProductsMicroserviceProxy(universalClient);
//});

// ********** Auto proxy with universal client **********
builder.Services.AddScoped<IProductsMicroservice>(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("ProductsMicroservice");
    var proxy = ProxyFactory<IProductsMicroservice>.Create(httpClient);
    return proxy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
