<table class="table-hover table-responsive-sm" cellspacing="0" cellpadding="4" id="MainContent_userPermissions" style="color:#333333;width:100%;border-collapse:collapse;">
    <tbody>
        <!-- Header Row -->
        <tr style="color:White;background-color:#49477a;font-size:Smaller;font-weight:bold;">
            <th align="left" scope="col">Form Name</th>
            <th scope="col">&nbsp;</th>
            <th scope="col">&nbsp;</th>
            <th scope="col">&nbsp;</th>
            <th scope="col">&nbsp;</th>
            <th scope="col">&nbsp;</th>
        </tr>

        @if (ViewBag.formList != null)
        {
            var formList = (List<dynamic>)ViewBag.formList;
            int rowIndex = 0;

            @foreach (var form in formList)
            {
                string bgColor = (rowIndex % 2 == 1 && rowIndex != 0) ? "#e3dff3" : "transparent";
                <tr style="color:#333333; background-color:@bgColor; font-size:Smaller;">
                    <td>
                        <span>@form.Description</span>
                    </td>
                    <td style="width:100px;">
                        <input type="checkbox"><label class="control-label">&nbsp;Read</label>
                    </td>
                    <td style="width:100px;">
                        <input type="checkbox"><label class="control-label">&nbsp;Create</label>
                    </td>
                    <td style="width:100px;">
                        <input type="checkbox"><label class="control-label">&nbsp;Modify</label>
                    </td>
                    <td style="width:100px;">
                        <input type="checkbox"><label class="control-label">&nbsp;Delete</label>
                    </td>
                    <td style="width:100px;">
                        <input type="checkbox"><label class="control-label">&nbsp;All</label>
                    </td>
                </tr>
                rowIndex++;
            }
        }
    </tbody>
</table>



 
var formList = context.AppFormDetails.Select(x => new
 {
     Pno = x.Id,
     Description = x.Description
 });
 ViewBag.formList = formList;


this is my table , make this dynamic , if Viewbag.formList has two data then two tr makes with description and for Bg color of the tr only on odds except first tr

  <table class="table-hover table-responsive-sm" cellspacing="0" cellpadding="4" id="MainContent_userPermissions" style="color:#333333;width:100%;border-collapse:collapse;">
      <tbody>
          <tr style="color:White;background-color:#49477a;font-size:Smaller;font-weight:bold;">
              <th align="left" scope="col">Form Name</th>
              <th scope="col">&nbsp;</th>
              <th scope="col">&nbsp;</th>
              <th scope="col">&nbsp;</th>
              <th scope="col">&nbsp;</th>
              <th scope="col">&nbsp;</th>
          </tr>
          <tr style="color:#333333;background-color:#e3dff3;font-size:Smaller;">
              <td>
                  <span>Description</span>
              </td>
              <td style="width:100px;">
                  <input type="checkbox"><label class="control-label">&nbsp;Read</label>
              </td>
              <td style="width:100px;">
                  <input type="checkbox"><label class="control-label">&nbsp;Create</label>
              </td>
              <td style="width:100px;">
                  <input type="checkbox"><label class="control-label">&nbsp;Modify</label>
              </td>
              <td style="width:100px;">
                  <input type="checkbox"><label class="control-label">&nbsp;Delete</label>
              </td>
              <td style="width:100px;">
                  <input type="checkbox"><label class="control-label">&nbsp;All</label>
              </td>
          </tr>
        

      </tbody>
  </table>
