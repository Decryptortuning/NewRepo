using System;

namespace SerialPortRead
{
	public class OBD
	{
		private static string[] AVT_interrupt;

		private static string[] AVT_id;

		private static string[] AVT_1xok;

		private static string[] AVT_1Xspeed;

		private static string[] mode27_header;

		private static string[] mode27_timedelay;

		private static string[] mode27_keypreamble;

		private static string[] mode27_ok;

		private static string[] mode27_invalidkey;

		private static string[] mode27_attempts;

		private static string[] mode34_header;

		private static string[] mode34_accept;

		private static string[] mode34_invmode;

		private static string[] mode34_invaddr;

		private static string[] mode34_invsize;

		private static string[] mode34_pcmerror;

		private static string[] mode36_header;

		private static string[] mode36_accept;

		private static string[] mode36E0_header;

		private static string[] mode36E0_flashready;

		private static string[] mode36E0_flashcsfail;

		private static string[] mode36E0_flashcspass;

		private static string[] mode36E0_flashrwfail;

		private static string[] mode36E0_flashlocked;

		private static string[] mode36E0_flashcycle;

		private static string[] mode36E0_flashcomplete;

		private static string[] mode36E0_rxtxoverflow;

		private static string[] mode36E0_rxtxerror;

		private static string[] mode36E0_pcmready;

		private static string[] mode36E1_header;

		private static string[] mode36E2_header;

		private static string[] modeA0_header;

		private static string[] modeA2_header;

		private static string[] modeA2_pcmerror;

		private static string[] modeA2_calerror;

		private static string[] modeA2_ramerror;

		private static string[] modeA2_booterror;

		private static string[] modeA2_cserror;

		private static string[] sendAVT_filter;

		private static string[] sendAVT_1X;

		private static string[] sendAVT_4X;

		private static string[] sendAVT_vpw;

		private static string[] sendAVT_id;

		private static string[] sendAVT_restart;

		private static string[] sendmode27_seed;

		private static string[] sendmode27_key;

		private static string[] sendmode34_request;

		private static string[] sendmode36_request;

		private static string[] sendmode36E0_ready;

		private static string[] sendmode36E1_data;

		private static string[] sendmode36E2_data;

		private static string[] sendmode36E3_lock;

		private static string[] sendmode36E3_unlock;

		private static string[] sendmode36E3_cs;

		private static string[] sendmode36E3_reset;

		private static string[] sendmode36E3_erasecal;

		private static string[] sendmode36E3_eraseos;

		private static string[] sendmodeA0_request;

		private static string[] sendmodeA0_begin;

		private static int uploadkernaladdr;

