function redirectToIframePage() {
    var iframeUrl = encodeURIComponent("https://servicesdev.juscoltd.com/AttendanceReport/Webform1.aspx");
    window.location.href = "/Geo/AttendanceReport?url=" + iframeUrl;
}


public IActionResult AttendanceReport(string url)
{
    if (string.IsNullOrEmpty(url))
    {
        return RedirectToAction("Login", "User"); // Handle unauthorized access
    }

    ViewBag.ReportUrl = url; // Pass URL to the view
    return View();
}

<div class="container">
    <iframe src="@ViewBag.ReportUrl" width="100%" height="600px" frameborder="0" class="report"></iframe>
</div>



this is my report url 

https://servicesdev.juscoltd.com/AttendanceReport/webform1

this is my view method when clicking on button 

 
public IActionResult AttendanceReport()
 {
    
     return View();
 }

this is my button 
<div class="text-center">
    <button onclick="redirectToIframePage()" class="btn btn-primary mt-2">Check your Attendance</button>
</div>

this is my script 
<script>
    function redirectToIframePage() {
        var iframeUrl = encodeURIComponent("https://servicesdev.juscoltd.com/AttendanceReport/AttendanceReport/Webform1.aspx"); // Replace with your URL
       window.location.href = "/Geo/AttendanceReport?url=" + iframeUrl;

        
    }
</script>

and this is my iframe 

<div class="container">
    <iframe src="@iframeUrl" width="100%" height="600px" frameborder="0" class="report"></iframe>
  
</div>


when clicking on button it shows 404 why?
