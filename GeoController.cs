using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.Util;
using Dapper;
using GFAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Collections;
using System.Data;
using Emgu.CV.CvEnum;

namespace GFAS.Controllers
{
    public class GeoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly GEOFENCEDBContext context;

        public GeoController(IConfiguration configuration,GEOFENCEDBContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        private string GetConnectionString()
        {
            return this.configuration.GetConnectionString("GeoDB");
        }

        private string GetRFIDConnectionString()
        {
            return this.configuration.GetConnectionString("RFID");
        }
        [Authorize]
        public IActionResult GeoFencing()
        {
            var session = HttpContext.Request.Cookies["Session"];
            var UserName = HttpContext.Request.Cookies["UserName"];

           
                var data = GetLocations();
                return View();
            

        }

        public IActionResult GetLocations()
        {
            var UserId = HttpContext.Request.Cookies["Session"];
            string connectionString = GetConnectionString();

            string query = @"SELECT ps.Worksite FROM GEOFENCEDB.DBO.App_Position_Worksite AS ps 
                     INNER JOIN GEOFENCEDB.DBO.App_Emp_position AS es ON es.position = ps.position 
                     WHERE es.Pno = @UserId";

            using (var connection = new SqlConnection(connectionString))
            {
               
                var worksiteNamesString = connection.QuerySingleOrDefault<string>(query, new { UserId });

                if (string.IsNullOrEmpty(worksiteNamesString))
                {
                    ViewBag.PolyData = new List<object>();
                    return View();
                }

                
                var worksiteNames = worksiteNamesString.Split(',').Select(w => w.Trim()).ToList();

               
                var formattedWorksites = worksiteNames
                    .Select(name => $"'{name.Replace("'", "''")}'") 
                    .ToList();

                string s = string.Join(",", formattedWorksites);

                string query2 = @$"SELECT Longitude, Latitude, Range FROM GEOFENCEDB.DBO.App_LocationMaster 
                           WHERE work_site IN ({s})";

                var locations = connection.Query(query2).Select(loc => new
                {
                    Latitude = (double)loc.Latitude,
                    Longitude = (double)loc.Longitude,
                    Range = loc.Range
                }).ToList();

                ViewBag.PolyData = locations;
                return View();
            }
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Session");
            return RedirectToAction("Login", "User");

        }




        [HttpPost]
        public IActionResult AttendanceData([FromBody] AttendanceRequest model)
        {
            try
            {
                
                var UserId = HttpContext.Request.Cookies["Session"];
                if (string.IsNullOrEmpty(UserId))
                    return Json(new { success = false, message = "User session not found!" });

                string Pno = UserId;

                
                string storedImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}.jpg");
                if (!System.IO.File.Exists(storedImagePath))
                {
                    return Json(new { success = false, message = "Stored image not found!" });
                }

                
                string capturedImagePath = SaveBase64Image(model.ImageData, Pno);
                if (string.IsNullOrEmpty(capturedImagePath))
                {
                    return Json(new { success = false, message = "Failed to save captured image!" });
                }

               
                using (Bitmap storedImage = new Bitmap(storedImagePath))
                using (Bitmap capturedImage = new Bitmap(capturedImagePath))
                {
                    bool isFaceMatched = VerifyFace(capturedImage, storedImage);
                    if (isFaceMatched)
                    {
                        string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
                        string currentTime = DateTime.Now.ToString("HH:mm");

                        if (model.Type == "Punch In")
                        {
                            StoreData(currentDate, currentTime, null, Pno);
                        }
                        else
                        {
                            StoreData(currentDate, null, currentTime, Pno);
                        }

                        return Json(new { success = true, message = "Attendance recorded successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Face does not match!" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private string SaveBase64Image(string base64String, string Pno)
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/", $"{Pno}_captured.jpg");
                byte[] imageBytes = Convert.FromBase64String(base64String.Split(',')[1]);
                System.IO.File.WriteAllBytes(filePath, imageBytes);
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving image: " + ex.Message);
                return null;
            }
        }

       


        private bool VerifyFace(Bitmap captured, Bitmap stored)
        {
            try
            {
                Mat matCaptured = BitmapToMat(captured);
                Mat matStored = BitmapToMat(stored);

               
                CvInvoke.CvtColor(matCaptured, matCaptured, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.CvtColor(matStored, matStored, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

               
                string cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Cascades/haarcascade_frontalface_default.xml");
                if (!System.IO.File.Exists(cascadePath))
                {
                    Console.WriteLine("Error: Haarcascade file not found!");
                    return false;
                }

                CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);
                Rectangle[] capturedFaces = faceCascade.DetectMultiScale(matCaptured, 1.1, 5);
                Rectangle[] storedFaces = faceCascade.DetectMultiScale(matStored, 1.1, 5);

                if (capturedFaces.Length == 0 || storedFaces.Length == 0)
                {
                    Console.WriteLine("No face detected in one or both images.");
                    return false;
                }

                
                Mat capturedFace = new Mat(matCaptured, capturedFaces[0]);
                Mat storedFace = new Mat(matStored, storedFaces[0]);

               
                CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));
                CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));

                
                using (var faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100))
                {
                    CvInvoke.EqualizeHist(capturedFace, capturedFace);
                    CvInvoke.EqualizeHist(storedFace, storedFace);

                    VectorOfMat trainingImages = new VectorOfMat();
                    trainingImages.Push(storedFace);
                    VectorOfInt labels = new VectorOfInt(new int[] { 1 });

                    faceRecognizer.Train(trainingImages, labels);
                    var result = faceRecognizer.Predict(capturedFace);

                    Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

                    return result.Label == 1 && result.Distance < 50;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in face verification: " + ex.Message);
                return false;
            }
        }



        private Mat BitmapToMat(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] imageData = ms.ToArray();

                Mat mat = new Mat();
                CvInvoke.Imdecode(new VectorOfByte(imageData), ImreadModes.Color, mat);

                if (mat.IsEmpty)
                {
                    Console.WriteLine("Error: Image conversion failed!");
                }

                return mat;
            }
        }



