if (((DropDownList)RequestGenerationRecord.Rows[0].FindControl("ShutdownType")).SelectedValue.ToString() == "type2" && ((DropDownList)RequestGenerationRecord.Rows[0].FindControl("Department")).SelectedItem.ToString() == "PSD-SK")
                {
                    string ShutdownStartDate= ((TextBox)RequestGenerationRecord.Rows[0].FindControl("ShutdownDate")).Text;
                    string ShutdownEndDate = ((TextBox)RequestGenerationRecord.Rows[0].FindControl("EndDate")).Text;
                    string CurrentDate = System.DateTime.Now.ToString();

                    DateTime currentTime = DateTime.Now; // Get the current server time
                    DateTime thresholdTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 12, 0, 0); // Set the threshold to 12 PM

                    if (currentTime > thresholdTime)
                    {
                        lblModalTitle.Text = "Warning!";
                        lblModalTitle.ForeColor = System.Drawing.Color.Red;
                        lblModalBody.Text = "You Can't Raise Request after 12:00 Pm ";
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
                        upModal.Update();
                        btnSave.Visible = false;
                        ((DropDownList)RequestGenerationRecord.Rows[0].FindControl("Department")).SelectedIndex = -1;
                    }
                    else
                    {

                        btnSave.Visible = true; // Enable the submit button
                    }

                }

this is my textbox 
string ShutdownStartDate= ((TextBox)RequestGenerationRecord.Rows[0].FindControl("ShutdownDate")).Text;

i want this condition that minimum value of start date is 2 days after that he can maximum any date , and validate if the value of start data is 16-07-2025 then he can submit before 2 days from this date means 14 and if the start date is 17 then it validates -2 days of the start date

