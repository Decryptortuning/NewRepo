using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Threading;

namespace SerialPortRead
{
	public partial class MainWindow : Window
	{
		private static SerialPort _serialPort;

		public System.Timers.Timer breakresponse;

		public System.Timers.Timer autorespond;

		public string searchforframe = "ECHO";

		public string[] rxserialbuffer;

		public byte[] DLBin;

		public static int calstart;

		public static int calend;

		public static int flashend;

		public byte[] ULBin = new byte[MainWindow.flashend];

		static MainWindow()
		{
			MainWindow.calstart = 32768;
			MainWindow.calend = 131072;
			MainWindow.flashend = 524288;
		}

		public MainWindow()
		{
			this.InitializeComponent();
			this.ComPorts.DropDownOpened += new EventHandler(this.ComPorts_DropDownOpened);
			this.ComPorts.DropDownClosed += new EventHandler(this.ComPorts_DropDownClosed);
			this.breakresponse = new System.Timers.Timer(60000)
			{
				Enabled = true
			};
			this.breakresponse.Stop();
			this.breakresponse.Elapsed += new ElapsedEventHandler(this.BreakOBDResponse);
			this.autorespond = new System.Timers.Timer(4000)
			{
				Enabled = true
			};
			this.autorespond.Stop();
			this.autorespond.Elapsed += new ElapsedEventHandler(this.OBDAutoResponse);
			this.resetoptions();
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			(new SerialPortRead.About()).ShowDialog();
		}

		private void AdvancedOff()
		{
			this.SendCommand.IsEnabled = false;
			this.SetAVT.IsEnabled = false;
			this.ResumeFlash.IsEnabled = false;
			this.AVTAdvanceLabel.Opacity = 0;
			this.SendCommandLabel.Opacity = 0;
			this.Configstring.Opacity = 0;
			this.Commandstring.Opacity = 0;
			this.SetAVT.Opacity = 0;
			this.SendCommand.Opacity = 0;
			this.ResumeFlash.Opacity = 0;
			this.FlashOS.Opacity = 0;
			this.Escape.Opacity = 0;
			this.Configstring.IsEnabled = false;
			this.Commandstring.IsEnabled = false;
			this.FlashOS.IsEnabled = false;
			this.Escape.IsEnabled = false;
		}

		private void BreakOBDReponseMessage()
		{
			this.breakresponse.Stop();
			this.autorespond.Stop();
			if (this.searchforframe != "ECHO" && this.searchforframe != "PCM_READY")
			{
				if (this.searchforframe == "1X_OK")
				{
					this.consolemessage("Error - Could not open OBD port");
					this.searchforframe = "ECHO";
					return;
				}
				if (this.searchforframe == "CALL_PCM")
				{
					this.consolemessage("Error - Entry into flash mode failed");
					MessageBox.Show("Entry into flash mode failed, reset and try again", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
					this.searchforframe = "ECHO";
					return;
				}
				if (this.searchforframe == "DOWNLOAD")
				{
					this.consolemessage("Error - BIN download failed");
					MessageBox.Show("Failed to download BIN from PCM, reset and try again", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
					this.resetdownload();
					this.searchforframe = "CALL_PCM";
					this.autorespond.Start();
					return;
				}
				if (this.searchforframe == "FLASH_UPLOAD")
				{
					this.consolemessage("Error - Reflash cycle failed");
					MessageBox.Show("Reflash cycle failed, no response from PCM", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
					this.resetupload();
					this.searchforframe = "CALL_PCM";
					this.autorespond.Start();
					return;
				}
				this.consolemessage(string.Concat("Error, no response from PCM for ", this.searchforframe));
				this.searchforframe = "ECHO";
			}
		}

		private void BreakOBDResponse(object sender, ElapsedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new MainWindow.BreakOBDREsponseDelegate(this.BreakOBDReponseMessage));
		}

		private void ClearConsole_Click(object sender, RoutedEventArgs e)
		{
			this.Serialout.Clear();
		}

		private void ComPorts_DropDownClosed(object sender, EventArgs e)
		{
			this.Portinuse.Text = this.ComPorts.Text;
		}

		private void ComPorts_DropDownOpened(object sender, EventArgs e)
		{
			this.ComPorts.Items.Clear();
			string[] portNames = SerialPort.GetPortNames();
			for (int i = 0; i < (int)portNames.Length; i++)
			{
				string str = portNames[i];
				this.ComPorts.Items.Add(str);
			}
		}

		public void Connect_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(this.ComPorts.Text))
			{
				this.Portinuse.Text = "No port selected";
				return;
			}
			MainWindow._serialPort = new SerialPort()
			{
				PortName = this.ComPorts.Text,
				BaudRate = 57600,
				Parity = (Parity)Enum.Parse(typeof(Parity), "None"),
				DataBits = 8,
				StopBits = (StopBits)Enum.Parse(typeof(StopBits), "1"),
				Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None"),
				ReadBufferSize = 8192,
				WriteBufferSize = 8192,
				ReceivedBytesThreshold = 2,
				ReadTimeout = 500,
				WriteTimeout = 500
			};
			MainWindow._serialPort.DataReceived += new SerialDataReceivedEventHandler(this.SerialPortHasData);
			MainWindow._serialPort.Open();
			this.ComPorts.IsEnabled = false;
			this.Connect.IsEnabled = false;
			this.Disconnect.IsEnabled = true;
			this.UserProgStart.IsEnabled = true;
			this.startAVT();
		}

