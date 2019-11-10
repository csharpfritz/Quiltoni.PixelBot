using Quiltoni.PixelBot.Core.Extensibility;

namespace PixelBot.StandardFeatures.ScreenWidgets.UserActivityTrain
{
	public class UserActivityTrainFeature : BaseFeature
	{
		public override string Name => "User Activity Train";

		public override void FeatureTriggered(string notifyAction)
		{

			if (!IsEnabled) return;

			// TODO: Do something with the new action received

		}
	}
}