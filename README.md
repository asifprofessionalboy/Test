var totalBenefitsquery = "select count(distinct Master_ID) as TotalBenefits from App_Innovation_Benefits";

var totalsafetyquery = "select count(distinct IB.Master_Id) as safety from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+ "' and Status = 'Approved' and Benefits  like '%safe%'";
var totalsustainquery = "select count(distinct IB.Master_Id) as Others from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+ "'and Status = 'Approved'  and Benefits  like '%sustain%'";
 var totalothersquery = "select count(distinct IB.Master_Id) as Others from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn>='"+startDate+"'and CreatedOn<='"+endDate+ "' and Status = 'Approved' and Benefits not like '%sustain%' and Benefits NOT like '%safe%'";

 int totalsustain = connection.QuerySingleOrDefault<int>(totalsustainquery);
 int totalsafety = connection.QuerySingleOrDefault<int>(totalsafetyquery);
 int totalothers = connection.QuerySingleOrDefault<int>(totalothersquery);
 
 ViewBag.totalsafety = totalsafety;
 ViewBag.totalsustainability = totalsustain;
 ViewBag.totalothers = totalothers;



this is my queries to count safety, sustainability and Other benefits of a Innovation. in this i want a condition that firstly it total no. of benefits then on second query it finds the value safety , then second it executes second query from totalbenefits whose value is not safety then 3rd query. In Simple words i mean to say that , let i have 160 Total Benefits , It finds safety data like 60 then the  2nd query who is finding the sustain that is finds from that rest 100 benefits if sustain is 50 then the rest data will be others
