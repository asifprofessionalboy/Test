success: function (response) {
    console.log("Full Response from API:", response); // Log the full response

    if (response.success) {
        let imageContainer = $("#imageContainer");
        imageContainer.empty(); // Clear previous images

        console.log("Images array:", response.images); // Log images array

        response.images.forEach((image, index) => {
            console.log(`Image ${index}:`, image); // Log each image object

            console.log("FileName:", image.FileName); // Log FileName
            console.log("Image URL:", image.ImageUrl); // Log ImageUrl

            if (image.ImageUrl) {
                let imgElement = `<img src="${image.ImageUrl}"
                    alt="Captured Image"
                    style="width:150px;height:150px;margin:5px;border:1px solid red;">`;

                imageContainer.append(imgElement);
            } else {
                console.log("⚠️ No image URL found for:", image.FileName);
            }
        });
    } else {
        alert(response.message);
    }
}

    
    
    

response.images.forEach(image => {
        console.log("Image URL:", image.ImageUrl); // Debugging log

        let imgElement = `<img src="${image.ImageUrl}"
alt="Captured Image"
style="width:150px;height:150px;margin:5px;border:1px solid red;">`;

        imageContainer.append(imgElement);
    });