		static OBD()
		{
			string[] strArrays = new string[] { "23", "83", "00", "20" };
			OBD.AVT_interrupt = strArrays;
			OBD.AVT_id = new string[] { "93", "28" };
			OBD.AVT_1xok = new string[] { "91", "07" };
			OBD.AVT_1Xspeed = new string[] { "C1", "00" };
			string[] strArrays1 = new string[] { "6C", "F0", "10", "67" };
			OBD.mode27_header = strArrays1;
			OBD.mode27_timedelay = new string[] { "01", "37", "EOF" };
			OBD.mode27_keypreamble = new string[] { "01" };
			OBD.mode27_ok = new string[] { "02", "34" };
			OBD.mode27_invalidkey = new string[] { "02", "35" };
			OBD.mode27_attempts = new string[] { "02", "36" };
			string[] strArrays2 = new string[] { "6C", "F0", "10", "74", "00" };
			OBD.mode34_header = strArrays2;
			OBD.mode34_accept = new string[] { "44" };
			OBD.mode34_invmode = new string[] { "41" };
			OBD.mode34_invaddr = new string[] { "42" };
			OBD.mode34_invsize = new string[] { "43" };
			OBD.mode34_pcmerror = new string[] { "99" };
			string[] strArrays3 = new string[] { "6C", "F0", "10", "76", "00" };
			OBD.mode36_header = strArrays3;
			OBD.mode36_accept = new string[] { "78" };
			string[] strArrays4 = new string[] { "6C", "F0", "10", "76", "E0" };
			OBD.mode36E0_header = strArrays4;
			OBD.mode36E0_flashready = new string[] { "60" };
			OBD.mode36E0_flashcsfail = new string[] { "61" };
			OBD.mode36E0_flashcspass = new string[] { "62" };
			OBD.mode36E0_flashrwfail = new string[] { "63" };
			OBD.mode36E0_flashlocked = new string[] { "65" };
			OBD.mode36E0_flashcycle = new string[] { "66" };
			OBD.mode36E0_flashcomplete = new string[] { "68" };
			OBD.mode36E0_rxtxoverflow = new string[] { "71" };
			OBD.mode36E0_rxtxerror = new string[] { "77" };
			OBD.mode36E0_pcmready = new string[] { "80" };
			string[] strArrays5 = new string[] { "6C", "F0", "10", "76", "E1" };
			OBD.mode36E1_header = strArrays5;
			string[] strArrays6 = new string[] { "6C", "F0", "10", "76", "E2" };
			OBD.mode36E2_header = strArrays6;
			string[] strArrays7 = new string[] { "6C", "F0", "10", "E0", "AA" };
			OBD.modeA0_header = strArrays7;
			string[] strArrays8 = new string[] { "6C", "F0", "10", "A2" };
			OBD.modeA2_header = strArrays8;
			OBD.modeA2_pcmerror = new string[] { "00" };
			OBD.modeA2_calerror = new string[] { "01" };
			OBD.modeA2_ramerror = new string[] { "52" };
			OBD.modeA2_booterror = new string[] { "53" };
			OBD.modeA2_cserror = new string[] { "54" };
			OBD.sendAVT_filter = new string[] { "52", "5C", "10" };
			OBD.sendAVT_1X = new string[] { "C1", "00" };
			OBD.sendAVT_4X = new string[] { "C1", "01" };
			OBD.sendAVT_vpw = new string[] { "E1", "33" };
			OBD.sendAVT_id = new string[] { "F0" };
			OBD.sendAVT_restart = new string[] { "F1", "A5" };
			string[] strArrays9 = new string[] { "6C", "10", "F0", "27", "01" };
			OBD.sendmode27_seed = strArrays9;
			string[] strArrays10 = new string[] { "6C", "10", "F0", "27", "02" };
			OBD.sendmode27_key = strArrays10;
			string[] strArrays11 = new string[] { "6C", "10", "F0", "34", "00" };
			OBD.sendmode34_request = strArrays11;
			string[] strArrays12 = new string[] { "6C", "10", "F0", "36", "80" };
			OBD.sendmode36_request = strArrays12;
			string[] strArrays13 = new string[] { "6C", "10", "F0", "36", "E0" };
			OBD.sendmode36E0_ready = strArrays13;
			string[] strArrays14 = new string[] { "6C", "10", "F0", "36", "E1" };
			OBD.sendmode36E1_data = strArrays14;
			string[] strArrays15 = new string[] { "6C", "10", "F0", "36", "E2" };
			OBD.sendmode36E2_data = strArrays15;
			string[] strArrays16 = new string[] { "6C", "10", "F0", "36", "E3", "A0" };
			OBD.sendmode36E3_lock = strArrays16;
			string[] strArrays17 = new string[] { "6C", "10", "F0", "36", "E3", "A1" };
			OBD.sendmode36E3_unlock = strArrays17;
			string[] strArrays18 = new string[] { "6C", "10", "F0", "36", "E3", "A2" };
			OBD.sendmode36E3_cs = strArrays18;
			string[] strArrays19 = new string[] { "6C", "10", "F0", "36", "E3", "A3" };
			OBD.sendmode36E3_reset = strArrays19;
			string[] strArrays20 = new string[] { "6C", "10", "F0", "36", "E3", "A4" };
			OBD.sendmode36E3_erasecal = strArrays20;
			string[] strArrays21 = new string[] { "6C", "10", "F0", "36", "E3", "A5" };
			OBD.sendmode36E3_eraseos = strArrays21;
			string[] strArrays22 = new string[] { "6C", "FE", "F0", "A0" };
			OBD.sendmodeA0_request = strArrays22;
			string[] strArrays23 = new string[] { "6C", "FE", "F0", "A1" };
			OBD.sendmodeA0_begin = strArrays23;
			OBD.uploadkernaladdr = 16761600;
		}

		public OBD()
		{
		}

		public static byte[] AVTFilter()
		{
			bool flag = false;
			return Tools.ConvertStringArray(OBD.sendAVT_filter, out flag);
		}

		public static byte[] AVTHighSpeed()
		{
			bool flag = false;
			return Tools.ConvertStringArray(OBD.sendAVT_4X, out flag);
		}

		public static byte[] AVTID()
		{
			bool flag = false;
			return Tools.ConvertStringArray(OBD.sendAVT_id, out flag);
		}

		public static byte[] AVTNormalSpeed()
		{
			bool flag = false;
			return Tools.ConvertStringArray(OBD.sendAVT_1X, out flag);
		}

