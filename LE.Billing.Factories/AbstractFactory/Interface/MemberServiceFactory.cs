using LE.Billing.Service.Services.Interface;

namespace LE.Billing.Factories.AbstractFactory.Interface
{
    public interface MemberServiceFactory
    {
        MemberService getCustomerService();
    }
}
