// JavaScript source code
// Add this to your wwwroot/js/app.js (or create this file and reference it in App.razor)
// This enables Blazor to trigger file downloads via JS interop

window.downloadFileFromBase64 = (fileName, mimeType, base64) => {
    const bytes = atob(base64);
    const buffer = new ArrayBuffer(bytes.length);
    const view = new Uint8Array(buffer);
    for (let i = 0; i < bytes.length; i++) {
        view[i] = bytes.charCodeAt(i);
    }
    const blob = new Blob([buffer], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};