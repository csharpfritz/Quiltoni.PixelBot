//(function () {

const reconnectNode = document.getElementById("components-reconnect-modal");
const reconnectClasses = ["components-reconnect-failed", "components-reconnect-rejected", "components-reconnect-show"];

function WatchBlazorDisconnect() {
new MutationObserver((mutations, observer) => {

	for (let m of mutations) {

		if (m.type === 'attributes' && m.attributeName === 'class' && reconnectClasses.includes(m.target.attributes[m.attributeName].textContent)) {
			console.log(`The ${m.attributeName} was modified`);

			console.log("Detected disconnect... attempting to reconnect");
			observer.disconnect();
			attemptReload();
			setInterval(attemptReload, 10000);

		}

	}

}).observe(reconnectNode, { attributes: true, childList: true, subtree: true });
}

async function attemptReload() {

	var response = await fetch("/health");
	var result = await response.text();
	console.log(result);

	if (result == "Healthy") { // Check if the server is back
		// window.Blazor.reconnect();
		// WatchBlazorDisconnect();
		// return;
		location.reload();
	}
}

WatchBlazorDisconnect();

// })();