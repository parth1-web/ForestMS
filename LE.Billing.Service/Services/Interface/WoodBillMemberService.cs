using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface WoodBillMemberService
    {
        void insert(List<WoodBillMemberDto> wood_bill_member_dto);
    }
}
