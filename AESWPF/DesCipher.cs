using DES;

///

using System;
using System.Collections;
using System.Collections.Generic;

namespace DESWF
{
	internal class DesCipher
	{
		#region Permutation tables

		public static readonly int[] IpValues =
		{
			58,50,42,34,26,18,10,2,60,52,44,36,28,20,12,4,
			62,54,46,38,30,22,14,6,64,56,48,40,32,24,16,8,
			57,49,41,33,25,17,9,1,59,51,43,35,27,19,11,3,
			61,53,45,37,29,21,13,5,63,55,47,39,31,23,15,7
		};

		public static readonly int[] InverseIPValues =
		{
			40,8,48,16,56,24,64,32,39,7,47,15,55,23,63,31,
			38,6,46,14,54,22,62,30,37,5,45,13,53,21,61,29,
			36,4,44,12,52,20,60,28,35,3,43,11,51,19,59,27,
			34,2,42,10,50,18,58,26,33,1,41,9,49,17,57,25
		};

		public static readonly int[] ExpansionValues =
		{
			32,1,2,3,4,5,4,5,6,7,8,9,8,9,10,11,12,13,12,13,14,15,16,17,16,17,18,19,20,21,20,21,22,23,24,25,24,25,26,27,28,29,28,29,30,31,32,1 //32nd moves to [0], 1st moves to[1] etc
		};

		public static readonly int[] NumberBitsShiftPerRound =
		{
			1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
		};

		public static readonly int[] ParityBitPermutation =
		{
			57,49,41,33,25,17,9

			,1,58,50,42,34,26,18

			,10,2,59,51,43,35,27

			,19,11,3,60,52,44,36

			,63,55,47,39,31,23,15

			,7,62,54,46,38,30,22

			,14,6,61,53,45,37,29

			,21,13,5,28,20,12,4
		};

		public static readonly int[] ShrinkKeyToFourtyEightBits =
		{
			14,17,11,24,1,5

				,3,28,15,6,21,10

				,23,19,12,4,26,8

				,16,7,27,20,13,2

				,41,52,31,37,47,55

				,30,40,51,45,33,48

				,44,49,39,56,34,53

				,46,42,50,36,29,32
		};

		public static readonly int[] COutputPermutation =
		{
			16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10,
			2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25
		};

