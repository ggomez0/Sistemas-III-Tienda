﻿using ShopGaspar.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShopGaspar.Admin
{
    public partial class lstcompradet : System.Web.UI.Page
    {
        private ProductContext _db = new ProductContext();
        string connectionString = ConfigurationManager.ConnectionStrings["ShopGaspar"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string nID = Request.QueryString["id1"];
                lblinvisible.Text = nID;
                lblord.Text += nID;
                this.databasecrud(connectionString, "SELECT ProductID as ID,ProductName as Producto,Description as " +
                    "Descripcion,UnitPrice as Precio,CategoryID,Stock,ProvID FROM Products", gvproductoslista);
                this.databasecrud(connectionString, "SELECT ProductName, UnitPrice, pr.ProvName, c.CategoryName, l.lstcpradetid FROM lstcompradetalles l" +
                    " inner join Products p on l.Product_ProductID=p.ProductID inner join proveedores pr on pr.ProvID=p.ProvID inner join Categories c" +
                    " on c.CategoryID=p.CategoryID where Lstcompra_lstcpraid =" + nID, gvlstcompradet);



            }
        }

        void databasecrud(string conexion, string sqlcomando, GridView tablag)
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(conexion))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlcomando, sqlCon);
                sqlDa.Fill(dtbl);
            }
            if (dtbl.Rows.Count > 0)
            {
                tablag.DataSource = dtbl;
                tablag.DataBind();
            }
            else
            {
                dtbl.Rows.Add(dtbl.NewRow());
                tablag.DataSource = dtbl;
                tablag.DataBind();
                tablag.Rows[0].Cells.Clear();
                tablag.Rows[0].Cells.Add(new TableCell());
                tablag.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                tablag.Rows[0].Cells[0].Text = "No se encontraron registros!";
                tablag.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
            tablag.UseAccessibleHeader = true;
            tablag.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void gvproductoslista_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try                    
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "insert into lstcompradetalles(cantidad,Product_ProductID,Lstcompra_lstcpraid) values (0,@product,@lstcompra);";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);                    
                    sqlCmd.Parameters.AddWithValue("@lstcompra", lblinvisible.Text);
                    sqlCmd.Parameters.AddWithValue("@product", Convert.ToInt32(gvproductoslista.DataKeys[e.RowIndex].Value.ToString()));

                    sqlCmd.ExecuteNonQuery();
                    gvproductoslista.EditIndex = -1;
                    this.databasecrud(connectionString, "SELECT * FROM Products", gvproductoslista);
                    lblSuccessMessage.Text = "Agregado con exito";
                    lblErrorMessage.Text = "";
                }
            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
            Response.Redirect(Request.RawUrl);

        }


        protected void gvlstcompradet_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "DELETE FROM lstcompradetalles WHERE lstcpradetid = @ProductID";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@ProductID", Convert.ToInt32(gvlstcompradet.DataKeys[e.RowIndex].Value.ToString()));
                    sqlCmd.ExecuteNonQuery();
                    this.databasecrud(connectionString, "SELECT * FROM lstcompradetalles", gvlstcompradet);

                }
            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;

            }
            Response.Redirect(Request.RawUrl);

        }

    

    }




}