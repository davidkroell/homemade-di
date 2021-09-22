using System;

namespace DavidKroell.HomemadeDI
{
    public class ServiceMappingException : Exception
    {
        public ServiceMappingException(string message) : base(message)
        {
        }
    }
}
