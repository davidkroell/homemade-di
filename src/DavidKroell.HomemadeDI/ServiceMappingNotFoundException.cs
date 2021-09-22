using System;

namespace DavidKroell.HomemadeDI
{
    public class ServiceMappingNotFoundException : Exception
    {
        public ServiceMappingNotFoundException(Type type) : base($"Service mapping for {type.Name} not found")
        {
        }
    }
}
