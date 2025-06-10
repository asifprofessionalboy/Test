this is my dropdown for punchIn and Punchout
<div class="col-sm-2">
    <select class="form-control">
        <option value="PunchIn">PunchIn</option>
        <option value="PunchOut">PunchOut</option>
    </select>
</div>

this is my query for PunchOut 

   SELECT  CONVERT(date, DateAndTime) AS AttemptDate,
                CASE 
                    WHEN PunchOut_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                    WHEN PunchOut_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                    WHEN PunchOut_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                    ELSE '10+'
                END AS AttemptRange,
                COUNT(DISTINCT Pno) AS NumberOfUsers
            FROM App_FaceVerification_Details
            WHERE CONVERT(date, DateAndTime) BETWEEN '2025-06-01' AND '2025-06-10'
            GROUP BY 
                CONVERT(date, DateAndTime),
                CASE 
                    WHEN PunchOut_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                    WHEN PunchOut_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                    WHEN PunchOut_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                    ELSE '10+'
                END
            ORDER BY AttemptDate desc

i want that if punchIn then first query executes and if punchout then this query executes 
