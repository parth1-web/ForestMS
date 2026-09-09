using LE.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Setup.Models
{
    public class OrganizationSetupKeys
    {
        public static List<string> getKeys()
        {
            return new List<string>()
            {
                getOrganizationNameKey,
                getAddressKey,
                getEmailKey,
                getMobileKey,
                getPhoneKey,
                getPanNoKey,
                
            };
        }

      
        public static string getOrganizationNameKey { get; } = OrganizationSetup.Organization_Name.ToString();
        public static string getAddressKey { get; } = OrganizationSetup.Address.ToString();
        public static string getEmailKey { get; } = OrganizationSetup.Email.ToString();
        public static string getMobileKey { get; } = OrganizationSetup.Mobile_No.ToString();
        public static string getPhoneKey { get; } = OrganizationSetup.Phone_No.ToString();
        public static string getPanNoKey { get; } = OrganizationSetup.Pan_No.ToString();
        public static string getLogoKey { get; } = OrganizationSetup.Logo.ToString();
    }
}
