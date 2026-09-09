using LE.Billing.Factories.AbstractFactory.Interface;
using LE.Billing.Service.Services.Interface;
using Unity;

namespace LE.Billing.Factories.AbstractFactory.Implementations
{
    public class WoodBillServiceFactoryImpl : WoodBillServiceFactory
    {
        public WoodBillService getSalesService()
        {
            var unityContainer = UnityFactory.UnityFactory.getUnityContainer();

            return unityContainer.Resolve<WoodBillService>();
        }
    }
}
