using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESWF;

namespace AESWPF
{
	class AesCipher
	{
		public string CipherText;

		public void Encrypt(List<Byte[]> plainTextBlocks)
		{
			//Each block will be encrypted, result should be appended to Ciphertext member
			foreach (var block in plainTextBlocks)
			{
				//Convert block to a matrix
				var plainTextMatrix = block.ConvertByteArrayToFourByFourMatrix();
				
				//Add Round Key
				

				//Run through the 10 rounds
				for (int i = 0; i < 10; i++)
				{
					
				}
				//CipherText += CipherTextResult;
			}
		}
		
	}
}
