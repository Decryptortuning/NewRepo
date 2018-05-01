using System;
using System.Globalization;

namespace SerialPortRead
{
	public class Tools
	{
		public Tools()
		{
		}

		public static byte[] AppendByteArray(byte[] arrayone, byte[] arraytwo)
		{
			int length = 0;
			int num = 0;
			if (arrayone == null && arraytwo != null)
			{
				return arraytwo;
			}
			if (arrayone == null && arraytwo == null)
			{
				return null;
			}
			length = (int)arrayone.Length;
			byte[] numArray = new byte[length + (int)arraytwo.Length];
			byte[] numArray1 = arrayone;
			for (int i = 0; i < (int)numArray1.Length; i++)
			{
				numArray[num] = numArray1[i];
				num++;
			}
			num = 0;
			byte[] numArray2 = arraytwo;
			for (int j = 0; j < (int)numArray2.Length; j++)
			{
				numArray[length + num] = numArray2[j];
				num++;
			}
			return numArray;
		}

		public static string[] AppendSerialData(string[] serialbuffer, string[] serialin)
		{
			int length = 0;
			int num = 0;
			if (serialbuffer == null || serialin == null)
			{
				if (serialbuffer == null && serialin != null)
				{
					return serialin;
				}
				return null;
			}
			length = (int)serialbuffer.Length;
			string[] strArrays = new string[length + (int)serialin.Length];
			string[] strArrays1 = serialbuffer;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				strArrays[num] = strArrays1[i];
				num++;
			}
			num = 0;
			string[] strArrays2 = serialin;
			for (int j = 0; j < (int)strArrays2.Length; j++)
			{
				strArrays[length + num] = strArrays2[j];
				num++;
			}
			serialbuffer = strArrays;
			return serialbuffer;
		}

