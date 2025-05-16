query = query4.Where(a => a.Cdate.HasValue && a.Cdate.Value.ToString("dd-MM-yyyy").Contains(Date));

if (DateTime.TryParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
{
    query = query4.Where(a => a.Cdate.HasValue && a.Cdate.Value.Date == parsedDate.Date);
}

<td>
    @{
        if (item.ApprovedYn == true)
        {
            @:Approved
        }
        else if (item.ApprovedYn == false)
        {
            @:Rejected
        }
        else
        {
            @:Pending
        }
    }
</td>

<td>
    @(item.ApprovedYn == true ? "Approved" : item.ApprovedYn == false ? "Rejected" : "Pending")
</td>



var query4 = context.AppCoas.Where(x => x.Pno== session).AsQueryable();

 if (!string.IsNullOrEmpty(Date))
 {
     query = query4.Where(a => a.Cdate.Contains(Date));
 }

error : 'DateTime?' does not contain a definition for 'Contains' and the best extension method overload 'MemoryExtensions.Contains<string>(ReadOnlySpan<string>, string)' requires a receiver of type 'System.ReadOnlySpan<string>'


and in this query4 i want that check ApprovedYn value if it is false then show Rejected on grid if true show approved and if null then Show pending 

<td>@item.ApprovedYn</td>