        public void StoreData(string ddMMyy, string tmIn, string tmOut, string Pno)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("RFID")))
            {
                connection.Open();

                if (!string.IsNullOrEmpty(tmIn))
                {
                    var query = @"
                INSERT INTO T_TRBDGDAT_EARS(TRBDGDA_BD_DATE, TRBDGDA_BD_TIME, TRBDGDA_BD_INOUT, TRBDGDA_BD_READER,
                TRBDGDA_BD_CHKHS, TRBDGDA_BD_SUBAREA, TRBDGDA_BD_PNO) 
                VALUES 
                (@TRBDGDA_BD_DATE,
                @TRBDGDA_BD_TIME, 
                @TRBDGDA_BD_INOUT,
                @TRBDGDA_BD_READER, 
                @TRBDGDA_BD_CHKHS, 
                @TRBDGDA_BD_SUBAREA, 
                @TRBDGDA_BD_PNO)";

                    var parameters = new
                    {
                        TRBDGDA_BD_DATE = ddMMyy,
                        TRBDGDA_BD_TIME = ConvertTimeToMinutes(tmIn),
                        TRBDGDA_BD_INOUT = "I",
                        TRBDGDA_BD_READER = "2",
                        TRBDGDA_BD_CHKHS = "2",
                        TRBDGDA_BD_SUBAREA = "JUSC12",
                        TRBDGDA_BD_PNO = Pno
                    };

                    connection.Execute(query, parameters);

                    var Punchquery = @"
                INSERT INTO T_TRPUNCHDATA_EARS(PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_INOUT,PDE_MACHINEID,
                PDE_READERNO,PDE_CHKHS,PDE_SUBAREA,PDE_PSRNO) 
                VALUES 
                (@PDE_PUNCHDATE,
                @PDE_PUNCHTIME, 
                @PDE_INOUT,
                @PDE_MACHINEID, 
                @PDE_READERNO, 
                @PDE_CHKHS, 
                @PDE_SUBAREA, 
                @PDE_PSRNO)";

                    var parameters2 = new
                    {
                        PDE_PUNCHDATE = ddMMyy,
                        PDE_PUNCHTIME = tmIn,
                        PDE_INOUT = "I",
                        PDE_MACHINEID = "2",
                        PDE_READERNO = "2",
                        PDE_CHKHS = "2",
                        PDE_SUBAREA = "JUSC12",
                        PDE_PSRNO = Pno
                    };

                    connection.Execute(Punchquery, parameters2);
                }

                if (!string.IsNullOrEmpty(tmOut))
                {
                    var queryOut = @"
                INSERT INTO T_TRBDGDAT_EARS(TRBDGDA_BD_DATE, TRBDGDA_BD_TIME, TRBDGDA_BD_INOUT, TRBDGDA_BD_READER, 
                 TRBDGDA_BD_CHKHS, TRBDGDA_BD_SUBAREA, TRBDGDA_BD_PNO) 
                VALUES 
                (@TRBDGDA_BD_DATE,
                @TRBDGDA_BD_TIME, 
                @TRBDGDA_BD_INOUT, 
                @TRBDGDA_BD_READER, 
                @TRBDGDA_BD_CHKHS,
                @TRBDGDA_BD_SUBAREA,
                @TRBDGDA_BD_PNO)";

                    var parametersOut = new
                    {
                        TRBDGDA_BD_DATE = ddMMyy,
                        TRBDGDA_BD_TIME = ConvertTimeToMinutes(tmOut),
                        TRBDGDA_BD_INOUT = "O",
                        TRBDGDA_BD_READER = "2",
                        TRBDGDA_BD_CHKHS = "2",
                        TRBDGDA_BD_SUBAREA = "JUSC12",
                        TRBDGDA_BD_PNO = Pno
                    };

                    connection.Execute(queryOut, parametersOut);

                    var Punchquery = @"
                INSERT INTO T_TRPUNCHDATA_EARS(PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_INOUT,PDE_MACHINEID,
                PDE_READERNO,PDE_CHKHS,PDE_SUBAREA,PDE_PSRNO) 
                VALUES 
                (@PDE_PUNCHDATE,
                @PDE_PUNCHTIME, 
                @PDE_INOUT,
                @PDE_MACHINEID, 
                @PDE_READERNO, 
                @PDE_CHKHS, 
                @PDE_SUBAREA, 
                @PDE_PSRNO)";

                    var parameters2 = new
                    {
                        PDE_PUNCHDATE = ddMMyy,
                        PDE_PUNCHTIME = tmOut,
                        PDE_INOUT = "O",
                        PDE_MACHINEID = "2",
                        PDE_READERNO = "2",
                        PDE_CHKHS = "2",
                        PDE_SUBAREA = "JUSC12",
                        PDE_PSRNO = Pno
                    };

                    connection.Execute(Punchquery, parameters2);
                }
            }
        }

      
        private int ConvertTimeToMinutes(string time)
        {
            var strtm = time.Split(':');
            return (Convert.ToInt32(strtm[0]) * 60) + Convert.ToInt32(strtm[1]);
        }

        public IActionResult AttendanceReport(string url)
        {
            
            string psrNo = HttpContext.Request.Cookies["Session"]; 

            if (string.IsNullOrEmpty(psrNo))
            {
                return RedirectToAction("Login", "User"); 
            }

           
            if (string.IsNullOrEmpty(url))
            {
                url = $"https://servicesdev.juscoltd.com/AttendanceReport/Webform1.aspx?pno={psrNo}";
               //url = $"https://localhost:44372/WebForm1.aspx?pno={psrNo}";
            }
            else
            {
                url += $"?pno={psrNo}";
            }

            ViewBag.ReportUrl = url; // Pass URL to View
            return View();
        }

        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(string Pno, string Name, string photoData)
        {
            if (!string.IsNullOrEmpty(photoData) && !string.IsNullOrEmpty(Pno) && !string.IsNullOrEmpty(Name))
            {
                try
                {
                   
                    byte[] imageBytes = Convert.FromBase64String(photoData.Split(',')[1]);

                   
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");

                 
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                  
                    string fileName = $"{Pno}-{Name}.jpg";
                    string filePath = Path.Combine(folderPath, fileName);

                   
                    System.IO.File.WriteAllBytes(filePath, imageBytes);

                   
                    var person = new AppPerson
                    {
                        Pno = Pno,
                        Name = Name,
                        Image = $"{fileName}" 
                    };

                    context.AppPeople.Add(person);
                    await context.SaveChangesAsync();

                    return RedirectToAction("GeoFencing");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving image: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Missing required fields!");
            }

            return View();
        }

        public IActionResult FaceRecognisation()
        
        {
             string storedImagePath = "wwwroot/Images/stored.jpg";
            string capturedImagePath = "wwwroot/Images/Captured.jpg";

            bool isFaceMatched = CompareFaces(storedImagePath, capturedImagePath);

            if (isFaceMatched)
            {
                Console.WriteLine("Face Matched!");
            }
            else
            {
                Console.WriteLine("Face Does Not Match!");
            }

            return View(); 
        }


        static bool CompareFaces(string storedImagePath, string capturedImagePath)
        {
            try
            {
                Mat storedImage = CvInvoke.Imread(storedImagePath, ImreadModes.Grayscale);
                Mat capturedImage = CvInvoke.Imread(capturedImagePath, ImreadModes.Grayscale);

                if (storedImage.IsEmpty || capturedImage.IsEmpty)
                {
                    Console.WriteLine("Error: One or both images are empty!");
                    return false;
                }

                string cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "D:/Irshad_Project/GFAS/GFAS/wwwroot/Cascades/haarcascade_frontalface_default.xml");
                Console.WriteLine($"Cascade Path: {cascadePath}");

                if (!System.IO.File.Exists(cascadePath))
                {
                    Console.WriteLine("Error: Haarcascade file not found!");
                    return false;
                }

                CascadeClassifier faceCascade = new CascadeClassifier(cascadePath);

                Rectangle[] storedFaces = faceCascade.DetectMultiScale(storedImage, 1.1, 5);
                Rectangle[] capturedFaces = faceCascade.DetectMultiScale(capturedImage, 1.1, 5);

                if (storedFaces.Length == 0 || capturedFaces.Length == 0)
                {
                    Console.WriteLine("No face detected in one or both images.");
                    return false;
                }

                Mat storedFace = new Mat(storedImage, storedFaces[0]);
                Mat capturedFace = new Mat(capturedImage, capturedFaces[0]);

                CvInvoke.Resize(storedFace, storedFace, new Size(100, 100));
                CvInvoke.Resize(capturedFace, capturedFace, new Size(100, 100));

                LBPHFaceRecognizer recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
                VectorOfMat trainingImages = new VectorOfMat();
                VectorOfInt labels = new VectorOfInt(new int[] { 1 });

                trainingImages.Push(storedFace);
                recognizer.Train(trainingImages, labels);

                var result = recognizer.Predict(capturedFace);

                Console.WriteLine($"Prediction Label: {result.Label}, Distance: {result.Distance}");

                return result.Label == 1 && result.Distance < 50; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in face comparison: " + ex.Message);
                return false;
            }
        }
    }

   
        
    
}
