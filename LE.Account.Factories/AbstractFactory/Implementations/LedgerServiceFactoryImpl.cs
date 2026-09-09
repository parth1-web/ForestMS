using LE.Account.Factories.AbstractFactory.Interface;
using LE.Account.Service.Services.Interface;
using Unity;

namespace LE.Account.Factories.AbstractFactory.Implementations
{
    public class LedgerServiceFactoryImpl : LedgerServiceFactory
    {
        public LedgerService getLedgerService()
        {
            var unityContainer = UnityFactory.UnityFactory.getUnityContainer();

            return unityContainer.Resolve<LedgerService>();
        }
    }
}
