if (actualCount > requiredCount)
{
    failedWorkorder.Add(workOrder);
    overCountWarnings.Add($"{workOrder}|{category}|{actualCount}|{requiredCount}");
}

if (overCountWarnings.Count > 0)
{
    string msg = $"Mismatches of No's of workers in workers' attendance count date: {str_date_to} Against the workorder No is found in Form C3:<br><br>";

    // Start HTML table
    msg += "<table border='1' cellpadding='4' cellspacing='0' style='border-collapse:collapse;'>";
    msg += "<tr style='background-color:#f2f2f2;'><th>WorkOrder</th><th>Category</th><th>Actual Count</th><th>Required Count</th></tr>";

    foreach (string item in overCountWarnings)
    {
        string[] parts = item.Split('|');
        if (parts.Length == 4)
        {
            msg += $"<tr><td>{parts[0]}</td><td>{parts[1]}</td><td>{parts[2]}</td><td>{parts[3]}</td></tr>";
        }
    }

    msg += "</table>";

    MyMsgBox.show(CLMS.Control.MyMsgBox.MessageType.Errors, msg);
}





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
