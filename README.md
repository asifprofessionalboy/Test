	

Pno	DateAndTime	          PunchIn_FailedCount	PunchIn_Success	PunchOut_FailedCount	PunchOut_Success
159695	2025-06-10 08:56:42.527	             1	              1	                 0	             0
841738	2025-06-10 08:51:00.350	             0	              1	                 0	             0


 this is my table 

select * from App_FaceVerification_Details where CAST(DateAndTime AS DATE)='2025-06-10'  


i want a query to use on line graph to find out numbers of attempts over time 


in y axis i want number of user and in x axis i date wise and color of lines decide punchInfailedcount like 0-2, 3-5, 5-10 and 10-above 

i have a sample image to understand
