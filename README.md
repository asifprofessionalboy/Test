var totalBenefitsQuery = @"SELECT COUNT(*) 
                           FROM App_Innovation 
                           WHERE Innovation IS NOT NULL AND Status = 'Approved' 
                           AND CreatedOn >= @startDate AND CreatedOn <= @endDate";
int totalBenefits = connection.QuerySingleOrDefault<int>(totalBenefitsQuery, new { startDate, endDate });

if (totalBenefits == 0)
{
    ViewBag.totalsafety = 0;
    ViewBag.totalsustainability = 0;
    ViewBag.totalothers = 0;
}
else
{
    var totalSafetyQuery = @"SELECT COUNT(*) 
                             FROM App_Innovation 
                             WHERE CreatedOn >= @startDate AND CreatedOn <= @endDate 
                             AND Innovation LIKE '%safe%' AND Status = 'Approved'";
    int totalSafety = connection.QuerySingleOrDefault<int>(totalSafetyQuery, new { startDate, endDate });

    var totalSustainQuery = @"SELECT COUNT(*) 
                              FROM App_Innovation 
                              WHERE CreatedOn >= @startDate AND CreatedOn <= @endDate 
                              AND Innovation LIKE '%sustain%' AND Status = 'Approved'";
    int totalSustain = connection.QuerySingleOrDefault<int>(totalSustainQuery, new { startDate, endDate });

    int totalOthers = totalBenefits - (totalSafety + totalSustain);

    ViewBag.totalsafety = totalSafety;
    ViewBag.totalsustainability = totalSustain;
    ViewBag.totalothers = totalOthers;
}





and this is my existing 

var totalBenefitsQuery = "select count(*) from App_Innovation where Innovation is not null and Status = 'Approved' and CreatedOn >= @startDate and CreatedOn <= @endDate";
int totalBenefits = connection.QuerySingleOrDefault<int>(totalBenefitsQuery, new { startDate, endDate });

if (totalBenefits == 0)
{
				
				ViewBag.totalsafety = 0;
				ViewBag.totalsustainability = 0;
				ViewBag.totalothers = 0;
}
else
{
				var totalsafetyQuery = "select Count(*) from App_Innovation where CreatedOn >= @startDate and CreatedOn <= @endDate and Innovation like '%safe%' and Status = 'Approved'";
				int totalsafety = connection.QuerySingleOrDefault<int>(totalsafetyQuery, new { startDate, endDate });

				int remainingBenefits = totalBenefits - totalsafety;

				string adjustedSustainQuery = "select count(distinct IB.Master_Id) as sustain from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= @startDate and IA.CreatedOn <= @endDate and IA.Status = 'Approved' and IB.Benefits like '%sustain%' and IB.Master_ID not in (select distinct IB.Master_ID from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= @startDate and IA.CreatedOn <= @endDate and IA.Status = 'Approved' and IB.Benefits like '%safe%')";
				int totalsustain = connection.QuerySingleOrDefault<int>(adjustedSustainQuery, new { startDate, endDate });

				int totalothers = remainingBenefits - totalsustain;

				ViewBag.totalsafety = totalsafety;
				ViewBag.totalsustainability = totalsustain;
				ViewBag.totalothers = totalothers;
}



i want to implement this query in place of above code , firstly it counts total Innovations and then it counts safe innovations and then sustain and rest is others 
select count(*) from App_Innovation where Innovation is not null and Status = 'Approved'

select Count(*) from App_Innovation where CreatedOn >= '04-01-2024 00:00:00' and 
CreatedOn <='03-31-2025 00:00:00' and Innovation  like '%safe%' and Status = 'Approved' 

select count(*) from App_Innovation where  CreatedOn >= '04-01-2024 00:00:00' and 
CreatedOn <='03-31-2025 00:00:00' and Innovation like '%sustain%' and Status = 'Approved'


