using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Threading;

namespace LockStock
{
    public partial class frmLockStock : Form
    {
        public string connStr="";
        public string sql_ddxx = "";
        public string interID = "0";
        public string entryID = "0";
        public string itemID = "0";
        public string qty = "0";

        public SqlDataAdapter da_ddxx;
        public DataSet ds_ddxx;
        public SqlDataAdapter da_skxx;
        public DataSet ds_skxx;

        public frmLockStock()
        {
            InitializeComponent();
            try
            {
                //初始化连接字符串,选择的单据
                Init();

                da_ddxx = new SqlDataAdapter(sql_ddxx, connStr);
                ds_ddxx = new DataSet();
                da_ddxx.Fill(ds_ddxx, "ddxx");
                gvDDXX.DataSource = ds_ddxx.Tables["ddxx"];
                gvDDXX.ReadOnly = true;
                gvDDXX.AllowUserToAddRows = false;
                gvSKXX.AllowUserToAddRows = false;
                gvDDXX.Columns["单据内码"].Visible = false;
                gvDDXX.Columns["物料内码"].Visible = false;

            }
            catch (Exception Err)
            {
                MessageBox.Show("未知错误"+Err.StackTrace+",请联系管理员! \n"+Err.Message);
                Exit();
            }


        }

        private void Init()
        {
            string str = Read("conn");
            string dataSource = "",
                    cataLog = "",
                    userID = "",
                    password = "";

            string[] i = str.Split(';');
            string[] tmp;
            foreach (string j in i)
            {
                tmp = j.Split('=');
                if (tmp[0] == "Data Source") dataSource = tmp[1];
                if (tmp[0] == "Initial Catalog") cataLog = tmp[1];
                if (tmp[0] == "User ID") userID = tmp[1];
                if (tmp[0] == "Password") password = tmp[1];
            }
            if (dataSource == "" || cataLog == "" || userID == "")
            {
                MessageBox.Show("非法的连接字符串！");
                Exit();
            }
            connStr = "Data Source=" + dataSource + ";Initial Catalog=" + cataLog + ";User ID=" + userID + ";Password=" + password;

            string sql = "";
            string where = "";
            try
            {
                string text = Read("temp");
                DataTable dtID = SplitStr(text);
                dtID.TableName = "ddInfo";

                sql =
@"SELECT
     O.FBillNo AS [单据号], 
     OE.FInterID AS [单据内码],
     OE.FEntryID AS [单据分录],
     OE.FItemID AS [物料内码],
     I1.FName AS [客户名称], 
     ICI.FNumber AS [物料代码], 
     ICI.FName AS [物料名称], 
     U.FName AS [计量单位], 
     CONVERT(DECIMAL(18,2),OE.FQty) AS [数量], 
     CASE WHEN ISNULL(L.FQty, 0) > 0 THEN 'Y' ELSE '' END AS [锁库标志]
    FROM SEOrder AS O
 INNER JOIN SEOrderEntry AS OE ON O.FInterID = OE.FInterID
 INNER JOIN t_Item AS I1 ON I1.FItemID = O.FCustID
 INNER JOIN t_ICItem AS ICI ON ICI.FItemID = OE.FItemID
 INNER JOIN t_MeasureUnit AS U ON U.FItemID = ICI.FUnitID
 LEFT JOIN (
    SELECT
        FInterID,
        FEntryID,
        FItemID,
        SUM(FQty) AS FQty
    FROM t_LockStock AS LS
    GROUP BY FInterID,FEntryID,FItemID
 ) AS L ON OE.FInterID = L.FInterID AND OE.FEntryID = L.FEntryID AND OE.FItemID = L.FItemID
";

                where = "";
                if (dtID.Rows.Count > 0)
                {
                    where = "WHERE (1=0)";
                    foreach (DataRow dr in dtID.Rows)
                    {
                        where += " OR (OE.FInterID='" + dr["InterID"] + "' AND OE.FEntryID='" + dr["EntryID"] + "')";
                    }

                }


            }
            catch (Exception Err)
            {
                MessageBox.Show("未知错误" + Err.StackTrace + ",请联系管理员!\n" + Err.Message);
            }
            sql_ddxx = sql + where;
        }

