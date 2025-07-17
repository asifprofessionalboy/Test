if (actualCount > requiredCount)
                                {
                                    //allCategoriesMet = false;
                                    failedWorkorder.Add(workOrder);
                                    //overCountWarnings.Add($"{workOrder} - {category}");

                                    overCountWarnings.Add($"{workOrder} - {category} -  {actualCount} - {requiredCount} ");// ?? Log with category
                                    //break;
                                }

failedWorkorderstring = string.Join(", ", overCountWarnings);
                    
                    string msg = string.Empty;
                    msg = "Mismatches of No's of workers in workesr's attendance count date : " + str_date_to + " Against the workorder No is  found in Form C3: <br>";

                    int k = 1;
                    string[] arr = failedWorkorderstring.Split(',');
                    HashSet<string> uniquePairs = new HashSet<string>();
                    string msg_data = "";
                    //int k = 1; 
                    for (int i = 0; i < arr.Length - 1; i += 2)

                    {
                        
                        msg += arr[i].Trim() + "<br>" + arr[i + 1].Trim() + "<br>";

                    }

 
                    if (failedWorkorderstring != "")
                    {
                        MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Errors, msg);

                    }



failedWorkorderstring = string.Join(", ", overCountWarnings); //  here i am getting data like 

4700022134 - UNSKILLED -  12 - 4 , 4700022134 - SEMI SKILLED -  6 - 0 , 4700022134 - HIGHLY SKILLED -  2 - 0 ,-  9 - 7 

i want msg like this :

Mismatches of No's of workers in workesr's attendance count date : 2025-07-17 Against the workorder No is  found in Form C3: 

insde table structure :

workOrder  category   		actualCount  	requiredCount
4700022134  UNSKILLED   	 12 		4
4700022134  SEMI SKILLED	 6		0
