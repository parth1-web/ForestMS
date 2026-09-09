using LE.Common.Library.DateConverter.Library;
using LE.Common.Library.DateConverter.Library.Interface;
using Unity;

namespace LE.Common.Library.DateConverter
{
    public class UnityFactory
    {
        IUnityContainer container = new UnityContainer();
        private static IUnityContainer _container = null;
        public static IUnityContainer getUnityContainer()
        {
            if (_container == null)
            {
                UnityFactory unity = new UnityFactory();
                unity.createContainer();
            }
            return _container;
        }
        private void createContainer()
        {
            IUnityContainer container = new UnityContainer();
            registerServices(container);
            _container = container;
        }

        private void registerServices(IUnityContainer container)
        {
            container.RegisterType<iConversionStartDateData, ConversionStartDate5YearsInterval>();
            container.RegisterType<iDateConverter, LE.Common.Library.DateConverter.Library.DateConverter>();
            container.RegisterType<iDateFunctions, DateFunctions>();
            container.RegisterType<iFiscalYearFunctions, FiscalYearFunctions>();
            container.RegisterType<iNepaliDateData, NepaliDateData>();
        }

    }
}
