using Akka.Actor;
using Akka.Event;
using Microsoft.AspNetCore.Hosting;

namespace PixelBot.ResolverActors.Actors
{
	public class TestActor : ReceiveActor
	{
		private IWebHostEnvironment envTemp;

		public TestActor()
		{


			Receive<string>(msg => Doit(msg));
		}

		private void Doit(string msg)
		{
			ILoggingAdapter _log = Logging.GetLogger(Context);

			_log.Error($"Got: {msg}");
		}

		public override void AroundPreStart()
		{
			base.AroundPreStart();
			envTemp = this.RequestService<IWebHostEnvironment>();
		}
	}
}
