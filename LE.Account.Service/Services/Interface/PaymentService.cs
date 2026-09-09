using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Services.Interface
{
    public interface PaymentService
    {
        void doPayment(PaymentDto paymentDto);
        void cancel(long payment_id, long user_id);

    }
}
