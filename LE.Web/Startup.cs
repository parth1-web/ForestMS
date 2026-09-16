using Autofac;
using AutoMapper;
using LE.Account.Common.Setup;
using LE.Account.Infrastructure.Reports.QueryService;
using LE.Account.Infrastructure.Reports.Reporter;
using LE.Account.Infrastructure.Repository.Implementations;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Implementations;
using LE.Account.Service.Assemblers.Interface;
using LE.Account.Service.Services.FinancialYearService;
using LE.Account.Service.Services.Implementations;
using LE.Account.Service.Services.Interface;
using LE.Billing.Factories.AbstractFactory.Implementations;
using LE.Billing.Factories.AbstractFactory.Interface;
using LE.Billing.Infrastructure.Repository.Implementations;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Implementations;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Implementations;
using LE.Billing.Service.Services.Interface;
using LE.Common.Library;
using LE.Common.Repository.Implementations;
using LE.Common.Repository.Interface;
using LE.Context.Data;
using LE.Context.Repository.Implementations;
using LE.Inventory.Infrastructure.Repository.Implementations;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Adapter.Implementations;
using LE.Inventory.Service.Adapter.Interface;
using LE.Inventory.Service.Assemblers.Implementations;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Implementations;
using LE.Inventory.Service.Services.Interface;
using LE.Service.Assembler.Implementations;
using LE.Service.Assembler.Interface;
using LE.Service.Repository.Interface;
using LE.Service.Services.Implementations;
using LE.Service.Services.Interface;
using LE.Web.Autofac.Modules;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rotativa.AspNetCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace LE.Web
{
    public class Startup
    {
        [Obsolete]
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public ILifetimeScope AutofacContainer { get; private set; }

        public IContainer ApplicationContainer { get; private set; }

        [Obsolete]
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModule());
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseLazyLoadingProxies()
                       .UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("LE.Web"));
                options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            });

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.Configure<ForwardedHeadersOptions>(options => { options.KnownProxies.Add(IPAddress.Parse("10.0.0.100")); });
            registerElements(services);
            services.AddAuthentication(options =>
            {
                // Cookie auth is the default for the web UI (login pages, MVC controllers).
                // JWT remains available for API controllers that explicitly opt in via
                // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)].
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
             .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
             {
                 options.Cookie.HttpOnly = true;
                 options.Cookie.Name = "LE.Auth";
                 // Read the configured expiry (hours) instead of the previous hard-coded 1 minute.
                 int cookieExpirationHours = Configuration.GetValue<int>("Security:CookieExpirationHours", 8);
                 options.ExpireTimeSpan = TimeSpan.FromHours(cookieExpirationHours);
                 options.SlidingExpiration = false;
                 options.LoginPath = "/account/login";
                 options.LogoutPath = "/account/logout";
                 options.AccessDeniedPath = "/error/403";
             })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"] ?? "thisisasecreteforauth")),
                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddMvc(options =>
            {
                // Require an authenticated user for every action unless it is explicitly
                // opted-out with [AllowAnonymous] (login pages, error pages, public assets).
                options.Filters.Add(new AuthorizeFilter());

                // P1/S3 fix: enforce role_permission_maps at action level. The filter is
                // resolved through DI (TypeFilter) so it can take repository dependencies;
                // it only guards requests routed to a permissioned MVC area.
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.TypeFilterAttribute(typeof(Helpers.ModulePermissionFilter)));
            }).AddControllersAsServices()
              .AddNewtonsoftJson(jsonOptions =>
              {
                  jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
              }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (t, f) => f.Create(typeof(SharedResource)));

            services.AddResponseCaching();
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            //for compressing data
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "imagejpeg", "png", "jpeg", "jpg" });
            });
            services.AddSession();

            services.Configure<GzipCompressionProviderOptions>(option =>
            option.Level = System.IO.Compression.CompressionLevel.Optimal);

            services.Configure<RequestLocalizationOptions>(opts =>
           {
               var supportedCultures = new List<CultureInfo>
               {
                    new CultureInfo("en"),
                    new CultureInfo("ne"),
               };

               opts.DefaultRequestCulture = new RequestCulture("en");
               opts.SupportedCultures = supportedCultures;
               opts.SupportedUICultures = supportedCultures;
           });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler("/Error/{0}");
                // S11: enforce HTTPS in production unless explicitly opted out
                // (e.g. TLS terminated at a proxy that is not configured here).
                if (Configuration.GetValue<bool>("Security:EnableHsts", true))
                {
                    app.UseHsts();
                }
            }

            if (Configuration.GetValue<bool>("Security:EnableHttpsRedirect", true))
            {
                // Dev runs on plain HTTP by default; turning this off avoids a
                // redirect loop when the app sits behind a non-HTTPS-aware proxy.
                app.UseHttpsRedirection();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Request-level logging (method, path, status, timing, user id).
            app.UseSerilogRequestLogging();

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });
            app.UseResponseCaching();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCompression();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Ledger}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            RotativaConfiguration.Setup(env, "Rotativa");
        }

        private void registerElements(IServiceCollection services)
        {
            registerRepos(services);
            registerAssemblers(services);
            registerHelpers(services);
            registerLibraries(services);
            registerSetup(services);
            registerServices(services);
            registerAdapers(services);
            registerFactories(services);
            registerProviders(services);
            registerSetupElements(services);
        }

        private void registerSetupElements(IServiceCollection services)
        {
            services.AddScoped<OrganizationSetupRepository, OrganizationSetupRepositoryImpl>();
            services.AddScoped<OrganizationSetupService, OrganizationSetupServiceImpl>();
            services.AddScoped<ModuleRepository, ModuleRepositoryImpl>();
        }

        private void registerProviders(IServiceCollection services)
        {
            registerAccountProviders(services);
        }

        private void registerAccountProviders(IServiceCollection services)
        {
            services.AddScoped<LedgerIdProvider, LegerIdProviderImpl>();
        }

        private void registerFactories(IServiceCollection services)
        {
            services.AddScoped<MemberServiceFactory, MemberServiceFactoryImpl>();
            services.AddScoped<WoodBillServiceFactory, WoodBillServiceFactoryImpl>();
        }

        private void registerAdapers(IServiceCollection services)
        {
            services.AddScoped<Movement_ItemAvailabilityAdapter, Movement_ItemAvailabilityAdapterImpl>();
        }

        private void registerLibraries(IServiceCollection services)
        {
            registerUserLibraries(services);
            services.AddSingleton<PaginatedMetaService, PaginatedMetaServiceImpl>();
            services.AddSingleton<DateConverterService, DateConverterServiceImpl>();
            services.AddSingleton<LoginAttemptTracker, LoginAttemptTracker>();
        }

        private void registerUserLibraries(IServiceCollection services)
        {
            services.AddScoped<EncryptDecrypt, EncryptDecryptImpl>();
            services.AddScoped<PasswordHash, PasswordHashImpl>();
        }

        private void registerHelpers(IServiceCollection services)
        {
            services.AddScoped<FileHelper, FileHelperImpl>();
        }

        private void registerAssemblers(IServiceCollection services)
        {
            registerUserMakers(services);
            registerAccountMakers(services);
            registerBillingAssemblers(services);
            registerInventoryAssemblers(services);
        }

        private void registerInventoryAssemblers(IServiceCollection services)
        {
            services.AddScoped<StockCategoryPurposeAssembler, StockCategoryPurposeAssemblerImpl>();
            services.AddScoped<WoodTypeAssembler, WoodTypeAssemblerImpl>();
            services.AddScoped<StockUnitAssembler, StockUnitAssemblerImpl>();
            services.AddScoped<StockItemAssembler, StockItemAssemblerImpl>();
            services.AddScoped<PurchaseAssembler, PurchaseAssemblerImpl>();
            services.AddScoped<PilingAssembler, PilingAssemblerImpl>();
            services.AddScoped<StockMovementAssembler, StockMovementAssemblerImpl>();
            services.AddScoped<StockItemAvailabilityAssembler, StockItemAvailabilityAssemblerImpl>();
            services.AddScoped<WoodDetailsAssembler, WoodDetailsAssemblerImpl>();
            services.AddScoped<DamagedWoodDetailAssembler, DamagedWoodDetailAssemblerImpl>();
        }

        private void registerBillingAssemblers(IServiceCollection services)
        {
            services.AddScoped<ChiranSalesAssembler, ChiranSalesAssemblerImpl>();
            services.AddScoped<ChiranSalesDetailAssembler, ChiranSalesDetailAssemblerImpl>();
            services.AddScoped<CounterSalesAssembler, CounterSalesAssemblerImpl>();
            services.AddScoped<CounterSalesDetailAssembler, CounterSalesDetailAssemblerImpl>();
            services.AddScoped<FirewoodSalesAssembler, FirewoodSalesAssemblerImpl>();
            services.AddScoped<FirewoodSalesDetailAssembler, FirewoodSalesDetailAssemblerImpl>();
            services.AddScoped<FurnitureSalesAssembler, FurnitureSalesAssemblerImpl>();
            services.AddScoped<FurnitureSalesDetailAssembler, FurnitureSalesDetailAssemblerImpl>();
            services.AddScoped<MembershipAssembler, MembershipAssemblerImpl>();
            services.AddScoped<MemberAssembler, MemberAssemblerImpl>();
            services.AddScoped<MembershipValidityAssembler, MembershipValidityAssemblerImpl>();
            services.AddScoped<MemberPunishmentAssembler, MemberPunishmentAssemblerImpl>();
            services.AddScoped<ServiceAssembler, ServiceAssemblerImpl>();
            services.AddScoped<ServiceCategoryAssembler, ServiceCaetegoryAssemblerImpl>();
            services.AddScoped<FurnitureAssembler, FurnitureAssemblerImpl>();
            services.AddScoped<FurnitureCategoryAssembler, FurnitureCategoryAssemblerImpl>();
            services.AddScoped<ToleAssembler, ToleAssemblerImpl>();
            services.AddScoped<WoodBillAssembler, WoodBillAssemblerImpl>();
            services.AddScoped<WoodBillDetailAssembler, WoodBillDetailAssemblerImpl>();
            services.AddScoped<WoodBillMembersAssembler, WoodBillMembersAssemblerImpl>();
            services.AddScoped<WoodBillMemberTransactionAssembler, WoodBillMemberTransactionAssemblerImpl>();
            services.AddScoped<DayCloseAssembler, DayCloseAssemblerImpl>();
        }

        private void registerAccountMakers(IServiceCollection services)
        {
            services.AddScoped<LedgerAssembler, LedgerAssemblerImpl>();
            services.AddScoped<LedgerBalanceAssembler, LedgerBalanceAssemblerImpl>();
            services.AddScoped<TransactionDtoAssembler, TransactionDtoAssemblerImpl>();
            services.AddScoped<TransactionDetailDtoAssembler, TransactionDetailDtoAssemblerImpl>();
            services.AddScoped<TransactionAssembler, TransactionAssemblerImpl>();
            services.AddScoped<LE.Account.Service.Assemblers.Interface.PaymentAssembler, LE.Account.Service.Assemblers.Implementations.PaymentAssemblerImpl>();
            services.AddScoped<ReceiptAssembler, ReceiptAssemblerImpl>();
        }

        private void registerUserMakers(IServiceCollection services)
        {
            services.AddScoped<AuthenticationMaker, AuthenticationMakerImpl>();
            services.AddScoped<LoginSessionMaker, LoginSessionMakerImpl>();
            services.AddScoped<UserMaker, UserMakerImpl>();
            services.AddScoped<ModuleAssembler, ModuleAssemblerImpl>();
            services.AddScoped<DynamicMenuAssembler, DynamicMenuAssemblerImpl>();
        }

        private void registerRepos(IServiceCollection services)
        {
            services.AddScoped(typeof(BaseRepository<>), typeof(BaseRepositoryImpl<>));
            registerUserRepos(services);
            registerAccountRepos(services);
            registerBillingRepos(services);
            registerInventoryRepos(services);
            registerOrganizationSetup(services);
        }

        private void registerInventoryRepos(IServiceCollection services)
        {
            services.AddScoped<StockCategoryPurposeRepository, StockCategoryPurposeRepositoryImpl>();
            services.AddScoped<WoodTypeRepository, WoodTypeRepositoryImpl>();
            services.AddScoped<StockUnitRepository, StockUnitRepositoryImpl>();
            services.AddScoped<StockItemRepository, StockItemRepositoryImpl>();
            services.AddScoped<StockItemAvailabilityRepository, StockItemAvailabilityRepositoryImpl>();
            services.AddScoped<PurchaseRepository, PurchaseRepositoryImpl>();
            services.AddScoped<PilingRepository, PilingRepositoryImpl>();
            services.AddScoped<StockMovementRepository, StockMovementRepositoryImpl>();
            services.AddScoped<WoodDetailsRepository, WoodDetailsRepositoryImpl>();
            services.AddScoped<FirewoodSalesRepository, FirewoodSalesRepositoryImpl>();
            services.AddScoped<FirewoodSalesDetailRepository, FirewoodSalesDetailRepositoryImpl>();
            services.AddScoped<DamagedWoodDetailRepository, DamagedWoodDetailRepositoryImpl>();
        }

        private void registerBillingRepos(IServiceCollection services)
        {
            services.AddScoped<ChiranSalesRepository, ChiranSalesRepositoryImpl>();
            services.AddScoped<ChiranSalesDetailRepository, ChiranSalesDetailRepositoryImpl>();
            services.AddScoped<BillingSettingsRepository, BillingSettingsRepositoryImpl>();
            services.AddScoped<CounterSalesDetailRepository, CounterSalesDetailRepositoryImpl>();
            services.AddScoped<CounterSalesRepository, CounterSalesRepositoryImpl>();
            services.AddScoped<FurnitureSalesRepository, FurnitureSalesRepositoryImpl>();
            services.AddScoped<FurnitureSalesDetailRepository, FurnitureSalesDetailRepositoryImpl>();
            services.AddScoped<FirewoodSalesDetailRepository, FirewoodSalesDetailRepositoryImpl>();
            services.AddScoped<FirewoodSalesRepository, FirewoodSalesRepositoryImpl>();
            services.AddScoped<MembershipRepository, MembershipRepositoryImpl>();
			services.AddScoped<MemberRepository, MemberRepositoryImpl>();
			services.AddScoped <MembershipValidityRepository, MembershipValidityRepositoryImpl>();
            services.AddScoped<MemberPunishmentRepository, MemberPunishmentRepositoryImpl>();
            services.AddScoped<ServiceCategoryRepository, ServiceCategoryRepositoryImpl>();
            services.AddScoped<ServiceRepository, ServiceRepositoryImpl>();
            services.AddScoped<ToleRepository, ToleRepositoryImpl>();
            services.AddScoped<FurnitureCategoryRepository, FurnitureCategoryRepositoryImpl>();
            services.AddScoped<FurnitureRepository, FurnitureRepositoryImpl>();
            services.AddScoped<WoodBillDetailRepository, WoodBillDetailRepositoryImpl>();
            services.AddScoped<WoodBillMemberRepository, WoodBillMemberRepositoryImpl>();
            services.AddScoped<WoodBillMemberTransactionRepository, WoodBillMemberTransactionRepositoryImpl>();
            services.AddScoped<WoodBillRepository, WoodBillRepositoryImpl>();
            services.AddScoped<DayCloseRepository, DayCloseRepositoryImpl>();
        }

        private void registerOrganizationSetup(IServiceCollection services)
        {
            services.AddScoped<OrganizationSetupRepository, OrganizationSetupRepositoryImpl>();
        }

        private void registerServices(IServiceCollection services)
        {
            registerUserServices(services);
            registerAccountServices(services);
            registerBillingServices(services);
            registerInventoryServices(services);
        }

        private void registerInventoryServices(IServiceCollection services)
        {
            services.AddScoped<StockCategoryPurposeService, StockCategoryPurposeServiceImpl>();
            services.AddScoped<WoodTypeService, WoodTypeServiceImpl>();
            services.AddScoped<StockUnitService, StockUnitServiceImpl>();
            services.AddScoped<StockItemService, StockItemServiceImpl>();
            services.AddScoped<PurchaseService, PurchaseServiceImpl>();
            services.AddScoped<PilingService, PilingServiceImpl>();
            services.AddScoped<StockMovementService, StockMovementServiceImpl>();
            services.AddScoped<StockItemAvailabilityService, StockItemAvailabilityServiceImpl>();
            services.AddScoped<WoodDetailsService, WoodDetailsServiceImpl>();
            services.AddScoped<DamagedWoodDetailService, DamagedWoodDetailServiceImpl>();
        }

        private void registerBillingServices(IServiceCollection services)
        {
            services.AddScoped<ChiranSalesService, ChiranSalesServiceImpl>();
            services.AddScoped<ChiranSalesDetailService, ChiranSalesDetailServiceImpl>();
            services.AddScoped<CounterSalesDetailService, CounterSalesDetailServiceImpl>();
            services.AddScoped<CounterSalesService, CounterSalesServiceImpl>();
            services.AddScoped<FirewoodSalesDetailService, FirewoodSalesDetailServiceImpl>();
            services.AddScoped<FirewoodSalesService, FirewoodSalesServiceImpl>();
            services.AddScoped<FurnitureSalesDetailService, FurnitureSalesDetailServiceImpl>();
            services.AddScoped<FurnitureSalesService, FurnitureSalesServiceImpl>();
            services.AddScoped<MembershipService, MembershipServiceImpl>();
            services.AddScoped<MemberService, MemberServiceImpl>();
			services.AddScoped<MembershipValidityService, MembershipValidityServiceImpl>();
			services.AddScoped<MemberPunishmentService, MemberPunishmentServiceImpl>();
			services.AddScoped<ServiceCategoryService, ServiceCategoryServiceImpl>();
            services.AddScoped<ServiceOfService, ServiceOfServiceImpl>();
            services.AddScoped<FurnitureCategoryService, FurnitureCategoryServiceImpl>();
            services.AddScoped<FurnitureService, FurnitureServiceImpl>();
            services.AddScoped<ToleService, ToleServiceImpl>();
            services.AddScoped<WoodBillDetailService, WoodBillDetailServiceImpl>();
            services.AddScoped<WoodBillMemberService, WoodBillMemberServiceImpl>();
            services.AddScoped<WoodBillMemberTransactionService, WoodBillMemberTransactionServiceImpl>();
            services.AddScoped<WoodBillService, WoodBillServiceImpl>();
            services.AddScoped<DayCloseService, DayCloseServiceImpl>();
        }

        private void registerOrganizationSetupServices(IServiceCollection services)
        {
            services.AddScoped<OrganizationSetupService, OrganizationSetupServiceImpl>();
        }

        private void registerUserRepos(IServiceCollection services)
        {
            services.AddScoped<AuthenticationRepository, AuthenticationRepositoryImpl>();
            services.AddScoped<DynamicMenuRepository, DynamicMenuRepositoryImpl>();
            services.AddScoped<LoginSessionRepository, LoginSessionRepositoryImpl>();
            services.AddScoped<ModuleRepository, ModuleRepositoryImpl>();
            services.AddScoped<RolePermissionMapRepository, RolePermissionMapRepositoryImpl>();
            services.AddScoped<UserRepository, UserRepositoryImpl>();
            services.AddScoped<UserRoleRepository, UserRoleRepositoryImpl>();
            services.AddScoped<RoleRepository, RoleRepositoryImpl>();
        }

        private void registerAccountRepos(IServiceCollection services)
        {
            services.AddScoped<LedgerGroupRepository, LedgerGroupRepositoryImpl>();
            services.AddScoped<LedgerRepository, LedgerRepositoryImpl>();
            services.AddScoped<LedgerBalanceRepository, LedgerBalanceRepositoryImpl>();
            services.AddScoped<TransactionRepository, TransactionRepositoryImpl>();
            services.AddScoped<TransactionDetailRepository, TransactionDetailRepositoryImpl>();
            services.AddScoped<PaymentRepository, PaymentRepositoryImpl>();
            services.AddScoped<ReceiptRepository, ReceiptRepositoryImpl>();
            services.AddScoped<AccountSettingsRepository, AccountSettingsRepositoryImpl>();
            services.AddScoped<LedgerSetupRepository, LedgerSetupRepositoryImpl>();
        }



        private void registerUserServices(IServiceCollection services)
        {
            services.AddScoped<AuthenticationService, AuthenticationServiceImpl>();
            services.AddScoped<LoginSessionService, LoginSessionServiceImpl>();
            services.AddScoped<ModuleService, ModuleServiceImpl>();
            services.AddScoped<DynamicMenuService, DynamicMenuServiceImpl>();
            services.AddScoped<RolePermissionMapService, RolePermissionMapServiceImpl>();
            services.AddScoped<UserRoleService, UserRoleServiceImpl>();
            services.AddScoped<RoleService, RoleServiceImpl>();
            services.AddScoped<UserService, UserServiceImpl>();
        }


        private void registerAccountServices(IServiceCollection services)
        {
            services.AddScoped<LedgerGroupService, LedgerGroupServiceImpl>();
            services.AddScoped<LedgerService, LedgerServiceImpl>();
            services.AddScoped<LedgerBalanceService, LedgerBalanceServiceImpl>();
            services.AddScoped<TransactionService, TransactionServiceImpl>();
            services.AddScoped<TransactionDetailService, TransactionDetailServiceImpl>();
            services.AddScoped<LedgerGroupIdProviderService, LedgerGroupIdProviderServiceImpl>();
            services.AddScoped<FiscalYearSetupService, FiscalYearSetupServiceImpl>();
            services.AddScoped<LE.Account.Service.Services.Interface.PaymentService, LE.Account.Service.Services.Implementations.PaymentServiceImpl>();
            services.AddScoped<ReceiptService, ReceiptServiceImpl>();
            services.AddScoped<JournalService, JournalServiceImpl>();
            services.AddScoped<ReportService, ReportServiceImpl>();
            services.AddScoped<LedgerSetupService, LedgerSetupServiceImpl>();
            services.AddScoped<IAccountingReportReporter, AccountingReportReporter>();
            services.AddScoped<IAccountingBaseQueryService, AccountingBaseQueryService>();
            services.AddScoped<FinancialYearServices, FinancialYearServices>();
        }

        private void registerSetup(IServiceCollection services)
        {
            registerAccountSetup(services);

        }

        private void registerAccountSetup(IServiceCollection services)
        {
            services.AddScoped<SettingSetup, SettingsSetupImpl>();
        }
    }

}
