using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Globalization;
using Japp;

namespace JappUI
{
    public partial class MainForm : Form, IDisposable
    {
        //
        // private members
        //
        private Comm comm;
        private DeviceConfig deviceConfig;
        private IPICProgrammer programmer;
        private byte[] programMemory;
		private byte[] configMemory;
        private Thread workerThread;
        private Device selectedDevice;
		private uint programIndex = 0;
		private ListViewItem lastProgramWord;
		private Stopwatch stopwatch;
		private bool useEnhancedICSP = false;
        //
        // these are used to send messages to UI elements
        // from worker threads
        //
        private delegate void AppendText(string text);
		private delegate void VoidMethod();

        /// <summary>
        /// Initializes the main form.
        /// </summary>
        public MainForm()
        {
			//
			// initialize ui components
			//
            this.InitializeComponent();
            //
            // try to load the config file
            //
            try
            {
                this.deviceConfig = (DeviceConfig)ConfigurationManager.GetSection("deviceFamilies");
				this.deviceConfig.UpdateFamilies();
            }
            catch (Exception ex)
            {
				//
				// display error
				//
				StringBuilder error = new StringBuilder();
				error.Append(Strings.ConfigurationErrorMessage);
				error.Append(Environment.NewLine);
				error.Append(Environment.NewLine);
				error.Append(ex.Message);
                MessageBox.Show(error.ToString(), Strings.ConfigurationError, MessageBoxButtons.OK, MessageBoxIcon.Error);
				//
				// exit app
				//
				throw new AppStartupException();
            }
            //
            // initialize the communications layer
            //
            this.comm = new Comm();
			//
			// stopwatch
			//
			this.stopwatch = new Stopwatch();
            //
            // adjust the height at runtime so the form doesn't take
            // too much space on designer
            //
            this.Height += 200;
            this.Top -= 100;
        }

		/// <summary>
		/// Instantiates the programming object for the selected device
		/// </summary>
		private void InstantiateProgrammer()
		{
			//
			// get the type of the programmmer for this device
			//
			Type type = Type.GetType(this.selectedDevice.Family.ProgrammerClass);
			//
			// if there's already an instance then return
			//
			if (this.programmer != null)
			{
				if (this.programmer.GetType() == type)
				{
					return;
				}
			}
			//
			// initialize the programmer instance
			//
			this.programmer = (IPICProgrammer)Activator.CreateInstance(type);
			this.programmer.Comm = this.comm;
			this.LoadConfigurationBits();
		}

		/// <summary>
		/// Populates the configuration bits datagrid
		/// </summary>
		private void LoadConfigurationBits()
		{
			//
			// clear all rows
			//
			this.dgConfigBits.Rows.Clear();
			//
			// add a row for each configuration bit
			//
			foreach (ConfigurationBit bit in this.selectedDevice.ConfigurationBits)
			{
				this.dgConfigBits.Rows.Add(bit.Namme, bit.Address, string.Empty);
			}
		}

        /// <summary>
        /// Updates the list of device families.
        /// </summary>
        private void UpdateDeviceFamiliesList()
        {
			//
			// clear list
			//
            this.comboDeviceFamilies.Items.Clear();
			//
			// populate it
			//
			foreach (DeviceFamily family in this.deviceConfig.Families)
			{
				this.comboDeviceFamilies.Items.Add(family.Name);
			}
        }

        /// <summary>
        /// Updates the list of devices
        /// </summary>
        private void UpdateDevicesList()
        {
            this.comboDevices.Items.Clear();
            foreach (DeviceFamily family in this.deviceConfig.Families)
            {
                if (family.Name == this.comboDeviceFamilies.Text || this.comboDeviceFamilies.Text == string.Empty)
                {
                    foreach (Device device in family.Devices)
                        this.comboDevices.Items.Add(device.Name);
                }
            }
        }

