using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Japp
{
    /// <summary>
    /// Enumerates all the commands supported by the PICProg protocol.
    /// </summary>
    public enum JappCommand
    {
        PP_CMD_INIT                 = 0x01,
        PULSE_VPP					= 0x02,
        HOLD_ON_RESET				= 0x03,
        RELEASE_FROM_RESET			= 0x04,
        PP_CMD_SET_VDD              = 0x05,
        PP_CMD_SET_VPP              = 0x06,
        WRITE						= 0x07,
        READ						= 0x08,
        PP_CMD_SEND_PGD_CYCLES		= 0x09,
        PP_CMD_WRITE_BITS			= 0x0A,
		PP_CMD_SEND_5_NOPS			= 0x0B,
		READ_TO_BUFFER		= 0x0C,
		READ_BUFFER					= 0x0D,
		GET_BUFFER_LEN				= 0x0E,
		READ_VISI					= 0x0F,
		READ_VISI_TO_BUFFER			= 0x10,
		SET_SPI_5MHZ				= 0x11,
		SET_SPI_1_85MHZ				= 0x12,
		READ_SPI					= 0x13,
		READ_TO_BUFFER_SPI			= 0x14,
        PP_CMD_TOGGLE_LED           = 0x7F,
        PP_CMD_RCON                 = 0x80,
        PP_CMD_LAST_CMD             = 0x81,
        PP_CMD_BOGUS                = 0xFF
    }

    /// <summary>
    /// Enumerates all the responses returned by the PICProg protocol.
    /// </summary>
    public enum JappResponse
    {
        ACK								= 0x01,
        PP_RSP_UNKNOWN_COMMAND          = 0x02,
        PP_RSP_COMMAND_NOT_SUPPORTED    = 0x03,
        PP_RSP_TIMEOUT                  = 0x04,
        PP_RSP_INVALID_ARGUMENTS        = 0x05,
		BUFFER_OVERFLOW					= 0x06
    }

    /// <summary>
    /// Structure used to store command response and
    /// associated data.
    /// </summary>
    public struct CommResponse
    {
        public JappResponse Response;
        public byte[] Data;
    }

    /// <summary>
    /// Handles all communication with the PICProg device.
    /// </summary>
    public class Comm : IDisposable    
    {

        private SerialPort serialPort;

		/// <summary>
        /// Indicates whether a communications channel with the programmer is open.
        /// </summary>
        public bool IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the Comm object.
        /// </summary>
        public Comm()
        {
            this.serialPort = new SerialPort();
            this.serialPort.ReadTimeout = 1000;
            this.serialPort.WriteTimeout = 1000;
            this.serialPort.ReadBufferSize = 512;
            this.serialPort.WriteBufferSize = 64;
            this.IsConnected = false;
        }

        /// <summary>
        /// Initializes the Comm object and opens the communications port.
        /// </summary>
        /// <param name="portName"></param>
        public Comm(string portName)
            : this()
        {
            this.Open(portName);
        }

        /// <summary>
        /// Opens the communications port used to communicate with programmer.
        /// </summary>
        /// <param name="portName"></param>
        public void Open(string portName)
        {
			try
			{
				this.serialPort.PortName = portName;
				this.serialPort.Open();
				//this.serialPort.DtrEnable = true;
				//this.serialPort.RtsEnable = true;
				//this.serialPort.Handshake = Handshake.RequestToSend;
				//this.serialPort.BaudRate = 19200;
				//this.serialPort.Handshake = Handshake.None;
				//this.serialPort.DataBits = 8;
				//this.serialPort.Parity = Parity.None;
				//this.serialPort.StopBits = StopBits.One;
				this.IsConnected = true;
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new CommException(ex);
			}
        }

        /// <summary>
        /// Sends a command to the programmer.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <returns></returns>
        public CommResponse SendCommand(JappCommand command)
        {
            return this.SendCommand(command, new byte[] { });
        }
        
        /// <summary>
        /// Sends a command with arguments to the programmer.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="arguments">The command argumments or data.</param>
        /// <returns></returns>
        public CommResponse SendCommand(JappCommand command, params byte[] arguments)
        {
            byte len;
            CommResponse response = new CommResponse();

			try
			{
				if (arguments.Count() > 0)
				{
					//
					// make sure the command is not too big
					//
					Debug.Assert(arguments.Count() <= 0xFFU - 0x2U);
					//
					// prepare the command
					//
					byte[] cmd = new byte[arguments.Count() + 2];
					cmd[0] = (byte)((int)command);
					cmd[1] = (byte)arguments.Count();
					for (int i = 0; i < arguments.Count(); i++)
						cmd[i + 2] = arguments[i];
					//
					// discard input buffer
					//
					//this.serialPort.DiscardInBuffer();


					//
					// send the command
					//
					this.serialPort.Write(cmd, 0, cmd.Count());

					//for (int i = 0; i < cmd.Count(); i++)
					//  this.serialPort.Write(new byte[] { cmd[i] }, 0, 1);
				}
				else
				{
					//
					// send the command without arguments
					//
					this.serialPort.Write(new byte[] { (byte)((int)command) }, 0, 1);
				}
				//Thread.Sleep(1);
				//
				// wait for the response
				//
				while (this.serialPort.BytesToRead == 0) ;
				response.Response = (JappResponse)this.serialPort.ReadByte();
				//
				// get the 2nd byte of the response. This byte indicates the length
				// of the rest of the message so we also initialize response.Data to
				// an array of this length
				//
				len = (byte)this.serialPort.ReadByte();
				response.Data = new byte[len];
				//
				// if there's any data in the response then copy it to the
				// Data array
				//
				for (int i = 0; i < len; i++)
				{
					while (this.serialPort.BytesToRead == 0) ;	// spin cpu until we got bytes to read
					response.Data[i] = (byte)this.serialPort.ReadByte();
				}
				//
				// discard any data left in buffer
				//
				this.serialPort.DiscardInBuffer();
			}
			catch (TimeoutException)
			{
				throw new CommTimeoutException();
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new CommException(ex);
			}
            finally
            {
            }
            //
            // return the response
            //
            return response;
        }

		/// <summary>
		/// Sends a commmand and returns without receiving a response
		/// </summary>
		/// <param name="command"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public void SendCommandSilent(JappCommand command, params byte[] arguments)
		{
			try
			{
				if (arguments.Count() > 0)
				{
					//
					// make sure the command is not too big
					//
					Debug.Assert(arguments.Count() <= 0xFFU - 0x2U);
					//
					// prepare the command
					//
					byte[] cmd = new byte[arguments.Count() + 2];
					cmd[0] = (byte)((int)command);
					cmd[1] = (byte)arguments.Count();
					for (int i = 0; i < arguments.Count(); i++)
						cmd[i + 2] = arguments[i];
					//
					// send the command
					//
					this.serialPort.Write(cmd, 0, cmd.Count());

					//for (int i = 0; i < cmd.Count(); i++)
					//  this.serialPort.Write(new byte[] { cmd[i] }, 0, 1);
				}
				else
				{
					//
					// send the command without arguments
					//
					this.serialPort.Write(new byte[] { (byte)((int)command) }, 0, 1);
				}
			}
			catch (TimeoutException)
			{
				throw new CommTimeoutException();
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new CommException(ex);
			}
			finally
			{
			}
		}

		/// <summary>
        /// Close the communications port.
        /// </summary>
        public void Close()
        {
            if (this.serialPort != null)
            {
				try
				{
					this.serialPort.DiscardInBuffer();
					this.serialPort.DiscardOutBuffer();
					this.serialPort.Close();
				}
				catch (ThreadAbortException)
				{
					throw;
				}
				catch (Exception ex)
				{
					throw new CommException(ex);
				}
                finally
                {
                    this.IsConnected = false;
                }
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.Close();
        }

        #endregion

    }
}
