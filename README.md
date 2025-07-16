var categoryCount = (from r in distinctWorker.AsEnumerable()
                     let rawCat = r["JobMainCategory"] == DBNull.Value ? "" : r["JobMainCategory"].ToString()
                     let cat = rawCat.Trim().ToUpper()
                     where !string.IsNullOrEmpty(cat)
                     group r by cat into g
                     select new
                     {
                         Key = g.Key,
                         Count = g.Count()
                     }).ToDictionary(x => x.Key, x => x.Count);




// Step 1: Create dictionary with normalized category keys
var categoryCount = (from r in distinctWorker.AsEnumerable()
                     let cat = r["JobMainCategory"].ToString().Trim().ToUpper()
                     group r by cat into g
                     select new
                     {
                         Key = g.Key,
                         Count = g.Count()
                     }).ToDictionary(x => x.Key, x => x.Count);

// Step 2: Check required vs actual category counts
bool allCategoriesMet = true;

foreach (DataRow row in Ds2.Tables[0].Rows)
{
    string category = row["EMP_TYPE"].ToString().Trim().ToUpper();  // Normalize
    int requiredCount = 0;

    // Safe parse for Total column
    if (row["Total"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["Total"].ToString()))
    {
        int.TryParse(row["Total"].ToString(), out requiredCount);
    }

    // Skip if nothing required
    if (requiredCount == 0)
        continue;

    // Get actual count safely
    int actualCount = categoryCount.ContainsKey(category) ? categoryCount[category] : 0;

    // Debug print (optional)
    Console.WriteLine($"Checking category: {category}, Required: {requiredCount}, Actual: {actualCount}");

    if (actualCount < requiredCount)
    {
        allCategoriesMet = false;
        break;
    }
}

// Step 3: Add to valid list if all categories satisfied
if (allCategoriesMet)
{
    validWorkOrders.Add(workOrder);
}






// Merge PageRecordDataSet
PageRecordDataSet.Merge(ds);

// Final result variable to store valid workorders
List<string> validWorkOrders = new List<string>();

// Step 1: Get distinct WorkOrderNo
DataView workOrderView = new DataView(PageRecordDataSet.Tables[0]);
DataTable distinctWorkOrders = workOrderView.ToTable(true, "WorkOrderNo");

foreach (DataRow woRow in distinctWorkOrders.Rows)
{
    string workOrder = woRow["WorkOrderNo"].ToString();

    // Step 2: Get expected category-wise count from Ds2
    DataSet Ds2 = blobj.Get_WoNo_Chk(workOrder);
    if (Ds2 != null && Ds2.Tables.Count > 0 && Ds2.Tables[0].Rows.Count > 0)
    {
        // Step 3: Filter PageRecordDataSet for current WorkOrderNo
        DataView filteredView = new DataView(PageRecordDataSet.Tables[0]);
        filteredView.RowFilter = $"WorkOrderNo = '{workOrder}'";

        // Step 4: Get distinct AadharNo and WorkManCategory for current WorkOrder
        DataTable distinctWorkmen = filteredView.ToTable(true, "AadharNo", "WorkManCategory");

        // Step 5: Count how many workmen per category
        var categoryCount = distinctWorkmen.AsEnumerable()
            .GroupBy(r => r.Field<string>("WorkManCategory"))
            .ToDictionary(g => g.Key, g => g.Count());

        bool allCategoriesMet = true;

        foreach (DataRow row in Ds2.Tables[0].Rows)
        {
            string category = row["EMP_TYPE"].ToString();
            int requiredCount = Convert.ToInt32(row["Total"]);

            // Skip categories with zero requirement
            if (requiredCount == 0)
                continue;

            int actualCount = categoryCount.ContainsKey(category) ? categoryCount[category] : 0;

            if (actualCount < requiredCount)
            {
                allCategoriesMet = false;
                break;
            }
        }

        // Step 6: If all required categories are met, store workorder
        if (allCategoriesMet)
        {
            validWorkOrders.Add(workOrder);
        }
    }
}

// Final string of valid work orders
string validWorkOrderList = string.Join(",", validWorkOrders);




PageRecordDataSet.Merge(ds);

                    //sangita_7/15/2025
                    // workman category wise checking start

                    DataView view = new DataView(PageRecordDataSet.Tables[0]);
                    DataTable distinctTable = view.ToTable(true, "WorkOrderNo");

                    foreach (DataRow row in distinctTable.Rows)
                    {
                        string Workorder = row["WorkOrderNo"].ToString();

                        DataSet Ds2 = new DataSet();
                        Ds2 = blobj.Get_WoNo_Chk(Workorder);
                        
                        if (Ds2 != null && Ds2.Tables[0].Rows.Count > 0)
                        {
                            DataSet Ds2 = new DataSet();
                            Ds2 = blobj.Get_WoNo_Chk(Workorder);
                            DataView view1 = new DataView(PageRecordDataSet.Tables[0]);
                            DataTable distinctTable1 = view.ToTable(true, "WorkOrderNo", "AadharNo", "WorkManCategory");
                        }


                    }



here first i want to get distinct workorder from page record data set after getting workorder i want to get those workorder details which
 shows total count of workman category now i have to pass workorder and distinct adhar no  in pagerecord data set and count how many workmancategory was there and compare this
with total and when i found gather then total category wise then store those workorder no  over string variable

i have in Ds2 data  here catory is like HIGHLYSKILLED,SKILLED,SEMISKILLED,UNSKILLED having total count also present from which i have to compare from PageRecordData data			 :

AUTHOR_ADHAR_NUMBER	EMP_TYPE	Total
385515337557	HIGHLYSKILLED	0
385515337557	SKILLED	30
385515337557	SEMISKILLED	30
385515337557	UNSKILLED	10
385515337557	OTHERS	0

