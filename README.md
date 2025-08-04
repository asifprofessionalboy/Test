  <label class="form-label">Department</label>

  <div class="dropdown">
      <input class="form-control form-control-sm" placeholder="Select Depts"
             type="button" id="DeptDropdown" data-bs-toggle="dropdown" aria-expanded="false"/>

      <ul class="dropdown-menu w-100" aria-labelledby="DeptDropdown" id="locationList" style="max-height: 200px; overflow-y: auto;">
          @foreach (var item in ViewBag.DeptList as List<SelectListItem>)
          {
              <li style="margin-left:5%;">
                  <div class="form-check">
                      <input type="checkbox" class="form-check-input Dept-checkbox"
                             value="@item.Value" id="Dept_@item.Value" />
                      <label class="form-check-label" for="Dept_@item.Value">@item.Text</label>
                  </div>
              </li>
          }
      </ul>
  </div>

  <!-- Hidden dept field for form -->
  <input type="hidden" id="Dept" name="Coordinators[0].DeptName" />
