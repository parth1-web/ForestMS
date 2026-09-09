using LE.Common.Library.DateConverter;
using LE.Common.Library.DateConverter.Library;
using LE.Common.Library.DateConverter.Library.Interface;
using Unity;

namespace DateConverter.Core.Service_Factory
{
    public class DateFunctionsFactory
    {
        public static iDateFunctions getDateFunctionsService()
        {
            IUnityContainer container = UnityFactory.getUnityContainer();
            var nepaliDateData = container.Resolve<iNepaliDateData>();
            return new DateFunctions(nepaliDateData);
        }
    }
}
