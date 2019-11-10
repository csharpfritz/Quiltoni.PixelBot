using System;
namespace Quiltoni.PixelBot.Core.Messages.Currency
{
	[Serializable]
	public class AddCurrencyMessage
	{
		public AddCurrencyMessage(string userName, int amount, string actingUser)
		{

			this.UserName = userName;
			this.Amount = amount;
			this.ActingUser = actingUser;

		}


		public string UserName { get; }

		public int Amount { get; }

		public string ActingUser { get; }

	}
}