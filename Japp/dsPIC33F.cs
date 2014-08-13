using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Japp
{
	/// <summary>
	/// dsPIC33F/PIC24F programming class.
	/// </summary>
	public class dsPIC33F : dsPICBase
	{

		public dsPIC33F() : base()
		{
			this.ROW_SIZE = 64;
		}

		/// <summary>
		/// Reads a program word using ICSP
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		protected override byte[] ReadProgramWordICSP(uint address)
		{
			UInt16 visi;
			byte[] pword = new byte[3];
			//
			// read program word and copy LSW to VISI register
			//
			this.ExecuteInstructions 
			(
				0x040200,										// goto 0x200                   // 040200
				0x040200,										// goto 0x200                   // 040200
				0x000000,										// nop                          // 000000
				0x200000 | ((address >> 16) << 4),				// mov devid<23:16>, w0         // 200xx0
				0x880190,										// mov w0, TBLPAG               // 880190 
				0x200006 | ((address & 0x0000FFFF) << 4),		// mov DEVID<16:0>, w6          // 2xxxx6
				0xEB0380,										// clr w7                       // EB0380
				0x000000,										// nop                          // 000000
				0xBA1B96,										// tblrdl [w6], [w7++]          // BA1B96
				0x000000,										// nop                          // 000000
				0x000000,										// nop                          // 000000
				0xBADBB6,										// tblrdh.b [w6++], [w7++]      // BADBB6
				0x000000,										// nop                          // 000000
				0x000000,										// nop                          // 000000
				0x000000,										// nop
				0x883c20,										// mov w0, VISI                 // 883c20
				0x000000,										// nop
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
				0x000000,					// nop                          // 000000
				0x883C21,					// mov w1, VISI                 // 883C21
				0x000000,					// nop                          // 000000
				0x000000					// nop                          // 000000
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

		protected override byte[] ReadProgramMemoryICSP(uint address, uint length)
		{
			int i = 0;
			byte[] pwords = new byte[length * 3];
			//
			// while there's more than 4 words of memory left to read
			// we'll read them 1 word at a time.
			//
			while (length > 4)
			{
				//
				// ; copy 1st LSW to VISI
				//
				// mov w0, VISI                 // 883C20
				// nop                          // 000000
				//
				this.ExecuteInstructions 
				(
					//
					// ;exit reset vector
					//
					0x040200,									// goto 0x200                   // 040200
					0x040200,									// goto 0x200                   // 040200
					0x000000,									// nop                          // 000000
					//
					// ;initialize pointers
					//
					0x200000 | ((address >> 16) << 4),			// mov devid<23:16>, w0         // 200xx0
					0x880190,									// mov w0, TBLPAG               // 880190
					0x200006 | ((address & 0x0000FFFF) << 4),	// mov DEVID<16:0>, w6          // 2xxxx6
					0xEB0380,									// clr w7                       // EB0380
					0x000000,									// nop                          // 000000
					//
					// ;ready program memory and pack it into registers w0:w6
					//
					0xBA1B96,									// tblrdl [w6], [w7++]          // BA1B96
					0x000000,									// nop                          // 000000
					0x000000,									// nop                          // 000000
					0xBADBB6,									// tblrdh.b [w6++], [w7++]      // BADBB6
					0x000000,									// nop                          // 000000
					0x000000,									// nop                          // 000000
					0xBADBD6,									// tblrdh.b [++w6], [w7++]      // BADBD6
					0x000000,									// nop                          // 000000
					0x000000,									// nop                          // 000000
					0xBA1BB6,									// tblrdl [w6++], [w7++]        // BA1BB6
					0x000000,									// nop                          // 000000
					0x000000,									// nop                          // 000000
					0xBA1B96,									// tblrdl [w6], [w7++]          // BA1B96
					0x000000,									// nop                          // 000000
					0x000000,									// nop                          // 000000
					0xBADBB6,									// tblrdh.b [w6++], [w7++]      // BADBB6
					0x000000,									// nop                          // 000000
					0x000000,									// nop                          // 000000
					0xBADBD6,									// tblrdh.b [++w6], [w7++]      // BADBD6 
					0x000000,									// nop                          // 000000
					0x000000,									// nop                          // 000000
					0xBA0BB6,									// tblrdl [w6++], [w7]          // BA0BB6
					0x000000,									// nop                          // 000000
					0x000000,									// nop                          // 000000
					//
					// ; copy 1st LSW to VISI
					//
					0x883C20,									// mov w0, VISI                 // 883C20
					0x000000									// nop                          // 000000
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
					0x000000,					// nop                              // 000000
					0x883C21,					// mov w1, VISI                     // 883C21
					0x000000,					// nop                              // 000000
					0x000000					// nop  ;padding                    // 000000
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
					0x000000,				// nop                              // 000000
					0x883C22,				// mov w2, VISI                     // 883C22
					0x000000,				// nop                              // 000000
					0x000000				// nop  ;padding                    // 000000
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
					0x000000,				// nop                              // 000000
					0x883C23,				// mov w3, VISI                     // 883C23
					0x000000,				// nop                              // 000000
					0x000000				// nop  ;padding                    // 000000
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
					0x000000,				// nop                              // 000000
					0x883C24,				// mov w4, VISI                     // 883C24
					0x000000,				// nop                              // 000000
					0x000000				// nop  ;padding                    // 000000	
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
					0x000000,				// nop                              // 000000
					0x883C25,				// mov w5, VISI                     // 883C25
					0x000000,				// nop                              // 000000
					0x000000				// nop  ;padding                    // 000000
				);
				//
				// read VISI
				//
				this.ReadVISIToBuffer();
				//
				// send commmand to read buffer
				//
				CommResponse rsp = this.comm.SendCommand(JappCommand.READ_BUFFER);
				Debug.Assert(rsp.Response == JappResponse.ACK && rsp.Data.Length == 12);
				//
				// read the instructions
				//
				pwords[(i * 4 * 3) + 00] = rsp.Data[00];
				pwords[(i * 4 * 3) + 01] = rsp.Data[01];
				pwords[(i * 4 * 3) + 02] = rsp.Data[02];
				pwords[(i * 4 * 3) + 05] = rsp.Data[03];
				pwords[(i * 4 * 3) + 03] = rsp.Data[04];
				pwords[(i * 4 * 3) + 04] = rsp.Data[05];
				pwords[(i * 4 * 3) + 06] = rsp.Data[06];
				pwords[(i * 4 * 3) + 07] = rsp.Data[07];
				pwords[(i * 4 * 3) + 08] = rsp.Data[08];
				pwords[(i * 4 * 3) + 11] = rsp.Data[09];
				pwords[(i * 4 * 3) + 09] = rsp.Data[10];
				pwords[(i * 4 * 3) + 10] = rsp.Data[11];
				
				//
				// increase the address by 4 words and the "round counter" by 1
				//
				address += (4 * 2);
				length -= 4;
				i++;
			}
			//
			// convert i to a byte index;
			//
			i = i * 4 * 3;
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
		/// Erases program meory.
		/// </summary>
		public override void EraseProgramMemory()
		{
			//
			// bulk erase the device
			//
			this.ExecuteInstructions 
			(
				0x040200,			// goto 0x200                   // 040200
				0x040200,			// goto 0x200                   // 040200
				0x000000,			// nop                          // 000000
				0x2404FA,			// mov #404f, w10               // 2404FA
				0x883B0A,			// mov w10, NVMCON              // 883B0A
				0xA8E761,			// bset NVMCON, #WR             // A8E761
				0x000000,			// nop                          // 000000
				0x000000,			// nop                          // 000000
				0x000000,			// nop                          // 000000
				0x000000			// nop                          // 000000
			);
			//
			// wait 330ms for bulk erase to complete
			//
			Thread.Sleep(330);
		}

		/// <summary>
		/// Writes 64 words to program memory.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public override void WriteProgramRow(uint address, byte[] pwords)
		{
			ushort visi;
			//
			// make sure the buffer size is divisible by 3 (program instructions
			// are 3 bytes on this device
			//
			if (pwords.Length != 64 * 3)
				throw new Exception("Invalid buffer size.");
			//
			// prepare to program 64 program rows
			//
			this.ExecuteInstructions 
			(
				//
				// ;exit reset vector
				// 
				0x040200,									// goto 0x200                   // 040200
				0x040200,									// goto 0x200                   // 040200
				0x000000,									// nop                          // 000000
				//
				// ;set NVMCON to program 64 words
				//
				0x24001A,									// mov #0x4001, w10             // 24001A
				0x883B0A,									// mov w10, NVMCON              // 883B0A 
				// 
				// ;init write pointer (w7) for TBLWT instruction
				//
				0x200000 | ((address >> 16) << 4),			// mov DEST<23:16>, w3          // 200xx0 // <--cheeck this w3??
				0x880190,									// mov w0, TBLPAG               // 880190
				0x200007 | ((address & 0x0000FFFF) << 4)	// mov DEST<15:0>, w7           // 2xxxx7
			);
			//
			// repeat 16 times to write 64 words
			//
			for (byte i = 0; i < 16; i++)
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
					0x200003 | (lsw002 << 4),		// mov #<LSW2>, w3              // 2xxxx3
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
				0xA8E761,			// bset NVMCON, #WR				// A8E761
				0x000000,			// nop							// 000000
				0x000000,			// nop							// 000000
				0x000000,			// nop							// 000000
				0x000000,			// nop							// 000000
				0x000000			// nop	;padding				// 000000
			);
			//
			// wait 1.28ms for row programming
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
					0x803B00,			// mov NVMCON, w0				// 803B00
					0x883C20,			// mov w0, VISI					// 883C20
					0x000000,			// nop							// 000000
					0x000000			// nop	;padding				// 000000
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
					0x040200,			// goto 0x200					// 040200
					0x000000			// nop							// 000000
				);
			}
			//
			// wait until NVMCON<WR> clears (programming completes)
			//
			while ((visi & 0x8000) != 0x0000);
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
}
