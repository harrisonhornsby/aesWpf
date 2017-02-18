using System.Collections;

namespace DES
{
	internal class Plaintext
	{
		/// <summary>
		/// This represents the plain text value
		/// </summary>
		public string ValueInString { get; set; }

		/// <summary>
		/// This is the plain text after encoded into a byte array
		/// </summary>
		public byte[] EncodedByteArray { get; set; }

		public BitArray ValueInBits { get; set; }
	}
}