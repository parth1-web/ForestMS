using LE.Account.Factories.AbstractFactory.Interface;
using LE.Account.Service.Services.Interface;
using Unity;

namespace LE.Account.Factories.AbstractFactory.Implementations
{
    public class ReceiptServiceFactoryImpl : ReceiptServiceFactory
    {
        public ReceiptService getReceiptService()
        {
            var unityContainer = UnityFactory.UnityFactory.getUnityContainer();

            return unityContainer.Resolve<ReceiptService>();
        }
    }
}
