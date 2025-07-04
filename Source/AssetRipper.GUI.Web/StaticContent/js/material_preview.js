const canvas = document.getElementById('babylonMaterialCanvas');

// Only execute if the corresponding canvas is present on the current page
if (canvas) {
    // Create Babylon.js engine
    const engine = new BABYLON.Engine(canvas, true);

    // Function to create the scene
    const createScene = function () {
        const scene = new BABYLON.Scene(engine);

        // Automatically add default camera and light for proper viewing
        scene.createDefaultCameraOrLight(true, true, true);

        // Retrieve camera reference for user interaction
        const camera = scene.activeCamera;
        camera.attachControl(canvas, true);

        // Create a simple sphere that will display the material
        const sphere = BABYLON.MeshBuilder.CreateSphere("sphere", { diameter: 1, segments: 32 }, scene);

        // Create a PBR material and apply textures if provided
        const material = new BABYLON.PBRMaterial("previewMaterial", scene);

        const albedoUrl = canvas.getAttribute('albedo-url');
        if (albedoUrl) {
            material.albedoTexture = new BABYLON.Texture(albedoUrl, scene, true, false, BABYLON.Texture.TRILINEAR_SAMPLINGMODE, null, null, albedoUrl.endsWith('.png') ? '.png' : undefined);
        } else {
            material.albedoColor = new BABYLON.Color3(1.0, 1.0, 1.0);
        }

        const normalUrl = canvas.getAttribute('normal-url');
        if (normalUrl) {
            material.bumpTexture = new BABYLON.Texture(normalUrl, scene, true, false, BABYLON.Texture.TRILINEAR_SAMPLINGMODE, null, null, normalUrl.endsWith('.png') ? '.png' : undefined);
            material.invertNormalMapY = true; // Unity normal maps might require Y inversion
        }

        sphere.material = material;

        // Optionally create an environment for better reflections
        scene.createDefaultEnvironment();

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