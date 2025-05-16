using System.Globalization;

// Parse the string into a DateTime from format "dd-MM-yyyy"
string dobString = data["Date of Birth"].ToString();  // e.g., "28-11-1985"
DateTime dob = DateTime.ParseExact(dobString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

// Store the DateTime object in the dataset (actual DateTime, not string)
PageRecordDataSet.Tables["App_EmployeeMaster"].Rows[0]["DOB"] = dob;




DateTime dobstr = data["Date of Birth"];
                             //DateTime dob = DateTime.ParseExact(dobstr,"dd-MM-yyyy",CultureInfo.InvariantCulture);
                            PageRecordDataSet.Tables["App_EmployeeMaster"].Rows[0]["AadharCard"] = data["Aadhar Number"];
                            PageRecordDataSet.Tables["App_EmployeeMaster"].Rows[0]["WorkManAddress"] = data["Address"];
                            //PageRecordDataSet.Tables["App_EmployeeMaster"].Rows[0]["DOB"] =  DateTime.ParseExact(data["Date of Birth"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            PageRecordDataSet.Tables["App_EmployeeMaster"].Rows[0]["DOB"] = dobstr;

                            PageRecordDataSet.Tables["App_EmployeeMaster"].Rows[0]["Father_Name"] = data["Father's name/Spouse Name"];
                            PageRecordDataSet.Tables["App_EmployeeMaster"].Rows[0]["Name"] = data["Name"];

date is coming like this in DateTime dobstr = data["Date of Birth"]; is in format of this  28-11-1985 this is coming as may be string 

and i want to store it in my PageRecordDataSet.Tables["App_EmployeeMaster"].Rows[0]["DOB"] and this column is in DateTime datatype i want to store in this like this format 28/11/1985 

