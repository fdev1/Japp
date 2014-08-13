using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Japp
{
	/// <summary>
	/// Base class shared by PIC24F/dsPIC33F and PIC24E/dsPIC33E families.
	/// </summary>
	public abstract class dsPICBase : IPICProgrammer
	{
		//
		// private memebrs
		//
		protected Comm comm;
		protected const byte SIX = 0X00;
		protected const byte REGOUT = 0x80;
		protected const uint DEVID = 0xFF0000;
		protected const uint DEVREV = 0xFF0002;
		protected const uint APPID = 0x800FF0;
		protected const byte PWORD_SIZE = 3;		/* this is the size allocated to each program word in the program memory array */
		protected ushort ROW_SIZE = 64;				/* this is the # of program words that we program at once (varies from dsPIC family to family */
		protected byte internalBufferSize = 0;		/* this is the size of the internal buffer on the programmer, can vary in device or firmware versions */
		protected byte APPID_VALUE = 0;

		/// <summary>
		/// Gets a value indicating whether the device is in ICSP mode.
		/// </summary>
		public bool IsICSPMode
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets a value indicating whether the device is in Enhanced ICSP mode.
		/// </summary>
		public bool IsEnhancedICSPMode
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the executive filename
		/// </summary>
		public virtual string ExecutiveFile
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the communications object.
		/// </summary>
		public Comm Comm
		{
			get
			{
				return this.comm;
			}
			set
			{
				this.comm = value;
			}
		}

		/// <summary>
		/// Enter ICSP mode.
		/// </summary>
		public void EnterICSP()
        {
			//
			// get the size of the internal buffer
			//
			CommResponse rsp = this.comm.SendCommand(JappCommand.GET_BUFFER_LEN);
			Debug.Assert(rsp.Data.Length == 1);
			this.internalBufferSize = rsp.Data[0];
			//
			// initialize programmer state to known values
			//
			this.comm.SendCommand(JappCommand.PP_CMD_INIT);
            //
            // set Vpp to Vss for about 100ms, then send a 500us pulse on Vpp and 
            // start clocking in the ENTER ICSP sequence and pull Vpp to Vdd.
            //
            // The PP_CMD_HOLD_IN_RESET command is used to pull Vpp down, then
            // we use PP_CMD_PULSE_VPP_US command to send the 500us pulse through
            // Vpp (the argument is the # of uSecs in unsigned 16-bit little-endian
            // format). Then we use PP_CMD_RELEASE_FROM_RESET command to pull Vpp
            // to Vdd. The timing of the 100ms delay is not important so we can just
            // use .Net's Thread.Sleep method.
            //
            this.comm.SendCommand(JappCommand.HOLD_ON_RESET);
            Thread.Sleep(100);
            this.comm.SendCommand(JappCommand.PULSE_VPP, 0xF4, 0x01);
            //
            // a 1us minimun delay is required before clocking the ICSP sequence, communications
            // delay may take care of this but since there's no maximun where just going to wait
            // 1ms before sending the command
            //
            Thread.Sleep(1);
            this.comm.SendCommand(JappCommand.WRITE, 0x4D, 0x43, 0x48, 0x51);
            this.comm.SendCommand(JappCommand.RELEASE_FROM_RESET);
            //
            // wait at least 25ms before presenting data on PGD
            //
            Thread.Sleep(25);
            //
            // send 5 clock cycles on PGC before 1st SIX instruction
            //
            this.comm.SendCommand(JappCommand.PP_CMD_SEND_PGD_CYCLES, 0x05);
			//
			// set the IsICSP mode property
			//
			this.IsICSPMode = true;
        }


		/// <summary>
		/// Exits ICSP and resets the device.
		/// </summary>
		public void ExitICSP()
		{
			//
			// exit reset vector
			//
			this.ExecuteInstructions
			(
				0x040200,				// goto 0x200                   // 040200
				0x040200				// goto 0x200                   // 040200
			);
			//
			// release from reset
			//
			this.comm.SendCommand(JappCommand.HOLD_ON_RESET);
			this.comm.SendCommand(JappCommand.RELEASE_FROM_RESET);
			//
			// clear icsp mode flag
			//
			this.IsICSPMode = false;
		}

		/// <summary>
		/// Places the device in enhanced ICSP mode
		/// </summary>
		public void EnterEnhancedICSP()
		{
			byte[] appid;
			//
			// enter ICSP mode
			//
			this.EnterICSP();
			//
			// read application id
			//
			appid = this.ReadProgramWord(APPID);
			//
			// if the programmer executive is not installed throw
			// an exception
			//
			if (appid[0] != this.APPID_VALUE)
			{
				this.ExitICSP();
				throw new PENotFoundExeption();
			}
			//
			// exit ICSP mode
			//
			this.ExitICSP();
			//
			// send MCLR pulse before clocking in extended icsp sequence
			//
			this.comm.SendCommand(JappCommand.HOLD_ON_RESET);
			Thread.Sleep(100);
			this.comm.SendCommand(JappCommand.PULSE_VPP, 0xF4, 0x01);
			//
			// a 1us minimun delay is required before clocking the ICSP sequence, communications
			// delay may take care of this but since there's no maximun where just going to wait
			// 1ms before sending the command
			//
			Thread.Sleep(1);
			//
			// send enhanced icsp sequence and pull MCLR up
			//
			this.comm.SendCommand(JappCommand.WRITE, 0x4D, 0x43, 0x48, 0x50);
			this.comm.SendCommand(JappCommand.RELEASE_FROM_RESET);
			//
			// wait at least 25ms before presenting data on PGD
			//
			Thread.Sleep(25);
			//
			// set the IsICSP mode property
			//
			this.IsICSPMode = true;
			this.IsEnhancedICSPMode = true;
		}

		/// <summary>
		/// Executes instructions in target device.
		/// </summary>
		/// <param name="instructions">The instructions to execute.</param>
		protected void ExecuteInstructions(params uint[] instructions)
		{
			this.comm.SendCommand(JappCommand.WRITE, this.PackInstructions(instructions));
		}

		/// <summary>
		/// Reads the DEVICEID register from program memory.
		/// </summary>
		/// <returns>A string representation of the register value in hexadecimal format.</returns>
		public virtual string GetDeviceID()
		{
			byte[] pword = this.ReadProgramWord(DEVID);
			StringBuilder id = new StringBuilder();
			id.Append(pword[1].ToString("X2"));
			id.Append(pword[0].ToString("X2"));
			return id.ToString();
		}

		/// <summary>
		/// Reads the DEVREV register from program memory.
		/// </summary>
		/// <returns>A string representation of the register value in hexadecimal format.</returns>
		public virtual string GetRevisionID()
		{
			byte[] pword = this.ReadProgramWord(DEVREV);
			StringBuilder id = new StringBuilder();
			id.Append(pword[1].ToString("X2"));
			id.Append(pword[0].ToString("X2"));
			return id.ToString();
		}
		
		/// <summary>
		/// Reads a single word to program memory.
		/// </summary>
		/// <param name="address">The address to program</param>
		/// <returns></returns>
		public byte[] ReadProgramWord(uint address)
		{
			if (this.IsEnhancedICSPMode == true)
			{
				return this.ReadProgramWordEICSP(address);
			}
			else
			{
				return this.ReadProgramWordICSP(address);
			}
		}

		/// <summary>
		/// Reads a word of program memory using ICSP
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		protected virtual byte[] ReadProgramWordICSP(uint address)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reads a word of program memory using enhanced ICSP.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		private byte[] ReadProgramWordEICSP(uint address)
		{
			return this.ReadProgramMemoryEICSP(address, 1);
		}

		/// <summary>
		/// Reads program memory.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public byte[] ReadProgramMemory(uint address, uint length)
		{
			if (this.IsEnhancedICSPMode == true)
			{
				return this.ReadProgramMemoryEICSP(address, length);
			}
			else
			{
				return this.ReadProgramMemoryICSP(address, length);
			}
		}

		/// <summary>
		/// Reads program memory in enhanced ICSP mode
		/// </summary>
		/// <param name="address"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		private byte[] ReadProgramMemoryEICSP(uint address, uint length)
		{
			//Debug.Assert(length % 2 == 0);
			List<byte> cmd = new List<byte>();
			CommResponse rsp;
			const byte PWORD_INTERNAL = 3;
			byte[] pwords = new byte[length * PWORD_SIZE];
			ushort wordsRead = 0;
			byte bytesToBuffer = (byte)((this.internalBufferSize - (this.internalBufferSize % PWORD_INTERNAL)));
			//
			// make sure SPI is clocked at less than 1.85 mhz
			//
			rsp = this.comm.SendCommand(JappCommand.SET_SPI_1_85MHZ);
			Debug.Assert(rsp.Response == JappResponse.ACK);
			//
			// make sure length is 16-bit (maybe change type to ushort)
			//
			Debug.Assert(length <= 32768);
			//
			// pack the command
			//
			cmd.AddRange(EnhancedICSP.PackCommand(EnhacedICSPCommand.READP, 4));	// CCCCLLLLLLLLLLLL (C=Command L=length)
			cmd.AddRange(new byte[] { (byte)(length >> 8), (byte)length });			// no of words to read
			cmd.AddRange(new byte[] { 0x0, (byte)(address >> 16) });				// msb of address
			cmd.AddRange(new byte[] { (byte)(address >> 8), (byte)address });		// lsw of address
			//
			// send the command to programmer
			//
			this.comm.SendCommand(JappCommand.WRITE, cmd.ToArray());
			//
			// we're supposed to wait until PGD goes low
			//
			Thread.Sleep(200);
			//
			// calculate the amount of bytes in the response
			//
			uint bytesToRead = ((((length & 1) == 0) ? (2 + 3 * length / 2) : (4 + 3 * (length - 1) / 2)) * PWORD_INTERNAL) - 4;
			//
			// read the response header
			//
			do
			{
				rsp = this.comm.SendCommand(JappCommand.READ_SPI, 1);
			}
			while (rsp.Data[0] == 0xFF);

			rsp = this.comm.SendCommand(JappCommand.READ_SPI, 3);

			//
			// loop while there's bytes to read
			//
			while (bytesToRead > 0)
			{
				//
				// calculate the size of the next chunk to read
				//
				byte chunkSize = (byte) Math.Min((uint) bytesToBuffer, bytesToRead);
				//
				// read the response to the internal buffer
				//
				this.comm.SendCommandSilent(JappCommand.READ_TO_BUFFER, chunkSize);
				//
				//
				//
				rsp = this.comm.SendCommand(JappCommand.READ_BUFFER);
				Debug.Assert(rsp.Response == JappResponse.ACK);
				Debug.Assert(rsp.Data.Length == chunkSize);
				//
				// copy the bytes
				//
				for (byte i = 0; i < chunkSize / PWORD_SIZE; i++)
				{
					if (wordsRead < length)
					{
						if ((wordsRead & 1) == 0)
						{
							pwords[wordsRead + 0] = rsp.Data[(i * PWORD_INTERNAL) + 0];
							pwords[wordsRead + 1] = rsp.Data[(i * PWORD_INTERNAL) + 1];
							pwords[wordsRead + 2] = rsp.Data[(i * PWORD_INTERNAL) + 2];
						}
						else
						{
							pwords[wordsRead + 2] = rsp.Data[(i * PWORD_INTERNAL) + 2];
							pwords[wordsRead + 0] = rsp.Data[(i * PWORD_INTERNAL) + 0];
							pwords[wordsRead + 1] = rsp.Data[(i * PWORD_INTERNAL) + 1];

						}
					}
					
					wordsRead++;
				}
				//
				//
				//
				bytesToRead -= chunkSize;
			}
			//
			// return array of program words
			//
			return pwords;
		}

		/// <summary>
		/// Reads a segment of program memory in ICSP mode
		/// </summary>
		/// <param name="address">The address of the segment to read.</param>
		/// <param name="length">The number of words to read.</param>
		/// <returns>A byte array with the program words read.</returns>
		protected virtual byte[] ReadProgramMemoryICSP(uint address, uint length)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Bulk erases the device memory.
		/// </summary>
		public virtual void EraseProgramMemory()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Bulk erases program and executive memory.
		/// </summary>
		public virtual void EraseProgramAndExecutiveMemory()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Writes a row of program memory.
		/// </summary>
		/// <param name="address">The address of the row.</param>
		/// <param name="pwords">An array contining the program words.</param>
		public virtual void WriteProgramRow(uint address, byte[] pwords)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Programs the device memory
		/// </summary>
		/// <param name="address"></param>
		/// <param name="pwords"></param>
		/// <param name="offset"></param>
		public void WriteProgramMemory(uint address, byte[] pwords, uint offset)
		{
			if (this.IsEnhancedICSPMode == true)
			{
				this.WriteProgramMemoryEICSP(address, pwords, offset);
			}
			else if (this.IsICSPMode == true)
			{
				this.WriteProgramMemoryICSP(address, pwords, offset);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Programs the device memory in enhanced ICSP mode
		/// </summary>
		/// <param name="address"></param>
		/// <param name="pwords"></param>
		/// <param name="offset"></param>
		private void WriteProgramMemoryEICSP(uint address, byte[] pwords, uint offset)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Programs the device memory in ICSP mode.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="pwords"></param>
		private void WriteProgramMemoryICSP(uint address, byte[] pwords, uint offset)
		{
			uint addy = address;
			byte[] bytesToWrite = new byte[this.ROW_SIZE * PWORD_SIZE];
			uint lastProgrammedAddress;
			//
			// verify that the address is valid
			//
			if (address % 2 != 0)
				throw new AddressException();
			//
			// get the last programmed address
			//
			lastProgrammedAddress = this.GetLastProgrammedAddress(pwords) + (address - offset);
			//
			// if there;s no data to program we're done
			//
			if (lastProgrammedAddress == 0)
				return;
			//
			// program memory
			//
			while (addy <= lastProgrammedAddress) //((addr / 2) * 3 < lastProgrammedAddress)
			{
				bool gotBytes = false;
				//
				// copy data to program
				//
				Array.Copy(pwords, ((addy - (address - offset)) / 2) * PWORD_SIZE, bytesToWrite, 0, this.ROW_SIZE * PWORD_SIZE);

				//
				// check if there's any word to program in thhis row
				//
				foreach (byte b in bytesToWrite)
				{
					if (b != 0xFF)
					{
						gotBytes = true;
						break;
					}
				}
				//
				// program memory
				//
				if (gotBytes == true)
					this.WriteProgramRow(addy, bytesToWrite);
				//
				// increase address by 64 program words
				//
				addy += ((uint)this.ROW_SIZE * 2);
			}
		}

		/// <summary>
		/// Verifies program memory
		/// </summary>
		/// <param name="address"></param>
		/// <param name="pwords"></param>
		/// <param name="length"></param>
		public void Verify(uint address, byte[] pwords, uint length, uint offset, bool configBytes, Device device)
		{
			uint lastProgrammedAddress;
			byte[] devicePwords;
			uint addy = address;
			byte verifyIncrement = (configBytes == true) ? (byte) PWORD_SIZE : (byte) 1U;
			//
			// verify that the address is valid
			//
			if (address % 2 != 0)
				throw new AddressException();
			//
			// get the last programmed address;
			//
			lastProgrammedAddress = this.GetLastProgrammedAddress(pwords) + (address - offset);
			//
			// if there's no words programmed in the device memory
			// or if the last programmed address is bellow our verify range
			// return without doing any work
			//
			if (lastProgrammedAddress == 0 /*|| lastProgrammedAddress < address*/)
				return;
			//
			// begin verification
			//
			while (addy < lastProgrammedAddress && length > 0)
			{
				uint wordsToReaad;
				bool gotBytes;
				//
				// calculate the # of words to read
				//
				wordsToReaad = Math.Min((ushort)32, length);

				gotBytes = false;
				//
				// find out if these locations are programmed and so we only
				// verify programmed sections
				//
				for (uint i = 0; i < wordsToReaad * PWORD_SIZE; i += verifyIncrement)
				{
					if (0xFF != pwords[(((addy - (address - offset)) / 2) * PWORD_SIZE) + i])
					{
						gotBytes = true;
						break;
					}
				}
				//
				// if section is programmed then verify it
				//
				if (gotBytes == true)
				{
					//
					// read 32 words of program memory
					//
					devicePwords = this.ReadProgramMemory(addy, wordsToReaad);
					//
					// verify every byte
					//
					for (int i = 0; i < wordsToReaad * PWORD_SIZE; i += verifyIncrement)
					{
						//
						// if we're verifying the configuration bits apply masks
						//
						if (configBytes == true)
						{
							foreach (ConfigurationBit bit in device.ConfigurationBits)
							{
								if ((addy + i) == bit.Address)
								{
									devicePwords[i] &= bit.Mask;
									devicePwords[i] |= bit.OnesMask;
									pwords[(((addy - (address - offset)) / 2) * PWORD_SIZE) + i] &= bit.Mask;
									pwords[(((addy - (address - offset)) / 2) * PWORD_SIZE) + i] |= bit.OnesMask;
								}
							}
						}
						//
						// verify program words
						//
						if (devicePwords[i] != pwords[(((addy - (address - offset)) / 2) * PWORD_SIZE) + i])
						{
							throw new VerifyException();
						}
					}
				}
				//
				// increment address
				//
				addy += wordsToReaad * 2;
				length -= wordsToReaad;
			}
		}

		/// <summary>
		/// Finds the address of the last programmed word on the program memory array
		/// on PC units relative to the start of the array.
		/// </summary>
		/// <param name="pwords"></param>
		/// <returns></returns>
		private uint GetLastProgrammedAddress(byte[] pwords)
		{
			uint lastProgrammedAddress = (((uint)pwords.Length / 3) - 1) * 2;

			while (lastProgrammedAddress > 0)
			{
				uint offset = (lastProgrammedAddress / 2) * 3;
				//
				// if the address is programmed exit loop
				//
				if (pwords[offset + 0] != 0xFF || pwords[offset + 1] != 0xFF || pwords[offset + 2] != 0xFF)
					break;
				//
				// decrement address
				//
				lastProgrammedAddress -= 2;
			}
			return lastProgrammedAddress;
		}

		/// <summary>
		/// Reads the contents of the VISI register.
		/// </summary>
		/// <returns></returns>
		protected ushort ReadVISI()
		{
			CommResponse rsp;
			rsp = this.comm.SendCommand(JappCommand.READ_VISI);
			//
			// if the response is OK return the value of VISI
			//
			if (rsp.Response == JappResponse.ACK)
			{
				return (ushort)((ushort)rsp.Data[0] | (ushort)(rsp.Data[1] << 8));
			}
			else
			{
				throw new Exception();
			}
		}

		/// <summary>
		/// Reads the contents of the VISI register into the
		/// programmers internal buffer
		/// </summary>
		protected void ReadVISIToBuffer()
		{
			this.comm.SendCommandSilent(JappCommand.READ_VISI_TO_BUFFER);
			/*
			//
			// send the REGOUT command. During the first 8 bits of the REGOUT
			// command the device reads the reads the command and during the next
			// 16 bits it clocks out the result of the VISI register. To support this
			// our programmer's PP_CMD_READ command writes and reads at the same
			// time so we must write as much data as we want to read, therefore the
			// bits that we're reading must be send as 1s.
			//
			// 
			CommResponse rsp;
			//
			// send the REGOUT command
			//
			Thread.Sleep(1);
			rsp = this.comm.SendCommand(PP_COMMAND.PP_CMD_WRITE_BITS, 0x4, REGOUT);
			Thread.Sleep(1);
			this.comm.SendCommand(PP_COMMAND.PP_CMD_SEND_PGD_CYCLES, new byte[] { 0x8 });
			Thread.Sleep(1);
			//
			// copy VISI to the internal buffer in the programmer
			//
			this.comm.SendCommandNoResponse(PP_COMMAND.PP_CMD_READ_TO_BUFFER, 0x2);
			Thread.Sleep(1);
			*/
		}

		/// <summary>
		/// Inverts the bits on a 32-bit integer and shifts the MSB out.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		protected uint PreprareInstruction(uint i)
		{
			//
			// reverse all bits
			//
			i = ((i >> 01) & 0x55555555) | ((i & 0x55555555) << 01);
			i = ((i >> 02) & 0x33333333) | ((i & 0x33333333) << 02);
			i = ((i >> 04) & 0x0F0F0F0F) | ((i & 0x0F0F0F0F) << 04);
			i = ((i >> 08) & 0x00FF00FF) | ((i & 0x00FF00FF) << 08);
			i = ((i >> 16) & 0xFFFFFFFF) | ((i & 0xFFFFFFFF) << 16);
			//
			// right shift them 8 bits
			//
			return i >> 8;

		}

		/// <summary>
		/// Packs an arbitrary number of instruction for execution in target device.
		/// </summary>
		/// <param name="instructions"></param>
		/// <returns></returns>
		protected byte[] PackInstructions(params uint[] instructions)
		{
			List<byte> packed_instructions = new List<byte>();
			uint pairs = (uint)instructions.Length / 2;
			//
			// pack instructions pairs
			//
			for (uint i = 0; i < pairs; i++)
			{
				packed_instructions.AddRange(this.Pack2Instructions(instructions[(i * 2)], instructions[(i * 2) + 1]));
			}
			//
			// if there's an unpaired instruction pair it with a nop
			//
			if (instructions.Length % 2 == 1)
			{
				packed_instructions.AddRange(this.Pack2Instructions(instructions[instructions.Length - 1], 0x000000));
			}
			//
			// return the array of packed instructions
			//
			return packed_instructions.ToArray();
		}

		/// <summary>
		/// Packs two instructions for execution in target device.
		/// </summary>
		/// <param name="i1">The 1st instruction to pack.</param>
		/// <param name="i2">The 2nd instruction to pack.</param>
		/// <returns></returns>
		protected byte[] Pack2Instructions(uint instruction1, uint instruction2)
		{
			//
			// reverse the bits in the instruction
			//
			instruction1 = this.PreprareInstruction(instruction1);
			instruction2 = this.PreprareInstruction(instruction2);
			//
			// SIX = 0b0000
			// REGOUT = 0b0001
			// 
			// SIX  24-bit instruction       SIX  24Bit instruction
			// 0000 000000000000000000000000 0000 000000000000000000000000 
			byte[] packedInstruction = new byte[7];
			packedInstruction[0] = (byte)(instruction1 >> 20);   // SIX | i1<23:20>
			packedInstruction[1] = (byte)(instruction1 >> 12);   // i1<19:12>
			packedInstruction[2] = (byte)(instruction1 >> 04);   // i1<11:4>
			packedInstruction[3] = (byte)(instruction1 << 04);   // i1<3:0> | SIX
			packedInstruction[4] = (byte)(instruction2 >> 16);   // i2<23:16>
			packedInstruction[5] = (byte)(instruction2 >> 08);   // i2<15:8>
			packedInstruction[6] = (byte)(instruction2 >> 00);   // i2<7:0>
			return packedInstruction;

		}
	
		/// <summary>
		/// Parses an instruction.
		/// </summary>
		/// <param name="instruction"></param>
		/// <returns></returns>
		public string ParseInstruction(byte[] instruction)
		{
			if (instruction[2] == 0xFF && instruction[1] == 0xFF && instruction[0] == 0xFF)
			{
				return "nopr";
			}

			else if (instruction[2] == 0x00 && instruction[1] == 0x00 && instruction[0] == 0x00)
			{
				return "nop";
			}
			else if (instruction[2] == 0x04)
			{
				return "goto 0x" + instruction[1].ToString("X2") + instruction[0].ToString("X2");
			}
			else if ((instruction[2] & 0xF0) == 0x20)
			{
				ushort arg1 = (ushort)((instruction[2] << 12) | (instruction[1] << 4) | ((instruction[0] >> 4) & 0x000F));
				byte arg2 = (byte)(instruction[0] & 0x0F);
				return string.Format("mov 0x{0:X4}, w{1}", arg1, arg2);
			}
			else
			{
				return string.Empty;
			}

		}
	}
}
