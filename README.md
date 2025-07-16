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
