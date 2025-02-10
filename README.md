var totalBenefitsQuery = "select count(distinct Master_ID) as TotalBenefits from App_Innovation_Benefits";
var totalsafetyQuery = "select count(distinct IB.Master_Id) as safety from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits like '%safe%'";
var totalsustainQuery = "select count(distinct IB.Master_Id) as sustain from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits like '%sustain%'";
var totalOthersQuery = "select count(distinct IB.Master_Id) as others from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits not like '%sustain%' and IB.Benefits not like '%safe%'";

int totalBenefits = connection.QuerySingleOrDefault<int>(totalBenefitsQuery);
int totalsafety = connection.QuerySingleOrDefault<int>(totalsafetyQuery);

// Calculate the remaining benefits after safety benefits are subtracted
int remainingBenefits = totalBenefits - totalsafety;

// Adjust the sustainability query to only search within the remaining benefits
string adjustedSustainQuery = "select count(distinct IB.Master_Id) as sustain from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits like '%sustain%' and IB.Master_ID not in (select distinct IB.Master_ID from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits like '%safe%')";
int totalsustain = connection.QuerySingleOrDefault<int>(adjustedSustainQuery);

// The remaining benefits are the "Others"
int totalothers = remainingBenefits - totalsustain;

ViewBag.totalsafety = totalsafety;
ViewBag.totalsustainability = totalsustain;
ViewBag.totalothers = totalothers;


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