		public static string[] AppendStringArray(string[] arrayone, string[] arraytwo)
		{
			int length = 0;
			int num = 0;
			length = (int)arrayone.Length;
			string[] strArrays = new string[length + (int)arraytwo.Length];
			if (arrayone == null && arraytwo != null)
			{
				return arraytwo;
			}
			if (arrayone == null && arraytwo == null)
			{
				return null;
			}
			string[] strArrays1 = arrayone;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				strArrays[num] = strArrays1[i];
				num++;
			}
			num = 0;
			string[] strArrays2 = arraytwo;
			for (int j = 0; j < (int)strArrays2.Length; j++)
			{
				strArrays[length + num] = strArrays2[j];
				num++;
			}
			return strArrays;
		}

		public static int ByteToNumber(byte[] bytestring)
		{
			int num = 0;
			int num1 = 1;
			if (bytestring == null)
			{
				return -1;
			}
			int length = (int)bytestring.Length;
			if (length > 4)
			{
				return -1;
			}
			byte[] numArray = bytestring;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				byte num2 = numArray[i];
				int num3 = 8 * (length - num1);
				num = num + (Convert.ToByte(num2) << (num3 & 31));
				num1++;
			}
			return num;
		}

		public static string[] ConvertByteArray(byte[] datavalues)
		{
			int num;
			string[] strArrays;
			string[] strArrays1 = new string[(int)datavalues.Length];
			string[] strArrays2 = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
			string[] strArrays3 = strArrays2;
			int num1 = 0;
			byte[] numArray = datavalues;
			int num2 = 0;
		Label1:
			while (num2 < (int)numArray.Length)
			{
				byte num3 = numArray[num2];
				try
				{
					num = Convert.ToUInt16(num3);
					goto Label0;
				}
				catch (Exception exception)
				{
					strArrays = null;
				}
				return strArrays;
			}
			return strArrays1;
		Label0:
			string str = strArrays3[num >> 4];
			int num4 = num & 15;
			str = string.Concat(str, strArrays3[num4]);
			strArrays1[num1] = str;
			num1++;
			num2++;
			goto Label1;
		}

		public static byte[] ConvertString(string hexvalues, out bool errorflagged)
		{
			string[] strArrays = hexvalues.Split(new char[] { ' ' });
			int num = 0;
			errorflagged = false;
			byte[] numArray = new byte[(int)strArrays.Length];
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str = strArrays1[i];
				try
				{
					numArray[num] = byte.Parse(str, NumberStyles.HexNumber);
				}
				catch (Exception exception)
				{
					numArray[num] = 0;
					errorflagged = true;
				}
				num++;
			}
			return numArray;
		}

		public static byte[] ConvertStringArray(string[] hexvalues, out bool errorflagged)
		{
			int num = 0;
			errorflagged = false;
			byte[] numArray = new byte[(int)hexvalues.Length];
			string[] strArrays = hexvalues;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				try
				{
					numArray[num] = byte.Parse(str, NumberStyles.HexNumber);
				}
				catch (Exception exception)
				{
					numArray[num] = 0;
					errorflagged = true;
				}
				num++;
			}
			return numArray;
		}

		public static bool MatchSerialArray(string[] serialstring, string[] datastring, int position)
		{
			int num = 0;
			if (serialstring == null | datastring == null | position == -1)
			{
				return false;
			}
			if ((int)serialstring.Length - (position + (int)datastring.Length) < 0)
			{
				return false;
			}
			string[] strArrays = datastring;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (serialstring[num + position] != str)
				{
					return false;
				}
				num++;
			}
			return true;
		}

		public static byte[] NumberToByte(int number, int elements)
		{
			int num = 0;
			if (elements > 4 | elements < 0)
			{
				return null;
			}
			byte[] numArray = new byte[elements];
			while (num < elements)
			{
				int num1 = (elements - num - 1) * 8;
				int num2 = 255 << (num1 & 31);
				int num3 = (number & num2) >> (num1 & 31);
				numArray[num] = Convert.ToByte(num3 & 255);
				num++;
			}
			return numArray;
		}

		public static string[] NumberToString(int number, int elements)
		{
			int num = 0;
			if (elements > 4 | elements < 0)
			{
				return null;
			}
			byte[] numArray = new byte[elements];
			while (num < elements)
			{
				int num1 = (elements - num - 1) * 8;
				int num2 = 255 << (num1 & 31);
				int num3 = (number & num2) >> (num1 & 31);
				numArray[num] = Convert.ToByte(num3 & 255);
				num++;
			}
			return Tools.ConvertByteArray(numArray);
		}

		public static ushort RotateRight(int value, int count)
		{
			ushort num = (ushort)count;
			ushort num1 = (ushort)value;
			ushort num2 = 16;
			return (ushort)(num1 >> (num & 31) | num1 << (num2 - num & 31));
		}

		public static int SearchSerialArray(string[] serialstring, string[] headerstring)
		{
			int num = 0;
			int num1 = 0;
			int num2 = 0;
			int length = -1;
			string str = null;
			string str1 = null;
			if (serialstring == null | headerstring == null)
			{
				return -1;
			}
			int length1 = (int)serialstring.Length;
			int length2 = (int)headerstring.Length;
			num1 = length1 - (num + length2);
			while (num2 < length2)
			{
				str = string.Concat(str, headerstring[num2]);
				num2++;
			}
			num2 = 0;
			while (num1 >= 0)
			{
				if (serialstring[num] == headerstring[0])
				{
					while (num2 < length2)
					{
						str1 = string.Concat(str1, serialstring[num + num2]);
						num2++;
					}
					if (str1 == str)
					{
						length = num + (int)headerstring.Length;
						return length;
					}
					str1 = null;
					num2 = 0;
				}
				num++;
				num1 = length1 - (num + length2);
			}
			return length;
		}

		public static int StringToNumber(string[] numstring)
		{
			int num;
			int num1 = 0;
			int num2 = 0;
			if (numstring == null)
			{
				return -1;
			}
			int length = (int)numstring.Length;
			if (length > 4)
			{
				return -1;
			}
			try
			{
				while (num2 < length)
				{
					int num3 = (length - num2 - 1) * 8;
					int num4 = int.Parse(numstring[num2], NumberStyles.HexNumber);
					num1 = num1 + (num4 << (num3 & 31));
					num2++;
				}
				return num1;
			}
			catch (Exception exception)
			{
				num = -1;
			}
			return num;
		}
	}
}