function UserActivity() {

	var _Connection;
	var _Component;

	this.Connect = function (channel, component) {

		console.log("Channel: " + channel);
		_Component = component;

		// Use SignalR to connect to the hub for this channel
		_Connection = new signalR.HubConnectionBuilder()
			.withUrl(`/useractivityhub?channel=${channel}`)
			.configureLogging(signalR.LogLevel.Trace)
			.build();

		// Setup the event handlers for the various methods from UserActivityClient
		_Connection.on("NewFollower", (name) => NewFollower(name));
		_Connection.on("NewSubscriber", (newSubscribedName, numberOfMonths, numberOfMonthsInRow, message) => NewSubscriber(newSubscribedName, numberOfMonths, numberOfMonthsInRow, message));
		_Connection.on("NewCheer", (name) => NewCheer(cheererName, amountCheered, message));

		_Connection.start().catch(function (err) {
			return console.error(err.toString());
		});

	};

	this.Stop = function () {

		if (_Connection) _Connection.stop();

	};

	function NewFollower(newFollowerName)
	{
		console.log(`New Follower: ${newFollowerName}`);
		_Component.invokeMethodAsync("NewFollower", newFollowerName);
	}

	function NewSubscriber(newSubscribedName, numberOfMonths, numberOfMonthsInRow, message)
	{
		_Component.invokeMethodAsync("NewSubscriber", newSubscribedName, numberOfMonths, numberOfMonthsInRow, message);
	}

	function NewCheer(cheererName, amountCheered, message)
	{
		_Component.invokeMethodAsync("NewCheer", cheererName, amountCheered, message);
	}


}

window.UserActivity = new UserActivity();