		private void consolemessage(string message)
		{
			if (message == null)
			{
				return;
			}
			TextBox serialout = this.Serialout;
			serialout.Text = string.Concat(serialout.Text, "System Message: ");
			TextBox textBox = this.Serialout;
			textBox.Text = string.Concat(textBox.Text, message);
			TextBox serialout1 = this.Serialout;
			serialout1.Text = string.Concat(serialout1.Text, "\r\n");
			bool? isChecked = this.AutoScroll.IsChecked;
			if ((!isChecked.GetValueOrDefault() ? false : isChecked.HasValue))
			{
				this.Serialout.ScrollToEnd();
			}
		}

		public void Disconnect_Click(object sender, RoutedEventArgs e)
		{
			this.breakresponse.Stop();
			this.autorespond.Stop();
			if (!this.UserProgStart.IsEnabled)
			{
				this.WriteSerialData(OBD.ResetPCM(), false);
				Thread.Sleep(100);
			}
			this.WriteSerialData(OBD.AVTRestart(), true);
			MainWindow._serialPort.Close();
			this.resetoptions();
			this.consolemessage("Serial port closed");
		}

		private void echotoconsole(string rxtx, byte[] hexvaluestring)
		{
			TextBox serialout = this.Serialout;
			serialout.Text = string.Concat(serialout.Text, rxtx);
			TextBox textBox = this.Serialout;
			textBox.Text = string.Concat(textBox.Text, BitConverter.ToString(hexvaluestring).Replace("-", " "));
			TextBox serialout1 = this.Serialout;
			serialout1.Text = string.Concat(serialout1.Text, "\r\n");
			bool? isChecked = this.AutoScroll.IsChecked;
			if ((!isChecked.GetValueOrDefault() ? false : isChecked.HasValue))
			{
				this.Serialout.ScrollToEnd();
			}
		}

