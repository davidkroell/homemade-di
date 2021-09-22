using System;

namespace DavidKroell.HomemadeDI
{
    public class ServiceCreationException : Exception
    {
        public ServiceCreationException(Type implType, Exception? exception = null) : base(
            $"Service of type {implType.FullName} can't be constructed", exception)
        {
        }
    }
}
