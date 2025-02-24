this is my popup card , i want notification badge to show in this , count total no. of subjects like this 

<span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                                @ViewBag.UnreadNotifications["L2 KPIs - Technical Services"]
                            </span>

for all cards in the popup-content and shows on the popup1
<div class="col-sm-4">
     <a href="#popup1">
       

             <div class="card l-bg-blue-dark">
                 <div class="card-statistic-3 p-4">

                     <div class="mb-4">
                         <h6 class="card-title mb-0 head">

                             KPIs

                         </h6>
                     </div>

                 </div>
             </div>
     </a>
 </div>



<div id="popup1" class="popup-container">
    <div class="popup-content">
        <a href="#" class="close"><i class="fas fa-window-close"></i></a>
        <div class="row col-md-12 d-flex justify-content-center">
            <div class="col-sm-4">
                <a asp-action="ViewerForm" asp-route-L2="L2 KPIs - Technical Services">
                    <div class="card l-bg-cherry">
                        @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("L2 KPIs - Technical Services"))
                        {
                            <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                                @ViewBag.UnreadNotifications["L2 KPIs - Technical Services"]
                            </span>
                        }
                        <div class="card-statistic-3 p-4">

                            <div class="mb-4">
                                <h6 class="card-title mb-0" name="L2">
                                    L2 KPIs
                                </h6>
                            </div>
                            <div class="row align-items-center mb-2 d-flex">
                                <div class="col-12">
                                    <h7 class="d-flex align-items-center mb-0 head">
                                        Technical Services
                                    </h7>
                                </div>

                            </div>
                        </div>
                    </div>
                </a>
            </div>
            
           
        </div>
        <div class="row col-md-12">
            <div class="col-sm-3">
                <a asp-action="ViewerForm" asp-route-Admin="L3 KPIs - Admin & CC">
                    <div class="card l-bg-grey-dark">
                        @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("L3 KPIs - Admin & CC"))
                        {
                            <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                                @ViewBag.UnreadNotifications["L3 KPIs - Admin & CC"]
                            </span>
                        }
                        <div class="card-statistic-3 p-4">

                            <div class="mb-4">
                                <h6 class="card-title mb-0">

                                    L3 KPIs

                                </h6>
                            </div>
                            <div class="row align-items-center mb-2 d-flex">
                                <div class="col-8">
                                    <h7 class="d-flex align-items-center mb-0 head">
                                        Admin & CC
                                    </h7>
                                </div>

                            </div>

                        </div>
                    </div>
                </a>
            </div>
            <div class="col-sm-3">
                <a asp-action="ViewerForm" asp-route-Bidding="L3 KPIs - Bidding">
                    <div class="card l-bg-blue-dark">
                        @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("L3 KPIs - Bidding"))
                        {
                            <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                                @ViewBag.UnreadNotifications["L3 KPIs - Bidding"]
                            </span>
                        }
                        <div class="card-statistic-3 p-4">

                            <div class="mb-4">
                                <h6 class="card-title mb-0">

                                    L3 KPIs

                                </h6>
                            </div>
                            <div class="row align-items-center mb-2 d-flex">
                                <div class="col-12">
                                    <h7 class="d-flex align-items-center mb-0 head">
                                        Bidding
                                    </h7>
                                </div>
                            </div>
                        </div>
                    </div>
                </a>
            </div>
            <div class="col-sm-3">
                <a asp-action="ViewerForm" asp-route-BE="L3 KPIs - BE,Data Analytics,BD,CRM">
                    <div class="card l-bg-orange-dark">
                        @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("L3 KPIs - BE,Data Analytics,BD,CRM"))
                        {
                            <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                                @ViewBag.UnreadNotifications["L3 KPIs - BE,Data Analytics,BD,CRM"]
                            </span>
                        }
                        <div class="card-statistic-3 p-4">

                            <div class="mb-4">
                                <h6 class="card-title mb-0">

                                    L3 KPIs

                                </h6>
                            </div>
                            <div class="row align-items-center mb-2 d-flex">
                                <div class="col-12">
                                    <h7 class="d-flex align-items-center mb-0 head">
                                        BE, Data Analytics, BD, CRM
                                    </h7>
                                </div>
                            </div>
                        </div>
                    </div>
                </a>
            </div>
            <div class="col-sm-3">
                <a asp-action="ViewerForm" asp-route-DETP="L3 KPIs - DETP">
                    <div class="card l-bg-green-dark">
                        @if (ViewBag.UnreadNotifications != null && ViewBag.UnreadNotifications.ContainsKey("L3 KPIs - DETP"))
                        {
                            <span class="badge rounded-pill badge-notification bg-danger position-absolute top-0 end-0 m-2">
                                @ViewBag.UnreadNotifications["L3 KPIs - DETP"]
                            </span>
                        }
                        <div class="card-statistic-3 p-4">

                            <div class="mb-4">
                                <h6 class="card-title mb-0">

                                    L3 KPIs

                                </h6>
                            </div>
                            <div class="row align-items-center mb-2 d-flex">
                                <div class="col-12">
                                    <h7 class="d-flex align-items-center mb-0 head">
                                        DETP
                                    </h7>
                                </div>

                            </div>

                        </div>
                    </div>
                </a>
            </div>
            
            
        </div>
        
       
       
    </div>
</div>
