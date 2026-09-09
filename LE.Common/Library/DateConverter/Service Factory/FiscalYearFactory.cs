using LE.Common.Library.DateConverter;
using LE.Common.Library.DateConverter.Library;
using LE.Common.Library.DateConverter.Library.Interface;
using Unity;

namespace DateConverter.Core.Service_Factory
{
    public class FiscalYearFactory
    {
        public static iFiscalYearFunctions getFiscalYearService()
        {
            IUnityContainer container = UnityFactory.getUnityContainer();
            var dateConverter = container.Resolve<iDateConverter>();
            var nepaliDateData = container.Resolve<iNepaliDateData>();
            return new FiscalYearFunctions(dateConverter, nepaliDateData);
        }
    }
}
