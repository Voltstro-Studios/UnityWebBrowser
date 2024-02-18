function aboutPage(): void {
    //Version variables
    const uwb = window.uwb;

    if(!uwb) {
        console.error('[UWB About] Failed to get version info! Probably not running in a UWB engine!')
        return;
    }

    const uwbEngineVersion = uwb.EngineVersion;
    const webEngineName = uwb.EngineName;
    const webEngineVersion = uwb.EngineVersion;

    //Doc Elements
    var uwbEngineVersionText = document.getElementById('uwbEngineVersionText');
    var webEngineNameText = document.getElementById('webEngineNameText');
    var webEngineVersionText = document.getElementById('webEngineVersionText');

    if(uwbEngineVersionText != null) {
        uwbEngineVersionText.textContent = uwbEngineVersion;
    }

    if(webEngineNameText != null) {
        webEngineNameText.textContent = webEngineName;
    }

    if(webEngineVersionText != null) {
        webEngineVersionText.textContent = webEngineVersion;
    } 

    console.log('[UWB About] Found version info!');
}

aboutPage();
