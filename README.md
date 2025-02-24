Dictionary<string, string> subjects = new Dictionary<string, string>();

if (!string.IsNullOrEmpty(Bidding)) subjects["Bidding"] = Bidding;
if (!string.IsNullOrEmpty(DETP)) subjects["DETP"] = DETP;
if (!string.IsNullOrEmpty(BE)) subjects["BE"] = BE;
if (!string.IsNullOrEmpty(Admin)) subjects["Admin"] = Admin;
if (!string.IsNullOrEmpty(MD)) subjects["MD"] = MD;
if (!string.IsNullOrEmpty(L2)) subjects["L2"] = L2;
if (!string.IsNullOrEmpty(Flash)) subjects["Flash"] = Flash;
if (!string.IsNullOrEmpty(Exception)) subjects["Exception"] = Exception;
if (!string.IsNullOrEmpty(Bi2nd)) subjects["Bi2nd"] = Bi2nd;
if (!string.IsNullOrEmpty(Bi4th)) subjects["Bi4th"] = Bi4th;
if (!string.IsNullOrEmpty(BDWeek)) subjects["BDWeek"] = BDWeek;

ViewBag.Subjects = subjects;

@foreach (var subject in ViewBag.Subjects as Dictionary<string, string>)
{
    <input type="hidden" name="@subject.Key" value="@subject.Value" />
}

<div class="card-header text-center" style="background-color:#49477a;color:white;font-weight:bold;">
    @string.Join(" ", (ViewBag.Subjects as Dictionary<string, string>).Values)
</div>




for these 
i want to use subject 

 ViewBag.Subjects = subjects;


<input type="hidden" name="L2" value="@ViewBag.L2" />
<input type="hidden" name="Bidding" value="@ViewBag.Bidding" />
<input type="hidden" name="Flash" value="@ViewBag.Flash" />
<input type="hidden" name="DETP" value="@ViewBag.DETP" />
<input type="hidden" name="BE" value="@ViewBag.BE" />
<input type="hidden" name="Admin" value="@ViewBag.Admin" />
<input type="hidden" name="MD" value="@ViewBag.MD" />
<input type="hidden" name="Exception" value="@ViewBag.Exception" />
<input type="hidden" name="Bi2nd" value="@ViewBag.Bi2nd" />
<input type="hidden" name="Bi4th" value="@ViewBag.Bi4th" />
<input type="hidden" name="BDWeek" value="@ViewBag.BDWeek" />

<div class="card-header text-center" style="background-color:#49477a;color:white;font-weight:bold;">@ViewBag.Bidding@ViewBag.L2@ViewBag.Flash@ViewBag.DETP
@ViewBag.BE@ViewBag.Admin@ViewBag.MD@ViewBag.Exception@ViewBag.Bi2nd@ViewBag.Bi4th@ViewBag.BDWeek
</div>
