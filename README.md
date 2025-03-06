<!-- Bootstrap CSS -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/css/bootstrap.min.css" rel="stylesheet">

<!-- jQuery (Required for Bootstrap) -->
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

<!-- Popper.js (Required for Bootstrap dropdowns) -->
<script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.11.6/dist/umd/popper.min.js"></script>

<!-- Bootstrap JS -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/js/bootstrap.min.js"></script>

<div class="col-sm-3">
    <div class="dropdown">
        <button class="btn btn-light form-control form-control-sm dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
            Select Position
        </button>
        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
            <!-- Search Box inside Dropdown -->
            <input type="text" class="form-control form-control-sm" id="searchDropdown" placeholder="Search...">
            <div id="dropdownItems" class="dropdown-list">
                <!-- Dropdown options will be generated here -->
            </div>
        </div>
    </div>
</div>

<script>
    $(document).ready(function () {
        var positions = @Html.Raw(Json.Serialize(ViewBag.PositionDDList)); // Convert ViewBag list to JavaScript array

        // Function to load dropdown options
        function loadDropdownItems(items) {
            var dropdownItems = $("#dropdownItems");
            dropdownItems.empty();
            items.forEach(function (item) {
                dropdownItems.append(`<a class="dropdown-item" href="#" data-value="${item.value}">${item.text}</a>`);
            });
        }

        // Load all positions initially
        loadDropdownItems(positions);

        // Show selected item in button text
        $(document).on("click", ".dropdown-item", function () {
            var selectedText = $(this).text();
            $("#dropdownMenuButton").text(selectedText);
        });

        // Filter dropdown based on search input
        $("#searchDropdown").on("keyup", function () {
            var searchText = $(this).val().toLowerCase();
            var filteredItems = positions.filter(item => item.text.toLowerCase().includes(searchText));
            loadDropdownItems(filteredItems);
        });
    });
</script>



<div class="col-sm-3">
    <div class="dropdown">
        <button class="btn btn-light form-control form-control-sm dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
            Select Position
        </button>
        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
            <!-- Search Box inside Dropdown -->
            <input type="text" class="form-control form-control-sm" id="searchDropdown" placeholder="Search...">
            <div id="dropdownItems" class="dropdown-list">
                <!-- Dropdown options will be generated here -->
            </div>
        </div>
    </div>
</div>

<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script>
    $(document).ready(function () {
        var positions = @Html.Raw(Json.Serialize(ViewBag.PositionDDList)); // Convert ViewBag list to JavaScript array

        // Function to load dropdown options
        function loadDropdownItems(items) {
            var dropdownItems = $("#dropdownItems");
            dropdownItems.empty();
            items.forEach(function (item) {
                dropdownItems.append(`<a class="dropdown-item" href="#" data-value="${item.value}">${item.text}</a>`);
            });
        }

        // Load all positions initially
        loadDropdownItems(positions);

        // Show selected item in button text
        $(document).on("click", ".dropdown-item", function () {
            var selectedText = $(this).text();
            $("#dropdownMenuButton").text(selectedText);
        });

        // Filter dropdown based on search input
        $("#searchDropdown").on("keyup", function () {
            var searchText = $(this).val().toLowerCase();
            var filteredItems = positions.filter(item => item.text.toLowerCase().includes(searchText));
            loadDropdownItems(filteredItems);
        });
    });
</script>



<script>
    $(document).ready(function () {
        $(".searchable-dropdown").select2({
            placeholder: "Select a Position",
            allowClear: true
        });
    });
</script>

 <div class="col-sm-3">
    <select asp-for="Position" asp-items="@ViewBag.PositionDDList" class="form-control form-control-sm searchable-dropdown">
        <option value=""></option>
    </select>
</div>

<link href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/jquery@3.6.0/dist/jquery.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>



 
 <div class="col-sm-3">
   
     <select asp-for="Position" asp-items="@ViewBag.PositionDDList" class="form-control form-control-sm custom-select" id="Position">
         <option value=""></option>
     </select>
     
 </div>
