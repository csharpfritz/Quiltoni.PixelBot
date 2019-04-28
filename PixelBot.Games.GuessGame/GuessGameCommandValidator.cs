using FluentValidation;

namespace PixelBot.Games.GuessGame
{
	public class GuessGameCommandValidator : AbstractValidator<GuessGameCommand>
	{
		public GuessGameCommandValidator() {
			RuleFor(a => a.ArgumentsAsList).NotNull();
			RuleFor(a => a.ChatUser).NotNull();
			RuleFor(a => a.ChatUser).SetValidator(new ChatUserValidator());
		}
	}

	public class ChatUserValidator : AbstractValidator<ChatUser>
	{
		public ChatUserValidator() {
			RuleFor(x => x.DisplayName).NotNull().NotEmpty();
			RuleFor(x => x.Username).NotNull().NotEmpty();
		}
	}
}
