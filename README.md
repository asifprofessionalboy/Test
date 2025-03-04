i have this two buttons 

<form asp-action="GeoFencing" id="form" asp-controller="Geo" >
<div class="row mt-5 form-group" style="margin-top:50%;">
    <div class="col d-flex justify-content-center ">
        <button class="Btn form-group" id="PunchIn">
            Punch In
        </button>
    </div>

        <div class="col d-flex justify-content-center form-group">
            <button class="Btn2 form-group" id="PunchOut" style="">
                Punch Out
            </button>
            </div>
</div>
</form>

and this is my action method 
 [HttpPost]
 protected IActionResult DataSaved()
 {
  
     try
     {
        
     }
     catch (Exception ex)
     {

     }
    
 }
  
i want to use this below code in Dapper , when i punch in it executes if(tm!=null) if i punch out it executes tmout

 protected void StoreData(string ddMMyy, string tm, string tmOut, string Pno, string EntryType)
        {
            if (Convert.ToDateTime(ddMMyy).Month == System.DateTime.Now.Month)
            {
                int intTm = 0;
                int intTmout = 0;
                ArrayList insquery = new ArrayList();
                
                try
                {
                    if (tm != "")
                    {
                        string[] strtm;
                        strtm = tm.Split(':');

                        intTm = Convert.ToInt32(strtm[0]) * 60 + Convert.ToInt32(strtm[1]);

                        OracleCommand cmd = new OracleCommand("INSERT INTO T_TRBDGDAT_EARS (TRBDGDA_BD_DATE,TRBDGDA_BD_TIME,TRBDGDA_BD_INOUT,TRBDGDA_BD_READER,TRBDGDA_BD_CHKHS,TRBDGDA_BD_SUBAREA,TRBDGDA_BD_PNO) " +
                                                "VALUES (:TRBDGDA_BD_DATE,:TRBDGDA_BD_TIME,:TRBDGDA_BD_INOUT,:TRBDGDA_BD_READER,:TRBDGDA_BD_CHKHS,:TRBDGDA_BD_SUBAREA,:TRBDGDA_BD_PNO)");

                        
                       // cmd.Parameters.Add(":TRBDGDA_BD_DATE", ddMMyy);
                        cmd.Parameters.Add(":TRBDGDA_BD_DATE", ddMMyy);
                        cmd.Parameters.Add(":TRBDGDA_BD_TIME", intTm);
                        cmd.Parameters.Add(":TRBDGDA_BD_INOUT", "I");
                        cmd.Parameters.Add(":TRBDGDA_BD_READER", "2");
                        cmd.Parameters.Add(":TRBDGDA_BD_CHKHS", "2");
                        cmd.Parameters.Add(":TRBDGDA_BD_SUBAREA", "JUSC12");
                        cmd.Parameters.Add(":TRBDGDA_BD_PNO", Pno); //.Substring(2, 6)
                        insquery.Add(cmd);

                        cmd = new OracleCommand("INSERT INTO rfidjusco.T_TRPUNCHDATA_EARS (PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_INOUT,PDE_MACHINEID,PDE_READERNO,PDE_CHKHS,PDE_SUBAREA,PDE_PSRNO) " +
                                                "VALUES (:PDE_PUNCHDATE,:PDE_PUNCHTIME,:PDE_INOUT,:PDE_MACHINEID,:PDE_READERNO,:PDE_CHKHS,:PDE_SUBAREA,:PDE_PSRNO)");
                        cmd.Parameters.Add(":PDE_PUNCHDATE", ddMMyy);
                        cmd.Parameters.Add(":PDE_PUNCHTIME", tm);
                        cmd.Parameters.Add(":PDE_INOUT", "I");
                        cmd.Parameters.Add(":PDE_MACHINEID", "2");
                        cmd.Parameters.Add(":PDE_READERNO", "2");
                        cmd.Parameters.Add(":PDE_CHKHS", "2");
                        cmd.Parameters.Add(":PDE_SUBAREA", "JUSC12");
                        cmd.Parameters.Add(":PDE_PSRNO", Pno); //.Substring(2, 6)
                        insquery.Add(cmd);

                        cmd = new OracleCommand("INSERT INTO rfidview.T_TRPUNCHDATA_EARS_HIS (PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_INOUT,PDE_MACHINEID,PDE_READERNO,PDE_CHKHS,PDE_SUBAREA,PDE_PSRNO,REMARKS,CREATEDBY,REFNO) " +
                                               "VALUES (:PDE_PUNCHDATE,:PDE_PUNCHTIME,:PDE_INOUT,:PDE_MACHINEID,:PDE_READERNO,:PDE_CHKHS,:PDE_SUBAREA,:PDE_PSRNO,:REMARKS,:CREATEDBY,:REFNO)");
                        cmd.Parameters.Add(":PDE_PUNCHDATE", ddMMyy);
                        cmd.Parameters.Add(":PDE_PUNCHTIME", tm);
                        cmd.Parameters.Add(":PDE_INOUT", "I");
                        cmd.Parameters.Add(":PDE_MACHINEID", "2");
                        cmd.Parameters.Add(":PDE_READERNO", "2");
                        cmd.Parameters.Add(":PDE_CHKHS", "2");
                        cmd.Parameters.Add(":PDE_SUBAREA", "JUSC12");
                        cmd.Parameters.Add(":PDE_PSRNO", Pno);//.Substring(2, 6)
                        cmd.Parameters.Add(":REMARKS", EntryType);
                        cmd.Parameters.Add(":CREATEDBY", Session["UserName"].ToString());
                        cmd.Parameters.Add(":REFNO", "");
                        insquery.Add(cmd);
                        ExecuteNonQuery(insquery, true);
                    }
                    if (tmOut != "")
                    {
                        ArrayList insquery1 = new ArrayList();
                        string[] strtm;
                        strtm = tmOut.Split(':');

                        intTm = Convert.ToInt32(strtm[0]) * 60 + Convert.ToInt32(strtm[1]);

                        OracleCommand cmd = new OracleCommand("INSERT INTO T_TRBDGDAT_EARS (TRBDGDA_BD_DATE,TRBDGDA_BD_TIME,TRBDGDA_BD_INOUT,TRBDGDA_BD_READER,TRBDGDA_BD_CHKHS,TRBDGDA_BD_SUBAREA,TRBDGDA_BD_PNO) " +
                                               "VALUES (:TRBDGDA_BD_DATE,:TRBDGDA_BD_TIME,:TRBDGDA_BD_INOUT,:TRBDGDA_BD_READER,:TRBDGDA_BD_CHKHS,:TRBDGDA_BD_SUBAREA,:TRBDGDA_BD_PNO)");
                        cmd.Parameters.Add(":TRBDGDA_BD_DATE", ddMMyy);
                        cmd.Parameters.Add(":TRBDGDA_BD_TIME", intTm);
                        cmd.Parameters.Add(":TRBDGDA_BD_INOUT", "I");
                        cmd.Parameters.Add(":TRBDGDA_BD_READER", "2");
                        cmd.Parameters.Add(":TRBDGDA_BD_CHKHS", "2");
                        cmd.Parameters.Add(":TRBDGDA_BD_SUBAREA", "JUSC12");
                        cmd.Parameters.Add(":TRBDGDA_BD_PNO", Pno); //.Substring(2, 6)
                        insquery1.Add(cmd);
                        // for out time suggested by Promod on 23/07/2017
                        cmd = new OracleCommand("INSERT INTO rfidjusco.T_TRPUNCHDATA_EARS (PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_INOUT,PDE_MACHINEID,PDE_READERNO,PDE_CHKHS,PDE_SUBAREA,PDE_PSRNO) " +
                                               "VALUES (:PDE_PUNCHDATE,:PDE_PUNCHTIME,:PDE_INOUT,:PDE_MACHINEID,:PDE_READERNO,:PDE_CHKHS,:PDE_SUBAREA,:PDE_PSRNO)");
                        cmd.Parameters.Add(":PDE_PUNCHDATE", ddMMyy);
                        cmd.Parameters.Add(":PDE_PUNCHTIME", tmOut);
                        cmd.Parameters.Add(":PDE_INOUT", "I");
                        cmd.Parameters.Add(":PDE_MACHINEID", "2");
                        cmd.Parameters.Add(":PDE_READERNO", "2");
                        cmd.Parameters.Add(":PDE_CHKHS", "2");
                        cmd.Parameters.Add(":PDE_SUBAREA", "JUSC12");
                        cmd.Parameters.Add(":PDE_PSRNO", Pno); //.Substring(2, 6)
                        insquery1.Add(cmd);

                        cmd = new OracleCommand("INSERT INTO rfidview.T_TRPUNCHDATA_EARS_HIS (PDE_PUNCHDATE,PDE_PUNCHTIME,PDE_INOUT,PDE_MACHINEID,PDE_READERNO,PDE_CHKHS,PDE_SUBAREA,PDE_PSRNO,REMARKS,CREATEDBY,REFNO) " +
                                               "VALUES (:PDE_PUNCHDATE,:PDE_PUNCHTIME,:PDE_INOUT,:PDE_MACHINEID,:PDE_READERNO,:PDE_CHKHS,:PDE_SUBAREA,:PDE_PSRNO,:REMARKS,:CREATEDBY,:REFNO)");
                        cmd.Parameters.Add(":PDE_PUNCHDATE", ddMMyy);
                        cmd.Parameters.Add(":PDE_PUNCHTIME", tmOut);
                        cmd.Parameters.Add(":PDE_INOUT", "I");
                        cmd.Parameters.Add(":PDE_MACHINEID", "2");
                        cmd.Parameters.Add(":PDE_READERNO", "2");
                        cmd.Parameters.Add(":PDE_CHKHS", "2");
                        cmd.Parameters.Add(":PDE_SUBAREA", "JUSC12");
                        cmd.Parameters.Add(":PDE_PSRNO", Pno);//.Substring(2, 6)
                        cmd.Parameters.Add(":REMARKS", EntryType);
                        cmd.Parameters.Add(":CREATEDBY", Session["UserName"].ToString());
                        cmd.Parameters.Add(":REFNO", "");
                        insquery1.Add(cmd);
                        //****************************END*******************
                        ExecuteNonQuery(insquery1, true);
    
