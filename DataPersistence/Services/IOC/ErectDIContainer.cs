using SharedInterfaces.Interfaces.IOC;
using System;

namespace DataPersistence.Services.IOC
{
    public class ErectDIContainer
    {
        public IIOCContainer Container { get; private set; }
        public ErectDIContainer()
        {
            try
            {
                EstablishIOC establish = new EstablishIOC();
                Container = establish.EstablishContainer(new UnityDIFactory());
                establish.StandUp(Container);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
        
    }
}