		public static byte[] AVTRestart()
		{
			bool flag = false;
			return Tools.ConvertStringArray(OBD.sendAVT_restart, out flag);
		}

		public static byte[] AVTVPW()
		{
			bool flag = false;
			return Tools.ConvertStringArray(OBD.sendAVT_vpw, out flag);
		}

		public static bool CallPCM(string[] serialdata, out byte[] messageframe, out string message)
		{
			bool flag;
			message = null;
			messageframe = null;
			int num = Tools.SearchSerialArray(serialdata, OBD.mode36E0_header);
			if (num != -1 && Tools.MatchSerialArray(serialdata, OBD.mode36E0_pcmready, num))
			{
				message = "In flash mode";
				return true;
			}
			messageframe = Tools.ConvertStringArray(OBD.sendmode36E0_ready, out flag);
			return false;
		}

		public static bool CheckAVTExit(string[] serialdata)
		{
			if (Tools.SearchSerialArray(serialdata, OBD.AVT_1Xspeed) != -1 | Tools.SearchSerialArray(serialdata, OBD.AVT_interrupt) != -1)
			{
				return true;
			}
			return false;
		}

		public static bool CheckForError(string[] serialdata, out string message, out bool fatalerror)
		{
			fatalerror = false;
			int num = Tools.SearchSerialArray(serialdata, OBD.modeA2_header);
			if (num == -1)
			{
				message = null;
				return false;
			}
			if (Tools.MatchSerialArray(serialdata, OBD.modeA2_calerror, num) | Tools.MatchSerialArray(serialdata, OBD.modeA2_cserror, num) | Tools.MatchSerialArray(serialdata, OBD.modeA2_pcmerror, num))
			{
				message = "Error - Flash fault detected, attempting recovery";
				return true;
			}
			if (Tools.MatchSerialArray(serialdata, OBD.modeA2_booterror, num) | Tools.MatchSerialArray(serialdata, OBD.modeA2_ramerror, num))
			{
				message = "FATAL ERROR - BOOT/RAM fault detected";
				fatalerror = true;
				return true;
			}
			message = "Error - Unknown PCM error present";
			fatalerror = true;
			return true;
		}

		public static bool CheckForFlashComplete(string[] serialdata, out string message)
		{
			message = null;
			int num = Tools.SearchSerialArray(serialdata, OBD.mode36E0_header);
			if (num == -1 || !Tools.MatchSerialArray(serialdata, OBD.mode36E0_flashcomplete, num))
			{
				return false;
			}
			message = "PCM reflash complete";
			return true;
		}

		public static bool CheckForFlashError(string[] serialdata, out string message)
		{
			message = null;
			int num = Tools.SearchSerialArray(serialdata, OBD.mode36E0_header);
			if (num == -1 || !(Tools.MatchSerialArray(serialdata, OBD.mode36E0_flashrwfail, num) | Tools.MatchSerialArray(serialdata, OBD.mode36E0_flashlocked, num) | Tools.MatchSerialArray(serialdata, OBD.mode36E0_flashcycle, num)))
			{
				return false;
			}
			message = "Error - Flash program failure";
			return true;
		}

		public static bool FindAVT1X(string[] serialdata, out string message)
		{
			if (Tools.SearchSerialArray(serialdata, OBD.AVT_1xok) != -1)
			{
				message = "In 1X VPW mode";
				return true;
			}
			message = null;
			return false;
		}

		public static byte[] FlashEraseCal()
		{
			bool flag;
			return Tools.ConvertStringArray(OBD.sendmode36E3_erasecal, out flag);
		}

		public static byte[] FlashEraseOS()
		{
			bool flag;
			return Tools.ConvertStringArray(OBD.sendmode36E3_eraseos, out flag);
		}

		public static bool GetAVTID(string[] serialdata, out string message, out bool error)
		{
			bool flag;
			string[] strArrays = new string[] { "00", "00" };
			int num = Tools.SearchSerialArray(serialdata, OBD.AVT_id);
			if (num == -1)
			{
				error = false;
				message = null;
				return false;
			}
			try
			{
				strArrays[0] = serialdata[num];
				strArrays[1] = serialdata[num + 1];
				if ((strArrays[0] == "EOF") | (strArrays[1] == "EOF"))
				{
					message = "Error - AVT ID not present";
					error = true;
					return true;
				}
				message = "AVT-";
				message = string.Concat(message, strArrays[0], strArrays[1]);
				message = string.Concat(message, " present");
				error = false;
				return true;
			}
			catch (Exception exception)
			{
				message = "Error - could not get AVT ID";
				error = true;
				flag = true;
			}
			return flag;
		}