        private void BTExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void BTUnLock_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow gr in gvSKXX.Rows)
                {

                    if (gr.Cells["选择"].Value != null && gr.Cells["选择"].Value.ToString() == "True")
                    {
                        if (interID != "" && entryID != "")
                        {
                            string sql =
    @"DELETE FROM t_LockStock WHERE FInterID=" + interID + @" AND FEntryID=" + entryID + @" AND FTranType=81
UPDATE SEOrderEntry SET FLockFlag=0 WHERE FInterID=" + interID + " AND FEntryID=" + entryID;
                            int i = ExcuteQuery(sql);
                            if (i > 0)
                            {
                                MessageBox.Show("成功解锁!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("无法获取单据内码,请重新选择单据!");
                        }
                    }
                }
                ds_ddxx = new DataSet();
                da_ddxx.Fill(ds_ddxx, "ddxx");
                gvDDXX.DataSource = ds_ddxx.Tables["ddxx"];
                ds_skxx = new DataSet();
                da_skxx.Fill(ds_skxx, "skxx");
                gvSKXX.DataSource = ds_skxx.Tables["skxx"];

            }
            catch (Exception Err)
            {
                MessageBox.Show("未知错误94,请联系管理管!\n" + Err.Message);
            }

        }

        private void BTLock_Click(object sender, EventArgs e)
        {
            try
            {
                string sql =
@"DELETE FROM t_LockStock WHERE FInterID=" + interID + @" AND FEntryID=" + entryID + @" AND FTranType=81
UPDATE SEOrderEntry SET FLockFlag=0 WHERE FInterID=" + interID + " AND FEntryID=" + entryID;

                int i = ExcuteQuery(sql);

                foreach (DataGridViewRow gr in gvSKXX.Rows)
                {

                    if (gr.Cells["选择"].Value != null && gr.Cells["选择"].Value.ToString() == "True")
                    {
                        if (interID != "" && entryID != "" && itemID != "")
                        {
                            if (Convert.ToDecimal(gr.Cells["本单锁库"].Value.ToString()) <= Convert.ToDecimal(gr.Cells["可锁数量"].Value.ToString()))
                            {

                                sql =
    @"INSERT INTO t_LockStock(FTranType, FInterID, FEntryID, FItemID, FStockID, FQty, FOrgQty,FBatchNo)
VALUES(
    81,
    " + interID + @",
    " + entryID + @",
    " + itemID + @",
    " + gr.Cells["FStockID"].Value.ToString() + @",
    " + gr.Cells["本单锁库"].Value.ToString() + @",
    " + gr.Cells["本单锁库"].Value.ToString() + @",
    '" + gr.Cells["批号"].Value.ToString() + @"'
)
UPDATE SEOrderEntry SET FLockFlag = 1 WHERE FInterID = " + interID + @" AND FEntryID = " + entryID;

                                i = ExcuteQuery(sql);
                                if (i > 0)
                                {
                                    MessageBox.Show("锁库成功!");
                                }
                                else
                                {
                                    MessageBox.Show("锁库失败,请重新选择数据!");
                                }

                            }
                            else
                            {
                                MessageBox.Show("锁库数量超过可锁数量!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("无法获取单据内码,请重新选择单据!");
                        }
                    }
                }

                ds_ddxx = new DataSet();
                da_ddxx.Fill(ds_ddxx, "ddxx");
                gvDDXX.DataSource = ds_ddxx.Tables["ddxx"];
                ds_skxx = new DataSet();
                da_skxx.Fill(ds_skxx, "skxx");
                gvSKXX.DataSource = ds_skxx.Tables["skxx"];
                


            }
            catch (Exception Err)
            {
                MessageBox.Show("未知错误" + Err.StackTrace + ",请联系管理管!\n" + Err.Message);
            }

        }


        private void GVddxx_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (gvSKXX.Columns["选择"] == null)
                {
                    DataGridViewCheckBoxColumn xz = new DataGridViewCheckBoxColumn();
                    xz.Name = "选择";
                    gvSKXX.Columns.Add(xz);
                }
                interID = gvDDXX.Rows[e.RowIndex].Cells["单据内码"].Value.ToString();
                entryID = gvDDXX.Rows[e.RowIndex].Cells["单据分录"].Value.ToString();
                itemID = gvDDXX.Rows[e.RowIndex].Cells["物料内码"].Value.ToString();
                qty = gvDDXX.Rows[e.RowIndex].Cells["数量"].Value.ToString();


                string sql = MakeSQL_skxx();

                if (sql != "")
                {
                    da_skxx = new SqlDataAdapter(sql, connStr);
                    ds_skxx = new DataSet();
                    da_skxx.Fill(ds_skxx, "skxx");

                    gvSKXX.DataSource = ds_skxx.Tables["skxx"];
                    gvSKXX.Columns["选择"].ReadOnly = false;
                    gvSKXX.Columns["仓库代码"].ReadOnly = true;
                    gvSKXX.Columns["仓库名称"].ReadOnly = true;
                    gvSKXX.Columns["库存数量"].ReadOnly = true;
                    gvSKXX.Columns["本单锁库"].ReadOnly = true;
                    gvSKXX.Columns["其他锁库"].ReadOnly = true;
                    gvSKXX.Columns["可锁数量"].ReadOnly = true;
                    gvSKXX.Columns["FItemID"].Visible = false;
                    gvSKXX.Columns["FStockID"].Visible = false;
                }
            }
            catch (Exception Err)
            {
                MessageBox.Show("未知错误197,请联系管理员! \n" + Err.Message);
            }

        }


