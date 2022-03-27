import './style.css'

declare global {
    interface Window {
        uwbVersion: string;
        cefVersion: string;
    }
}