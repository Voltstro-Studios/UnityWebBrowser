import './style.css'

declare global {
    interface Window {
        uwbEngineVersion: string;
        webEngineName: string;
        webEngineVersion: string;
    }
}