 function DOE_Validation(fld) {

          alert("ok");
          var DOJ = document.getElementById("MainContent_EmployeeMasterFormID_Record_DOJ_0").value;
          var DOE = document.getElementById("DOE").value;

          var DOJparts = DOJ.split("/");
          var DOEparts = DOE.split('/');

          var Date_Of_Joining = new Date(DOJparts[2], DOJparts[1] - 1, DOJparts[0]);
          var Date_Of_Exit = new Date(DOEparts[2], DOEparts[1] - 1, DOEparts[0]);

          //var Date_Of_Joining = new Date(parseInt(DoJparts[2], 10), parseInt(DOJparts[1], 10) - 1, parseInt(DOJparts[0], 10));
          //var Date_Of_Exit = new Date(parseInt(DOEparts[2], 10), parseInt(DOEparts[1], 10) - 1, parseInt(DOEparts[0], 10));

          alert(Date_Of_Joining);
          alert(Date_Of_Exit);
          var Current_Date = new Date();

       
          if (Date_Of_Exit >= Date_Of_Joining) {
              alert("Date of Exit Not less Than Date Of Joining.");
              document.getElementById("DOE").value = "";
              return false;
          }

          if (Date_Of_Exit > Current_Date) {
              alert("Date of Exit can't be greater than Current Date.");
              document.getElementById("DOE").value = "";
              return false;
          }

          return true;
      }