		private void EnableAdvance_Click(object sender, RoutedEventArgs e)
		{
			if (!this.SendCommand.IsEnabled && this.Disconnect.IsEnabled)
			{
				this.SendCommand.IsEnabled = true;
				this.SetAVT.IsEnabled = true;
				this.ResumeFlash.IsEnabled = true;
				this.AVTAdvanceLabel.Opacity = 1;
				this.SendCommandLabel.Opacity = 1;
				this.Configstring.Opacity = 1;
				this.Commandstring.Opacity = 1;
				this.SetAVT.Opacity = 1;
				this.SendCommand.Opacity = 1;
				this.ResumeFlash.Opacity = 1;
				this.FlashOS.Opacity = 1;
				this.Escape.Opacity = 1;
				this.Configstring.IsEnabled = true;
				this.Commandstring.IsEnabled = true;
				this.FlashOS.IsEnabled = true;
				this.Escape.IsEnabled = true;
				return;
			}
			this.SendCommand.IsEnabled = false;
			this.SetAVT.IsEnabled = false;
			this.ResumeFlash.IsEnabled = false;
			this.AVTAdvanceLabel.Opacity = 0;
			this.SendCommandLabel.Opacity = 0;
			this.Configstring.Opacity = 0;
			this.Commandstring.Opacity = 0;
			this.SetAVT.Opacity = 0;
			this.SendCommand.Opacity = 0;
			this.ResumeFlash.Opacity = 0;
			this.FlashOS.Opacity = 0;
			this.Escape.Opacity = 0;
			this.Configstring.IsEnabled = false;
			this.Commandstring.IsEnabled = false;
			this.FlashOS.IsEnabled = false;
			this.Escape.IsEnabled = false;
		}

		private void Escape_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Abort operation, are you sure?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
			{
				this.consolemessage("OPERATION ABORTED");
				this.resetdownload();
				this.resetupload();
				this.searchforframe = "CALL_PCM";
				this.autorespond.Start();
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			(new SerialPortRead.Help()).ShowDialog();
		}

		private void OBDAutoResponse(object sender, ElapsedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new MainWindow.OBDAutoResponseDelegate(this.OBDAutoResponseMessage));
		}

		private void OBDAutoResponseMessage()
		{
			if (this.searchforframe != "ECHO")
			{
				this.OBDresponsehandler(null);
			}
		}

