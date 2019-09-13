"use strict";

(function UserActivity() {

	var _Connection;
	var _Component;

	this.Connect = new function (channel, component) {

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

	function NewFollower(newFollowerName)
	{
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




	this.Stop = new function () {

		_Connection.stop();

	};

	window.UserActivity = this;

})();
