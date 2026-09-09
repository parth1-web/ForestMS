using Autofac;
using LE.Account.Infrastructure.Reports.Reporter;
using LE.Common.Provider;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Accounting.Controllers;
using LE.Web.Areas.Administration.Controllers;
using LE.Web.Areas.Billing.Controllers;
using LE.Web.Areas.Inventory.Controllers;
using LE.Web.Controllers;

namespace LE.Web.Autofac.Modules
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BaseController>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<HomeController>().PropertiesAutowired();
            builder.RegisterType<AccountingReportReporter>().PropertiesAutowired();
            builder.RegisterType<UserController>().PropertiesAutowired();
            builder.RegisterType<MembershipController>().PropertiesAutowired();
            builder.RegisterType<DayCloseController>().PropertiesAutowired();
            builder.RegisterType<LedgerController>().PropertiesAutowired();
            builder.RegisterType<LE.Web.Areas.Accounting.Controllers.ReportController>().PropertiesAutowired();
            builder.RegisterType<WoodDetailsController>().PropertiesAutowired();
            builder.RegisterType<WoodBillingController>().PropertiesAutowired();
            builder.RegisterType<ChiranBillingController>().PropertiesAutowired();
            builder.RegisterType<FurnitureBillingController>().PropertiesAutowired();
            builder.RegisterType<CounterBillingReportController>().PropertiesAutowired();
            builder.RegisterType<FirewoodBillingController>().PropertiesAutowired();
            builder.RegisterType<PaymentController>().PropertiesAutowired();
            builder.RegisterType<ReceiptController>().PropertiesAutowired();
            builder.RegisterType<LE.Web.Areas.Inventory.Controllers.ReportController>().PropertiesAutowired();

            builder.RegisterType<DbConnectionProvider>().As<IConnectionProvider>();

            builder.Register(c => new BaseController
            {
                _authenticationRepo = c.Resolve<AuthenticationRepository>(),
                _userRepo = c.Resolve<UserRepository>(),
            }).PropertiesAutowired();
        }
    }
}
