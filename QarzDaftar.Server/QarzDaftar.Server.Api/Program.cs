using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Services.Foundations.Customers;
using QarzDaftar.Server.Api.Services.Foundations.Debts;
using QarzDaftar.Server.Api.Services.Foundations.Payments;
using QarzDaftar.Server.Api.Services.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Services.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Services.Foundations.UserNotes;
using QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Services.Foundations.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StorageBroker>();
builder.Services.AddTransient<IStorageBroker, StorageBroker>();
builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
builder.Services.AddTransient<IDateTimeBroker, DateTimeBroker>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IDebtService, DebtService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IUserNoteService, UserNoteService>();
builder.Services.AddTransient<ISubscriptionHistoryService, SubscriptionHistoryService>();
builder.Services.AddTransient<IUserPaymentLogService, UserPaymentLogService>();
builder.Services.AddTransient<ISuperAdminService, SuperAdminService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();