//(function () {

const reconnectNode = document.getElementById("components-reconnect-modal");
const reconnectClasses = ["components-reconnect-failed", "components-reconnect-rejected", "components-reconnect-show"];

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

	/**
	var classList = document.querySelector("#components-reconnect-modal").classList;
	console.log(classList);

	if (classList.contains("components-reconnect-show") || classList.contains("components-reconnect-rejected")) {
		console.log("Detected disconnect... attempting to reconnect");
		observer.disconnect();
		attemptReload();
		setInterval(attemptReload, 10000);
	}
	**/
}).observe(reconnectNode, { attributes: true, childList: true, subtree: true });

async function attemptReload() {
	await fetch(''); // Check if the server is back
	location.reload();
}

// })();