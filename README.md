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
