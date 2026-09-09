using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Assemblers.Interface
{
    public interface PaymentAssembler
    {
        void copy(Payment payment, PaymentDto payment_dto);
    }
}
