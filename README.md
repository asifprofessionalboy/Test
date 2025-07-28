<select name="Coordinators[0].DeptName" class="form-control" id="DeptName" required>
    <option value="">-- Select Department --</option>
    @foreach (var item in ViewBag.DeptList as List<SelectListItem>)
    {
        <option value="@item.Value">@item.Text</option>
    }
</select>
