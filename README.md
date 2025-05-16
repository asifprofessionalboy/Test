var query4 = context.AppCoas.Where(x => x.Pno== session).AsQueryable();

 if (!string.IsNullOrEmpty(Date))
 {
     query = query4.Where(a => a.Cdate.Contains(Date));
 }

error : 'DateTime?' does not contain a definition for 'Contains' and the best extension method overload 'MemoryExtensions.Contains<string>(ReadOnlySpan<string>, string)' requires a receiver of type 'System.ReadOnlySpan<string>'


and in this query4 i want that check ApprovedYn value if it is false then show Rejected on grid if true show approved and if null then Show pending 

<td>@item.ApprovedYn</td>