		public static byte[] GetMode3436Frame(byte[] uploadblock, string mode)
		{
			byte[] numArray = null;
			bool flag = false;
			int length = (int)uploadblock.Length;
			length = length & 65535;
			if (mode != "34")
			{
				if (mode != "36")
				{
					return null;
				}
				numArray = Tools.ConvertStringArray(OBD.sendmode36_request, out flag);
			}
			else
			{
				numArray = Tools.ConvertStringArray(OBD.sendmode34_request, out flag);
			}
			numArray = Tools.AppendByteArray(numArray, Tools.NumberToByte(length, 2));
			numArray = Tools.AppendByteArray(numArray, Tools.NumberToByte(OBD.uploadkernaladdr, 3));
			if (mode == "36")
			{
				numArray = Tools.AppendByteArray(numArray, uploadblock);
				byte[] checkSum = BinFile.GetCheckSum(4, numArray);
				numArray = Tools.AppendByteArray(numArray, checkSum);
			}
			return numArray;
		}

		public static bool GetMode3436Response(string[] serialdata, out byte[] messageframe, out string message)
		{
			bool flag;
			message = null;
			messageframe = null;
			int num = Tools.SearchSerialArray(serialdata, OBD.mode34_header);
			if (num != -1)
			{
				if (Tools.MatchSerialArray(serialdata, OBD.mode34_invaddr, num))
				{
					message = "Error - Invalid upload address";
					return true;
				}
				if (Tools.MatchSerialArray(serialdata, OBD.mode34_invmode, num))
				{
					message = "Error - Invalid upload mode";
					return true;
				}
				if (Tools.MatchSerialArray(serialdata, OBD.mode34_invsize, num))
				{
					message = "Error - Invalid upload size";
					return true;
				}
				if (Tools.MatchSerialArray(serialdata, OBD.mode34_pcmerror, num))
				{
					message = "Error - PCM fault detected";
					messageframe = Tools.ConvertStringArray(OBD.sendmodeA0_request, out flag);
					return true;
				}
				if (Tools.MatchSerialArray(serialdata, OBD.mode34_accept, num))
				{
					message = "Uploading flash interface...";
					messageframe = Tools.ConvertStringArray(OBD.sendmodeA0_request, out flag);
					return true;
				}
			}
			return false;
		}

		public static bool GetModeA0Response(string[] serialdata, out byte[] messageframe, out string message)
		{
			bool flag;
			message = null;
			messageframe = null;
			if (Tools.SearchSerialArray(serialdata, OBD.modeA0_header) == -1)
			{
				return false;
			}
			message = "Starting 4X mode";
			messageframe = Tools.ConvertStringArray(OBD.sendmodeA0_begin, out flag);
			return true;
		}

		public static bool GetSecurityFrameCase15(string[] serialdata, out byte[] messageframe, out string message, out bool PCMerror)
		{
			bool flag;
			bool flag1;
			string[] strArrays = new string[2];
			string[] str = new string[2];
			int number = -1;
			message = null;
			messageframe = null;
			PCMerror = false;
			int num = Tools.SearchSerialArray(serialdata, OBD.mode27_header);
			if (num != -1)
			{
				if (Tools.MatchSerialArray(serialdata, OBD.mode27_timedelay, num))
				{
					message = "Error - Time delay has not expired, wait 30 seconds...";
					return true;
				}
				if (Tools.MatchSerialArray(serialdata, OBD.mode27_keypreamble, num))
				{
					num++;
					try
					{
						strArrays[1] = serialdata[num];
						strArrays[0] = serialdata[num + 1];
						number = Tools.StringToNumber(strArrays);
						if (number == 0)
						{
							PCMerror = true;
							message = "Skipping security sequence...";
						}
						else if (number != -1)
						{
							number = number + 2835;
							number = number & 65535;
							int num1 = Tools.RotateRight(number, 4);
							str = Tools.NumberToString(num1, 2);
							string[] strArrays1 = Tools.AppendSerialData(OBD.sendmode27_key, str);
							message = "Sending security key...";
							messageframe = Tools.ConvertStringArray(strArrays1, out flag);
						}
						else if (number == -1)
						{
							message = "Error - Could not get security seed";
						}
						return true;
					}
					catch (Exception exception)
					{
						message = "Error - Could not get security seed";
						flag1 = true;
					}
					return flag1;
				}
			}
			return false;
		}

