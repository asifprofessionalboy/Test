var categoryCount = (from r in distinctWorkmen.AsEnumerable()
                     let cat = r["WorkManCategory"].ToString().Trim().ToUpper().Replace(" ", "")
                     where !string.IsNullOrEmpty(cat)
                     group r by cat into g
                     select new
                     {
                         Key = g.Key,
                         Count = g.Count()
                     }).ToDictionary(x => x.Key, x => x.Count);

string category = row["EMP_TYPE"].ToString().Trim().ToUpper().Replace(" ", "");



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
                            var categoryCount = (from r in distinctWorkmen.AsEnumerable()
                                                 let cat = r["WorkManCategory"].ToString().Trim().ToUpper()
                                                 where !string.IsNullOrEmpty(cat)
                                                 group r by cat into g
                                                 select new
                                                 {
                                                     Key = g.Key,
                                                     Count = g.Count()
                                                 }).ToDictionary(x => x.Key, x => x.Count);

                            //var categoryCount = (from r in distinctWorkmen.AsEnumerable()
                            //                     let rawCat = r["WorkManCategory"] == DBNull.Value ? "" : r["WorkManCategory"].ToString()
                            //                     let cat = rawCat.Trim().ToUpper()
                            //                     where !string.IsNullOrEmpty(cat)
                            //                     group r by cat into g
                            //                     select new
                            //                     {
                            //                         Key = g.Key,
                            //                         Count = g.Count()
                            //                     }).ToDictionary(x => x.Key, x => x.Count);
                            
                            // Step 5: Check required vs actual category counts
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
                                //if (requiredCount == 0)
                                    //continue;

                                // Get actual count safely
                                int actualCount = categoryCount.ContainsKey(category) ? categoryCount[category] : 0;

                                // Debug print (optional)
                                Console.WriteLine($"Checking category: {category}, Required: {requiredCount}, Actual: {actualCount}");

                                if (actualCount < requiredCount)
                                {
                                    allCategoriesMet = false;
                                    //break;
                                }
                            }

                            // Final string of valid work orders
                            
                            if (allCategoriesMet)
                            {
                                validWorkOrders.Add(workOrder);
                            }
                        }


                    }
                    string validWorkOrderList = string.Join(",", validWorkOrders);


this is my full code here 
var categoryCount = (from r in distinctWorkmen.AsEnumerable()
                                                 let cat = r["WorkManCategory"].ToString().Trim().ToUpper()
                                                 where !string.IsNullOrEmpty(cat)
                                                 group r by cat into g
                                                 select new
                                                 {
                                                     Key = g.Key,
                                                     Count = g.Count()
                                                 }).ToDictionary(x => x.Key, x => x.Count);

category is  came as - SEMI SKILLED


and string category = row["EMP_TYPE"].ToString().Trim().ToUpper();

here category is  came as - SEMISKILLED  that is the reason i am getting wrong actual category count how to resolve it         
