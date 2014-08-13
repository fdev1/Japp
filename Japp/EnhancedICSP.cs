using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japp
{
	/// <summary>
	/// Enhanced ICSP commands
	/// </summary>
	internal enum EnhacedICSPCommand
	{
		SCHECK			= 0x00,
		READC			= 0x01,
		READP			= 0x02,
		PROG2W			= 0x03,
		PROGP			= 0X04,
		ERASEP			= 0x09,
		QVER			= 0x0B,
		CRCP			= 0x0C,
		QBLANK			= 0x0E
	}

	static class EnhancedICSP
	{

		public static byte[] PackCommand(EnhacedICSPCommand command, byte length)
		{
			uint cmd = (((uint)command) << 12) | (uint)length;
			return new byte[] { (byte)(cmd >> 8), (byte)cmd };

		}

	}

}
