const canvas = document.getElementById('babylonMaterialCanvas');

// Only execute if the corresponding canvas is present on the current page
if (canvas) {
    // Create Babylon.js engine
    const engine = new BABYLON.Engine(canvas, true);

    // Function to create the scene
    const createScene = function () {
        const scene = new BABYLON.Scene(engine);

        // Add a light source
        const light = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(0, 1, 0), scene);

        // Create an ArcRotateCamera
        const camera = new BABYLON.ArcRotateCamera("Camera", Math.PI / 2, Math.PI / 2, 2, BABYLON.Vector3.Zero(), scene);
        camera.attachControl(canvas, true);

        // Create a simple sphere that will display the material
        const sphere = BABYLON.MeshBuilder.CreateSphere("sphere", { diameter: 1, segments: 32 }, scene);

        // Create a default PBR material (placeholder until full material data mapping is implemented)
        const material = new BABYLON.PBRMaterial("previewMaterial", scene);
        material.albedoColor = new BABYLON.Color3(1.0, 1.0, 1.0);
        sphere.material = material;

        return scene;
    };

    const scene = createScene();

    // Start the render loop
    engine.runRenderLoop(function () {
        engine.resize();
        scene.render();
    });

    // Handle browser resize
    window.addEventListener('resize', function () {
        engine.resize();
    });
}