const uwbVersion = window.uwbVersion;
const cefVersion = window.cefVersion;

if(uwbVersion != 'undefined') {
    document.getElementById('uwbVersion').textContent = uwbVersion;
    document.getElementById('cefVersion').textContent = cefVersion;
}
