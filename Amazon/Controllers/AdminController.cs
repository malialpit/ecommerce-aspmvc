using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amazon.Controllers
{
    public class AdminController : Controller
    {
        DB.AmazonEntities db = new DB.AmazonEntities();
        SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Amazon"].ConnectionString);


        // GET: Admin
        public ActionResult manageuser()
        {
            List<Amazon.Models.marketuser> imd = new List<Amazon.Models.marketuser>();
            var query = (from data in db.user_master select data);
            foreach (var item in query)
                imd.Add(new Models.marketuser()
                {
                    name = item.name,
                    emailid = item.emailid,
                    password = item.password,
                    securityquestion = item.securityquestion,
                    answer = item.answer,
                });
            return View(imd);

        }
        [HttpGet]
        public ActionResult viewfeedback()
        {
            List<Amazon.Models.marketuser> imd = new List<Amazon.Models.marketuser>();
            var query = (from data in db.user_feedback select data);
            foreach (var item in query)
                imd.Add(new Models.marketuser()
                {
                    feedbackid = item.feedbackid,
                    name = item.name,
                    emailid = item.emailid,
                    contactno = item.contactno,
                    subject = item.subject,
                    dateofmsg = (DateTime)item.dateofmsg
                });



            return View(imd);
        }
        [HttpGet]
        public ActionResult updateuser(string name, string password, string emailid)
        {
            var cm = (from data in db.user_master where data.emailid == emailid select data).FirstOrDefault();
            cm.name = name;
            cm.password = password;
            db.SaveChanges();
            string result = "updated successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult deleteuser(string emailid)
        {
            var cm = (from data in db.user_master where data.emailid == emailid select data).FirstOrDefault();
            db.user_master.Remove(cm);
            db.SaveChanges();
            string result = "Deleted successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);


        }
        [HttpGet]
        public ActionResult deletefeedback(int feedbackid)
        {
            var cm = (from data in db.user_feedback where data.feedbackid == feedbackid select data).FirstOrDefault();
            db.user_feedback.Remove(cm);
            db.SaveChanges();
            //con.Open();
            //SqlCommand cmd = new SqlCommand("delete from user_feedback where FeedbackId='" + feedbackid + "'", con);
            //cmd.ExecuteNonQuery();
            //con.Close();
            string result = "Deleted Successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult deletecancelorder(int orderid)
        {
            var cm = (from data in db.order_details where data.orderid == orderid select data).FirstOrDefault();
            db.order_details.Remove(cm);
            db.SaveChanges();
            string result = "Deleted Successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult deletecategory(int categoryid)
        {
            var cm = (from data in db.product_category where data.categoryid == categoryid select data).FirstOrDefault();
            db.product_category.Remove(cm);
            db.SaveChanges();
            //con.Open();
            //SqlCommand cmd = new SqlCommand("delete from product_category where CategoryId='" + categoryid + "'", con);
            //cmd.ExecuteNonQuery();
            //con.Close();
            string result = "Deleted Successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult updatecategory(int categoryid, string category, string subcategory, string type)
        {
            var cm = (from data in db.product_category where data.categoryid == categoryid select data).FirstOrDefault();
            cm.category = category;
            cm.subcategory = subcategory;
            cm.type = type;
            db.SaveChanges();
            //con.Open();
            //SqlCommand cmd = new SqlCommand("update  product_category set category='"+category+"',subcategory='"+subcategory+"',type='"+type+"' where categoryid='"+categoryid+"'", con);
            //cmd.ExecuteNonQuery();
            //con.Close();
            string result = "Update Successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult fetchsubcategory(string category)
        {
            string result = "<option>Select</option>";
            var query = (from data in db.product_category
                         where data.category == category
                         select new
                         {
                             data.subcategory
                         }).Distinct();
            foreach (var item in query)
            {
                result += "<option>" + item.subcategory + "</option>";

            }

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult fetchtype(string category, string subcategory)
        {
            string result = "<option>Select</option>";
            var query = (from data in db.product_category
                         where data.category == category && data.subcategory == subcategory
                         select new
                         {
                             data.type
                         }).Distinct();
            foreach (var item in query)
            {
                result += "<option>" + item.type + "</option>";

            }

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult managecategory()
        {
            List<Amazon.Models.admin> md = new List<Models.admin>();
            var query = (from data in db.product_category select data);
            foreach (var item in query)
            {
                md.Add(new Models.admin()
                {
                    categoryid = item.categoryid,
                    type = item.type,
                    category = item.category,
                    subcategory = item.subcategory
                });
            }
            return View(md);
        }
        public ActionResult addproduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addproduct(Amazon.Models.admin md)
        {
            int categoryid = 1;
            var a = (from clist in db.product_category
                     where clist.category == md.category && clist.subcategory == md.subcategory
                     select new
                     {
                         clist.categoryid
                     }).ToList();
            if (a.FirstOrDefault() != null)
            {
                categoryid = a.FirstOrDefault().categoryid;
            }
            try
            {
                DB.product_master obj = new DB.product_master();
                obj.productid = md.productid;
                obj.categoryid = categoryid;
                obj.productname = md.productname;
                obj.mrp = md.mrp;
                obj.sellingprice = md.sellingprice;
                obj.brand = md.brand;
                obj.quantity = md.quantity;
                obj.size = md.size;
                obj.color = md.color;
                obj.shortdescp = md.shortdescp;
                obj.longdescp = md.longdescp;
                obj.productimage = "noimg.jpg";
                obj.sellerid = Session["adminid"].ToString();
                obj.updatedate = DateTime.Now;
                obj.addeddate = DateTime.Now;
                obj.status = md.status;
                db.product_master.Add(obj);
                db.SaveChanges();
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ViewBag.msg = "Successfully Submitted";
                ModelState.Clear();
            }

        }
        [HttpGet]
        public ActionResult updateproduct(int productid, string productname, string longdescp, string shortdescp, string color, string size, string brand, int quantity, string mrp, string sellingprice, string status)
        {
            var cm = (from data in db.product_master where data.productid == productid select data).FirstOrDefault();
            cm.productname = productname;
            cm.longdescp = longdescp;
            cm.shortdescp = shortdescp;
            cm.color = color;
            cm.size = size;
            cm.brand = brand;
            cm.quantity = quantity;
            cm.mrp = double.Parse(mrp);
            cm.sellingprice = double.Parse(sellingprice);
            cm.status = status;
            db.SaveChanges();

            string result = "Update Successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult manageproduct()
        {
            List<Amazon.Models.admin> imd = new List<Models.admin>();
            var query = (from data in db.product_master select data);
            foreach (var item in query)
                imd.Add(new Models.admin()
                {
                    productid = item.productid,
                    productname = item.productname,
                    mrp = (float)item.mrp,
                    sellingprice = (float)item.sellingprice,
                    brand = item.brand,
                    status = item.status,
                    quantity = (int)item.quantity,
                    updatedate = (DateTime)item.updatedate,
                    productimage = item.productimage

                });
            return View(imd);
        }
        public ActionResult viewproductdetails(int productid)
        {
            List<Amazon.Models.admin> imd = new List<Models.admin>();
            var a = (from plist in db.product_master
                     join clist in db.product_category on plist.categoryid equals clist.categoryid
                     where plist.productid == productid
                     select new
                     {
                         plist.productid,
                         clist.category,
                         clist.subcategory,
                         clist.type,
                         plist.productname,
                         plist.mrp,
                         plist.sellingprice,
                         plist.sellerid,
                         plist.brand,
                         plist.status,
                         plist.quantity,
                         plist.shortdescp,
                         plist.longdescp,
                         plist.color,
                         plist.size,
                         plist.addeddate,
                         plist.updatedate,
                         plist.productimage
                     });
            if (a.FirstOrDefault() != null)
            {
                ViewBag.productid = a.FirstOrDefault().productid;

                imd.Add(new Models.admin()
                {

                    category = a.FirstOrDefault().category,
                    subcategory = a.FirstOrDefault().subcategory,
                    type = a.FirstOrDefault().type,
                    productid = a.FirstOrDefault().productid,
                    productname = a.FirstOrDefault().productname,
                    mrp = (float)a.FirstOrDefault().mrp,
                    sellingprice = (float)a.FirstOrDefault().sellingprice,
                    brand = a.FirstOrDefault().brand,
                    status = a.FirstOrDefault().status,
                    quantity = (int)a.FirstOrDefault().quantity,
                    updatedate = (DateTime)a.FirstOrDefault().updatedate,
                    productimage = a.FirstOrDefault().productimage,
                    shortdescp = a.FirstOrDefault().shortdescp,
                    longdescp = a.FirstOrDefault().longdescp,
                    color = a.FirstOrDefault().color,
                    addeddate = (DateTime)a.FirstOrDefault().addeddate,
                    size = a.FirstOrDefault().size,
                    sellerid = a.FirstOrDefault().sellerid,
                });
            }
            return View(imd);
        }
        [HttpPost]
        public ActionResult imageupload(HttpPostedFileBase file, Amazon.Models.admin md)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string filename = "pimg" + md.productid + ".jpg";
                    string path = Path.Combine(Server.MapPath("~/productimage"),
                                               Path.GetFileName(filename));
                    file.SaveAs(path);

                    var cm = (from data in db.product_master where data.productid == md.productid select data).FirstOrDefault();
                    cm.productimage = filename;
                    db.SaveChanges();



                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return RedirectToAction("manageproduct", "Admin");
        }
        public ActionResult neworders()
        {
            List<Amazon.Models.admin> md = new List<Models.admin>();
            var query = (from data in db.order_details where data.orderstatus == "pending" select data);
            foreach (var item in query)
            {
                md.Add(new Models.admin()
                {
                    orderid = item.orderid,
                    orderdate = (DateTime)item.orderdate,
                    customerid = item.customerid,
                    totalprice = (float)item.totalprice
                });
            }
            return View(md);

        }
        public ActionResult progressorder()
        {
            List<Amazon.Models.admin> md = new List<Models.admin>();
            var query = (from data in db.order_details where data.orderstatus == "progress" select data);
            foreach (var item in query)
            {
                md.Add(new Models.admin()
                {
                    orderid = item.orderid,
                    orderdate = (DateTime)item.orderdate,
                    customerid = item.customerid,
                    totalprice = (float)item.totalprice
                });
            }
            return View(md);
        }
        public ActionResult deliveredorder()
        {
            List<Amazon.Models.admin> md = new List<Models.admin>();
            var query = (from data in db.order_details where data.orderstatus == "delivered" select data);
            foreach (var item in query)
            {
                md.Add(new Models.admin()
                {
                    orderid = item.orderid,
                    orderdate = (DateTime)item.orderdate,
                    customerid = item.customerid,
                    totalprice = (float)item.totalprice
                });
            }
            return View(md);
        }
        public ActionResult cancelorder()
        {
            List<Amazon.Models.admin> md = new List<Models.admin>();
            var query = (from data in db.order_details where data.orderstatus == "pending" select data);
            foreach (var item in query)
            {
                md.Add(new Models.admin()
                {
                    orderid = item.orderid,
                    orderdate = (DateTime)item.orderdate,
                    customerid = item.customerid,
                    totalprice = (float)item.totalprice
                });
            }
            return View(md);
        }
        public ActionResult viewneworder(int oid)
        {
            var cm = (from data in db.order_details where data.orderid == oid select data).FirstOrDefault();

            ViewBag.orderid = cm.orderid;
            ViewBag.customerid = cm.customerid;
            ViewBag.orderstatus = cm.orderstatus;
            ViewBag.orderdate = cm.orderdate;
            ViewBag.totalprice = cm.totalprice;
            ViewBag.shippingaddress = cm.shippingaddress;
            var productid = cm.productids.Split(',');
            List<Amazon.Models.admin> imd = new List<Models.admin>();

            foreach (var pid in productid)
            {
                int pid1 = int.Parse(pid);
                var u = (from data in db.product_master
                         where data.productid == pid1
                         select data).FirstOrDefault();
                var subtotalprice = (float)u.sellingprice * 1;
                imd.Add(new Amazon.Models.admin
                {
                    productid = u.productid,
                    productname = u.productname,
                    sellingprice = (float)u.sellingprice,
                    quantity = 1,
                    subtotalprice = subtotalprice,
                });
            }

            return View(imd);
        }
        [HttpGet]
        public ActionResult updateshipdata(int orderid, string courierno, string couriertype)
        {

            var cm = (from data in db.order_details where data.orderid == orderid select data).FirstOrDefault();
            cm.courierno = courierno;
            cm.couriertype = couriertype;
            cm.shippingdate = DateTime.Now.Date;
            cm.orderstatus = "progress";
            db.SaveChanges();
            string result = "updated successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult updateprogressdata(int orderid, string deliverby)
        {
            var dm = (from data in db.order_details where data.orderid == orderid select data).FirstOrDefault();
            dm.deliverby = deliverby;
            dm.orderstatus = "delivered";
            dm.delivereddate = DateTime.Now.Date;
            db.SaveChanges();
            string result = "updated successfully";
            return Json(new { result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult viewprogressorder(int oid)
        {
            var cm = (from data in db.order_details where data.orderid == oid select data).FirstOrDefault();

            ViewBag.orderid = cm.orderid;
            ViewBag.customerid = cm.customerid;
            ViewBag.orderstatus = cm.orderstatus;
            ViewBag.orderdate = cm.orderdate;
            ViewBag.totalprice = cm.totalprice;
            ViewBag.shippingaddress = cm.shippingaddress;
            ViewBag.courierno = cm.courierno;
            ViewBag.couriertype = cm.couriertype;
            ViewBag.shippingdate = cm.shippingdate;
            var productid = cm.productids.Split(',');
            List<Amazon.Models.admin> imd = new List<Models.admin>();

            foreach (var pid in productid)
            {
                int pid1 = int.Parse(pid);
                var u = (from data in db.product_master
                         where data.productid == pid1
                         select data).FirstOrDefault();
                var subtotalprice = (float)u.sellingprice * 1;
                imd.Add(new Amazon.Models.admin
                {
                    productid = u.productid,
                    productname = u.productname,
                    sellingprice = (float)u.sellingprice,
                    quantity = 1,
                    subtotalprice = subtotalprice,


                });
            }

            return View(imd);

        }
        public ActionResult viewdeliveredorder(int oid)
        {
            var cm = (from data in db.order_details where data.orderid == oid select data).FirstOrDefault();

            ViewBag.orderid = cm.orderid;
            ViewBag.customerid = cm.customerid;
            ViewBag.orderstatus = cm.orderstatus;
            ViewBag.orderdate = cm.orderdate;
            ViewBag.totalprice = cm.totalprice;
            ViewBag.shippingaddress = cm.shippingaddress;
            ViewBag.courierno = cm.courierno;
            ViewBag.couriertype = cm.couriertype;
            ViewBag.shippingdate = cm.shippingdate;
            ViewBag.deliverby = cm.deliverby;
            var productid = cm.productids.Split(',');
            List<Amazon.Models.admin> imd = new List<Models.admin>();

            foreach (var pid in productid)
            {
                int pid1 = int.Parse(pid);
                var u = (from data in db.product_master
                         where data.productid == pid1
                         select data).FirstOrDefault();
                var subtotalprice = (float)u.sellingprice * 1;
                imd.Add(new Amazon.Models.admin
                {
                    productid = u.productid,
                    productname = u.productname,
                    sellingprice = (float)u.sellingprice,
                    quantity = 1,
                    subtotalprice = subtotalprice,


                });
            }

            return View(imd);

        }

        public ActionResult viewcancelorder(int oid)
        {
            var cm = (from data in db.order_details where data.orderid == oid select data).FirstOrDefault();

            ViewBag.orderid = cm.orderid;
            ViewBag.customerid = cm.customerid;
            ViewBag.orderstatus = cm.orderstatus;
            ViewBag.orderdate = cm.orderdate;
            ViewBag.totalprice = cm.totalprice;
            ViewBag.shippingaddress = cm.shippingaddress;
            ViewBag.courierno = cm.courierno;
            ViewBag.couriertype = cm.couriertype;
            ViewBag.shippingdate = cm.shippingdate;
            ViewBag.deliverby = cm.deliverby;
            var productid = cm.productids.Split(',');
            List<Amazon.Models.admin> imd = new List<Models.admin>();

            foreach (var pid in productid)
            {
                int pid1 = int.Parse(pid);
                var u = (from data in db.product_master
                         where data.productid == pid1
                         select data).FirstOrDefault();
                var subtotalprice = (float)u.sellingprice * 1;
                imd.Add(new Amazon.Models.admin
                {
                    productid = u.productid,
                    productname = u.productname,
                    sellingprice = (float)u.sellingprice,
                    quantity = 1,
                    subtotalprice = subtotalprice,


                });
            }
            return View(imd);
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Invoice(int oid)
        {
            var cm = (from data in db.order_details where data.orderid == oid select data).FirstOrDefault();
            ViewBag.orderid = cm.orderid;
            ViewBag.customerid = cm.customerid;
            ViewBag.orderstatus = cm.orderstatus;
            ViewBag.orderdate = cm.orderdate;
            ViewBag.totalprice = cm.totalprice;
            ViewBag.shippingaddress = cm.shippingaddress;
            ViewBag.courierno = cm.courierno;
            ViewBag.couriertype = cm.couriertype;
            ViewBag.shippingdate = cm.shippingdate;
            ViewBag.deliverby = cm.deliverby;
            var productid = cm.productids.Split(',');
            List<Amazon.Models.admin> imd = new List<Models.admin>();

            foreach (var pid in productid)
            {
                int pid1 = int.Parse(pid);
                var u = (from data in db.product_master
                         where data.productid == pid1
                         select data).FirstOrDefault();
                var subtotalprice = (float)u.sellingprice * 1;
                imd.Add(new Amazon.Models.admin
                {
                    productid = u.productid,
                    productname = u.productname,
                    sellingprice = (float)u.sellingprice,
                    quantity = 1,
                    subtotalprice = subtotalprice,
                });
            }
            return View(imd);
        }
        public ActionResult addcategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addcategory(Amazon.Models.admin md)
        {
            try
            {
                DB.product_category obj = new DB.product_category();
                obj.categoryid = md.categoryid;
                obj.category = md.category;
                obj.subcategory = md.subcategory;
                obj.type = md.type;
                db.product_category.Add(obj);
                db.SaveChanges();
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ViewBag.msg = "Successfully Submitted";
                ModelState.Clear();
            }

        }
        public ActionResult report()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select sum(totalprice),Month(orderdate) from order_details Group By MONTH(orderdate)", con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                float tprice = float.Parse(dr[0].ToString());
                string salemonth = dr[1].ToString();
                if (salemonth == "2")
                {
                    salemonth = "Feb";
                }
                

                ViewBag.salemonth = salemonth;
                ViewBag.tprice = tprice;
            }
            dr.Close();
            con.Close();
            return View();
        }


        [HttpGet]
        public ActionResult adminlogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult adminlogin(Amazon.Models.admin md)
        {
            var a = (from userlist in db.admin_master
                     where userlist.adminid == md.adminid && userlist.password == md.password
                     select new
                     {
                         userlist.adminid,
                         userlist.password
                     }).ToList();
            if (a.FirstOrDefault() != null)
            {
                Session["adminid"] = a.FirstOrDefault().adminid;
                return RedirectToAction("adminhome", "Admin");
            }
            else
            {
                ViewBag.msg = "Invalid login Credentials.";
            }
            return View();

        }
        [HttpGet]
        public ActionResult adminforgotpassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult adminforgotpassword(Amazon.Models.admin md)
        {
            var u = (from userlist in db.admin_master
                     where userlist.adminid == md.adminid && userlist.emailid == md.emailid

                     select new
                     {
                         userlist.password
                     }).ToList();
            if (u.FirstOrDefault() != null)
            {
                ViewBag.msg = "Your Password is:" + u.FirstOrDefault().password;
            }
            else
            {
                ViewBag.msg = "Invalid input Credentials.";
            }
            return View();
        }

        public ActionResult adminhome()
        {
            return View();
        }
        public ActionResult logout()
        {
            Session.Remove("adminid");
            Session.Abandon();
            return RedirectToAction("adminlogin", "Admin");
        }
    }
}