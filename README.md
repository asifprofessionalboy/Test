 <form asp-action="UploadImage" method="post" id="form2">
     <div class="form-group row">
         <div class="col-sm-1">
             <label>Pno</label>
         </div>
         <div class="col-sm-3">
             <input id="Pno" name="Pno" class="form-control" type="number" oninput="javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);" maxlength="6" autocomplete="off"/>
         </div>
         <div class="col-sm-1">
             <label>Name</label>
         </div>
         <div class="col-sm-3">
             <input id="Name" name="Name" class="form-control"/>
         </div>
         <div class="col-sm-1">
             <label>Capture Photo</label>
         </div>
         <div class="col-sm-3">
             <video id="video" width="320" height="240" autoplay playsinline></video>
             <canvas id="canvas" style="display:none;"></canvas>

           
             <img id="previewImage" src="" alt="Captured Image" style="width: 200px; display: none; border: 2px solid black; margin-top: 5px;" />

            
             <button type="button" id="captureBtn" class="btn btn-primary">Capture</button>
             <button type="button" id="retakeBtn" class="btn btn-danger" style="display: none;">Retake</button>

            
             <input type="hidden" id="photoData" name="photoData" />
         </div>
     </div>

     <button type="submit" class="btn btn-success" id="submitBtn" disabled>Save Details</button>
 </form>

 document.getElementById('form2').addEventListener('submit', function (event) {
     event.preventDefault();


     var isValid = true;
     var elements = this.querySelectorAll('input, select, textarea');
  
    

     elements.forEach(function (element) {
        
         if (['ApprovalFile'].includes(element.id)) {
             return;
         }

      
         if (element.value.trim() === '') {
             isValid = false;
             element.classList.add('is-invalid');
         } else {
             element.classList.remove('is-invalid');
         }
     });




    
     if (isValid) {
         Swal.fire({
             title: "Success!",
             text: "Data Saved Successfully",
             icon: "success",
             confirmButtonText: false
         });
         this.submit();
     }
 }); 

is this proper way ? is there any issue in this code 
