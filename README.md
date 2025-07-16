PageRecordDataSet.Merge(ds);

string failedWorkorderstring = string.Empty;
List<string> validWorkOrders = new List<string>();
List<string> failedWorkorder = new List<string>();
List<string> overCountWarnings = new List<string>(); // 👈 Add this

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

        var categoryCount = (from r in distinctWorkmen.AsEnumerable()
                             let cat = r["WorkManCategory"].ToString().Trim().ToUpper().Replace(" ", "")
                             where !string.IsNullOrEmpty(cat)
                             group r by cat into g
                             select new
                             {
                                 Key = g.Key,
                                 Count = g.Count()
                             }).ToDictionary(x => x.Key, x => x.Count);

        foreach (DataRow row in Ds2.Tables[0].Rows)
        {
            string category = row["EMP_TYPE"].ToString().Trim().ToUpper().Replace(" ", "");  // Normalize
            int requiredCount = 0;

            // Safe parse for Total column
            if (row["Total"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["Total"].ToString()))
            {
                int.TryParse(row["Total"].ToString(), out requiredCount);
            }

            int actualCount = categoryCount.ContainsKey(category) ? categoryCount[category] : 0;

            Console.WriteLine($"Checking category: {category}, Required: {requiredCount}, Actual: {actualCount}");

            if (actualCount > requiredCount)
            {
                failedWorkorder.Add(workOrder);
                overCountWarnings.Add($"{workOrder} - {category}"); // 👈 Log with category
            }
        }
    }
}

// Create output strings
failedWorkorderstring = string.Join(",", failedWorkorder);
string overCountMessage = string.Join("\n", overCountWarnings); // For showing line by line

// Show message if any over-count found
if (!string.IsNullOrWhiteSpace(overCountMessage))
{
    MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Info, $"Following categories exceeded required count:\n{overCountMessage}");
}

		    
      
      

PageRecordDataSet.Merge(ds);




                    string failedWorkorderstring = string.Empty;
                    List<string> validWorkOrders = new List<string>();
                    List<string> failedWorkorder = new List<string>();

                    //HashSet<string> failedWorkorder = new HashSet<string>();

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

                            var categoryCount = (from r in distinctWorkmen.AsEnumerable()
                                                 let cat = r["WorkManCategory"].ToString().Trim().ToUpper()
                                                 where !string.IsNullOrEmpty(cat)
                                                 group r by cat into g
                                                 select new
                                                 {
                                                     Key = g.Key,
                                                     Count = g.Count()
                                                 }).ToDictionary(x => x.Key, x => x.Count);


                            //bool allCategoriesMet = true;

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
                                //if (requiredCount == 0)
                                //continue;

                                // Get actual count safely
                                int actualCount = categoryCount.ContainsKey(category) ? categoryCount[category] : 0;

                                // Debug print (optional)
                                Console.WriteLine($"Checking category: {category}, Required: {requiredCount}, Actual: {actualCount}");

                                if (actualCount > requiredCount)
                                {
                                    //allCategoriesMet = false;
                                    failedWorkorder.Add(workOrder);
                                    
                                    //break;
                                }
                            }

                            

                        }

                    }
                        //string validWorkOrderList = string.Join(",", failedWorkorder);
                    failedWorkorderstring = string.Join(",", failedWorkorder);



                    if(failedWorkorderstring != "" || failedWorkorderstring != null)

                    {
                        //MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Info, "WorkOrder : " + failedWorkorder + " Category :" + category );

                    }


here i want if actualCount > requiredCount  then add workorder with category like this and alert message show

 eg -    workorder - Category 

        4700024002 - Skilled
	4700024002 - Other
