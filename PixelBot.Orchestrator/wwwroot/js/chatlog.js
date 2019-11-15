"use strict";

var connection = new signalR.HubConnectionBuilder()
	.withUrl("/loggerhub")
	.configureLogging(signalR.LogLevel.Trace)
	.build();

connection.on("LogMessage", function (level, message) {
	console.debug("Received message: " + message);
	var li = document.createElement("li");
	li.textContent = message;
	if (document.getElementById("messagesList") != null) {
		document.getElementById("messagesList").appendChild(li);
	} else {
		StopLogging();
		return;
	}
	li.scrollIntoView();
});

console.log("Created signalR connection");

function StartLogging() {

	if (connection.connectionState === "Connected") return;

	connection.start().catch(function (err) {
		return console.error(err.toString());
	});

}

function StopLogging() {

	connection.stop();

}