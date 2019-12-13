using System;

namespace zsq.UserApi.Infrastructure
{
    public class UserException : Exception
    {
        public UserException()
        {

        }

        public UserException(string message) : base(message)
        {

        }

        public UserException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
