using Dapper;
using System.Data;
using System.Data.SqlClient; // Use Oracle.ManagedDataAccess.Client for Oracle

public void StoreData(string ddMMyy, string tm, string tmOut, string Pno, string EntryType)
{
    using (var connection = new SqlConnection("Your_Connection_String"))
    {
        connection.Open();
        
        if (!string.IsNullOrEmpty(tm))
        {
            var query = @"
                INSERT INTO T_TRBDGDAT_EARS 
                (TRBDGDA_BD_DATE, TRBDGDA_BD_TIME, TRBDGDA_BD_INOUT, TRBDGDA_BD_READER, 
                 TRBDGDA_BD_CHKHS, TRBDGDA_BD_SUBAREA, TRBDGDA_BD_PNO) 
                VALUES 
                (@TRBDGDA_BD_DATE, @TRBDGDA_BD_TIME, @TRBDGDA_BD_INOUT, @TRBDGDA_BD_READER, 
                 @TRBDGDA_BD_CHKHS, @TRBDGDA_BD_SUBAREA, @TRBDGDA_BD_PNO)";

            var parameters = new
            {
                TRBDGDA_BD_DATE = ddMMyy,
                TRBDGDA_BD_TIME = ConvertTimeToMinutes(tm),
                TRBDGDA_BD_INOUT = "I",
                TRBDGDA_BD_READER = "2",
                TRBDGDA_BD_CHKHS = "2",
                TRBDGDA_BD_SUBAREA = "JUSC12",
                TRBDGDA_BD_PNO = Pno
            };

            connection.Execute(query, parameters);
        }

        if (!string.IsNullOrEmpty(tmOut))
        {
            var queryOut = @"
                INSERT INTO T_TRBDGDAT_EARS 
                (TRBDGDA_BD_DATE, TRBDGDA_BD_TIME, TRBDGDA_BD_INOUT, TRBDGDA_BD_READER, 
                 TRBDGDA_BD_CHKHS, TRBDGDA_BD_SUBAREA, TRBDGDA_BD_PNO) 
                VALUES 
                (@TRBDGDA_BD_DATE, @TRBDGDA_BD_TIME, @TRBDGDA_BD_INOUT, @TRBDGDA_BD_READER, 
                 @TRBDGDA_BD_CHKHS, @TRBDGDA_BD_SUBAREA, @TRBDGDA_BD_PNO)";

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
        }
    }
}

// Convert HH:mm format to total minutes
private int ConvertTimeToMinutes(string time)
{
    var parts = time.Split(':');
    return (Convert.ToInt32(parts[0]) * 60) + Convert.ToInt32(parts[1]);
}




{
  "success": false,
  "message": "Incorrect syntax near ':'."
}
