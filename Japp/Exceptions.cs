using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japp
{
    public class JappException : Exception 
    {
        public JappException() 
            : base("An unknown Japp exception has occurred.")
        {
        }

        public JappException(string message)
            : base(message)
        {
        }
    }

	/// <summary>
	/// Exception that is thrown when the address is misaligned
	/// or out of bounds
	/// </summary>
	public class AddressException : Exception
	{
		public AddressException()
			: base()
		{
		}
	}

	/// <summary>
	/// Exceptions that is thrown when the verification
	/// failes.
	/// </summary>
	public class VerifyException : Exception
	{
		public VerifyException()
			: base()
		{
		}
	}

    public class BufferOverflowException : JappException
    {
        public BufferOverflowException()
            : base("An internal buffer overflow has occurred.")
        {
        }
    }

	public class PENotFoundExeption : JappException
	{
		public PENotFoundExeption()
			: base()
		{
		}
	}

	public class InvalidOperationException : JappException
	{
		public InvalidOperationException()
			: base()
		{
		}
	}

    /// <summary>
    /// A generic exception for all errors not covered by other exceptions. 
    /// </summary>
    public class CommException : Exception
    {
        public CommException(Exception innerException)
            : base("PICProg communication error.", innerException)
        {
        }

        public CommException(string message)
            : base(message)
        {
        }

        public CommException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when a timeout occurs.
    /// </summary>
    public class CommTimeoutException : CommException
    {
        public CommTimeoutException()
            : base("A timeout has occurred.")
        {
        }
    }



}
