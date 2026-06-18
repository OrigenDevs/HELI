// Coloca este archivo en: Assets/Plugins/WebGL/IsMobileDevice.jslib
// La carpeta Plugins/WebGL es especial — Unity la incluye automáticamente en builds WebGL

mergeInto(LibraryManager.library, {
    IsMobileDevice: function() {
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    }
});
