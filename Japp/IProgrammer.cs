using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Japp
{
	/// <summary>
	/// Defines the programmer interface.
	/// </summary>
	public interface IPICProgrammer
	{
		/// <summary>
		/// Gets or sets the communications object.
		/// </summary>
		Comm Comm { get; set; }

		/// <summary>
		/// Gets the filename of the programming executive.
		/// </summary>
		string ExecutiveFile
		{
			get;
		}

		/// <summary>
		/// Gets a value that indicates whether the target device is in ICSP mode.
		/// </summary>
		bool IsICSPMode { get; }

		/// <summary>
		/// Places the target device in ICSP mode.
		/// </summary>
		void EnterICSP();

		/// <summary>
		/// Enters Enhaced ICSP mode.
		/// </summary>
		void EnterEnhancedICSP();

		/// <summary>
		/// Exits ICSP mode and resets the target device.
		/// </summary>
		void ExitICSP();

		/// <summary>
		/// Reads the value of the DEVID register.
		/// </summary>
		/// <returns></returns>
		string GetDeviceID();

		/// <summary>
		/// Reads the value of the DEVREV register.
		/// </summary>
		/// <returns></returns>
		string GetRevisionID();

		/// <summary>
		/// Reads a single program memory location.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		byte[] ReadProgramWord(uint address);

		/// <summary>
		/// Writes to program memory.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		byte[] ReadProgramMemory(uint address, uint length);

		/// <summary>
		/// Erases program memory.
		/// </summary>
		void EraseProgramMemory();

		/// <summary>
		/// Bulk erases program and executive memory.
		/// </summary>
		void EraseProgramAndExecutiveMemory();

		/// <summary>
		/// Writes to program memory.
		/// </summary>
		/// <param name="address">The address where the memory will be written to.</param>
		/// <param name="pwords">The array of program memory.</param>
		/// <param name="offset">The offset within the array where the address begins.</param>
		void WriteProgramMemory(uint address, byte[] pwords, uint offset);

		/// <summary>
		/// Verify the device memory.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="pwords"></param>
		/// <param name="length"></param>
		/// <param name="offset"></param>
		void Verify(uint address, byte[] pwords, uint length, uint offset, bool configBytes, Device device);

		/// <summary>
		/// Parses an instruction.
		/// </summary>
		/// <param name="instructionBytes">The instruction.</param>
		/// <returns>The instruction mnemonic.</returns>
		string ParseInstruction(byte[] instructionBytes);
	}

}