		public List<int[,]> SboxList = new List<int[,]>
		{
			new int[,]
			{
				{14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7},
				{0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8},
				{4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0},
				{15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13}
			},
			new int[,]
			{
				{15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},
				{3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
				{0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},
				{13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9}
			},
			new int[,]
			{
				{10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
				{13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
				{13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7},
				{1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12}
			},
			new int[,]
			{
				{7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15},
				{13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9},
				{10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4},
				{3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14}
			},
			new int[,]
			{
				{2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9},
				{14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},
				{4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},
				{11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3}
			},
			new int[,]
			{
				{12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11},
				{10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8},
				{9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6},
				{4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13}
			},
			new int[,]
			{
				{4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1},
				{13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6},
				{1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2},
				{6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12}
			},
			new int[,]
			{
				{13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7},
				{1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2},
				{7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8},
				{2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}
			}
			};

		#endregion Permutation tables

		private BitArray _left32Bits = new BitArray(32);
		public Plaintext Message = new Plaintext();
		public BitArray MessageAfterInitialPermutation = new BitArray(64);
		private BitArray _right32Bits = new BitArray(32);
		private List<BitArray> _listOfBitBlocks;
		private BitArray _originalKey;
		private readonly List<BitArray> _roundKeyList = new List<BitArray>();
		private BitArray _keyAfterDroppedParityBits;
		public List<BitArray> EncryptedBlockList = new List<BitArray>();
		public string CipherTextString;

		public static BitArray ApplyInitialPermutation(BitArray messageBeforeIp)
		{
			var messageAfterIp = new BitArray(messageBeforeIp.Count);
			for (var i = 0; i < messageBeforeIp.Count; i++)
			{
				messageAfterIp[i] = messageBeforeIp[IpValues[i] - 1];
			}
			return messageAfterIp;
		}

		public static BitArray ApplyFinalPermutation(BitArray messageBeforeFinal)
		{
			var messageAfterIp = new BitArray(messageBeforeFinal.Count);
			for (var i = 0; i < messageBeforeFinal.Count; i++)
			{
				messageAfterIp[i] = messageBeforeFinal[InverseIPValues[i] - 1];
			}
			return messageAfterIp;
		}

		/// <summary>
		/// Apply parity bit permutation. Takes 64 bit key down to 56 bits for use within key scheduler.
		/// </summary>
		/// <param name="originalKey"></param>
		/// <returns></returns>
		public static BitArray GetKeyAfterParityBitPermutation(BitArray originalKey)
		{
			var keyAfterParityPermutation = new BitArray(56);
			for (int i = 0; i < 56; i++)
			{
				keyAfterParityPermutation[i] = originalKey[ParityBitPermutation[i]];
			}
			return keyAfterParityPermutation;
		}

		public static BitArray GetShrunkenKey(BitArray bitShiftedKey)
		{
			var keyAfterShrinking = new BitArray(48);
			for (int i = 0; i < 48; i++)
			{
				keyAfterShrinking[i] = bitShiftedKey[ShrinkKeyToFourtyEightBits[i] - 1];
			}
			return keyAfterShrinking;
		}

		public void CreateRoundKeyList()
		{
			//Drop parity bits then use that result in the below for loop, key should be 56 bits when entering for loop
			_keyAfterDroppedParityBits = GetKeyAfterParityBitPermutation(_originalKey);
			for (int i = 0; i < 16; i++)
			{
				//Bit shift the shortened key from before loop entry
				//shrink output of above to 48 bits then store in round key list
				var bitShiftedKey = ShiftBits(NumberBitsShiftPerRound[i], _keyAfterDroppedParityBits);
				var shrunkenKey = GetShrunkenKey(bitShiftedKey);
				_roundKeyList.Add(shrunkenKey);
			}
		}

		/// <summary>
		/// Pass in the number of bits to shift as well as the original key. Original key is bit shifted to generate a round key.
		/// </summary>
		/// <param name="shiftSize"></param>
		/// <param name="originalKey"></param>
		/// <returns></returns>
		public BitArray ShiftBits(int shiftSize, BitArray originalKey)
		{
			var arraySize = originalKey.Count;
			for (int i = 0; i < arraySize; i++)
			{
				if (i - shiftSize < 0)
				{
					originalKey[originalKey.Count - 1] = originalKey[i];
				}
				else originalKey[i - shiftSize] = originalKey[i];
			}
			var newRoundKey = originalKey;
			return newRoundKey;
		}

		/// <summary>
		/// Accepts 8 bytes and converts to a bit array of 64 bits.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>

		public void DecryptWithDes()
		{
			//TODO: Pass in cipher text, update a input property, display input in text area of win form
		}

		/// <summary>
		/// Method creates a byte[] from input string.
		/// Extracts 64 bits at a time until Message.ValueInString is totally encrypted.
		/// Will set cipher text.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="encrypt"></param>
		/// <param name="key"></param>
		public void EncryptWithDes(BitArray input, bool encrypt, BitArray key)
		{
			_originalKey = key;
			Message.ValueInString = ConversionService.BitArrayToString(input);
			Message.ValueInBits = input;//Message.Encoded bit array stores all bits from plain text conversion
			ConvertMessageBitArrayToListOfBitBlocks();

			CreateRoundKeyList();

			//encrypt each 64 bit block and append cipher text result to ciphertext member
			//then finally call method from mainform.cs to display the ciphertext property
			foreach (var block in _listOfBitBlocks)
			{
				MessageAfterInitialPermutation = ApplyInitialPermutation(block);
				SetLeft32Bits(MessageAfterInitialPermutation);
				SetRight32Bits(MessageAfterInitialPermutation);

				if (encrypt) ExecuteEncryptionRounds();
				else ExecuteDecryptionRounds();

				var temp = _left32Bits;
				_left32Bits = _right32Bits;
				_right32Bits = temp;

				BitArray FinalOutputForBlock = ApplyFinalPermutation(GetCombinedLeftRightSides(_left32Bits, _right32Bits));
				EncryptedBlockList.Add(FinalOutputForBlock);
			}

			foreach (var encryptedBlock in EncryptedBlockList)
			{
				CipherTextString += ConversionService.BitArrayToString(encryptedBlock);
			}
		}

		private void ExecuteEncryptionRounds()
		{
			for (int round = 0; round < 16; round++)
			{
				var cOutputAfterPermutation = GetResultOfKeyAndRightSide(round); // == F(k[round],RightSide)
				var right = cOutputAfterPermutation.Xor(_left32Bits);
				var left = _right32Bits;

				_left32Bits = left;
				_right32Bits = right;
			}
		}

		private void ExecuteDecryptionRounds()
		{
			for (int round = 15; round >= 0; round--)
			{
				var cOutputAfterPermutation = GetResultOfKeyAndRightSide(round); // == F(k[round],RightSide)
				var right = cOutputAfterPermutation.Xor(_left32Bits);
				var left = _right32Bits;

				_left32Bits = left;
				_right32Bits = right;
			}
		}

		private BitArray GetResultOfKeyAndRightSide(int round)
		{
			var right32Expanded = GetRight32Expanded();
			var xorResult = right32Expanded.Xor(_roundKeyList[round]);

			List<BitArray> sixBitBlocks = new List<BitArray>();

			var rightSideCounter = 0;
			for (int blockIndex = 0; blockIndex < 8; blockIndex++)
			{
				var ba = new BitArray(6);
				for (int bitPosition = 0; bitPosition < 6; bitPosition++)
				{
					ba[bitPosition] = xorResult[rightSideCounter];
					rightSideCounter++;
				}
				sixBitBlocks.Add(ba);
			}

			var binaryString = "";
			for (int i = 0; i < 8; i++)
			{
				var row = GetSboxRow(sixBitBlocks[i]);
				var col = GetSboxColumn(sixBitBlocks[i]);
				binaryString += Convert.ToString(SboxList[i][row, col], 2).PadLeft(4, '0');
			}

			var cOutput = ConversionService.StringToBitArray(binaryString);

			BitArray cOutputAfterPermutation = new BitArray(32);
			for (int i = 0; i < 32; i++)
			{
				cOutputAfterPermutation[i] = cOutput[COutputPermutation[i] - 1];
			}
			return cOutputAfterPermutation;
		}

		private BitArray GetRight32Expanded()
		{
			BitArray right32Expanded = new BitArray(48);
			for (int i = 0; i < 48; i++)
			{
				right32Expanded[i] = _right32Bits[ExpansionValues[i] - 1];
			}
			return right32Expanded;
		}

		public int GetSboxRow(BitArray b)
		{
			BitArray temp = new BitArray(2);
			temp[0] = b[0];
			temp[1] = b[5];

			return ConversionService.BitArrayToInt(temp);
		}

		public BitArray GetCombinedLeftRightSides(BitArray left, BitArray right)
		{
			BitArray result = new BitArray(64);
			for (int i = 0; i < 32; i++)
			{
				result[i] = left[i];
			}

			for (int i = 32, j = 0; i < 64 && j < 32; i++, j++)
			{
				result[i] = right[j];
			}

			return result;
		}

		public int GetSboxColumn(BitArray b)
		{
			BitArray temp = new BitArray(4);
			temp[0] = b[1];
			temp[1] = b[2];
			temp[2] = b[3];
			temp[3] = b[4];

			return ConversionService.BitArrayToInt(temp);
		}

		public void SetLeft32Bits(BitArray messageAfterIp)
		{
			for (var i = 0; i < 32; i++)
			{
				_left32Bits[i] = messageAfterIp[i];
			}
		}

		public void SetRight32Bits(BitArray messageAfterIp)
		{
			for (int i = 0, j = 32; i < 32 && j < 64; i++, j++)
			{
				_right32Bits[i] = messageAfterIp[j];
			}
		}

		private void ConvertMessageBitArrayToListOfBitBlocks()
		{
			_listOfBitBlocks = new List<BitArray>();
			_listOfBitBlocks.Capacity = (Message.ValueInBits.Length % 64 != 0)
				? (Message.ValueInBits.Count / 64) + 1
				: Message.ValueInBits.Count / 64;
			for (int i = 0; i < _listOfBitBlocks.Capacity; i++)
			{
				var temp = new BitArray(64);
				for (int j = i * 64, k = 0; j < (i * 64) + 64 && j < Message.ValueInBits.Count; j++, k++)
				{
					temp[k] = Message.ValueInBits[j];
				}
				_listOfBitBlocks.Add(temp);
			}
		}
	}
}