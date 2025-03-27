    response.images.forEach(image => {
        console.log("Image URL:", image.ImageUrl); // Debugging log

        let imgElement = `<img src="${image.ImageUrl}"
alt="Captured Image"
style="width:150px;height:150px;margin:5px;border:1px solid red;">`;

        imageContainer.append(imgElement);
    });