		/// <summary>
		/// Loads 1000 words of program memory to listview.
		/// </summary>
		private void LoadProgramWords()
		{
			ushort wordsToLoad = 1000;
			uint iAddr = this.programIndex / 3;
			Device device = this.selectedDevice;
			//iAddr *= 2;

			//
			// display program memory on listview
			//
			while (wordsToLoad-- != 0 && iAddr < (device.ProgramMemroySize * 2))
			{
				string symbol;
				StringBuilder bytes = new StringBuilder();
				bytes.Append(this.programMemory[(iAddr * 3) + 2].ToString("X2"));
				bytes.Append(this.programMemory[(iAddr * 3) + 1].ToString("X2"));
				bytes.Append(this.programMemory[(iAddr * 3) + 0].ToString("X2"));
				uint addr = iAddr * 2;


				symbol = this.programmer.ParseInstruction(new byte[] {
					this.programMemory[(iAddr * 3) + 0],
					this.programMemory[(iAddr * 3) + 1],
					this.programMemory[(iAddr * 3) + 2] });

				//
				// add program word to listview
				//
				this.lastProgramWord  = 
					this.lvPMem.Items.Add(new ListViewItem(new string[] { addr.ToString("X6"), bytes.ToString(), symbol }));
				//
				// increase the index to reflect load loaded instruction
				//
				this.programIndex += 3;
				//
				// increase the address
				//
				iAddr++;
				if (iAddr % 100 == 0)
					Application.DoEvents();
			}
		}

