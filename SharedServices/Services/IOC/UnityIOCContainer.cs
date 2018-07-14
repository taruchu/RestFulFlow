using Unity; 
using SharedServices.Interfaces.IOC;
using System;

namespace SharedServices.Services
{
    public class UnityIOC : IIOCContainer
    {
        private UnityContainer _container { get; set; }
        public UnityIOC()
        {
            _container = new UnityContainer();
        }
        public IIOCContainer Register<Type, ForClass>() where ForClass : Type
        {
            try
            {
                _container.RegisterType<Type, ForClass>();
                return this;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public IIOCContainer RegisterSingleton<Type, ForClass>() where ForClass : Type
        {
            try
            {
                _container.RegisterSingleton<Type, ForClass>();
                return this;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public Type Resolve<Type>()
        {
            try
            {
                return _container.Resolve<Type>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