		private void OBDresponsehandler(string[] serialdata)
		{
			string str;
			byte[] numArray;
			byte[] numArray1;
			int num = 2048;
			int num1 = 0;
			bool flag = false;
			bool flag1 = false;
			bool flag2 = false;
			bool flag3 = false;
			MainWindow._serialPort.DataReceived -= new SerialDataReceivedEventHandler(this.SerialPortHasData);
			if (this.searchforframe == "ID_AVT" && serialdata != null && OBD.GetAVTID(serialdata, out str, out flag1))
			{
				this.WriteSerialData(OBD.AVTVPW(), true);
				this.consolemessage(str);
				if (!flag1)
				{
					this.searchforframe = "1X_OK";
				}
			}
			if (this.searchforframe == "1X_OK" && serialdata != null && OBD.FindAVT1X(serialdata, out str))
			{
				this.consolemessage(str);
				this.searchforframe = "ECHO";
			}
			if (this.searchforframe == "SECURITY_SEED")
			{
				if (OBD.GetSecurityFrameCase15(serialdata, out numArray, out str, out flag2))
				{
					this.consolemessage(str);
					if (flag2)
					{
						this.searchforframe = "MODE34_OK";
					}
					else if (!flag2 && numArray != null)
					{
						this.WriteSerialData(numArray, false);
						this.searchforframe = "SECURITY_OK";
					}
					else if (!flag2 && numArray == null)
					{
						this.searchforframe = "ECHO";
					}
				}
				if (OBD.CheckForError(serialdata, out str, out flag3))
				{
					this.consolemessage(str);
					if (flag3)
					{
						MessageBox.Show("PCM internal hardware failure detected, reflash not possible", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
						this.searchforframe = "ECHO";
					}
					else
					{
						this.searchforframe = "MODE34_OK";
					}
				}
				if (serialdata == null)
				{
					this.WriteSerialData(OBD.RequestSecuritySeed(), false);
				}
				this.rxserialbuffer = null;
			}
			if (this.searchforframe == "SECURITY_OK" && serialdata != null && OBD.GetSecurityResponse(serialdata, out flag, out str))
			{
				this.consolemessage(str);
				if (!flag)
				{
					this.rxserialbuffer = null;
					this.searchforframe = "ECHO";
				}
				else
				{
					numArray1 = this.readflashkernal();
					if (numArray1 == null)
					{
						this.searchforframe = "ECHO";
					}
					else
					{
						this.WriteSerialData(OBD.GetMode3436Frame(numArray1, "34"), false);
						this.searchforframe = "MODE34_OK";
					}
				}
			}
			if (this.searchforframe == "MODE34_OK" && serialdata != null)
			{
				if (OBD.GetMode3436Response(serialdata, out numArray, out str))
				{
					this.consolemessage(str);
					if (numArray == null)
					{
						this.searchforframe = "ECHO";
					}
					else
					{
						this.WriteSerialData(numArray, false);
						this.searchforframe = "4X_OK";
					}
					this.rxserialbuffer = null;
				}
				if (OBD.CheckForError(serialdata, out str, out flag3))
				{
					this.consolemessage(str);
					if (flag3)
					{
						this.searchforframe = "ECHO";
					}
					else
					{
						numArray1 = this.readflashkernal();
						this.WriteSerialData(OBD.GetMode3436Frame(numArray1, "34"), false);
					}
					this.rxserialbuffer = null;
				}
			}
			if (this.searchforframe == "4X_OK" && serialdata != null && OBD.GetModeA0Response(serialdata, out numArray, out str))
			{
				this.WriteSerialData(numArray, false);
				this.WriteSerialData(OBD.AVTHighSpeed(), true);
				Thread.Sleep(100);
				numArray1 = this.readflashkernal();
				this.WriteSerialData(OBD.GetMode3436Frame(numArray1, "36"), false);
				this.WriteSerialData(OBD.AVTFilter(), true);
				Thread.Sleep(150);
				this.rxserialbuffer = null;
				this.searchforframe = "CALL_PCM";
			}
			if (this.searchforframe == "CALL_PCM")
			{
				if (!OBD.CallPCM(serialdata, out numArray, out str))
				{
					this.WriteSerialData(numArray, false);
				}
				else
				{
					this.breakresponse.Stop();
					this.UserProgStart.IsEnabled = false;
					this.Disconnect.IsEnabled = true;
					this.consolemessage(str);
					this.searchforframe = "PCM_READY";
					this.WriteSerialData(OBD.LockFlash(), false);
					this.rxserialbuffer = null;
				}
			}
			if (this.searchforframe == "DOWNLOAD")
			{
				num = 2048;
				if (OBD.ModeE1UpdateBIN(serialdata, this.DLBin, out this.DLBin, out flag))
				{
					this.breakresponse.Stop();
					if (this.DLBin != null)
					{
						this.FlashProgressBar.Value = (double)((int)this.DLBin.Length);
						if ((int)this.DLBin.Length >= MainWindow.flashend)
						{
							this.consolemessage("BIN download complete");
							this.saveflashbin();
							this.resetdownload();
							this.searchforframe = "CALL_PCM";
						}
					}
				}
				num = (!flag ? 128 : 2048);
				numArray = OBD.RequestModeE1Data(this.DLBin, num, MainWindow.flashend);
				this.WriteSerialData(numArray, false);
				this.rxserialbuffer = null;
			}
			if (this.searchforframe == "FLASH_UPLOAD" && serialdata != null)
			{
				if (OBD.CheckForFlashError(serialdata, out str))
				{
					this.consolemessage(str);
					MessageBox.Show("Flash program failed", "Flash Error", MessageBoxButton.OK, MessageBoxImage.Hand);
					this.searchforframe = "CALL_PCM";
				}
				if (OBD.ModeE2UploadBIN(serialdata, this.ULBin, out numArray, out num1))
				{
					this.breakresponse.Stop();
					if (numArray != null && num1 != -1)
					{
						this.FlashProgressBar.Value = (double)num1;
						this.WriteSerialData(numArray, false);
					}
				}
				if (OBD.CheckForFlashComplete(serialdata, out str))
				{
					this.WriteSerialData(OBD.LockFlash(), false);
					this.consolemessage(str);
					this.resetupload();
					this.searchforframe = "CALL_PCM";
				}
				this.rxserialbuffer = null;
			}
			if (this.searchforframe == "FLASH_ERASE")
			{
				this.breakresponse.Stop();
				this.WriteSerialData(OBD.UnlockFlash(), false);
				Thread.Sleep(1500);
				bool? isChecked = this.FlashOS.IsChecked;
				if ((!isChecked.GetValueOrDefault() ? true : !isChecked.HasValue))
				{
					this.FlashProgressBar.Minimum = (double)MainWindow.calstart;
					this.FlashProgressBar.Maximum = (double)MainWindow.calend;
					this.WriteSerialData(OBD.FlashEraseCal(), false);
				}
				else
				{
					this.FlashProgressBar.Minimum = (double)MainWindow.calend;
					this.FlashProgressBar.Maximum = (double)((int)this.ULBin.Length);
					this.WriteSerialData(OBD.FlashEraseOS(), false);
				}
				this.searchforframe = "FLASH_UPLOAD";
				this.rxserialbuffer = null;
			}
			if (this.searchforframe == "VERIFY_EXIT" && serialdata != null)
			{
				if (OBD.CheckForError(serialdata, out str, out flag3))
				{
					if (flag3)
					{
						MessageBox.Show("FATAL ERROR - PCM BOOT or RAM fault", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
					}
					else
					{
						MessageBox.Show("Reflash failed, enter Flash Mode and repeat flash cycle", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
					}
					this.consolemessage(str);
					this.rxserialbuffer = null;
					this.searchforframe = "ECHO";
				}
				else if (!OBD.CheckAVTExit(serialdata))
				{
					this.consolemessage("PCM reset...");
					this.searchforframe = "ECHO";
				}
				if (OBD.CheckAVTExit(serialdata))
				{
					this.rxserialbuffer = null;
				}
				this.UserProgStart.IsEnabled = true;
			}
			if (this.searchforframe != "ECHO")
			{
				this.autorespond.Start();
			}
			MainWindow._serialPort.DataReceived += new SerialDataReceivedEventHandler(this.SerialPortHasData);
		}

		private bool openflashbin()
		{
			bool flag;
			byte[] numArray = new byte[1];
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				DefaultExt = "bin",
				Filter = "Binary Files | *.bin"
			};
			bool? nullable = openFileDialog.ShowDialog();
			if ((!nullable.GetValueOrDefault() ? true : !nullable.HasValue))
			{
				this.consolemessage("Load BIN file aborted");
				return false;
			}
			try
			{
				numArray = File.ReadAllBytes(openFileDialog.FileName);
				if ((int)numArray.Length != MainWindow.flashend)
				{
					MessageBox.Show("BIN length incorrect", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
					this.consolemessage("Supported BIN size is 512K");
					return false;
				}
				this.ULBin = numArray;
				this.consolemessage("Reflashing PCM, please wait...");
				this.AdvancedOff();
				this.Disconnect.IsEnabled = false;
				this.searchforframe = "FLASH_ERASE";
				this.OBDresponsehandler(null);
				return true;
			}
			catch (ArgumentException argumentException)
			{
				MessageBox.Show("Invalid file path", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				flag = false;
			}
			catch (PathTooLongException pathTooLongException)
			{
				MessageBox.Show("File path is too long", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				flag = false;
			}
			catch (DirectoryNotFoundException directoryNotFoundException)
			{
				MessageBox.Show("Invalid directory", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				flag = false;
			}
			catch (IOException oException)
			{
				MessageBox.Show("Error accessing media", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				flag = false;
			}
			catch (UnauthorizedAccessException unauthorizedAccessException)
			{
				MessageBox.Show("No permission to read file", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				flag = false;
			}
			return flag;
		}

		private void OpenFlashBin_Click(object sender, RoutedEventArgs e)
		{
			if (this.searchforframe != "PCM_READY")
			{
				this.consolemessage("Flash Error - PCM not ready/not in flash mode");
				return;
			}
			if (MessageBox.Show("Load BIN and flash to PCM?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
			{
				this.openflashbin();
				return;
			}
			this.consolemessage("Flash operation cancelled");
		}

		private byte[] readflashkernal()
		{
			byte[] numArray = null;
			BinaryReader binaryReader = new BinaryReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SerialPortRead.FLASH.BIN"));
			try
			{
				numArray = binaryReader.ReadBytes(2942);
			}
			catch (Exception exception)
			{
				MessageBox.Show("Error reading embedded flash interface", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			return numArray;
		}

		private byte[] ReadSerialData(out bool readerror)
		{
			int num;
			byte[] numArray;
			readerror = false;
			int bytesToRead = 0;
			int num1 = 1;
			int num2 = 0;
			bytesToRead = MainWindow._serialPort.BytesToRead;
			if (bytesToRead == 0)
			{
				numArray = new byte[1];
				numArray[num2] = Convert.ToByte(0);
				readerror = true;
				return numArray;
			}
			if (bytesToRead > 8192)
			{
				num = 8192;
				num1 = 8192;
			}
			else
			{
				num = bytesToRead;
				num1 = bytesToRead;
			}
			numArray = new byte[num1];
			while (num > 0)
			{
				try
				{
					numArray[num2] = Convert.ToByte(MainWindow._serialPort.ReadByte());
				}
				catch (Exception exception)
				{
					numArray[num2] = Convert.ToByte(0);
					readerror = true;
				}
				num2++;
				num--;
			}
			return numArray;
		}

		public void ReceiveSerialData()
		{
			string[] strArrays = new string[] { "EOF" };
			bool flag = false;
			if (MainWindow._serialPort.IsOpen)
			{
				byte[] numArray = this.ReadSerialData(out flag);
				if (!flag)
				{
					this.autorespond.Stop();
					if (this.searchforframe == "ECHO")
					{
						this.rxserialbuffer = null;
						this.echotoconsole("Rx: ", numArray);
					}
					else
					{
						this.rxserialbuffer = Tools.AppendSerialData(this.rxserialbuffer, Tools.ConvertByteArray(numArray));
						this.rxserialbuffer = Tools.AppendSerialData(this.rxserialbuffer, strArrays);
						this.breakresponse.Start();
						this.OBDresponsehandler(this.rxserialbuffer);
						if (this.searchforframe == "PCM_READY")
						{
							this.rxserialbuffer = null;
							return;
						}
					}
				}
			}
		}

		private void resetdownload()
		{
			this.FlashProgressBar.Minimum = 0;
			this.FlashProgressBar.Value = 0;
			this.Disconnect.IsEnabled = true;
		}

		private void resetoptions()
		{
			this.ComPorts.Items.Clear();
			string[] portNames = SerialPort.GetPortNames();
			for (int i = 0; i < (int)portNames.Length; i++)
			{
				string str = portNames[i];
				this.ComPorts.Items.Add(str);
			}
			this.Portinuse.Text = null;
			this.ComPorts.IsEnabled = true;
			this.Connect.IsEnabled = true;
			this.Disconnect.IsEnabled = false;
			this.SendCommand.IsEnabled = false;
			this.SetAVT.IsEnabled = false;
			this.UserProgStart.IsEnabled = false;
			this.ResumeFlash.IsEnabled = false;
			this.AVTAdvanceLabel.Opacity = 0;
			this.SendCommandLabel.Opacity = 0;
			this.Configstring.Opacity = 0;
			this.Commandstring.Opacity = 0;
			this.SetAVT.Opacity = 0;
			this.SendCommand.Opacity = 0;
			this.ResumeFlash.Opacity = 0;
			this.FlashOS.Opacity = 0;
			this.Escape.Opacity = 0;
			this.Configstring.IsEnabled = false;
			this.Commandstring.IsEnabled = false;
			this.FlashOS.IsEnabled = false;
			this.Escape.IsEnabled = false;
		}

		private void ResetPCM_Click(object sender, RoutedEventArgs e)
		{
			if (this.searchforframe != "PCM_READY")
			{
				this.consolemessage("PCM busy/not in flash mode");
				return;
			}
			if (MessageBox.Show("Exit flash mode and reset, are you sure?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) != MessageBoxResult.Yes)
			{
				this.consolemessage("Reset cancelled");
				return;
			}
			this.rxserialbuffer = null;
			this.WriteSerialData(OBD.ResetPCM(), false);
			Thread.Sleep(100);
			this.WriteSerialData(OBD.AVTNormalSpeed(), true);
			this.Serialout.Clear();
			this.searchforframe = "VERIFY_EXIT";
		}

		private void resetupload()
		{
			this.FlashProgressBar.Minimum = 0;
			this.FlashProgressBar.Value = 0;
			this.Disconnect.IsEnabled = true;
			this.EnableAdvance.IsEnabled = true;
			this.FlashOS.IsChecked = new bool?(false);
		}

		private void ResumeFlash_Click(object sender, RoutedEventArgs e)
		{
			this.searchforframe = "CALL_PCM";
			this.autorespond.Start();
		}

		private void saveflashbin()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				DefaultExt = "bin",
				Filter = "Binary Files | *.bin",
				OverwritePrompt = true
			};
			bool? nullable = saveFileDialog.ShowDialog();
			if ((!nullable.GetValueOrDefault() ? true : !nullable.HasValue))
			{
				this.consolemessage("BIN save aborted");
			}
			else
			{
				try
				{
					File.WriteAllBytes(saveFileDialog.FileName.ToString(), this.DLBin);
				}
				catch (ArgumentException argumentException)
				{
					MessageBox.Show("Invalid file path", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
				catch (PathTooLongException pathTooLongException)
				{
					MessageBox.Show("File path is too long", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
				catch (DirectoryNotFoundException directoryNotFoundException)
				{
					MessageBox.Show("Invalid directory", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
				catch (IOException oException)
				{
					MessageBox.Show("Error accessing media", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
				catch (UnauthorizedAccessException unauthorizedAccessException)
				{
					MessageBox.Show("No permission to write file (is file read only?)", "File Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
			}
		}

		private void SavefromFlash_Click(object sender, RoutedEventArgs e)
		{
			if (this.searchforframe != "PCM_READY")
			{
				this.consolemessage("Download Error - PCM not ready/not in flash mode");
				return;
			}
			this.DLBin = null;
			this.FlashProgressBar.Maximum = (double)MainWindow.flashend;
			this.Disconnect.IsEnabled = false;
			this.consolemessage("Downloading BIN from flash, please wait...");
			this.searchforframe = "DOWNLOAD";
			this.autorespond.Start();
		}

		public void SendCommand_Click(object sender, RoutedEventArgs e)
		{
			bool flag;
			if (string.IsNullOrEmpty(this.Commandstring.Text))
			{
				MessageBox.Show("No command entered", "Null Value");
				return;
			}
			string text = this.Commandstring.Text;
			byte[] numArray = Tools.ConvertString(text, out flag);
			if (flag)
			{
				MessageBox.Show("Error translating user command", "Error");
			}
			if (!this.WriteSerialData(numArray, false))
			{
				MessageBox.Show("Command Tx error", "Tx error");
			}
			else
			{
				this.echotoconsole("Tx: ", numArray);
			}
			this.searchforframe = "ECHO";
		}

		private void SerialPortHasData(object sender, SerialDataReceivedEventArgs e)
		{
			if (e.EventType == SerialData.Eof)
			{
				Thread.Sleep(300);
				return;
			}
			Thread.Sleep(75);
			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new MainWindow.ReadSerialDataDelegate(this.ReceiveSerialData));
		}

		private void SetAVT_Click(object sender, RoutedEventArgs e)
		{
			bool flag;
			if (string.IsNullOrEmpty(this.Configstring.Text))
			{
				MessageBox.Show("No command entered", "Null Value");
				return;
			}
			string text = this.Configstring.Text;
			if (text == "E1 33" || text == "e1 33")
			{
				this.searchforframe = "1X_OK";
			}
			byte[] numArray = Tools.ConvertString(text, out flag);
			if (flag)
			{
				MessageBox.Show("Error translating user command", "error");
			}
			if (!this.WriteSerialData(numArray, true))
			{
				MessageBox.Show("Command Tx error", "Tx error");
			}
			else
			{
				this.echotoconsole("Tx: ", numArray);
			}
			this.searchforframe = "ECHO";
		}

		public void startAVT()
		{
			this.WriteSerialData(OBD.AVTRestart(), true);
			Thread.Sleep(200);
			this.WriteSerialData(OBD.AVTID(), true);
			this.searchforframe = "ID_AVT";
			this.consolemessage("Connecting to AVT, please wait...");
		}

		private void UserProgStart_Click(object sender, RoutedEventArgs e)
		{
			this.consolemessage("Entering flash mode, please wait...");
			this.WriteSerialData(OBD.RequestSecuritySeed(), false);
			this.searchforframe = "SECURITY_SEED";
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (!this.Connect.IsEnabled)
			{
				this.consolemessage("Must disconnect from PCM before closing window");
				e.Cancel = true;
				return;
			}
			MessageBoxResult messageBoxResult = MessageBox.Show("Quit Program?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
			e.Cancel = messageBoxResult == MessageBoxResult.No;
		}

		private bool WriteSerialData(byte[] serialdata, bool AVTConfig)
		{
			bool flag;
			int num = 0;
			int num1 = 0;
			byte[] numArray = new byte[2];
			if (serialdata == null)
			{
				return false;
			}
			int length = (int)serialdata.Length;
			byte[] numArray1 = new byte[length];
			if (length >= 8192)
			{
				MessageBox.Show("Buffer overflow", "Tx Error");
				return false;
			}
			if (length >= 12)
			{
				numArray1 = new byte[length + 3];
				numArray1[0] = 18;
				numArray = Tools.NumberToByte(length, 2);
				numArray1[1] = numArray[0];
				numArray1[2] = numArray[1];
				length = length + 3;
				num1 = 3;
			}
			if (length < 12 && !AVTConfig)
			{
				numArray1 = new byte[length + 1];
				numArray1[0] = Convert.ToByte(length);
				length++;
				num1 = 1;
			}
			while (num1 < length)
			{
				numArray1[num1] = serialdata[num];
				num++;
				num1++;
			}
			try
			{
				MainWindow._serialPort.Write(numArray1, 0, length);
				return true;
			}
			catch (ArgumentNullException argumentNullException)
			{
				MessageBox.Show("Buffer out of range", "Tx Error");
				flag = false;
			}
			catch (InvalidOperationException invalidOperationException)
			{
				MessageBox.Show("Serial port not open", "Tx Error");
				flag = false;
			}
			catch (TimeoutException timeoutException)
			{
				MessageBox.Show("Serial port has timed out", "Tx Error");
				flag = false;
			}
			return flag;
		}

		public delegate void BreakOBDREsponseDelegate();

		public delegate void OBDAutoResponseDelegate();

		public delegate void ReadSerialDataDelegate();
	}
}