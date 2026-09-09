using LE.Common.Library.DateConverter;
using LE.Common.Library.DateConverter.Library.Interface;
using Unity;

namespace DateConverter.Core.Service_Factory
{
    public class DateConverterFactory
    {
        public static iDateConverter getDateConverterService()
        {
            IUnityContainer container = UnityFactory.getUnityContainer();
            var dateFunctions = container.Resolve<iDateFunctions>();
            var startDates = container.Resolve<iConversionStartDateData>();
            var nepaliDateData = container.Resolve<iNepaliDateData>();
            return new LE.Common.Library.DateConverter.Library.DateConverter(startDates, dateFunctions, nepaliDateData);
        }
    }
}
