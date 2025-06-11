                SELECT 
                    CONVERT(date, DateAndTime) AS AttemptDate,
                    CASE 
                        WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                        WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                        WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                        ELSE '10+'
                    END AS AttemptRange,
                    COUNT(DISTINCT Pno) AS NumberOfUsers
                FROM App_FaceVerification_Details
                WHERE CONVERT(date, DateAndTime) BETWEEN '2025-06-09' AND '2025-06-11'
                GROUP BY 
                    CONVERT(date, DateAndTime),
                    CASE 
                        WHEN PunchIn_FailedCount BETWEEN 0 AND 2 THEN '0-2'
                        WHEN PunchIn_FailedCount BETWEEN 3 AND 5 THEN '3-5'
                        WHEN PunchIn_FailedCount BETWEEN 6 AND 10 THEN '6-10'
                        ELSE '10+'
                    END
                ORDER BY AttemptDate


in this query NumberOfUsers are in count i want the average means percentage vise , if record is 700 and 0-2 is 600 then it shows in percentage wise for all