		public static bool GetSecurityResponse(string[] serialdata, out bool securityok, out string message)
		{
			message = null;
			securityok = false;
			int num = Tools.SearchSerialArray(serialdata, OBD.mode27_header);
			if (num != -1)
			{
				if (Tools.MatchSerialArray(serialdata, OBD.mode27_attempts, num))
				{
					message = "Error - Number of security attempts exceeded";
					return true;
				}
				if (Tools.MatchSerialArray(serialdata, OBD.mode27_invalidkey, num))
				{
					message = "Error - Invalid security key";
					return true;
				}
				if (Tools.MatchSerialArray(serialdata, OBD.mode27_ok, num))
				{
					message = "Security key accepted";
					securityok = true;
					return true;
				}
			}
			return false;
		}

		public static byte[] LockFlash()
		{
			bool flag;
			return Tools.ConvertStringArray(OBD.sendmode36E3_lock, out flag);
		}

		public static bool ModeE1UpdateBIN(string[] serialdata, byte[] oldbin, out byte[] newbin, out bool datavalid)
		{
			int num = -1;
			int num1 = -1;
			int frameChecksum = -1;
			int number = -1;
			byte[] frameData = null;
			datavalid = false;
			newbin = null;
			int num2 = Tools.SearchSerialArray(serialdata, OBD.mode36E1_header);
			if (num2 == -1)
			{
				newbin = oldbin;
				return false;
			}
			if (BinFile.GetFrameAddr(num2, serialdata, out num, out num1))
			{
				num2 = num2 + 5;
				frameData = BinFile.GetFrameData(num2, num, serialdata);
				num2 = num2 + num;
				frameChecksum = BinFile.GetFrameChecksum(num2, serialdata);
				number = Tools.ByteToNumber(BinFile.GetCheckSum(0, frameData));
			}
			if (frameChecksum == -1 | number == -1 | num == -1 | num1 == -1)
			{
				newbin = oldbin;
				return true;
			}
			if (frameChecksum != number)
			{
				newbin = oldbin;
				return true;
			}
			datavalid = true;
			newbin = BinFile.UpdateBIN(oldbin, frameData, num, num1);
			return true;
		}

		public static bool ModeE2UploadBIN(string[] serialdata, byte[] bin, out byte[] messageframe, out int currentaddr)
		{
			bool flag;
			int num = -1;
			int num1 = -1;
			currentaddr = -1;
			messageframe = null;
			int num2 = Tools.SearchSerialArray(serialdata, OBD.mode36E2_header);
			if (num2 == -1)
			{
				return false;
			}
			if (BinFile.GetFrameAddr(num2, serialdata, out num, out num1))
			{
				byte[] numArray = BinFile.MakeFrameData(num, num1, bin);
				if (numArray != null)
				{
					messageframe = Tools.ConvertStringArray(OBD.sendmode36E2_data, out flag);
					messageframe = Tools.AppendByteArray(messageframe, Tools.NumberToByte(num, 2));
					messageframe = Tools.AppendByteArray(messageframe, Tools.NumberToByte(num1, 3));
					messageframe = Tools.AppendByteArray(messageframe, numArray);
					messageframe = Tools.AppendByteArray(messageframe, BinFile.GetCheckSum(0, numArray));
					currentaddr = num1;
				}
			}
			return true;
		}

		public static byte[] RequestModeE1Data(byte[] bindata, int maxmessagesize, int lastbinaddr)
		{
			bool flag;
			int num = 0;
			int length = 0;
			byte[] numArray = null;
			numArray = Tools.ConvertStringArray(OBD.sendmode36E1_data, out flag);
			if (bindata == null)
			{
				num = maxmessagesize;
			}
			else
			{
				num = lastbinaddr - (int)bindata.Length;
				if (num <= 0)
				{
					return null;
				}
				if (num > maxmessagesize)
				{
					num = maxmessagesize;
				}
				length = (int)bindata.Length;
			}
			numArray = Tools.AppendByteArray(numArray, Tools.NumberToByte(num, 2));
			numArray = Tools.AppendByteArray(numArray, Tools.NumberToByte(length, 3));
			return numArray;
		}

		public static byte[] RequestSecuritySeed()
		{
			bool flag;
			return Tools.ConvertStringArray(OBD.sendmode27_seed, out flag);
		}

		public static byte[] ResetPCM()
		{
			bool flag;
			return Tools.ConvertStringArray(OBD.sendmode36E3_reset, out flag);
		}

		public static byte[] UnlockFlash()
		{
			bool flag;
			return Tools.ConvertStringArray(OBD.sendmode36E3_unlock, out flag);
		}
	}
}