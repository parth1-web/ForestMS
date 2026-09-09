using LE.Account.Common.Setup;
using LE.Account.Infrastructure.Repository.Implementations;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Implementations;
using LE.Account.Service.Assemblers.Interface;
using LE.Account.Service.Services.Implementations;
using LE.Account.Service.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace LE.Account.Service
{
    public static class IserviceCollectionExtension
    {
        public static IServiceCollection AddAccountLibrary(this IServiceCollection services)
        {
            registerRepositories(services);
            registerMakers(services);
            registerServices(services);
            registerSetups(services);
            registerProviders(services);

            return services;
        }

        private static void registerSetups(IServiceCollection services)
        {
            services.AddScoped<SettingSetup, SettingsSetupImpl>();
        }

        private static void registerRepositories(IServiceCollection services)
        {
            services.AddScoped<LedgerGroupRepository, LedgerGroupRepositoryImpl>();
            services.AddScoped<LedgerRepository, LedgerRepositoryImpl>();
            services.AddScoped<TransactionRepository, TransactionRepositoryImpl>();
            services.AddScoped<TransactionDetailRepository, TransactionDetailRepositoryImpl>();
            services.AddScoped<PaymentRepository, PaymentRepositoryImpl>();
            services.AddScoped<ReceiptRepository, ReceiptRepositoryImpl>();
            services.AddScoped<AccountSettingsRepository, AccountSettingsRepositoryImpl>();
            services.AddScoped<LedgerSetupRepository, LedgerSetupRepositoryImpl>();
        }

        private static void registerServices(IServiceCollection services)
        {
            services.AddScoped<LedgerGroupService, LedgerGroupServiceImpl>();
            services.AddScoped<LedgerService, LedgerServiceImpl>();
            services.AddScoped<TransactionService, TransactionServiceImpl>();
            services.AddScoped<TransactionDetailService, TransactionDetailServiceImpl>();
            services.AddScoped<LedgerGroupIdProviderService, LedgerGroupIdProviderServiceImpl>();
            services.AddScoped<FiscalYearSetupService, FiscalYearSetupServiceImpl>();
            services.AddScoped<PaymentService, PaymentServiceImpl>();
            services.AddScoped<ReceiptService, ReceiptServiceImpl>();
            services.AddScoped<JournalService, JournalServiceImpl>();
            services.AddScoped<ReportService, ReportServiceImpl>();
            services.AddScoped<LedgerSetupService, LedgerSetupServiceImpl>();
        }

        private static void registerMakers(IServiceCollection services)
        {
            services.AddScoped<LedgerAssembler, LedgerAssemblerImpl>();
            services.AddScoped<TransactionDtoAssembler, TransactionDtoAssemblerImpl>();
            services.AddScoped<TransactionDetailDtoAssembler, TransactionDetailDtoAssemblerImpl>();
            services.AddScoped<TransactionAssembler, TransactionAssemblerImpl>();
            services.AddScoped<PaymentAssembler, PaymentAssemblerImpl>();
            services.AddScoped<ReceiptAssembler, ReceiptAssemblerImpl>();
        }

        private static void registerProviders(IServiceCollection services)
        {
            services.AddScoped<LedgerIdProvider, LegerIdProviderImpl>();
        }
    }
}
