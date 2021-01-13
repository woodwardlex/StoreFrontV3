using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreFrontV3.DATA.EF;

namespace StoreFrontV3.UI.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private StoreFrontV3Entities db = new StoreFrontV3Entities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Department).Include(p => p.Employee).Include(p => p.ProductStatus).Include(p => p.Shipper);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.DeptID = new SelectList(db.Departments, "DeptID", "DepartmentName");
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName");
            ViewBag.ProdStatusID = new SelectList(db.ProductStatuses, "ProdStatusID", "StatusName");
            ViewBag.ShippingID = new SelectList(db.Shippers, "ShippingID", "CompanyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductsID,ProdName,Description,CategoryID,ProdStatusID,UnitsSold,Price,ShippingID,DeptID,EmployeeID,ProdImage")] Product product, HttpPostedFileBase prodImg)
        {
            if (ModelState.IsValid)
            {
                #region File Upload
                //default image
                string imgName = "noImage.png";
                if (prodImg != null)
                {
                    imgName = prodImg.FileName;

                    string ext = imgName.Substring(imgName.LastIndexOf('.'));

                    //valid extensions
                    string[] goodExts = { ".jpeg", ".jpg", ".gif", ".png" };

                    //check ext variable
                    if (goodExts.Contains(ext.ToLower()) && (prodImg.ContentLength <= 4194304))
                    {
                        imgName = Guid.NewGuid() + ext;

                        prodImg.SaveAs(Server.MapPath("~/Content/Products/" + imgName));
                    }
                    else
                    {
                        imgName = "noImage.png";
                    }
                }

                product.ProdImage = imgName;
                #endregion

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.DeptID = new SelectList(db.Departments, "DeptID", "DepartmentName", product.DeptID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", product.EmployeeID);
            ViewBag.ProdStatusID = new SelectList(db.ProductStatuses, "ProdStatusID", "StatusName", product.ProdStatusID);
            ViewBag.ShippingID = new SelectList(db.Shippers, "ShippingID", "CompanyName", product.ShippingID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.DeptID = new SelectList(db.Departments, "DeptID", "DepartmentName", product.DeptID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", product.EmployeeID);
            ViewBag.ProdStatusID = new SelectList(db.ProductStatuses, "ProdStatusID", "StatusName", product.ProdStatusID);
            ViewBag.ShippingID = new SelectList(db.Shippers, "ShippingID", "CompanyName", product.ShippingID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductsID,ProdName,Description,CategoryID,ProdStatusID,UnitsSold,Price,ShippingID,DeptID,EmployeeID,ProdImage")] Product product, HttpPostedFileBase prodImage)
        {
            if (ModelState.IsValid)
            {
                #region File Upload
				string file = "NoImage.png";

                if (prodImage != null)
                {
                    file = prodImage.FileName;
                    string ext = file.Substring(file.LastIndexOf('.'));
                    string[] goodExts = { ".jpeg", ".jpg", ".png", ".gif" };

                    //Check that the uploaded file ext is in our list of good file extensions
                    if (goodExts.Contains(ext.ToLower()))
                    {
                        //if valid, check file size <= 4mb (Max by default from ASP.NET)
                        //Can change this using the MaxRequestLength in the web.config
                        if (prodImage.ContentLength <= 4194304)
                        {
                            //create a new file name using a guid
                            file = Guid.NewGuid() + ext;

                            #region Resize Image
                            //Points to the contents of the file(bookCover) and converts it to an 
                            //image datatype
                            //Image convertedImage = Image.FromStream(prodImage.InputStream);

                            ////Pixel size
                            //int maxImageSize = 200;
                            //int maxThumbSize = 100;

                            ////Resize our image and save it as a default and thumbnail version in our path
                            //prodImage.SaveAs(Server.MapPath("~/Content/Products/" + ));
                            #endregion

                            if (product.ProdImage != null && product.ProdImage != "NoImage.png")
                            {
                                System.IO.File.Delete(Server.MapPath("~/Content/Products/" + product.ProdImage));
                            }
                        }
                        product.ProdImage = file;
                    }
                }
                #endregion
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.DeptID = new SelectList(db.Departments, "DeptID", "DepartmentName", product.DeptID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", product.EmployeeID);
            ViewBag.ProdStatusID = new SelectList(db.ProductStatuses, "ProdStatusID", "StatusName", product.ProdStatusID);
            ViewBag.ShippingID = new SelectList(db.Shippers, "ShippingID", "CompanyName", product.ShippingID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product.ProdImage != null && product.ProdImage != "noImage.png")
            {
                //remove the original file
                System.IO.File.Delete(Server.MapPath("~/Content/Products/" + Session["currentImage"].ToString()));
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
