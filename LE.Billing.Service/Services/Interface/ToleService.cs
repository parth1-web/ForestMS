using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface ToleService
    {
        void insert(ToleDto tole_dto);
        void update(ToleDto tole_dto);
        void delete(long tole_id);
    }
}
