//Version variables
const uwbVersion = window.uwbVersion;
const cefVersion = window.cefVersion;

//Doc Elements
var uwbVersionText = document.getElementById('uwbVersionText');
var engineVersionText = document.getElementById('webEngineVersionText');

//If we have version info, then set it
if(uwbVersion !== undefined) {
    if(uwbVersionText != null)
        uwbVersionText.textContent = uwbVersion;

    if(engineVersionText != null)
        engineVersionText.textContent = cefVersion;

    console.log('[UWB About] Found version info!')
}
