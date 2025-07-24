      public DataSet Get_Data(string strSql)
      {

          Dictionary<string, object> objParam = new Dictionary<string, object>();
          DataHelper dh = new DataHelper();
          return dh.GetDataset(strSql, "DataSet1", null);

      }
