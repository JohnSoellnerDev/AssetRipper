// Get the canvas element
const canvas = document.getElementById('babylonMaterialCanvas');

// Check if the canvas exists on this page
if (canvas) {
	// Retrieve the GLB file path from the custom attribute
	const glbPath = canvas.getAttribute('material-data-path');

	// Create Babylon.js engine
	const engine = new BABYLON.Engine(canvas, true);

	// Create the scene
	const createScene = function () {
		const scene = new BABYLON.Scene(engine);

		// Create a basic light
		const light = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(0, 1, 0), scene);
		light.intensity = 0.8;

		// Add a directional light for better material visualization
		const directionalLight = new BABYLON.DirectionalLight("dirLight", new BABYLON.Vector3(-1, -1, -1), scene);
		directionalLight.intensity = 0.6;

		// Create an ArcRotateCamera that rotates around a target position
		const camera = new BABYLON.ArcRotateCamera("Camera", Math.PI / 2, Math.PI / 2, 3, new BABYLON.Vector3(0, 0, 0), scene);
		camera.attachControl(canvas, true);

		// Load the GLB file from the path stored in the custom attribute
		BABYLON.SceneLoader.Append("", glbPath, scene, function (scene) {
			scene.createDefaultCameraOrLight(true, true, true);
			scene.activeCamera.alpha += Math.PI;
		});

		return scene;
	};

	// Create the scene
	const scene = createScene();

	// Render the scene
	engine.runRenderLoop(function () {
		engine.resize();
		scene.render();
	});

	// Resize the engine on window resize
	window.addEventListener('resize', function () {
		engine.resize();
	});
}