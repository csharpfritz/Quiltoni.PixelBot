using System;
namespace Quiltoni.PixelBot.Core.Messages.Currency
{
	[Serializable]
	public class MyCurrencyMessage
	{

		public MyCurrencyMessage(string userName) {

			this.UserName = userName;

		}

		public string UserName { get; }

	}
}
