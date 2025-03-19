protected void Page_Load(object sender, EventArgs e)
        {
            //ImprestCard_Records.DataSource = PageRecordsDataSet;



            if (!IsPostBack)
            {
                //GetRecords(GetFilterCondition(), ImprestCard_Records.PageSize, 10, "");
                //ImprestCard_Records.DataBind();

                string Pno = Session["UserName"].ToString();

                BL_ImprestCard_Request blobj = new BL_ImprestCard_Request();
                DataSet ds2 = new DataSet();

                ds2 = blobj.Chk_Pno(Pno);
             
                if(ds2.Tables[0].Rows.Count>0)
                {

                }

                else
                {
                    Response.Redirect("~/Default.aspx");
                }

            }
        }


this is my page load here i want to open modal when if(ds2.Tables[0].Rows.Count>0) means there is no data in ds2.tables.rows.count,if there is no value then open Modal . if there is value then redirect to homepage 

							Declaration for taking Imprest Card
Responsibilities of the card holder:

1.	The card holder will be responsible for Imprest card‘s security, its safe keeping & secrecy of PIN.
2.	Once the card is issued, it is not transferable.
3.	In case of loss of card, the card holder should immediately block the card & inform F&A. Card re-issuance charges are to be borne by the card holder only.
4.	ATM withdrawals in HDFC ATM is free of charges, non-HDFC ATM usage will be chargeable & the same will be charged to the concerned Imprest card. The charges are mentioned below.

Description of Charges	Amount
ATM withdrawal charges (HDFC Bank)	Free
ATM withdrawal charges (Non HDFC Bank)	Up to Rs.1000 – Rs.21 + GST
Above Rs.1000 – 1% + GST
(Cash withdrawal per transaction)
Balance Enquiry in HDFC & NON-HDFC ATMs
Online Check Free	Rs. 10 + 18% GST

hdfcbankprepaid.hdfcbank.com/hdfcportal/index


Please also note:
•	Card will be pre-loaded with the approved Imprest amount or to the extent of bills submitted for reimbursement whichever is earlier.
•	Expenses should be restricted to the items related to establishment. 
•	Items of regular use or repetitive nature should be catered through ARC only. If no such item code exists, then effort should be made to create them rather than processing these payments through Imprest card. 
•	No expenses for services should be made where Tax deducted at source (TDS) is applicable. 
•	 If any purchase made from registered GST supplier, invoice must contain company’s name (TSUISL), address, GST no. Taxable value, GST amount.
•	Expenses should be claimed by submission of appropriate documents/vouchers & time bound expense statement to be submitted to F&A for recoupment with HOD’s approval.
•	No cash payment to be made more than Rs. 10,000/- to single party on same day.
•	The cash in hand is subject to verification at any point of time.
•	In case of separation of employee, the employee needs to hand over the card and cash in hand, otherwise, the same will be recovered from the full & final settlement.
•	At the end of each FY cash in hand is to be handed over to the F&A cash office.
I have taken the possession of HDFC MONEY PLUS CARD bearing No…………………………………………. and understood the dos and don’ts, usage of card & the charges associated with it. I shall provide the information related to card and any required documents as and when required by F&A. 


                                                                   Agree  Next 
