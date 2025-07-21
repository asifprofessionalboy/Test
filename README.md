
const descriptors = [
    await loadDescriptor(`/AS/Images/${userId}-Captured.jpg`),
    await loadDescriptor(`/AS/Images/${userId}-${safeUserName}.jpg`)
].filter(d => d !== null);

when i delete the image from my folder Images but it holds the image may be in browser and shows face matched .
