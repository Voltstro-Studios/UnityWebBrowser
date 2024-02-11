import './style.css'

interface UWB {
    EngineVersion: string;
    EngineName: string;
    bEngineVersion: string;
}

declare global {

    interface Window {
        uwb: UWB;
    }
}