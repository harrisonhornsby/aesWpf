using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DESWF
{
	internal static class ConversionService
	{
		public static string TextToBinaryString(string data)
		{
			StringBuilder sb = new StringBuilder();

			foreach (char c in data.ToCharArray())
			{
				sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
			}
			return sb.ToString();
		}

		public static byte[,] ConvertByteArrayToFourByFourMatrix(this byte[] input)
		{
			var matrix = new byte[4,4];
			var extractedRow = new byte[4];
			for (int row = 0, pos = 0; row < 4; row++, pos+=4)
			{
				extractedRow = input.Skip(pos).Take(4).ToArray();
				for (int col = 0; col < 4; col++)
				{
					matrix[row, col] = extractedRow[col];
				}
			}
			return matrix;
		}

		public static string BinaryStringToText(string data)
		{
			List<byte> byteList = new List<byte>();

			for (int i = 0; i < data.Length; i += 8)
			{
				byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
			}
			return Encoding.ASCII.GetString(byteList.ToArray());
		}

		public static int BitArrayToInt(BitArray bitArray)
		{
			if (bitArray.Length > 32)
				throw new ArgumentException("Argument length shall be at most 32 bits.");

			int[] array = new int[1];
			bitArray.CopyTo(array, 0);
			return array[0];
		}

		public static List<BitArray> ConvertMessageBitArrayToListOfBitBlocks(int blockSizeInBits, BitArray message)
		{
			var listOfBitBlocks = new List<BitArray>();
			listOfBitBlocks.Capacity = (message.Length % blockSizeInBits != 0)
				? (message.Count / blockSizeInBits) + 1
				: message.Count / blockSizeInBits;
			for (int i = 0; i < listOfBitBlocks.Capacity; i++)
			{
				var temp = new BitArray(blockSizeInBits);
				for (int j = i * blockSizeInBits, k = 0; j < (i * blockSizeInBits) + blockSizeInBits && j < message.Count; j++, k++)
				{
					temp[k] = message[j];
				}
				listOfBitBlocks.Add(temp);
			}
			return listOfBitBlocks;
		}

		public static List<Byte[]> ConvertMessageBitArrayToListOfByteBlocks(int blockSizeInBytes, Byte[] message)
		{
			var listOfBitBlocks = new List<Byte[]>();
			listOfBitBlocks.Capacity = (message.Length % blockSizeInBytes != 0)
				? (message.Length / blockSizeInBytes) + 1
				: message.Length / blockSizeInBytes;
			for (int i = 0; i < listOfBitBlocks.Capacity; i++)
			{
				var temp = new byte[blockSizeInBytes];
				for (int j = i * blockSizeInBytes, k = 0; j < (i * blockSizeInBytes) + blockSizeInBytes && j < message.Length; j++, k++)
				{
					temp[k] = message[j];
				}
				listOfBitBlocks.Add(temp);
			}
			return listOfBitBlocks;
		}

		public static BitArray StringOfBinaryLikeCharactersToBitArray(string binaryString)
		{
			BitArray cOutput = new BitArray(binaryString.Length);
			var num = binaryString.Length;
			var cOutputCounter = 0;
			foreach (var letter in binaryString)
			{
				if (letter == '0')
				{
					cOutput[cOutputCounter] = false;
				}
				else
				{
					cOutput[cOutputCounter] = true;
				}

				cOutputCounter++;
			}
			return cOutput;
		}

		public static byte[] ConvertBitArrayToByteArray(this BitArray bits)
		{
			int numBytes = bits.Count / 8;
			if (bits.Count % 8 != 0) numBytes++;

			byte[] bytes = new byte[numBytes];
			int byteIndex = 0, bitIndex = 0;

			for (int i = 0; i < bits.Count; i++)
			{
				if (bits[i])
					bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

				bitIndex++;
				if (bitIndex == 8)
				{
					bitIndex = 0;
					byteIndex++;
				}
			}

			return bytes;
		}

		/*public static byte[] BitArrayToByteArray(BitArray bits)
		{
			byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
			bits.CopyTo(ret, 0);
			return ret;
		}*/

		public static string BinaryStringToHexString(string binary)
		{
			StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

			int mod4Len = binary.Length % 8;
			if (mod4Len != 0)
			{
				// pad to length multiple of 8
				binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
			}

			for (int i = 0; i < binary.Length; i += 8)
			{
				string eightBits = binary.Substring(i, 8);
				result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
			}

			return result.ToString();
		}

		public static BitArray ByteArrayToBitArrayInProperOrder(byte[] input)
		{
			BitArray array = new BitArray(input);
			int length = array.Length;
			int mid = (length / 2);

			for (int i = 0; i < mid; i++)
			{
				bool bit = array[i];
				array[i] = array[length - i - 1];
				array[length - i - 1] = bit;
			}
			return array;
		}

		public static string BitArrayToString(BitArray bitArray, bool? splitBytes = false)
		{
			var count = 0;
			string output = "";
			foreach (var bit in bitArray)
			{
				output += bit.Equals(true) ? "1" : "0";
				count++;
				if (count == 8 && splitBytes == true)
				{
					output += "  ";
					count = 0;
				}
			}
			return output;
		}
	}
}