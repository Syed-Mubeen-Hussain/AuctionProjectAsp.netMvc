using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Auction.ViewModels;
using Auction.Models;
using System.Text;

namespace Auction.Controllers
{
    public class AdminController : Controller
    {

        db_AuctionEntities db = new db_AuctionEntities();

        #region Dashboard

        public ActionResult Dashboard()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }

        #endregion

        #region Category
        public ActionResult Category()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }

        public ActionResult CategoryTable(string search, int? pageNo)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                CategoryViewModel model = new CategoryViewModel();

                int pageSize = 5;

                pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

                model.pageNo = pageNo.Value;

                var category = db.Categories.ToList();

                var totalCategory = db.Categories.Count();

                if (!string.IsNullOrEmpty(search))
                {
                    model.SearchTerm = search;
                    category = db.Categories.Where(x => x.cat_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                    totalCategory = category.Count();
                }

                model.categories = category.OrderByDescending(x => x.cat_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

                model.page = new Pager(totalCategory, pageNo, pageSize);

                return PartialView(model);
            }
        }

        public ActionResult CreateCategory()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return PartialView();
            }
        }

        [HttpPost]
        public ActionResult CreateCategory(Category cat)
        {
            if (cat != null)
            {
                var check_category = db.Categories.Where(x => x.cat_name.ToLower().Trim() == cat.cat_name.ToLower().Trim()).FirstOrDefault();
                if (check_category != null)
                {
                    ViewBag.error = "no";
                    return Json(ViewBag.error);
                }
                else
                {
                    Category c = new Category();
                    c.cat_name = cat.cat_name;
                    c.cat_image = cat.cat_image;
                    c.cat_status = 1;
                    db.Categories.Add(c);
                    db.SaveChanges();
                    return RedirectToAction("CategoryTable");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult EditCategory(int id, int? pageNo, string search)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    var data = db.Categories.Find(id);
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        ViewBag.pageNo = pageNo.Value;
                        ViewBag.search = search.ToString();
                    }
                    return PartialView(data);
                }
                else
                {
                    return RedirectToAction("CategoryTable");
                }
            }
        }

        [HttpPost]
        public ActionResult EditCategory(Category cat, int? pageNo, string search)
        {
            if (cat != null)
            {
                var check_category = db.Categories.Find(cat.cat_id);
                var category = db.Categories.Where(x => x.cat_name == cat.cat_name).FirstOrDefault();
                if (category != null)
                {
                    if (category.cat_id == check_category.cat_id)
                    {
                        category.cat_name = cat.cat_name;
                        category.cat_status = cat.cat_status;
                        if (!string.IsNullOrEmpty(cat.cat_image))
                        {
                            category.cat_image = cat.cat_image;
                        }

                        db.Entry(category).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.error = "no";
                        return Json(ViewBag.error);
                    }
                }
                if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                {
                    return RedirectToAction("CategoryTable", new { pageNo = pageNo.Value, search = search });
                }
                else
                {
                    return RedirectToAction("CategoryTable");
                }
            }
            else
            {
                return RedirectToAction("CategoryTable");
            }
        }

        public ActionResult DeleteCategory(int id, int? pageNo, string search)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    var data = db.Categories.Find(id);
                    data.cat_status = 0;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        return RedirectToAction("CategoryTable", new { pageNo = pageNo.Value, search = search });
                    }
                    else
                    {
                        return RedirectToAction("CategoryTable");
                    }
                }
                else
                {
                    return RedirectToAction("CategoryTable");
                }
            }
        }

        public ActionResult DetailsCategory(int id, int? pageNo, string search)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    var data = db.Categories.Find(id);
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        ViewBag.pageNo = pageNo.Value;
                        ViewBag.search = search;
                    }
                    return PartialView(data);
                }
                else
                {
                    return RedirectToAction("CategoryTable");
                }
            }
        }
        #endregion

        #region Product

        public ActionResult Product()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ProductTable(string search, int? pageNo)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                ProductViewModel model = new ProductViewModel();

                int pageSize = 5;

                pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

                model.pageNo = pageNo.Value;

                var product = db.Products.ToList();

                var totalProducts = db.Products.Count();

                if (!string.IsNullOrEmpty(search))
                {
                    model.SearchTerm = search;
                    product = db.Products.Where(x => x.p_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                    totalProducts = product.Count();
                }

                model.products = product.OrderByDescending(x => x.p_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

                model.image = db.images.Where(x => x.img_selected == 1).ToList();

                model.page = new Pager(totalProducts, pageNo, pageSize);

                return PartialView(model);
            }
        }

        public ActionResult CreateProduct()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                var category = db.Categories.Where(x => x.cat_status == 1).ToList();
                ViewBag.category = new SelectList(category, "cat_id", "cat_name");
                return PartialView();
            }
        }

        [HttpPost]
        public ActionResult CreateProduct(Product pro, string image)
        {
            if (pro != null)
            {
                var check_product = db.Products.Where(x => x.p_name.ToLower() == pro.p_name.ToLower().Trim()).FirstOrDefault();
                if (check_product != null)
                {
                    ViewBag.error = "no";
                    return Json(ViewBag.error);
                }
                else
                {
                    Product p = new Product();
                    p.p_name = pro.p_name.Trim();
                    p.p_price = pro.p_price;
                    p.p_description = pro.p_description;
                    p.p_dateOfCreation = DateTime.Now.ToLongDateString();
                    p.p_fk_cat = pro.p_fk_cat;
                    p.p_start_date = DateTime.Now.ToString();
                    p.p_end_date = DateTime.Now.AddDays(2).ToString();
                    p.p_increment = 0;
                    p.p_featured_product = pro.p_featured_product;
                    p.p_status = 1;
                    db.Products.Add(p);

                    image img = new image();
                    img.img_text = image;
                    img.img_fk_product_id = p.p_id;
                    img.img_selected = 1;
                    db.images.Add(img);

                    db.SaveChanges();
                    return RedirectToAction("ProductTable");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult EditProduct(int id, int? pageNo, string search)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    ViewBag.category = db.Categories.Where(x => x.cat_status == 1).ToList();
                    var data = db.Products.Find(id);
                    var image = db.images.Where(x => x.img_fk_product_id == id && x.img_selected == 1).FirstOrDefault();
                    if (image == null)
                    {
                        ViewBag.image = "/content/images/no-image.png";
                    }
                    else
                    {
                        ViewBag.image = image.img_text;
                    }
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        ViewBag.pageNo = pageNo.Value;
                        ViewBag.search = search.ToString();
                    }
                    return PartialView(data);
                }
                else
                {
                    return RedirectToAction("ProductTable");
                }
            }
        }

        [HttpPost]
        public ActionResult EditProduct(Product pro, string ImageURL, string OldImageURL, int? pageNo, string search)
        {
            if (pro != null)
            {
                var check_product = db.Products.Find(pro.p_id);
                var product = db.Products.Where(x => x.p_name == pro.p_name).FirstOrDefault();
                if (product != null)
                {
                    if (product.p_id == check_product.p_id)
                    {
                        product.p_name = pro.p_name;
                        product.p_price = pro.p_price;
                        product.p_description = pro.p_description;
                        product.p_fk_cat = pro.p_fk_cat;
                        product.p_featured_product = pro.p_featured_product;
                        product.p_status = pro.p_status;

                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();

                        if (!string.IsNullOrEmpty(ImageURL))
                        {
                            if (!string.IsNullOrEmpty(OldImageURL))
                            {
                                var check_image = db.images.Where(x => x.img_fk_product_id == product.p_id && x.img_text == ImageURL).FirstOrDefault();
                                if (check_image != null)
                                {
                                    check_image.img_selected = 1;
                                    db.Entry(check_image).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    var old_image = db.images.Where(x => x.img_text == OldImageURL.ToString() && x.img_fk_product_id == product.p_id).FirstOrDefault();
                                    if (old_image != null)
                                    {
                                        db.Entry(old_image).State = EntityState.Deleted;
                                    }
                                    var image = new image();
                                    image.img_text = ImageURL.ToString();
                                    image.img_fk_product_id = product.p_id;
                                    image.img_selected = 1;
                                    db.images.Add(image);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.error = "no";
                        return Json(ViewBag.error);
                    }

                }
                else
                {
                    var editproduct = db.Products.Find(pro.p_id);
                    editproduct.p_name = pro.p_name;
                    editproduct.p_price = pro.p_price;
                    editproduct.p_description = pro.p_description;
                    editproduct.p_fk_cat = pro.p_fk_cat;
                    editproduct.p_featured_product = pro.p_featured_product;
                    editproduct.p_status = pro.p_status;

                    db.Entry(editproduct).State = EntityState.Modified;
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(ImageURL))
                    {
                        if (!string.IsNullOrEmpty(OldImageURL))
                        {
                            var check_image = db.images.Where(x => x.img_fk_product_id == editproduct.p_id && x.img_text == ImageURL).FirstOrDefault();
                            if (check_image != null)
                            {
                                check_image.img_selected = 1;
                                db.Entry(check_image).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                var old_image = db.images.Where(x => x.img_text == OldImageURL.ToString() && x.img_fk_product_id == editproduct.p_id).FirstOrDefault();
                                if (old_image != null)
                                {
                                    db.Entry(old_image).State = EntityState.Deleted;
                                }
                                var image = new image();
                                image.img_text = ImageURL.ToString();
                                image.img_fk_product_id = editproduct.p_id;
                                image.img_selected = 1;
                                db.images.Add(image);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                {
                    return RedirectToAction("ProductTable", new { pageNo = pageNo.Value, search = search });
                }
                else
                {
                    return RedirectToAction("ProductTable");
                }
            }
            else
            {
                if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                {
                    return RedirectToAction("ProductTable", new { pageNo = pageNo.Value, search = search });
                }
                else
                {
                    return RedirectToAction("ProductTable");
                }
            }
        }

        public ActionResult DeleteProduct(int id, int? pageNo, string search)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    var data = db.Products.Find(id);
                    data.p_status = 0;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        return RedirectToAction("ProductTable", new { pageNo = pageNo.Value, search = search });
                    }
                    else
                    {
                        return RedirectToAction("ProductTable");
                    }
                }
                else
                {
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        return RedirectToAction("ProductTable", new { pageNo = pageNo.Value, search = search });
                    }
                    else
                    {
                        return RedirectToAction("ProductTable");
                    }
                }
            }
        }

        public ActionResult DetailsProduct(int id, int? pageNo, string search)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    var image = db.images.Where(x => x.img_fk_product_id == id && x.img_selected == 1).FirstOrDefault();
                    if (image == null)
                    {
                        ViewBag.image = "/content/images/no-image.png";
                    }
                    else
                    {
                        ViewBag.image = image.img_text;
                    }
                    var data = db.Products.Find(id);
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        ViewBag.pageNo = pageNo.Value;
                        ViewBag.search = search;
                    }
                    return PartialView(data);
                }
                else
                {
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        return RedirectToAction("ProductTable", new { pageNo = pageNo.Value, search = search });
                    }
                    else
                    {
                        return RedirectToAction("ProductTable");
                    }
                }
            }
        }

        public ActionResult ProductImages(int id, int? pageNo, string search)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    var data = db.images.Where(x => x.img_fk_product_id == id).OrderByDescending(x => x.img_id).ToList();
                    ViewBag.pid = id;
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        TempData["pageNo"] = pageNo.Value;
                        TempData["search"] = search;
                    }
                    return PartialView(data);
                }
                else
                {
                    return RedirectToAction("ProductTable");
                }
            }
        }

        [HttpPost]
        public ActionResult AddProductImage(string img, int id, int? pageNo, string search)
        {
            if (!string.IsNullOrEmpty(img) && id > 0)
            {
                var image_exist = db.images.Where(x => x.img_text == img && x.img_fk_product_id == id).FirstOrDefault();
                if (image_exist != null)
                {
                    ViewBag.error = "no";
                    return Json(ViewBag.error);
                }

                var check_image = db.images.Where(x => x.img_fk_product_id == id).Count();
                var imgx = new image();
                if (check_image == 0)
                {
                    imgx.img_text = img;
                    imgx.img_fk_product_id = id;
                    imgx.img_selected = 1;
                    db.images.Add(imgx);
                    db.SaveChanges();
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        return RedirectToAction("ProductImages", new { id = imgx.img_fk_product_id, pageNo = pageNo.Value, search = search });
                    }
                    else
                    {
                        return RedirectToAction("ProductImages", new { id = imgx.img_fk_product_id });
                    }
                }
                else
                {
                    imgx.img_text = img;
                    imgx.img_fk_product_id = id;
                    imgx.img_selected = 0;
                    db.images.Add(imgx);
                    db.SaveChanges();
                    if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                    {
                        return RedirectToAction("ProductImages", new { id = imgx.img_fk_product_id, pageNo = pageNo.Value, search = search });
                    }
                    else
                    {
                        return RedirectToAction("ProductImages", new { id = imgx.img_fk_product_id });
                    }
                }

            }
            else
            {
                return RedirectToAction("ProductTable");
            }
        }

        [HttpPost]
        public ActionResult DeleteProductImage(int img_id, int? pageNo, string search)
        {
            if (img_id > 0)
            {
                var img = db.images.Find(img_id);
                int pid = img.img_fk_product_id.Value;
                db.Entry(img).State = EntityState.Deleted;
                db.SaveChanges();
                if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                {
                    return RedirectToAction("ProductImages", new { id = pid, pageNo = pageNo.Value, search = search });
                }
                else
                {
                    return RedirectToAction("ProductImages", new { id = pid });
                }
            }
            else
            {
                return RedirectToAction("ProductTable");
            }
        }

        [HttpPost]
        public ActionResult SetProductImage(int img_id, int? pageNo, string search)
        {
            if (img_id > 0)
            {
                var image = db.images.Where(x => x.img_id == img_id).FirstOrDefault();
                var pid = image.img_fk_product_id;
                image.img_selected = 1;
                db.Entry(image).State = EntityState.Modified;
                var unselected_image = db.images.Where(x => x.img_fk_product_id == pid && x.img_id != img_id).ToList();
                foreach (var item in unselected_image)
                {
                    item.img_selected = 0;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                db.SaveChanges();
                if (pageNo.HasValue || !string.IsNullOrEmpty(search))
                {
                    return RedirectToAction("ProductImages", new { id = pid, pageNo = pageNo.Value, search = search });
                }
                else
                {
                    return RedirectToAction("ProductImages", new { id = pid });
                }
            }
            else
            {
                return RedirectToAction("ProductTable");
            }
        }

        #endregion

        #region Customer

        public ActionResult Customer()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }

        public ActionResult CustomerTable(string search, int? pageNo)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                CustomerViewModel model = new CustomerViewModel();

                int pageSize = 5;

                pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

                model.pageNo = pageNo.Value;

                var customer = db.Customers.ToList();

                var totalCustomer = db.Customers.Count();

                if (!string.IsNullOrEmpty(search))
                {
                    model.SearchTerm = search;
                    customer = db.Customers.Where(x => x.c_userName.ToLower().Contains(search.Trim().ToLower())).ToList();
                    totalCustomer = customer.Count();
                }

                model.customers = customer.OrderByDescending(x => x.c_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

                model.page = new Pager(totalCustomer, pageNo, pageSize);

                return PartialView(model);
            }
        }

        public ActionResult DetailsCustomer(int id)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    var data = db.Customers.Find(id);
                    return PartialView(data);
                }
                else
                {
                    return RedirectToAction("CustomerTable");
                }
            }
        }

        #endregion

        #region Contact

        public ActionResult Contact()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ContactTable(string search, int? pageNo)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                ContactViewModel model = new ContactViewModel();

                int pageSize = 5;

                pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

                model.pageNo = pageNo.Value;

                var contact = db.Contacts.ToList();

                var totalContact = db.Contacts.Count();

                if (!string.IsNullOrEmpty(search))
                {
                    model.SearchTerm = search;
                    contact = db.Contacts.Where(x => x.cont_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                    totalContact = contact.Count();
                }

                model.contacts = contact.OrderByDescending(x => x.cont_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

                model.page = new Pager(totalContact, pageNo, pageSize);

                return PartialView(model);
            }
        }

        public ActionResult DetailsContact(int id)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (id > 0)
                {
                    var data = db.Contacts.Find(id);
                    return PartialView(data);
                }
                else
                {
                    return RedirectToAction("ContactTable");
                }
            }
        }

        #endregion

        #region Login

        public ActionResult Login()
        {
            if (Session["username"] != null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["AdminRememberMe"];
                if (cookie != null)
                {
                    ViewBag.username = cookie["username"].ToString();

                    string EncryptedPassword = cookie["password"].ToString();
                    byte[] b = Convert.FromBase64String(EncryptedPassword);
                    string decryptedPassword = ASCIIEncoding.ASCII.GetString(b);
                    ViewBag.password = decryptedPassword.ToString();
                }
                return View();
            }
        }

        [HttpPost]
        public JsonResult Login(Admin admin, string rememberMe)
        {
            var result = new JsonResult();
            if (admin.ad_username != "" && admin.ad_password != "")
            {
                var data = db.Admins.Where(x => x.ad_username.ToLower() == admin.ad_username.Trim().ToLower() && x.ad_password.ToLower() == admin.ad_password.Trim().ToLower()).FirstOrDefault();
                if (data != null)
                {
                    Session["username"] = data.ad_username;

                    HttpCookie cookie = new HttpCookie("AdminRememberMe");
                    if (!string.IsNullOrEmpty(rememberMe))
                    {
                        cookie["username"] = data.ad_username;

                        byte[] b = ASCIIEncoding.ASCII.GetBytes(data.ad_password);
                        string EncryptedPassword = Convert.ToBase64String(b);
                        cookie["password"] = EncryptedPassword;

                        HttpContext.Response.Cookies.Add(cookie);
                        cookie.Expires = DateTime.Now.AddDays(2);
                    }
                    else
                    {
                        HttpCookie check_cookie = Request.Cookies["AdminRememberMe"];
                        if (check_cookie != null)
                        {
                            HttpContext.Response.Cookies.Add(cookie);
                            cookie.Expires = DateTime.Now.AddDays(-1);
                        }
                    }

                    result.Data = new { Success = true };
                }
                else
                {
                    result.Data = new { Success = false };
                }
            }
            else
            {
                result.Data = new { Success = false };
            }
            return result;
        }

        #endregion

        #region SignOut

        public ActionResult SignOut()
        {
            Session["username"] = null;
            return RedirectToAction("Login", "Admin");
        }

        #endregion
    }
}