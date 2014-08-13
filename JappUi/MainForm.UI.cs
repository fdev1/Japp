using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Japp;
using System.Windows.Forms;
using System.ComponentModel;

namespace JappUI
{
	partial class MainForm
	{
		private void OpenHEX()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "HEX Files (*.hex)|*.hex";
			ofd.Title = "Select HEX file...";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				this.txtHEXFile.Text = ofd.FileName;
				this.LoadHEXFile();

			}
		}


		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.workerThread != null)
			{
				if (this.workerThread.IsAlive)
				{
					DialogResult r =
						MessageBox.Show(Strings.AbortOpInProgress, Strings.Abort, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (r == DialogResult.Yes)
					{
						this.workerThread.Abort();
						this.workerThread.Join();
					}
					else
					{
						e.Cancel = true;
					}
				}
			}
		}

		private void mnuConnect_Click(object sender, EventArgs e)
		{
			this.Connect();
		}

		private void mnuExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void mnuDiagnostics_Click(object sender, EventArgs e)
		{
			DiagnosticsForm diagnostics = new DiagnosticsForm(this.comm);
			diagnostics.ShowDialog();
		}

		private void mnuAbout_Click(object sender, EventArgs e)
		{
			using (AboutBox about = new AboutBox())
				about.ShowDialog(this);
		}

		private void txtOutput_TextChanged(object sender, EventArgs e)
		{
			this.tabControl1.SelectTab(this.tabOutput);
		}

		private void btnBrowseHEX_Click(object sender, EventArgs e)
		{
			this.OpenHEX();
		}

		private void mnuOpenHEX_Click(object sender, EventArgs e)
		{
			this.OpenHEX();
		}

		private void mnuCheckDevice_Click(object sender, EventArgs e)
		{
			this.EnterICSPMode();
		}

		private void tbOpenHEX_Click(object sender, EventArgs e)
		{
			this.OpenHEX();
		}

		private void tbCheckDevice_Click(object sender, EventArgs e)
		{
			this.EnterICSPMode();
		}

		private void comboCOMPorts_DropDown(object sender, EventArgs e)
		{
			this.UpdateCOMPortList();
		}

		private void comboDeviceFamilies_DropDown(object sender, EventArgs e)
		{
			this.UpdateDeviceFamiliesList();
		}

		private void comboDevices_DropDown(object sender, EventArgs e)
		{
			this.UpdateDevicesList();
		}

		private void mnuClearOutput_Click(object sender, EventArgs e)
		{
			this.txtOutput.Text = string.Empty;
		}

		private void mnuReadPM_Click(object sender, EventArgs e)
		{
			this.ReadMemory();
		}

		private void lvPMem_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.lastProgramWord != null)
			{
				foreach (ListViewItem item in this.lvPMem.SelectedItems)
				{
					if (item == this.lastProgramWord)
					{
						this.LoadProgramWords();
					}
				}
			}
		}

		private void comboDevices_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (DeviceFamily family in this.deviceConfig.Families)
			{
				foreach (Device device in family.Devices)
				{
					if (device.Name == this.comboDevices.Text)
					{
						if (this.rbFullProgramming.Checked == true)
						{
							this.txtPMStart.Text = "000000";
							this.txtPMEnd.Text = (device.ProgramMemroySize * 2).ToString("X6");
						}
						this.selectedDevice = device;
						this.InstantiateProgrammer();
						this.btnBrowseHEX.Enabled = true;
						this.mnuOpenHEX.Enabled = true;
						this.tbOpenHEX.Enabled = true;
						//this.mnuReadPM.Enabled = true;
						this.ClearProgram();
						return;
					}
				}
			}
			if (this.rbFullProgramming.Checked == true)
			{
				this.txtPMStart.Text = "000000";
				this.txtPMEnd.Text = "000000";
			}
			this.selectedDevice = null;
			this.programmer = null;
			this.btnBrowseHEX.Enabled = false;
			this.tbOpenHEX.Enabled = false;
			this.mnuOpenHEX.Enabled = false;
			this.mnuReadPM.Enabled = false;
			this.ClearProgram();
		}

		private void mnuClear_Click(object sender, EventArgs e)
		{
			this.ClearProgram();
		}

		private void tbProgram_Click(object sender, EventArgs e)
		{
			this.WriteProgramMemory();
		}

		private void tbErase_Click(object sender, EventArgs e)
		{
			this.EraseDevice();
		}

		private void tbReset_Click(object sender, EventArgs e)
		{
			this.Reset();
		}

		private void rbFullProgramming_CheckedChanged(object sender, EventArgs e)
		{
			this.txtPMStart.ReadOnly = this.rbFullProgramming.Checked;
			this.txtPMEnd.ReadOnly = this.rbFullProgramming.Checked;
		}
	}
}
