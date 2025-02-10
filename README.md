i have this two js , i want that if actionType is delete then it shows swal fire as data deleted successfully
if actiontype is save then shows data saved successfully
<script>

	document.getElementById('form2').addEventListener('submit', function (event) {
		event.preventDefault();

		var isValid = true;
		var elements = this.querySelectorAll('input, select, textarea');

		elements.forEach(function (element) {
			if (element.id === 'ApprovalFile' || element.id === 'Id' || element.id === 'fileInput' || element.id === 'dropdown-template' || element.id === 'status' || element.id === 'remarks' || element.id === 'StatusField' || element.id === 'Parameterid' || element.id === 'Paracode' || element.id === 'created' || element.id === 'ScoreId' || element.id === 'scorecode' || element.id === 'actionType' || element.id === 'daretotry' || element.id === 'daretotry-dropdown') {
				return;
			}


			if (element.value.trim() === '') {
				isValid = false;
				element.classList.add('is-invalid');
			} else {
				element.classList.remove('is-invalid');
			}
		});


		if (isValid) {
			Swal.fire({
				title: "Data Saved Successfully",
				width: 600,
				padding: "3em",
				color: "#28a745",
				background: "#fff",
				backdrop: `rgba(0,0,123,0.4)`,
				timer: 5000
			}).then(() => {
				this.submit();
			});
		}
	});
	</script>
<script>
	function setAction(actionValue) {
		document.getElementById('actionField').value = actionValue;
	}

</script>

this is my buttons 
<input type="hidden" name="action" id="actionField" />

<input type="submit" value="Save" class="btn" style="border-radius:7px" onclick="setAction('Save')" />
<input type="submit" value="Delete" class="btn" style="border: 1px solid;background: #f34848;padding:10px;border-radius:7px;" onclick="setAction('Delete')" />