        private void GVskxx_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                if (gvSKXX.Columns[e.ColumnIndex].Name == "选择")
                {
                    if (gvSKXX.Rows[e.RowIndex].Cells["选择"].Value.ToString() == "True")
                    {
                        gvSKXX.Rows[e.RowIndex].Cells["本单锁库"].ReadOnly = false;
                        gvSKXX.Rows[e.RowIndex].Cells["本单锁库"].Style.BackColor = Color.Pink;
                        if (Convert.ToDecimal(gvSKXX.Rows[e.RowIndex].Cells["可锁数量"].Value) >=Convert.ToDecimal(qty))
                        {
                            gvSKXX.Rows[e.RowIndex].Cells["本单锁库"].Value = qty;
                        }
                        else
                        {
                            gvSKXX.Rows[e.RowIndex].Cells["本单锁库"].Value = gvSKXX.Rows[e.RowIndex].Cells["可锁数量"].Value;
                        }
                    }
                    else
                    {
                        gvSKXX.Rows[e.RowIndex].Cells["本单锁库"].ReadOnly = true;
                        gvSKXX.Rows[e.RowIndex].Cells["本单锁库"].Style.BackColor = Color.Empty;
                    }

                }
            }
            catch(Exception Err)
            {
                MessageBox.Show("未知错误" + Err.StackTrace + ",请联系管理员! \n" + Err.Message);
            }

        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">要执行的语句</param>
        /// <returns>受影响行数</returns>
        private int ExcuteQuery(string sql)
        {
            int i = 0;
            try
            {
                if (connStr == "")
                {
                    MessageBox.Show("连接字符串获取失败,请重试!");
                    Exit();
                }
                else
                {
                    if (sql != "")
                    {
                        SqlCommand cmd = new SqlCommand();
                        SqlConnection conn = new SqlConnection();
                        cmd.CommandType = CommandType.Text;
                        conn.ConnectionString = connStr;
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.CommandText = sql;

                        i = cmd.ExecuteNonQuery();
                        cmd.Connection.Close();

                    }
                    else
                    {
                        MessageBox.Show("SQL代码为空,不能执行!");
                    }
                }
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message);
            }
            return i;
        }

        private string MakeSQL_skxx()
        {
            string sql = "";
            if (interID != "" && entryID != "" && itemID != "")
            {
                sql =
@"DECLARE @InterID INT,
        @EntryID INT,
        @ItemID INT
SELECT  @InterID=" + interID + @"
       ,@EntryID=" + entryID + @"
       ,@ItemID=" + itemID + @"

SELECT
    S1.FNumber AS [仓库代码],
    ICI.FItemID,
    INV.FStockID,
    ICI.FName AS [物料名称],
    S1.FName AS [仓库名称],
    CONVERT(DECIMAL(18,2),INV.FQty) AS [库存数量],
    INV.FBatchNo AS [批号],
    CONVERT(DECIMAL(18,2),ISNULL(LS.FQty,0)) AS [本单锁库],
    CONVERT(DECIMAL(18,2),ISNULL(OLS.FQty,0)-ISNULL(LS.FQty,0)) AS [其他锁库],
    CONVERT(DECIMAL(18,2),INV.FQty-ISNULL(OLS.FQty,0)) AS [可锁数量]
FROM t_ICItem AS ICI
INNER JOIN ICInventory AS INV ON ICI.FItemID=INV.FItemID AND INV.FQty<>0
INNER JOIN t_Stock AS S1 ON INV.FStockID=S1.FItemID
LEFT JOIN(
    SELECT
         LS.FStockID
        ,LS.FItemID
        ,LS.FBatchNo
        ,SUM(LS.FQty) AS FQty
    FROM t_LockStock AS LS
    WHERE LS.FInterID=@InterID AND LS.FEntryID=@EntryID
    GROUP BY LS.FStockID,LS.FItemID,LS.FBatchNo
    ) AS LS ON LS.FStockID=INV.FStockID AND LS.FItemID=INV.FItemID AND LS.FBatchNo=INV.FBatchNo
LEFT JOIN(
    SELECT
         LS.FStockID
        ,LS.FItemID
        ,LS.FBatchNo
        ,SUM(LS.FQty) AS FQty
    FROM t_LockStock AS LS
    GROUP BY LS.FStockID,LS.FItemID,LS.FBatchNo
    ) AS OLS ON OLS.FStockID=INV.FStockID AND OLS.FItemID=INV.FItemID  AND OLS.FBatchNo=INV.FBatchNo
WHERE ICI.FItemID=@ItemID";
                
            }
            else
            {
                MessageBox.Show("单据内码无法定位,请从新选择单据!");
            }
            return sql;
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件内容</returns>
        private string Read(string filename)
        {
            string text = "";
            //MessageBox.Show(System.AppDomain.CurrentDomain.BaseDirectory);
            try
            {
                StreamReader file = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + filename + ".txt", Encoding.Default);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    text = text + line;
                }
                file.Close();
                File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + filename + ".txt");

            }
            catch(Exception Err)
            {
                
                MessageBox.Show("文件读取错误！\n"+Err.Message);
                Exit();
            }

            return text;

        }

        /// <summary>
        /// 退出程序
        /// </summary>
        private void Exit()
        {
            System.Environment.Exit(0);
        }

        /// <summary>
        /// 将获取的单据内码和分录号转换为表
        /// </summary>
        /// <param name="text">连续字符串</param>
        /// <returns>对应的内码表表</returns>
        DataTable SplitStr(string text)
        {
            string[] i = text.Split('I');

            DataTable dtlist = new DataTable();
            DataRow dr;
            dtlist.Columns.Add("InterID");
            dtlist.Columns.Add("EntryID");
            foreach (string k in i)
            {
                if (k != "")
                {
                    dr = dtlist.NewRow();
                    dr.ItemArray = k.Split('E');
                    dtlist.Rows.Add(dr);
                }
            }

            return dtlist;

        }
    }
}
