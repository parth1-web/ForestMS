using LE.Billing.Service.Services.Interface;
using Unity;
using Unity.RegistrationByConvention;

namespace LE.Billing.Factories.UnityFactory
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
            registerAllTypes(container);
            _container = container;
        }
        
        private void registerAllTypes(IUnityContainer container)
        {
            var repositoryAssembly = typeof(MemberService).Assembly;

            container.RegisterTypes(
                     AllClasses.FromAssemblies(repositoryAssembly),
                     WithMappings.FromAllInterfaces,
                     WithName.Default,
                     WithLifetime.ContainerControlled,
                     overwriteExistingMappings: true);
        }
    }
}