        /// <summary>
        /// Connects to the target device and places it on ICSP mode.
        /// </summary>
        private void EnterICSPMode()
        {

            if (!this.comm.IsConnected)
                return;

            string devid;
            string devrev;
            //
            // put device inn ICSP mode
            //
			if (this.useEnhancedICSP)
			{
				try
				{
					this.programmer.EnterEnhancedICSP();
				}
				catch (PENotFoundExeption)
				{
					this.txtOutput.AppendText("Programming executive not found.");
					this.txtOutput.AppendText(Environment.NewLine);
					try
					{
						//
						// program the programmer executive
						//
						this.programmer.EnterICSP();
						this.workerThread = new Thread(new ThreadStart(this.ProgramExecutive));
						this.workerThread.Start();

						while (this.workerThread.IsAlive)
						{
							Application.DoEvents();
							Thread.Sleep(0);
						}

						this.programmer.ExitICSP();
						//
						// enter enhanced ICSP
						//
						this.programmer.EnterEnhancedICSP();
					}
					catch
					{
						this.programmer.EnterICSP();
					}
				}
			}
			else
			{
				this.programmer.EnterICSP();
			}
            //
            // read the DEVID and DEVREV registers
            //
            devid = programmer.GetDeviceID();
            devrev = programmer.GetRevisionID();
            //
            // output DEVID and DEVREV to output window
            //
            this.txtOutput.AppendText(string.Format(Strings.DeviceIdAndRevision, devid, devrev));
			this.txtOutput.AppendText(Environment.NewLine);
            //
            // look for the device in the configuration file, if found get ready
            // for a program operation and return
            //
            foreach (DeviceFamily family in this.deviceConfig.Families)
            {
                foreach (Device device in family.Devices)
                {
                    if (device.DEVID == devid && device.DEVREV == devrev)
                    {
						//
						// display device recognized info
						//
                        this.txtOutput.AppendText(string.Format(Strings.DeviceRecognizedAs, device.Name));
						this.txtOutput.AppendText(Environment.NewLine);

                        if (this.comboDevices.Text == device.Name)
                        {
							this.tbErase.Enabled = true;
							this.tbReset.Enabled = true;
                            this.mnuReadPM.Enabled = true;
							this.comboDeviceFamilies.Enabled = false;
							this.tbCheckDevice.Enabled = false;
							this.comboDevices.Enabled = false;
							//
							// show "ready" message
							//
							this.txtOutput.AppendText(Strings.ReadyForNextOperation);
							this.txtOutput.AppendText(Environment.NewLine);
							return;
                        }
                        else
                        {
                            this.mnuReadPM.Enabled = false;
							this.tbErase.Enabled = false;
							this.tbReset.Enabled = false;
							this.tbProgram.Enabled = false;
							this.comboDevices.Enabled = true;
							this.comboDeviceFamilies.Enabled = true;
							this.tbCheckDevice.Enabled = true;
							//
							// show error message
							//
							this.txtOutput.AppendText(Strings.DeviceIdDoesNotMatch);
							this.txtOutput.AppendText(Environment.NewLine);
							MessageBox.Show(Strings.DeviceIdDoesNotMatch, Strings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //
                            // exit ICSP
                            //
                            this.programmer.ExitICSP();
                            return;
                        }
                    }
                }
            }
            //
            // display error message
            //
            this.txtOutput.AppendText(Strings.DeviceNotSupported);
            this.txtOutput.AppendText(Environment.NewLine);
			MessageBox.Show(Strings.DeviceNotSupported, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			//
			// disable controls
			//
			this.mnuReadPM.Enabled = false;
			this.tbReset.Enabled = false;
			this.tbErase.Enabled = false;
			this.comboDevices.Enabled = true;
			this.comboDeviceFamilies.Enabled = true;
			this.tbCheckDevice.Enabled = true;
			this.tbProgram.Enabled = false;
            //
            // exit ICSP mode
            //
            programmer.ExitICSP();
        }


		private void ProgramExecutive()
		{
			try
			{
				AppendText appendText = new AppendText(this.txtOutput.AppendText);

				string file = string.Empty;
				string executivePath = ConfigurationManager.AppSettings["ExecutivePath"];

				this.Invoke(appendText, string.Format("Looking for executive in: {0}", executivePath));
				this.Invoke(appendText, Environment.NewLine);
				//
				//
				//
				foreach (string f in Directory.GetFiles(executivePath))
				{
					//
					// strip path from filename
					//
					file = Path.GetFileName(f);

					if (file.StartsWith(this.programmer.ExecutiveFile) && file.EndsWith(".hex"))
					{
						this.Invoke(appendText, string.Format("Found executive file: {0}", file));
						this.Invoke(appendText, Environment.NewLine);
						file = f;
						break;
					}
				}
				//
				// 
				//
				byte[] executiveMemory = new byte[2048 * 3];
				for (int i = 0; i < executiveMemory.Length; i++)
				{
					executiveMemory[i] = 0xFF;
				}
				//
				// load the executive HEX file
				//
				HEXFile executive = new HEXFile(file);
				executive.ReadFile(new byte[] {}, executiveMemory, 0x800000);
				//
				// erase executive memory
				//
				this.Invoke(appendText, "Erasing program and executive memory...");
				this.programmer.EraseProgramAndExecutiveMemory();
				this.Invoke(appendText, Strings.Success);
				this.Invoke(appendText, Environment.NewLine);
				//
				// program the executive
				//
				this.Invoke(appendText, "Programming executive...");
				this.programmer.WriteProgramMemory(0x800000, executiveMemory, 0x000000);
				this.Invoke(appendText, Strings.Success);
				this.Invoke(appendText, Environment.NewLine);
			}
			catch (DirectoryNotFoundException)
			{
				throw;
			}

		}


        /// <summary>
        /// Loads the selected hex file.
        /// </summary>
        private void LoadHEXFile()
        {
			//
			// unload HEX file
			//
			this.ClearProgram();
			//
			// initialize program memory array
			//
			this.AllocateProgramArray();
			//
			// read and process the hex file
			//
			try
			{
				//
				// read the HEX file
				//
				HEXFile hex = new HEXFile(this.txtHEXFile.Text);
				hex.ReadFile(this.programMemory, this.configMemory, this.selectedDevice.ConfigAddress);
				//
				// load it into listview and enable programming
				//
				this.LoadProgramWords();
				//
				// enable program buttons
				//
				this.tbProgram.Enabled = true;
				this.tbVerify.Enabled = true;
				//
				// display message
				//
				this.txtOutput.AppendText(Strings.HEXFileLoaded);
				this.txtOutput.AppendText(Environment.NewLine);
			}
			catch (IndexOutOfRangeException)
			{
				//
				// display error (device does not have enough memory for this
				// HEX file
				//
				MessageBox.Show(Strings.InsufficientDeviceMemory, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				//
				// disable program buttons
				//
				this.tbProgram.Enabled = false;
			}
			catch (Exception ex)
			{
				//
				// something went wrong, display error
				//
				this.txtOutput.AppendText(ex.Message);
				this.txtOutput.AppendText(Environment.NewLine);
				MessageBox.Show(ex.Message, Strings.Error);
				//
				// disable program button
				//
				this.tbProgram.Enabled = false;
			}
        }

        /// <summary>
        /// Updates the list of COM ports
        /// </summary>
        private void UpdateCOMPortList()
        {
			//
			// remember the currently selected item
			//
            string selectedItem = this.comboCOMPorts.Text.Trim();
			//
			// clear the list of COM ports and the selected item
			//
            this.comboCOMPorts.Items.Clear();
            this.comboCOMPorts.Text = string.Empty;
			//
			// populate the list of COM ports
			//
            foreach (string port in SerialPort.GetPortNames())
            {
                this.comboCOMPorts.Items.Add(port);
				//
				// if this is the item that was previously selected
				// we'll keep it selected
				//
                if (port == selectedItem)
                    this.comboCOMPorts.Text = port;
            }
        }

        /// <summary>
        /// Initializes a program memory read.
        /// </summary>
        private void ReadMemory()
        {
            //Debug.Assert(!this.workerThread.IsAlive);
            //Debug.Assert(this.programmer != null);
            //Debug.Assert(this.programmer.IsICSPMode);
            //
            // if we're already programming return
            //
            if (this.workerThread != null)
            {
                if (this.workerThread.IsAlive)
                    return;
                this.workerThread = null;
            }

            if (this.programmer == null)
                return;

            if (!this.programmer.IsICSPMode)
                return;
            //
            // if there's a program loaded then uload it.
            //
			this.ClearProgram();
            //
            // display message
            //
            this.txtOutput.AppendText(string.Format(Strings.ReadingProgramMemory, this.txtPMStart.Text, this.txtPMEnd.Text));
			//
			// start program read thread
			//
			this.workerThread = new Thread(new ThreadStart(this.ReadMemoryWorker));
			this.workerThread.Start();
			//
			// return
			//
			return;
        }

		/// <summary>
		/// Allocates the program memory array for the selected device..
		/// </summary>
		private void AllocateProgramArray()
		{
			//
			// calculate the size of the program memory array
			//
			uint totalMemory = (this.selectedDevice.ProgramMemroySize * 3); // +(this.selectedDevice.ConfigBytes / 3);
			//
			// allocate program memory array and the configuration 
			// memory array.
			//
			this.programMemory = new byte[totalMemory];
			this.configMemory = new byte[this.selectedDevice.ConfigBytes];
			//
			// initialize it to 1s (nopr)
			//
			for (uint i = 0; i < programMemory.Length; i++)
			{
				this.programMemory[i] = 0xFF;
			}
			//
			// initialize configuration memory to 1s
			//
			for (uint i = 0; i < this.configMemory.Length; i++)
			{
				this.configMemory[i] = 0xFF;
			}
		}

		/// <summary>
		/// Reads program memory in a background thread.
		/// </summary>
		private void ReadMemoryWorker()
        {
			//
			// delegates for messaging UI thread
			//
            AppendText appendText = new AppendText(this.txtOutput.AppendText);
			VoidMethod populateListView = new VoidMethod(this.LoadProgramWords);
			VoidMethod updateControls = new VoidMethod(this.UpdateCOMPortList);
			//
			// start stopwatch
			//
			this.stopwatch.Reset();
			this.stopwatch.Start();

			try
			{
				//
				// download the program from the device's memory
				//
				if (rbPartialProgramming.Checked == true)
				{
					uint start = uint.Parse(this.txtPMStart.Text, NumberStyles.HexNumber);
					uint end = uint.Parse(this.txtPMEnd.Text, NumberStyles.HexNumber);
					//
					// read programm memory range
					//
					byte[] readBytes = this.programmer.ReadProgramMemory(start, (end - start) / 2);
					//
					// allocate program memory array
					//
					this.AllocateProgramArray();
					//
					// copy read bytes to program memory array
					//
					Array.Copy(readBytes, 0, this.programMemory, (start / 2) * 3, (end - start) / 2);
				}
				else
				{
					//
					// read program memory
					//
					uint totalWords = this.selectedDevice.ProgramMemroySize + (this.selectedDevice.ConfigBytes / 3);
					this.programMemory = this.programmer.ReadProgramMemory(0x000000, totalWords);
				}
				//
				// stop timer
				//
				this.stopwatch.Stop();
				//
				// display success message
				//
				this.Invoke(appendText, Strings.Success);
				this.Invoke(appendText, string.Format(" ({0})", this.stopwatch.Elapsed.ToString()));
				this.Invoke(appendText, Environment.NewLine);
				//
				// populate listview
				//
				this.Invoke(populateListView);
			}
			catch (ThreadAbortException)
			{
				//this.Invoke(appendText, Strings.Aborted);
				//this.Invoke(appendText, Environment.NewLine);
			}
			catch (Exception ex)
			{
				this.stopwatch.Stop();
				//
				// display error message
				//
				this.Invoke(appendText, Strings.Failed);
				this.Invoke(appendText, Environment.NewLine);
				this.Invoke(appendText, ex.Message);
				this.Invoke(appendText, Environment.NewLine);
				MessageBox.Show(ex.Message, Strings.Error);
			}
			finally
			{
				this.Invoke(updateControls);
			}
        }

		private void UpdateControls()
		{
			if (this.programmer != null)
			{
				if (this.programmer.IsICSPMode == true)
				{
					this.mnuReadPM.Enabled = true;

					if (this.programMemory != null)
					{
						this.tbProgram.Enabled = true;
						this.tbVerify.Enabled = true;
					}
				}
			}
		}

		/// <summary>
		/// Erases the device memory.
		/// </summary>
		/// <returns></returns>
		private bool EraseDevice()
		{
			try
			{
				//
				// display message
				//
				this.txtOutput.AppendText(Strings.ErasingPart);
				//
				// erase part
				//
				this.programmer.EraseProgramMemory();
				//
				// display success message
				//
				this.txtOutput.AppendText(Strings.Success);
				this.txtOutput.AppendText(Environment.NewLine);
				//
				// return true
				//
				return true;
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex)
			{
				//
				// display error
				//
				this.txtOutput.AppendText(Strings.Failed);
				this.txtOutput.AppendText(Environment.NewLine);
				this.txtOutput.AppendText(ex.Message);
				this.txtOutput.AppendText(Environment.NewLine);
				//
				// return false
				//
				return false;
			}
		}

		/// <summary>
		/// Initialize a program memory write.
		/// </summary>
		private void WriteProgramMemory()
		{
			//
			// erase the device memory. if this fails return.
			//
			if (this.EraseDevice() == false)
				return;
			//
			// show programming message
			//
			this.txtOutput.AppendText(Strings.ProgrammingDevice);
			//
			// start program read thread
			//
			this.workerThread = new Thread(new ThreadStart(this.WriteProgramMemoryWorker));
			this.workerThread.Start();
		}

		/// <summary>
		/// Writes to program memory.
		/// </summary>
		private void WriteProgramMemoryWorker()
		{
			//
			// delegates for messaging UI thread
			//
			AppendText appendText = new AppendText(this.txtOutput.AppendText);
			//VoidMethod verify = new VoidMethod(this.Verify);
			VoidMethod updateControls = new VoidMethod(this.UpdateControls);
			//
			// start stopwatch
			//
			this.stopwatch.Reset();
			this.stopwatch.Start();

			try
			{
				//
				// program device
				//
				this.programmer.WriteProgramMemory(0x000000, this.programMemory, 0x000000);
				this.stopwatch.Stop();
				//
				// show completed message
				//
				this.Invoke(appendText, Strings.Success);
				this.Invoke(appendText, string.Format(" ({0})", this.stopwatch.Elapsed.ToString()));
				this.Invoke(appendText, Environment.NewLine);
                //
                // verify program memory
                //
                //this.VerifyWorker2(VerifyMode.VerifyProgram);
                //
				// restart stopwatch
				//
				this.stopwatch.Reset();
				this.stopwatch.Start();
				//
				// write configuration bits programming message
				//
				this.Invoke(appendText, Strings.ProgrammingConfigBits);
				//
				// write configuration bits
				//
				this.programmer.WriteProgramMemory(this.selectedDevice.ConfigAddress, this.configMemory, 0x000000);
				//
				// stop the stopwatch
				//
				this.stopwatch.Stop();
				//
				// display success message
				//
				this.Invoke(appendText, Strings.Success);
				this.Invoke(appendText, string.Format(" ({0})", this.stopwatch.Elapsed.ToString()));
				this.Invoke(appendText, Environment.NewLine);
				//
				// verify program memory
				//
                this.VerifyWorker2(VerifyMode.VerifyConfiguration);
			}
			catch (ThreadAbortException)
			{
				//this.Invoke(appendText, Strings.Aborted);
				//this.Invoke(appendText, Environment.NewLine);
			}
			catch (Exception ex)
			{
				//
				// display error message
				//
				this.Invoke(appendText, Strings.Failed);
				this.Invoke(appendText, Environment.NewLine);
				this.Invoke(appendText, ex.Message);
				this.Invoke(appendText, Environment.NewLine);
			}
			finally
			{
				this.Invoke(updateControls);
			}
		}

		/// <summary>
		/// Verifies program and configuration memory.
		/// </summary>
		private void Verify()
		{
			/*
			if (this.workerThread != null)
			{
				if (this.workerThread.IsAlive == true)
				{
					this.workerThread.Join(500);
					if (this.workerThread.IsAlive == true)
					{
						this.txtOutput.AppendText("Thread error.\r\n");
						return;
					}
				}
			}
			 */
			//
			// display message
			//
			//this.txtOutput.AppendText(Strings.VerifyingProgramMemory);
			//
			// start verify thread
			//
			this.workerThread = new Thread(new ThreadStart(this.VerifyWorker));
			this.workerThread.Name = "Verify Worker";
			this.workerThread.Start();
		}

        [Flags()]
        private enum VerifyMode
        {
            VerifyAll               = 0x03,
            VerifyProgram           = 0x01,
            VerifyConfiguration     = 0x02
        }

		/// <summary>
		/// Verify worker thread entry.
		/// </summary>
        private void VerifyWorker()
        {
            this.VerifyWorker2(VerifyMode.VerifyAll);
        }

		/// <summary>
		/// Verify worker thread.
		/// </summary>
		private void VerifyWorker2(VerifyMode mode)
		{
			//
			// delegates for messaging UI thread
			//
			AppendText appendText = new AppendText(this.txtOutput.AppendText);
			VoidMethod updateControls = new VoidMethod(this.UpdateControls);

			try
			{
                if ((mode & VerifyMode.VerifyProgram) == VerifyMode.VerifyProgram)
                {
                    //
                    // start stopwatch
                    //
                    this.stopwatch.Reset();
                    this.stopwatch.Start();
                    //
                    // display verifying message
                    //
                    this.Invoke(appendText, Strings.VerifyingProgramMemory);
                    //
                    // verify program memory
                    //
                    this.programmer.Verify(0x000000, this.programMemory, this.selectedDevice.ProgramMemroySize, 0x000000, false, this.selectedDevice);
                    //
                    // stop the stopwatch
                    //
                    this.stopwatch.Stop();
                    //
                    // display success message
                    //
                    this.Invoke(appendText, Strings.Success);
                    this.Invoke(appendText, string.Format(" ({0})", this.stopwatch.Elapsed.ToString()));
                    this.Invoke(appendText, Environment.NewLine);
                }

                if ((mode & VerifyMode.VerifyConfiguration) == VerifyMode.VerifyConfiguration)
                {
                    //
                    // restart the stopwatch
                    //
                    this.stopwatch.Reset();
                    this.stopwatch.Start();
                    //
                    // display message
                    //
                    this.Invoke(appendText, "Verifying configuration bits...");
                    //
                    // verify configuration memory
                    //
                    this.programmer.Verify(this.selectedDevice.ConfigAddress, this.configMemory, this.selectedDevice.ConfigBytes / 3, 0x000000, true, this.selectedDevice);
                    //
                    // stop the stopwatch
                    //
                    this.stopwatch.Stop();
                    //
                    // display success mmessage
                    //
                    this.Invoke(appendText, Strings.Success);
                    this.Invoke(appendText, string.Format(" ({0})", this.stopwatch.Elapsed.ToString()));
                    this.Invoke(appendText, Environment.NewLine);
                }
			}
			catch (ThreadAbortException)
			{
				//
				// display abort message
				//
				//this.Invoke(appendText, Strings.Aborted);
				//this.Invoke(appendText, Environment.NewLine);
			}
			catch (AddressException)
			{
				//
				// display error
				//
				this.Invoke(appendText, Strings.Failed);
				this.Invoke(appendText, Environment.NewLine);
				this.Invoke(appendText, Strings.AddressError);
				this.Invoke(appendText, Environment.NewLine);
			}
			catch (VerifyException)
			{
				//
				// display error
				//
				this.Invoke(appendText, Strings.Failed);
				this.Invoke(appendText, Environment.NewLine);
			}
			catch (Exception ex)
			{
				//
				// display error
				//
				this.Invoke(appendText, Strings.Failed);
				this.Invoke(appendText, Environment.NewLine);
				this.Invoke(appendText, ex.Message);
				this.Invoke(appendText, Environment.NewLine);
			}
			finally
			{
				//
				// stop the stopwatch
				//
				if (this.stopwatch.IsRunning)
				{
					this.stopwatch.Stop();
				}
				//
				// update controls
				//
				this.Invoke(updateControls);
			}
		}

		/// <summary>
		/// Exit ICSP and reset device.
		/// </summary>
		private void Reset()
		{
			try
			{
				//
				// release the device from reset
				//
				this.programmer.ExitICSP();
				//
				// display message
				//
				this.txtOutput.AppendText(Strings.TargetRunning);
				this.txtOutput.AppendText(Environment.NewLine);
				//
				// update controls
				//
				this.tbReset.Enabled = false;
				this.mnuReadPM.Enabled = false;
				this.tbProgram.Enabled = false;
				this.tbErase.Enabled = false;
				this.comboDevices.Enabled = true;
				this.comboDeviceFamilies.Enabled = true;
				this.tbCheckDevice.Enabled = true;
			}
			catch (Exception ex)
			{
				//
				// display error message
				//
				this.txtOutput.AppendText(ex.Message);
				this.txtOutput.AppendText(Environment.NewLine);
				MessageBox.Show(ex.Message, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
        /// Connects/disconnects programmer using selected COM port
        /// </summary>
        private void Connect()
        {
            if (this.comm.IsConnected)
            {
                try
                {
					//
					// close communications port
					//
                    this.comm.Close();
                }
                catch (CommException ex)
                {
					//
					// display error message
					//
                    this.txtOutput.AppendText(ex.Message);
                    this.txtOutput.AppendText(Environment.NewLine);
                }
                finally
                {
					//
					// update controls
					//
                    this.comboCOMPorts.Enabled = true;
                    this.tbCheckDevice.Enabled = false;
                    this.mnuConnect.Text = Strings.Connect;
					//
					// display message
					//
                    this.txtOutput.AppendText(Strings.Disconnected);
					this.txtOutput.AppendText(Environment.NewLine);
                }
            }
            else
            {
                try
                {
					//
					// open communications port
					//
                    this.comm.Open(this.comboCOMPorts.Text);
					//
					// update controls
					//
                    this.comboCOMPorts.Enabled = false;
                    this.tbCheckDevice.Enabled = true;
                    this.mnuConnect.Text = Strings.Disconnect;
					//
					// display message
					//
                    this.txtOutput.AppendText(Strings.Connected);
					this.txtOutput.AppendText(Environment.NewLine);
                }
                catch (CommException ex)
                {
					//
					// display error
					//
                    this.txtOutput.AppendText(ex.Message);
                    this.txtOutput.AppendText(Environment.NewLine);
                }
            }
        }

		/// <summary>
		/// Unloads the loaded program (hex file)
		/// </summary>
		private void ClearProgram()
		{
			if (this.programMemory != null)
			{
				this.lvPMem.Items.Clear();
				this.lastProgramWord = null;
				this.programMemory = null;
				this.programIndex = 0;
				this.tbProgram.Enabled = false;
				//
				// show message
				//
				this.txtOutput.AppendText(Strings.HEXFileUnloaded);
				this.txtOutput.AppendText(Environment.NewLine);
			}
		}

		private void tbVerify_Click(object sender, EventArgs e)
		{
			this.Verify();
		}
    }
}
