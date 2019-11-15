using Quiltoni.PixelBot.Core.Extensibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace PixelBot.StandardFeatures.ScreenWidgets.UserActivityTrain
{


	public class UserActivityConfiguration : BaseFeatureConfiguration
	{

		public enum UserActivityTrainType
		{
			Follow,
			Subscription,
			Cheer
		}

		public UserActivityTrainType Type { get; set; } = UserActivityTrainType.Follow;

		public int MaxTimeBetweenActionsInSeconds { get; set; } = 300;

	}
}