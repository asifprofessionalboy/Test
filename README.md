string[] year = Fin_Year.SelectedValue.Split('-');
string Fin_Start = year[0];
string Fin_END = year[1];

string Fin_Start_Date = $"{Fin_Start}-04-01";
string Fin_End_Date = $"{Fin_END}-04-01";

string finYearRange = $"Start - {Fin_Start_Date} | End - {Fin_End_Date}";

ReportParameter Fin_Years = new ReportParameter("Fin_Year", finYearRange, true);
ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { Fin_Years });
