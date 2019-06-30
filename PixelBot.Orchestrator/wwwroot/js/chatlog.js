"use strict";

var connection = new signalR.HubConnectionBuilder()
	.withUrl("/loggerhub")
	.configureLogging(signalR.LogLevel.Trace)
	.build();

connection.on("LogMessage", function (level, message) {
	console.debug("Received message: " + message);
	var li = document.createElement("li");
	li.textContent = message;
	document.getElementById("messagesList").appendChild(li);
});

console.log("Created signalR connection");

connection.start().catch(function (err) {
	return console.error(err.toString());
});