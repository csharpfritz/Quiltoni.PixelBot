(function () {

const reconnectNode = document.getElementById("components-reconnect-modal");
const reconnectClasses = ["components-reconnect-failed", "components-reconnect-rejected", "components-reconnect-show"];
var reconnectEnabled = true;


function WatchBlazorDisconnect() {
new MutationObserver((mutations, observer) => {

	for (let m of mutations) {

		if (m.type === 'attributes' && m.attributeName === 'class' && reconnectClasses.includes(m.target.attributes[m.attributeName].textContent)) {
			console.log(`The ${m.attributeName} was modified.  New value: ${m.target.attributes[m.attributeName].textContent}`);

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
		if (reconnectEnabled) location.reload();
		console.error("Attempting to reload the browser");
	}
}

window.onbeforeunload = function() {
	reconnectEnabled = false;
};

WatchBlazorDisconnect();

})();