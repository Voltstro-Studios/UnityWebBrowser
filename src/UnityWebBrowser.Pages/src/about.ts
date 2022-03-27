//Version variables
const uwbEngineVersion = window.uwbEngineVersion;
const webEngineName = window.webEngineName;
const webEngineVersion = window.webEngineVersion;

//Doc Elements
var uwbEngineVersionText = document.getElementById('uwbEngineVersionText');
var webEngineNameText = document.getElementById('webEngineNameText');
var webEngineVersionText = document.getElementById('webEngineVersionText');

//If we have version info, then set it
if(uwbEngineVersion !== undefined) {
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
//Best to assume that if uwbEngineVersion is undefined, then we aren't a UWB engine
else {
    console.error('[UWB About] Failed to get version info! Probably not running in a UWB engine!')
}