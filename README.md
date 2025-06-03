app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true, // VERY IMPORTANT
    DefaultContentType = "application/octet-stream"
});




this is my program.cs


using GFAS.Email;
using GFAS.Models;
using GFAS.PasswordHasher;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
var Provider = builder.Services.BuildServiceProvider();
var Config = Provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<INNOVATIONDBContext>(item => item.UseSqlServer(Config.GetConnectionString("LoginDB")));
builder.Services.AddDbContext<UserLoginDBContext>(item => item.UseSqlServer(Config.GetConnectionString("UserLoginDB")));
builder.Services.AddDbContext<TSUISLRFIDDBContext>(item => item.UseSqlServer(Config.GetConnectionString("RFID")));

builder.Services.AddTransient<Hash_Password>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("Cookies")
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/User/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(365);
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
   
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated)
    {
        var userId = context.Request.Cookies["Session"];
        var UserName = context.Request.Cookies["UserName"];
        var Pno = context.Request.Cookies["UserSession"];
        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Pno))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,UserName),
                new Claim("Pno",Pno),
                new Claim("Session",userId)
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

        }
    }
        await next();
});
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Geo}/{action=GeoFencing}/{id?}");

app.Run();
this is my script

<script>
    window.addEventListener("DOMContentLoaded", async () => {
        const video = document.getElementById("video");
        const canvas = document.getElementById("canvas");
        const EntryTypeInput = document.getElementById("EntryType");
        const successSound = document.getElementById("successSound");
        const errorSound = document.getElementById("errorSound");

        let blinked = false;
        let lastBlinkTime = 0;
        const BLINK_INTERVAL = 3000;
        const EAR_THRESHOLD = 0.23;

        try {

            Promise.all([
                faceapi.nets.tinyFaceDetector.loadFromUri('/faceApi'),
                faceapi.nets.faceLandmark68Net.loadFromUri('/faceApi')
            ])
                .then(startVideo)
                .catch(error => {
                    console.error("Failed to load face-api models:", error);
                });
        } catch (e) {
            console.error("Failed to load face-api models:", e);
        }

        function startVideo() {
            navigator.mediaDevices.getUserMedia({ video: { facingMode: "user" } })
                .then(stream => {
                    video.srcObject = stream;
                    video.play();
                    video.addEventListener("play", () => {
                        detectBlink();
                    });
                })
                .catch(err => {
                    console.error("Camera error:", err);
                });
        }

        function getEAR(eye) {
            const a = distance(eye[1], eye[5]);
            const b = distance(eye[2], eye[4]);
            const c = distance(eye[0], eye[3]);
            return (a + b) / (2.0 * c);
        }

        function distance(p1, p2) {
            return Math.hypot(p1.x - p2.x, p1.y - p2.y);
        }

        async function detectBlink() {
            const detection = await faceapi
                .detectSingleFace(video, new faceapi.TinyFaceDetectorOptions())
                .withFaceLandmarks();

            if (detection) {
                const leftEye = detection.landmarks.getLeftEye();
                const rightEye = detection.landmarks.getRightEye();

                const leftEAR = getEAR(leftEye);
                const rightEAR = getEAR(rightEye);
                const avgEAR = (leftEAR + rightEAR) / 2.0;

                const now = Date.now();
                if (avgEAR < EAR_THRESHOLD && now - lastBlinkTime > BLINK_INTERVAL) {
                    blinked = true;
                    lastBlinkTime = now;
                    console.log("Blink detected!");
                }
            }

            requestAnimationFrame(detectBlink);
        }

        window.captureImageAndSubmit = function (entryType) {
            if (!blinked) {
                Swal.fire({
                    title: "Liveness Check Failed",
                    text: "Please blink to verify you're not using a static image.",
                    icon: "warning"
                });
                return;
            }

            EntryTypeInput.value = entryType;

            const context = canvas.getContext("2d");
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            const imageData = canvas.toDataURL("image/jpeg");

            Swal.fire({
                title: "Verifying Face...",
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            fetch("/AS/Geo/AttendanceData", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    Type: entryType,
                    ImageData: imageData
                })
            })
                .then(response => response.json())
                .then(data => {
                    const now = new Date();
                    const formattedDateTime = now.toLocaleString();

                    if (data.success) {
                        successSound.play();
                        triggerHapticFeedback("success");

                        Swal.fire({
                            title: "Face Matched!",
                            text: "Attendance Recorded.\nDate & Time: " + formattedDateTime,
                            icon: "success",
                            timer: 3000,
                            showConfirmButton: false
                        }).then(() => {
                            location.reload();
                        });
                    } else {
                        errorSound.play();
                        triggerHapticFeedback("error");

                        Swal.fire({
                            title: "Face Not Recognized.",
                            text: "Click the button again to retry.\nDate & Time: " + formattedDateTime,
                            icon: "error",
                            confirmButtonText: "Retry"
                        });
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    triggerHapticFeedback("error");

                    Swal.fire({
                        title: "Error!",
                        text: "An error occurred while processing your request.",
                        icon: "error"
                    });
                });
        };

        function triggerHapticFeedback(type) {
            if ("vibrate" in navigator) {
                if (type === "success") {
                    navigator.vibrate(100);
                } else if (type === "error") {
                    navigator.vibrate([200, 100, 200]);
                }
            }
        }
    });
</script>

this is path
bin\Debug\net6.0\wwwroot\faceApi and in this file is already exist with non zero kb
