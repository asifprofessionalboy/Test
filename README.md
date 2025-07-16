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

