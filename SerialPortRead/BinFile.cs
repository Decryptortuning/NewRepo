using System;
using System.Globalization;

namespace SerialPortRead
{
	internal class BinFile
	{
		public BinFile()
		{
		}

		public static byte[] GetCheckSum(int startpos, byte[] data)
		{
			int num = 0;
			int i = 0;
			if (startpos < 0 | data == null)
			{
				return null;
			}
			for (i = startpos; i < (int)data.Length; i++)
			{
				num = num + Convert.ToInt32(data[i]);
			}
			num = num & 65535;
			return Tools.NumberToByte(num, 2);
		}

		public static bool GetFrameAddr(int startpos, string[] serialdata, out int size, out int addr)
		{
			bool flag;
			string[] strArrays = new string[2];
			string[] strArrays1 = new string[3];
			size = -1;
			addr = -1;
			if (startpos < 0 | serialdata == null)
			{
				return false;
			}
			int num = startpos;
			try
			{
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					strArrays[i] = serialdata[num];
					num++;
				}
				for (int j = 0; j < (int)strArrays1.Length; j++)
				{
					strArrays1[j] = serialdata[num];
					num++;
				}
				size = Tools.StringToNumber(strArrays);
				addr = Tools.StringToNumber(strArrays1);
				if (!(size == -1 | addr == -1))
				{
					return true;
				}
				size = -1;
				addr = -1;
				return false;
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		public static int GetFrameChecksum(int startpos, string[] serialdata)
		{
			int num;
			int number = -1;
			string[] strArrays = new string[2];
			if (startpos < 0 | serialdata == null)
			{
				return -1;
			}
			try
			{
				strArrays[0] = serialdata[startpos];
				strArrays[1] = serialdata[startpos + 1];
				number = Tools.StringToNumber(strArrays);
				return number;
			}
			catch (Exception exception)
			{
				num = -1;
			}
			return num;
		}

		public static byte[] GetFrameData(int startpos, int size, string[] serialdata)
		{
			byte[] numArray;
			int num = 0;
			if (startpos < 0 | size <= 0 | serialdata == null)
			{
				return null;
			}
			byte[] numArray1 = new byte[size];
			try
			{
				while (num < size)
				{
					numArray1[num] = byte.Parse(serialdata[startpos + num], NumberStyles.HexNumber);
					num++;
				}
				return numArray1;
			}
			catch (Exception exception)
			{
				numArray = null;
			}
			return numArray;
		}

		public static byte[] MakeFrameData(int size, int address, byte[] bindata)
		{
			byte[] numArray;
			int num = 0;
			try
			{
				byte[] numArray1 = new byte[size];
				while (num < size)
				{
					numArray1[num] = bindata[address + num];
					num++;
				}
				return numArray1;
			}
			catch (Exception exception)
			{
				numArray = null;
			}
			return numArray;
		}

		public static byte[] UpdateBIN(byte[] oldbin, byte[] data, int size, int address)
		{
			if ((int)data.Length != size)
			{
				return oldbin;
			}
			if (oldbin == null && address == 0)
			{
				return data;
			}
			if (oldbin == null)
			{
				return oldbin;
			}
			if ((int)oldbin.Length != address)
			{
				return oldbin;
			}
			return Tools.AppendByteArray(oldbin, data);
		}
	}
}