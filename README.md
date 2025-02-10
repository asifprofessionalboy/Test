this is my full logic
  if (!string.IsNullOrEmpty(FinYear3) || FinYear3 == "")
  {
      DateTime startDate = DateTime.MinValue;
      DateTime endDate = DateTime.MaxValue;


      if (FinYear3 == "24-25" || FinYear3 == "")
      {
          startDate = new DateTime(2024, 04, 1);
          endDate = new DateTime(2025, 03, 31);
      }
      else if (FinYear3 == "25-26")
      {
          startDate = new DateTime(2025, 04, 1);
          endDate = new DateTime(2026, 03, 31);
      }

			var totalBenefitsQuery = "select count(distinct Master_ID) as TotalBenefits from App_Innovation_Benefits as IB inner join App_Innovation as IA on IA.Id = IB.Master_ID where Status = 'Approved'";
			var totalsafetyQuery = "select count(distinct IB.Master_Id) as safety from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits like '%safe%'";
			var totalsustainQuery = "select count(distinct IB.Master_Id) as sustain from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits like '%sustain%'";
			var totalOthersQuery = "select count(distinct IB.Master_Id) as others from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits not like '%sustain%' and IB.Benefits not like '%safe%'";

			int totalBenefits = connection.QuerySingleOrDefault<int>(totalBenefitsQuery);
			int totalsafety = connection.QuerySingleOrDefault<int>(totalsafetyQuery);

			
			int remainingBenefits = totalBenefits - totalsafety;

			
			string adjustedSustainQuery = "select count(distinct IB.Master_Id) as sustain from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits like '%sustain%' and IB.Master_ID not in (select distinct IB.Master_ID from App_Innovation_Benefits as IB left join App_Innovation as IA on IB.Master_ID = IA.Id where IA.CreatedOn >= '" + startDate + "' and IA.CreatedOn <= '" + endDate + "' and IA.Status = 'Approved' and IB.Benefits like '%safe%')";
			int totalsustain = connection.QuerySingleOrDefault<int>(adjustedSustainQuery);

			int totalothers = remainingBenefits - totalsustain;

			ViewBag.totalsafety = totalsafety;
			ViewBag.totalsustainability = totalsustain;
			ViewBag.totalothers = totalothers;

		}


in this one issue is having that if there is no data in any finyear it shows me the count of totalothers , i dont want it , if there is no data in finyear then all query value is 0
