using LE.Entities.OrganizationSetup;
using System.Collections.Generic;

namespace LE.Service.Services.Interface
{
    public interface OrganizationSetupService
    {
        void saveOrUpdate(string key, string value);
        void saveOrUpdate(List<OrganizationSetup> keyValue);

    }

}
