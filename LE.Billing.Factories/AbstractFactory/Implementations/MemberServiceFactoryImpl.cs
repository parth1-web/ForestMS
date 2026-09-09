using LE.Billing.Factories.AbstractFactory.Interface;
using LE.Billing.Service.Services.Interface;
using Unity;

namespace LE.Billing.Factories.AbstractFactory.Implementations
{
    public class MemberServiceFactoryImpl : MemberServiceFactory
    {
        public MemberService getCustomerService()
        {
            var unityContainer = UnityFactory.UnityFactory.getUnityContainer();

            return unityContainer.Resolve<MemberService>();
        }
    }
}
