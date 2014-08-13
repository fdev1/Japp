using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;

namespace Japp
{
	public class HEXFile
	{

		private string filename;

		public HEXFile(string filename)
		{
			this.filename = filename;
		}

		public void ReadFile(byte[] pmem, byte[] cmem, uint cmemOffset)
		{
			ushort len;
			uint address;
			byte type;
			string data;
			byte checksum;
			uint extended_address = 0;

			using (StreamReader file = new StreamReader(this.filename))
			{
				while (file.EndOfStream == false)
				{

					string record;
					//
					// read the next line
					//
					record = file.ReadLine();
					//
					// make sure the record starts with a colon
					//
					if (record.Substring(0, 1) != ":")
						throw new Exception("Invalid hex file.");
					//
					// get the length of the record
					//
					len = ushort.Parse(record.Substring(1, 2), NumberStyles.HexNumber);
					//
					// get the address of the record
					//
					address = uint.Parse(record.Substring(3, 4), NumberStyles.HexNumber);
					//
					// get the type of the record
					//
					type = byte.Parse(record.Substring(7, 2), NumberStyles.HexNumber);
					//
					// get the data on the record
					//
					if (len > 0)
					{
						data = record.Substring(9, len * 2);
					}
					else
					{
						data = string.Empty;
					}
					//
					// get the checksum
					//
					checksum = byte.Parse(record.Substring(9 + (len * 2), 2), NumberStyles.HexNumber);

					switch (type)
					{
						case 00:	// data record
							uint pAddress;
							byte[] dataBytes = new byte[data.Length / 2];
							//
							// parse the data section into byte array
							//
							for (int i = 0; i < dataBytes.Length; i++)
							{
								dataBytes[i] = byte.Parse(data.Substring(i * 2, 2), NumberStyles.HexNumber);
							}
							//
							// calculate the actual address
							//
							pAddress = address | (extended_address << 16);

							if (pAddress >= cmemOffset)
							{
								//
								// adjust the address relative to the start of
								// connfiguration memory
								//
								pAddress = pAddress / 4;
								pAddress -= cmemOffset / 2;
								//pAddress *= 3;
								//
								// load data into program memory array
								//
								for (int i = 0; i < dataBytes.Length / 4; i++)
								{
									cmem[(pAddress * 3) + 2] = dataBytes[(i * 4) + 2];
									cmem[(pAddress * 3) + 1] = dataBytes[(i * 4) + 1];
									cmem[(pAddress * 3) + 0] = dataBytes[(i * 4) + 0];
									pAddress++;
								}
							}
							else
							{
								pAddress = pAddress / 4;
								//pAddress *= 3;
								//
								// load data into program memory array
								//
								for (int i = 0; i < dataBytes.Length / 4; i++)
								{
									pmem[(pAddress * 3) + 2] = dataBytes[(i * 4) + 2];
									pmem[(pAddress * 3) + 1] = dataBytes[(i * 4) + 1];
									pmem[(pAddress * 3) + 0] = dataBytes[(i * 4) + 0];
									pAddress++;

								}
							}
							break;

						case 04:	// extended-address
							extended_address = ushort.Parse(data, NumberStyles.HexNumber);
							break;

						case 01:	// end-of-file
							return;

						default:
							throw new Exception("Invalid record type.");
					}
				}
			}
		}
	}
}
