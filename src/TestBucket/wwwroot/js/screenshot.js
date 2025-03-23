

async function captureDesktopScreenshot() {
    const stream = await navigator.mediaDevices.getDisplayMedia({ video: true });
    const track = stream.getVideoTracks()[0];
    const imageCapture = new ImageCapture(track);
    const bitmap = await imageCapture.grabFrame();

    console.log("grabFrame", bitmap);

    const canvas = document.createElement('canvas');
    canvas.width = bitmap.width;
    canvas.height = bitmap.height;
    const context = canvas.getContext('2d');
    context.drawImage(bitmap, 0, 0);

    const dataUrl = canvas.toDataURL('image/png');

    console.log("dataUrl", dataUrl);

    for (const track of stream.getTracks()) {
        track.stop();
    }

    return dataUrl;
};
