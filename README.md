  DataSet ds = strQuery;

 if (ds != null && ds.Tables.Count > 0)
 {
      Mis_Records.DataSource = ds;
    Mis_Records.DataBind();
  }
