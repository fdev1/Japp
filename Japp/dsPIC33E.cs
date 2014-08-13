using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Japp
{
	/// <summary>
	/// dsPIC33F/PIC24F programming base class.
	/// </summary>
	public abstract class dsPIC33EBase : dsPICBase
	{
		/// <summary>
		/// Reads a word from program memory.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		protected override byte[] ReadProgramWordICSP(uint address)
		{
			uint visi;
			byte[] pword = new byte[3];
			//
			// read program word and copy LSW to VISI register
			//
			this.ExecuteInstructions
			(
				//
				// exit reset vector
				//
				0x000000,										// nop
				0x000000,										// nop
				0x000000,										// nop
				0x040200,										// goto 0x200
				0x000000,										// nop
				0x000000,										// nop
				0x000000,										// nop
				//
				// initialize TBLPAG
				//
				0x200000 | ((address >> 16) << 4),				// mov devid<23:16>, w0
				0x8802A0,										// mov w0, TBLPAG
				0x200006 | ((address & 0x0000FFFF) << 4),		// mov DEVID<16:0>, w6
				//
				//
				//
				0xEB0380,										// clr w7
				0x000000,										// nop
				0xBA1B96,										// tblrdl [w6], [w7++]
				0x000000,										// nop
				0x000000,										// nop
				0x000000,										// nop
				0x000000,										// nop
				0x000000,										// nop
				0xBADBB6,										// tblrdh.b [w6++], [w7++]
				0x000000,										// nop
				0x000000,										// nop
				0x000000,										// nop
				0x000000,										// nop
				0x000000,										// nop
				0x887C40,										// mov w0, VISI
				0x000000										// nop
			);
			// 
			// read VISI register
			//
			visi = this.ReadVISI();
			//
			// copy the low word to buffer
			//
			pword[0] = (byte)visi;
			pword[1] = (byte)(visi >> 8);
			//
			// copy MSB to VISI
			//
			this.ExecuteInstructions
			(
				0x000000,					// nop
				0x887C41,					// mov w1, VISI
				0x000000,					// nop
				0x000000					// nop
			);
			//
			// read VISI register
			//
			visi = this.ReadVISI();
			//
			// copy the upper byte to the buffer
			//
			pword[2] = (byte)visi;
			//
			// return program word
			//
			return pword;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		protected override byte[] ReadProgramMemoryICSP(uint address, uint length)
		{
			int i = 0;
			byte[] pwords = new byte[length * 3];
			byte wordsBuffered = 0;
			const byte INTERNAL_PWORD_SIZE = 3;
			//
			// calculate how many words we can store in the internal buffer
			//
			byte wordsToBuffer = (byte)((this.internalBufferSize - (this.internalBufferSize % (4 * INTERNAL_PWORD_SIZE))) / INTERNAL_PWORD_SIZE);
			//Debug.Assert(wordsToBuffer == 8);
			//
			// while there's more than 4 words of memory left to read
			// we'll read them 1 word at a time.
			//
			while (length >= 4)
			{
				this.ExecuteInstructions
				(
					//
					// ;exit reset vector
					//
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x040200,									// goto 0x200
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					//
					// ;initialize pointers
					//
					0x200000 | ((address >> 16) << 4),			// mov address<23:16>, w0
					0X8802A0,									// mov w0, TBLPAG
					0x200006 | ((address & 0x0000FFFF) << 4),	// mov address<16:0>, w6
					0xEB0380,									// clr w7
					0x000000,									// nop
					//
					// ;ready program memory and pack it into registers w0:w6
					//
					0xBA1B96,									// tblrdl [w6], [w7++]
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0xBADBB6,									// tblrdh.b [w6++], [w7++]
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0xBADBD6,									// tblrdh.b [++w6], [w7++]
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0xBA1BB6,									// tblrdl [w6++], [w7++]
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0xBA1B96,									// tblrdl [w6], [w7++]
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0xBADBB6,									// tblrdh.b [w6++], [w7++]
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0xBADBD6,									// tblrdh.b [++w6], [w7++]
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0xBA0BB6,									// tblrdl [w6++], [w7]
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					//
					// ; copy 1st LSW to VISI
					//
					0x887C40,									// mov w0, VISI
					0x000000									// nop
				);
				
				//
				// read VISI
				//
				this.ReadVISIToBuffer();
				//
				// ;copy w1 to VISI
				//
				this.ExecuteInstructions
				(
					0x000000,					// nop
					0x887C41,					// mov w1, VISI
					0x000000,					// nop
					0x000000					// nop  ;padding
				);
				//
				// read visi
				//
				this.ReadVISIToBuffer();
				//
				// copy w2 to visi 
				//
				this.ExecuteInstructions
				(
					0x000000,				// nop
					0x887C42,				// mov w2, VISI
					0x000000,				// nop
					0x000000				// nop  ;padding
				);
				//
				// read VISI
				//
				this.ReadVISIToBuffer();
				//
				// copy w3 to VISI
				//
				this.ExecuteInstructions
				(
					0x000000,				// nop
					0x887C43,				// mov w3, VISI
					0x000000,				// nop
					0x000000				// nop  ;padding
				);
				//
				// read VISI
				//
				this.ReadVISIToBuffer();
				//
				// copy w4 to visi 
				//
				this.ExecuteInstructions
				(
					0x000000,				// nop
					0x887C44,				// mov w4, VISI
					0x000000,				// nop
					0x000000				// nop  ;padding
				);
				//
				// read visi
				//
				this.ReadVISIToBuffer();
				//
				// copy w5 to VISI
				//
				this.ExecuteInstructions
				(
					0x000000,				// nop
					0x887C45,				// mov w5, VISI
					0x000000,				// nop
					0x000000				// nop  ;padding
				);
				//
				// read VISI
				//
				this.ReadVISIToBuffer();
				//
				// increment the count of words buffered;
				//
				wordsBuffered += 4;
                length -= 4;
                address += (4 * 2);
                //
                // if we have filled
                //
				if (wordsBuffered == wordsToBuffer || length < 4)
				{
					//
					// send commmand to read buffer
					//
					CommResponse rsp = this.comm.SendCommand(JappCommand.READ_BUFFER);
					//Debug.Assert(rsp.Response == JappResponse.ACK && (rsp.Data.Length % 12 == 0));

                    if (rsp.Response != JappResponse.ACK)
                    {
                        if (rsp.Response == JappResponse.BUFFER_OVERFLOW)
                        {
                            //
                            // this should never happens, if it does it is a bug, either
                            // in this software or the firmware
                            //
                            throw new BufferOverflowException();
                        }
                        else
                        {
                            throw new JappException();
                        }
                    }

					//
					// copy instructions
					//
					for (uint a = 0; a < wordsBuffered / 4; a++)
					{
						//
						// read the instructions
						//
						pwords[(i * 4 * PWORD_SIZE) + 00] = rsp.Data[(a * 12) + 00];
						pwords[(i * 4 * PWORD_SIZE) + 01] = rsp.Data[(a * 12) + 01];
						pwords[(i * 4 * PWORD_SIZE) + 02] = rsp.Data[(a * 12) + 02];
						pwords[(i * 4 * PWORD_SIZE) + 05] = rsp.Data[(a * 12) + 03];
						pwords[(i * 4 * PWORD_SIZE) + 03] = rsp.Data[(a * 12) + 04];
						pwords[(i * 4 * PWORD_SIZE) + 04] = rsp.Data[(a * 12) + 05];
						pwords[(i * 4 * PWORD_SIZE) + 06] = rsp.Data[(a * 12) + 06];
						pwords[(i * 4 * PWORD_SIZE) + 07] = rsp.Data[(a * 12) + 07];
						pwords[(i * 4 * PWORD_SIZE) + 08] = rsp.Data[(a * 12) + 08];
						pwords[(i * 4 * PWORD_SIZE) + 11] = rsp.Data[(a * 12) + 09];
						pwords[(i * 4 * PWORD_SIZE) + 09] = rsp.Data[(a * 12) + 10];
						pwords[(i * 4 * PWORD_SIZE) + 10] = rsp.Data[(a * 12) + 11];

						i++;
					}
					//
					//
					//
					wordsBuffered = 0;
				}
				//
				// increase the address by 4 words and the "round counter" by 1
				//
				//address += (4 * 2);
				//length -= 4;
				//i++;
			}
			//
			// convert i to a byte index;
			//
			i = i * 4 * PWORD_SIZE;
			//
			// if we still got any words left to read we'll read them one by one
			//
			while (length > 0)
			{
				//
				// read the next word
				//
				byte[] pword = this.ReadProgramWord(address);
				//
				// copy it's bytes
				//
				pwords[i++] = pword[0];
				pwords[i++] = pword[1];
				pwords[i++] = pword[2];
				//
				// decrement length and increment address
				//
				length--;
				address++;
			}
			//
			// return program memory buffer
			//
			return pwords;
		}

		/// <summary>
		/// Tests the communication link by writing a value to
		/// the VISI register and reading it back.
		/// </summary>
		private void TestLink()
		{
			//
			// goto 0x200                   // 040200
			// goto 0x200                   // 040200
			// nop                          // 000000
			// mov #0xAAAA, w0              // 2AAAA0
			// mov w0, VISI                 // 883c20
			// nop
			// nop
			// nop
			//
			this.ExecuteInstructions
			(
				0x040200, 
				0x040200,
				0x000000, 
				0x2AAAA0,
				0x883c20, 
				0x000000,
				0x000000, 
				0x000000
			);

			if (this.ReadVISI() != 0xAAAA)
			{
				throw new Exception();
			}
		}
	}

	/// <summary>
	/// Class for programming dsPIC33E/PIC24E devices with volatile
	/// configuration bits.
	/// </summary>
	public class dsPIC33EVolatile : dsPIC33EBase
	{

		public override string ExecutiveFile
		{
			get
			{
				return "RIPE_10a";
			}
		}

		/// <summary>
		/// Initialize object.
		/// </summary>
		public dsPIC33EVolatile() : base()
		{
			this.ROW_SIZE = 2;
			this.APPID_VALUE = 0xDE;
		}

		/// <summary>
		/// Bulk erase program memory.
		/// </summary>
		public override void EraseProgramMemory()
		{
			//
			// bulk erase the device
			//
			this.ExecuteInstructions
			(
				//
				// exit program reset vector
				//
				0x000000,			// nop
				0x000000,			// nop
				0x000000,			// nop
				0x040200,			// goto 0x200
				0x000000,			// nop
				0x000000,			// nop
				0x000000,			// nop
				//
				// set NVMCON to erase program memory
				//
				0x2400DA,			// mov #0x400D w10
				0x88394A,			// mov w10, NVMCON
				0x000000,			// nop
				0x000000,			// nop
				//
				// initialize erase cycle
				//
				0x200551,			// mov #0x55, w1
				0x883971,			// mov w1, NVMKEY
				0x200AA1,			// mov #0xAA, w1
				0x883971,			// mov w1, NVMKEY
				0xA8E729,			// bset NVMCON, #WR
				0x000000,			// nop
				0x000000,			// nop
				0x000000			// nop
			);
			//
			// wait 116ms (P11) for bulk erase to complete
			//
			Thread.Sleep(200);
		}

		/// <summary>
		/// Bulk erases program and executive memory.
		/// </summary>
		public override void EraseProgramAndExecutiveMemory()
		{
			//
			// bulk erase the device
			//
			this.ExecuteInstructions
			(
				//
				// exit program reset vector
				//
				0x000000,			// nop
				0x000000,			// nop
				0x000000,			// nop
				0x040200,			// goto 0x200
				0x000000,			// nop
				0x000000,			// nop
				0x000000,			// nop
				//
				// set NVMCON to erase program memory
				//
				0x2400FA,			// mov #0x400F, w10
				0x88394A,			// mov w10, NVMCON
				0x000000,			// nop
				0x000000,			// nop
				//
				// initialize erase cycle
				//
				0x200551,			// mov #0x55, w1
				0x883971,			// mov w1, NVMKEY
				0x200AA1,			// mov #0xAA, w1
				0x883971,			// mov w1, NVMKEY
				0xA8E729,			// bset NVMCON, #WR
				0x000000,			// nop
				0x000000,			// nop
				0x000000			// nop
			);
			//
			// wait 116ms (P11) for bulk erase to complete
			//
			Thread.Sleep(200);
		}

		/// <summary>
		/// Writes a row of program memory to devices with volatile memory.
		/// </summary>
		/// <param name="address">The address of the row.</param>
		/// <param name="pwords">The buffer containing the program words to write.</param>
		public override void WriteProgramRow(uint address, byte[] pwords)
		{
			ushort visi;
			uint msb1_0;
			uint lsw000, lsw001;
			//
			// make sure the buffer size is divisible by 3 (program instructions
			// are 3 bytes on this device
			//
			if (pwords.Length != 2 * 3)
				throw new Exception("Invalid buffer size.");
			//
			// prepare to program 2 program words
			//
			this.ExecuteInstructions
			(
				//
				// ;exit reset vector
				// 
				0x000000,									// nop
				0x000000,									// nop
				0x000000,									// nop
				0x040200,									// goto 0x200
				0x000000,									// nop
				0x000000,									// nop
				0x000000,									// nop
				//
				// initialize write pointer and TBLPAG for writing to latches
				//
				0x200FAC,									// mov #0xFA, w12
				0x8802AC									// mov w12, TBLPAG
			);
			//
			// lets pack the instructions ahead of time for more readability
			//
			lsw000 = (ushort)(pwords[0] | pwords[1] << 8);
			lsw001 = (ushort)(pwords[3] | pwords[4] << 8);
			msb1_0 = (ushort)(pwords[2] | pwords[5] << 8);
			//
			// execute ICSP algorithm to latch 4 progrogram words
			//
			this.ExecuteInstructions
			(
				//
				// ;load next 4 instructions into regers w0:w4
				//
				0x200000 | (lsw000 << 4),		// mov #<LSW0>, w0              // 2xxxx0
				0x200001 | (msb1_0 << 4),		// mov #<MSB1:MSB0>, w1         // 2xxxx1
				0x200002 | (lsw001 << 4),		// mov #<LSW1>, w2              // 2xxxx2
				//
				// ;set read pointers (w6 and w7)
				//
				0xEB0300,						// clr w6                       // EB0300
				0x000000,						// nop                          // 000000
				0xEB0380,						// clr w7
				0x000000,						// nop
				//
				// load write latches
				//
				0xBB0BB6,						// tblwtl [w6++], [w7]          // BB0BB6
				0x000000,						// nop                          // 000000
				0x000000,						// nop                          // 000000
				0xBBDBB6,						// tblwth.b [w6++], [w7++]		// BBDBB6
				0x000000,						// nop							// 000000
				0x000000,						// nop							// 000000
				0xBBEBB6,						// tblwth.b [w6++], [++w7]		// BBEBB6
				0x000000,						// nop							// 000000
				0x000000,						// nop							// 000000
				0xBB1BB6,						// tblwtl [w6++], [w7++]		// BB1BB6
				0x000000,						// nop							// 000000
				0x000000,						// nop							// 000000
				//
				// set NVMADDRU/NVMADDR to point to the correct row
				//
				0x200003 | ((address & 0x0000FFFF) << 4),	// mov DEST<15:0>, w3
				0x200004 | ((address >> 16) << 4),			// mov DEST<23:16>, w4
				0x883953,									// mov w3, NVMADDR
				0x883964,									// MOV W4, NVMADDRU
				//
				// set NVMCON to program 128 words
				//
				0x24001A,									// mov #0x4001, w10
				0x000000,									// nop
				0x88394A,									// mov w10, NVMCON
				0x000000,									// nop
				0x000000,									// nop
				//
				// initialize write cycle. 
				//
				0x200551,									// mov #0x55, w1
				0x883971,									// mov w1, NVMKEY
				0x200AA1,									// mov #0xAA, w1
				0x883971,									// mov w1, NVMKEY
				0xA8E729,									// bset NVMCON, #WR
				0x000000,									// nop
				0x000000,									// nop
				0x000000,									// nop
				0x000000,									// nop
				0x000000									// nop 
			);
			//
			// wait 1.28ms (P13) for row programming
			// 
			Thread.Sleep(2);
			//
			// now wait for NVMCON<WR> to clear
			//
			do
			{
				//
				// copy NVMCON to VISI
				//
				this.ExecuteInstructions
				(
					0x000000,			// nop
					0x803940,			// mov NVMCON, w0
					0x887C40,			// mov w0, VISI
					0x000000			// nop
				);
				//
				// read VISI
				//
				visi = this.ReadVISI();
				//
				// exit reset vector
				//
				this.ExecuteInstructions
				(
					0x000000,			// nop							// 000000
					0x000000,			// nop							// 000000
					0x000000,			// nop							// 000000
					0x040200,			// goto 0x200					// 040200
					0x000000,			// nop							// 000000
					0x000000,			// nop							// 000000
					0x000000			// nop							// 000000
				);
			}
			//
			// wait until NVMCON<WR> clears (programming completes)
			//
			while ((visi & 0x8000) != 0x0000);
		}

		/// <summary>
		/// Class for programming dsPIC33/PIC24 devices with non-volatile
		/// configuration bits.
		/// </summary>
		public class dsPIC33ENonVolatile : dsPIC33EBase
		{
			public dsPIC33ENonVolatile() : base()
			{
				this.ROW_SIZE = 128;
			}

			/// <summary>
			/// Bulk erase program memory.
			/// </summary>
			public override void EraseProgramMemory()
			{
				//
				// bulk erase the device
				//
				this.ExecuteInstructions
				(
					//
					// exit program reset vector
					//
					0x000000,			// nop
					0x000000,			// nop
					0x000000,			// nop
					0x040200,			// goto 0x200
					0x000000,			// nop
					0x000000,			// nop
					0x000000,			// nop
					//
					// set NVMCON to erase program memory
					//
					0x2400FA,			// mov #0x400F w10   <-- is it 400F or 400E ???
					0x88394A,			// mov w10, NVMCON
					0x000000,			// nop
					0x000000,			// nop
					//
					// initialize erase cycle
					//
					0x200551,			// mov #0x55, w1
					0x883971,			// mov w1, NVMKEY
					0x200AA1,			// mov #0xAA, w1
					0x883971,			// mov w1, NVMKEY
					0xA8E729,			// bset NVMCON, #WR
					0x000000,			// nop
					0x000000,			// nop
					0x000000			// nop
				);
				//
				// wait 116ms (P11) for bulk erase to complete
				//
				Thread.Sleep(200);
			}

			/// <summary>
			/// Write a row of program memory.
			/// </summary>
			/// <param name="address">The address of the row.</param>
			/// <param name="pwords">The program words to write.</param>
			public override void WriteProgramRow(uint address, byte[] pwords)
			{
				ushort visi;
				//
				// make sure the buffer size is divisible by 3 (program instructions
				// are 3 bytes on this device
				//
				if (pwords.Length != 128 * 3)
					throw new Exception("Invalid buffer size.");
				//
				// prepare to program 128 program words
				//
				this.ExecuteInstructions
				(
					//
					// ;exit reset vector
					// 
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					0x040200,									// goto 0x200
					0x000000,									// nop
					0x000000,									// nop
					0x000000,									// nop
					//
					// initialize write pointer and TBLPAG for writing to latches
					//
					0x200FAC,									// mov #0xFA, w12
					0x8802AC,									// mov w12, TBLPAG
					0x200007									// mov #0, w7
				);
				//
				// repeat 32 times to write 128 words
				//
				for (byte i = 0; i < 32; i++)
				{
					uint msb1_0, msb3_2;
					uint lsw000, lsw001, lsw002, lsw003;
					ushort offset = (ushort)((ushort)i * 3 * 4);
					//
					// lets pack the instructions ahead of time for more readability
					//
					lsw000 = (ushort)(pwords[offset + 0] | pwords[offset + 01] << 8);
					lsw001 = (ushort)(pwords[offset + 3] | pwords[offset + 04] << 8);
					lsw002 = (ushort)(pwords[offset + 6] | pwords[offset + 07] << 8);
					lsw003 = (ushort)(pwords[offset + 9] | pwords[offset + 10] << 8);
					msb1_0 = (ushort)(pwords[offset + 2] | pwords[offset + 05] << 8);
					msb3_2 = (ushort)(pwords[offset + 8] | pwords[offset + 11] << 8);
					//
					// execute ICSP algorithm to latch 4 progrogram words
					//
					this.ExecuteInstructions
					(
						//
						// ;load next 4 instructions into regers w0:w4
						//
						0x200000 | (lsw000 << 4),		// mov #<LSW0>, w0              // 2xxxx0
						0x200001 | (msb1_0 << 4),		// mov #<MSB1:MSB0>, w1         // 2xxxx1
						0x200002 | (lsw001 << 4),		// mov #<LSW1>, w2              // 2xxxx2
						0x200003 | (lsw002 << 4),		// mov #<LSW2>, w3              // 2xxxx3\\
						0x200004 | (msb3_2 << 4),		// mov #<MSB3:MSB2>, w4         // 2xxxx4
						0x200005 | (lsw003 << 4),		// mov #<LSW3>, w5              // 2xxxx5
						//
						// ;set read pointer (w6) and load write latches
						//
						0xEB0300,						// clr w6                       // EB0300
						0x000000,						// nop                          // 000000
						0xBB0BB6,						// tblwtl [w6++], [w7]          // BB0BB6
						0x000000,						// nop                          // 000000
						0x000000,						// nop                          // 000000
						0xBBDBB6,						// tblwth.b [w6++], [w7++]		// BBDBB6
						0x000000,						// nop							// 000000
						0x000000,						// nop							// 000000
						0xBBEBB6,						// tblwth.b [w6++], [++w7]		// BBEBB6
						0x000000,						// nop							// 000000
						0x000000,						// nop							// 000000
						0xBB1BB6,						// tblwtl [w6++], [w7++]		// BB1BB6
						0x000000,						// nop							// 000000
						0x000000,						// nop							// 000000
						0xBB0BB6,						// tblwtl [w6++], [w7]			// BB0BB6 
						0x000000,						// nop							// 000000
						0x000000,						// nop							// 000000
						0xBBDBB6,						// tblwth.b [w6++], [w7++]		// BBDBB6
						0x000000,						// nop							// 000000
						0x000000,						// nop							// 000000
						0xBBEBB6,						// tblwth.b [w6++], [++w7]		// BBEBB6
						0x000000,						// nop							// 000000
						0x000000,						// nop							// 000000
						0xBB1BB6,						// tblwtl [w6++], [w7++]		// BB1BB6
						0x000000,						// nop							// 000000 
						0x000000						// nop							// 000000
					);
				}
				//
				// set NVMCON<WR> to initialize row programming
				//
				this.ExecuteInstructions
				(
					//
					// set NVMADDRU/NVMADDR to point to the correct row
					//
					0x200002 | ((address & 0x0000FFFF) << 4),	// mov DEST<15:0>, w2
					0x200003 | ((address >> 16) << 4),			// mov DEST<23:16>, w3
					0x883963,									// mov w3, NVMADDRU
					0x883952,									// MOV W2, NVMADDR
					//
					// set NVMCON to program 128 words
					//
					0x24002A,									// mov #0x4002, w10
					0x88394A,									// mov w10, NVMCON
					0x000000,									// nop
					0x000000,									// nop
					//
					// initialize write cycle. 
					//
					0x200551,									// mov #0x55, w1
					0x883971,									// mov w1, NVMKEY
					0x200AA1,									// mov #0xAA, w1
					0x883971/*,									// mov w1, NVMKEY
				0xA8E729,									// bset NVMCON, #WR
				0x000000,									// nop
				0x000000,									// nop
				0x000000,									// nop
				0x000000,									// nop
				0x000000									// nop */
				);
				// 10101000bbbffffffffffffb
				//            011100101000 
				// 101010001110111001010001
				// 101010001110011100101001
				//
				// the last 5 NOPs above need to be clocked in at more than 2 MHz,
				// to make sure this happens we'll cycle the clock line instead. This is the
				// same as sending 5 nops
				//
				//this.comm.SendCommand(PP_COMMAND.PP_CMD_SEND_PGD_CYCLES, 0x01, 28 * 5);
				if (this.comm.SendCommand(JappCommand.PP_CMD_SEND_5_NOPS).Response != JappResponse.ACK)
					Debug.Assert(false);
				//
				// wait 1.28ms (P13) for row programming
				// 
				Thread.Sleep(2);
				//
				// now wait for NVMCON<WR> to clear
				//
				do
				{
					//
					// copy NVMCON to VISI
					//
					this.ExecuteInstructions
					(
						0x000000,			// nop
						0x803940,			// mov NVMCON, w0
						0x000000,			// nop
						0x887C40,			// mov w0, VISI
						0x000000			// nop
					);
					//
					// read VISI
					//
					visi = this.ReadVISI();
					//
					// exit reset vector
					//
					this.ExecuteInstructions
					(
						0x000000,			// nop							// 000000
						0x000000,			// nop							// 000000
						0x000000,			// nop							// 000000
						0x040200,			// goto 0x200					// 040200
						0x000000,			// nop							// 000000
						0x000000,			// nop							// 000000
						0x000000			// nop							// 000000
					);
				}
				//
				// wait until NVMCON<WR> clears (programming completes)
				//
				while ((visi & 0x8000) != 0x0000);
			}
		}
	}
}
