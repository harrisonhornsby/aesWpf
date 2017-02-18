using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DESWF;
using MlkPwgen;

namespace AESWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

		}

		private void btn_GenerateKey(object sender, RoutedEventArgs e)
		{

			var pw = PasswordGenerator.Generate(length: 16, allowed: Sets.Alphanumerics);

			BitArray key = new BitArray(Encoding.UTF8.GetBytes(pw));
			tbKey.Text = ConversionService.BitArrayToString(key);
		}

		private void btn_Encrypt(object sender, RoutedEventArgs e)
		{
			var plaintext = tbPlaintext.Text;
			var plaintextBitArray = ConversionService.StringOfBinaryLikeCharactersToBitArray(plaintext);
			plaintextBitArray.ConvertBitArrayToByteArray();

			var listOfByteBlocks = ConversionService.ConvertMessageBitArrayToListOfByteBlocks(16,
				plaintextBitArray.ConvertBitArrayToByteArray());
			var aesCipher = new AesCipher();
			aesCipher.Encrypt(listOfByteBlocks);
		}
	}
}
