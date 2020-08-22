using System;

namespace Application.Core.Exceptions
{
	public class ApplicationUIException : ApplicationExceptionBase
	{
		public ApplicationUIException()
        {
        }

        public ApplicationUIException(string message)
            : base(message)
        {
        }

		public ApplicationUIException(string message, Exception inner)
            : base(message, inner)
        {
        }
	}
}
