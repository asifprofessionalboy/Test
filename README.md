app.UseStaticFiles(); // default

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images")),
    RequestPath = "/AS/Images"
});
ViewBag.UserId = HttpContext.Request.Cookies["Session"];
ViewBag.UserName = HttpContext.Request.Cookies["UserName"];
<script>
    const userId = '@ViewBag.UserId';
    const userName = '@ViewBag.UserName';
</script>
const safeUserName = userName.replace(/\s+/g, "_"); // Or "-" if your files use that

async function loadStoredFaceDescriptor(imagePath) {
    try {
        const img = await faceapi.fetchImage(imagePath);
        const detection = await faceapi
            .detectSingleFace(img, detectorOptions)
            .withFaceLandmarks()
            .withFaceDescriptor();

        return detection ? detection.descriptor : null;
    } catch (err) {
        console.warn("Failed to load image: " + imagePath, err);
        return null;
    }
}

const safeUserName = userName.replace(/\s+/g, "_"); // sanitize name

const descriptors = [
    await loadStoredFaceDescriptor(`/AS/Images/${userId}-Captured.jpg`),
    await loadStoredFaceDescriptor(`/AS/Images/${userId}-${safeUserName}.jpg`)
].filter(d => d !== null);

const faceMatcher = new faceapi.FaceMatcher([
    new faceapi.LabeledFaceDescriptors(userId, descriptors)
], 0.4); // Use 0.4 for strict matching

const result = await recognizeFace();

if (result.matched) {
    statusText.textContent = `✅ ${result.label} - Face Matched (Distance: ${result.distance.toFixed(2)})`;
    videoContainer.style.borderColor = "green";
    punchInButton?.style.display = "inline-block";
    punchOutButton?.style.display = "inline-block";
    setTimeout(() => { statusText.textContent = ""; }, 2500);
} else {
    statusText.textContent = `❌ Unknown - Face Not Recognized`;
    videoContainer.style.borderColor = "red";
    punchInButton?.style.display = "none";
    punchOutButton?.style.display = "none";
    setTimeout(() => {
        statusText.textContent = "";
        video.style.display = "block";
        capturedImage.style.display = "none";
    }, 2500);
}

async function recognizeFace() {
    const detection = await faceapi
        .detectSingleFace(canvas, detectorOptions)
        .withFaceLandmarks()
        .withFaceDescriptor();

    if (!detection) {
        return { matched: false, message: "Face not detected in captured image." };
    }

    const bestMatch = faceMatcher.findBestMatch(detection.descriptor);

    if (bestMatch.distance > 0.4 || bestMatch.label === "unknown") {
        return {
            matched: false,
            label: "unknown",
            distance: bestMatch.distance,
            message: "Not matched or distance too high"
        };
    }

    return {
        matched: true,
        label: bestMatch.label,
        distance: bestMatch.distance,
        message: `Matched with ${bestMatch.label}`
    };
}





previously i am fetching image like this from controller through post method
string Pno = UserId;
 string Name = UserName;

 string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-{Name}.jpg");
 string lastCapturedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}-Captured.jpg");


but i want like this in place of these hardcoded path of image 

        const descriptors = [
    await loadStoredFaceDescriptor('/AS/Images/151514-Captured.jpg'),
            await loadStoredFaceDescriptor('/AS/Images/151514-Shashi Kumar.jpg')
].filter(d => d !== null);

const faceMatcher = new faceapi.FaceMatcher([
            new faceapi.LabeledFaceDescriptors("159445", descriptors)
], 0.4);
