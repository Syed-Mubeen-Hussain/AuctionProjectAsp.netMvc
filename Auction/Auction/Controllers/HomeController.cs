using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Auction.Models;
using System.Data.Entity;
using Auction.ViewModels;

namespace Auction.Controllers
{
    public class HomeController : Controller
    {
        db_AuctionEntities db = new db_AuctionEntities();

        #region Home Page

        public ActionResult Index()
        {
            HomePageViewModel model = new HomePageViewModel();
            model.categories = db.Categories.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            model.products = db.Products.Where(x => x.p_status == 1).OrderByDescending(x => x.p_id).ToList();
            model.images = db.images.Where(x => x.img_selected == 1).ToList();
            model.currentBid = db.Bids.OrderByDescending(x => x.bid_amount).ToList();
            model.Reviews = db.Reviews.Where(x=>x.r_status == 1).OrderByDescending(x => x.r_id).Take(3).ToList();
            model.customers = db.Customers.OrderByDescending(x => x.c_id).ToList();
            return View(model);
        }

        #endregion

        #region About Us

        public ActionResult AboutUs()
        {
            AboutUsViewModel model = new AboutUsViewModel();
            model.Reviews = db.Reviews.Where(x => x.r_status == 1).OrderByDescending(x => x.r_id).Take(3).ToList();
            model.customers = db.Customers.OrderByDescending(x => x.c_id).ToList();
            return View(model);
        }

        #endregion

        #region Contact Us

        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(Contact contact)
        {
            JsonResult result = new JsonResult();

            if (contact != null)
            {
                db.Contacts.Add(contact);
                db.SaveChanges();
                result.Data = new { Success = true };
            }
            else
            {
                result.Data = new { Success = false };
            }

            return result;
        }

        #endregion

        #region Shop Page

        public ActionResult Shop(string search)
        {
            ShopViewModel model = new ShopViewModel();

            model.featuredProducts = db.Products.Where(x => x.p_featured_product == 1 && x.p_status == 1).OrderByDescending(x => x.p_id).Take(3).ToList();

            model.image = db.images.Where(x => x.img_selected == 1).ToList();

            Session["loadMore"] = 6;

            model.search = search;

            model.products = db.Products.Where(x => x.p_status == 1 && x.p_featured_product == 0).OrderByDescending(x => x.p_id).Take(6).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                model.products = db.Products.Where(x => x.p_name.ToLower().Contains(search.Trim().ToLower()) && x.p_status == 1 && x.p_featured_product == 0).OrderByDescending(x => x.p_id).ToList();
            }

            return View(model);
        }

        public ActionResult FilterShop(string search, int? sortBy, string loadMore)
        {
            FilterShopViewModel model = new FilterShopViewModel();

            model.search = search;

            model.sortBy = sortBy;

            if (!string.IsNullOrEmpty(loadMore))
            {
                Session["loadMore"] = int.Parse(Session["loadMore"].ToString()) + 6;
            }

            model.products = FilterShopMethod(search, sortBy);

            model.image = db.images.Where(x => x.img_selected == 1).ToList();

            return PartialView(model);

        }

        public List<Product> FilterShopMethod(string search, int? sortBy)
        {
            var product = db.Products.Where(x => x.p_status == 1 && x.p_featured_product == 0).OrderByDescending(x => x.p_id).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                product = product.Where(x => x.p_name.ToLower().Contains(search.Trim().ToLower())).ToList();
            }
            if (sortBy.HasValue)
            {
                switch (sortBy.Value)
                {
                    case 1:
                        product = product.OrderByDescending(x => x.p_id).ToList();
                        break;
                    case 2:
                        product = product.OrderByDescending(x => x.p_price).ToList();
                        break;
                    case 3:
                        product = product.OrderBy(x => x.p_price).ToList();
                        break;
                    case 4:
                        product = product.OrderBy(x => x.p_id).ToList();
                        break;
                    default:
                        product = product.ToList();
                        break;
                }
            }

            int countOfProducts = 6;

            if (Session["loadMore"] != null)
            {
                countOfProducts = int.Parse(Session["loadMore"].ToString());
                return product.Take(countOfProducts).ToList();
            }
            else
            {
                return product.Take(countOfProducts).ToList();
            }
        }

        #endregion

        #region Single Product Page

        public ActionResult SingleProduct(int? id)
        {
            SingleProductViewModel model = new SingleProductViewModel();
            if (id > 0)
            {

                var product = db.Products.Where(x => x.p_id == id.Value && x.p_status == 1).FirstOrDefault();
                if (product != null)
                {
                    model.product = db.Products.Where(x => x.p_id == id.Value && x.p_status == 1).FirstOrDefault();
                    model.images = db.images.Where(x => x.img_fk_product_id == id.Value).OrderByDescending(x => x.img_selected == 1).ToList();
                    model.bids = db.Bids.Where(x => x.bid_fk_product == id).Take(6).OrderByDescending(x => x.bid_id).ToList();
                    int customer_id = Convert.ToInt32(Session["customer_id"]);
                    model.customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                    if (model.bids != null)
                    {
                        model.totalBids = db.Bids.Where(x => x.bid_fk_product == id).Count();
                        var bids = db.Bids.Where(x => x.bid_fk_product == id).Select(x => x.bid_fk_cus).ToList();
                        model.totalBidders = db.Customers.Where(x => bids.Contains(x.c_id)).Count();
                        model.highestBid = db.Bids.Where(x => x.bid_fk_product == id).OrderByDescending(x => x.bid_amount).FirstOrDefault();
                    }
                    var p_views = db.Product_View.Where(x => x.av_fk_product_id == id).FirstOrDefault();
                    if (p_views != null)
                    {
                        p_views.av_veiws_count += 1;
                        db.Entry(p_views).State = EntityState.Modified;
                        db.SaveChanges();

                        model.ProductViews = p_views.av_veiws_count.Value;
                    }
                    else
                    {
                        Product_View product_View = new Product_View();
                        product_View.av_fk_product_id = id;
                        product_View.av_veiws_count = 1;
                        db.Product_View.Add(product_View);
                        db.SaveChanges();

                        model.ProductViews = product_View.av_veiws_count.Value;
                    }


                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public long highestBid(int id)
        {
            if (id > 0)
            {
                var highestBid = db.Bids.Where(x => x.bid_fk_product == id).OrderByDescending(x => x.bid_amount).FirstOrDefault();
                if (highestBid != null)
                {
                    return highestBid.bid_amount;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public ActionResult AuctionBids(int id)
        {
            SingleProductViewModel model = new SingleProductViewModel();
            if (id > 0)
            {
                model.bids = db.Bids.Where(x => x.bid_fk_product == id).Take(6).OrderByDescending(x => x.bid_id).ToList();
                if (model.bids != null)
                {
                    model.highestBid = db.Bids.Where(x => x.bid_fk_product == id).OrderByDescending(x => x.bid_amount).FirstOrDefault();
                }

                return PartialView(model);
            }
            else
            {
                return PartialView();
            }
        }

        public ActionResult AddAuctionBid(string amount, int id)
        {
            int p_id = id;
            if (!string.IsNullOrEmpty(amount))
            {
                Bid bid = new Bid();
                bid.bid_fk_cus = Convert.ToInt32(Session["customer_id"]);
                bid.bid_amount = int.Parse(amount.ToString());
                bid.bid_timeStamp = DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt");
                bid.bid_fk_product = id;
                db.Bids.Add(bid);
                db.SaveChanges();

                return RedirectToAction("AuctionBids", new { id = p_id });
            }
            else
            {
                return RedirectToAction("SingleProduct", new { id = p_id });
            }

        }

        public ActionResult TotalBidsAndTotalBiddersCount(int id)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (id > 0)
            {
                var bids = db.Bids.Where(x => x.bid_fk_product == id).Select(x => x.bid_fk_cus).ToList();

                var totalBids = db.Bids.Where(x => x.bid_fk_product == id).Count();
                var totalBidders = db.Customers.Where(x => bids.Contains(x.c_id)).Count();
                result.Data = new { Success = true, TotalBidders = totalBidders, TotalBids = totalBids };
            }
            else
            {
                result.Data = new { Success = true };
            }
            return result;
        }

        public ActionResult ShowBiddersHistory(int id)
        {
            ShowBiddersHistoryViewModel model = new ShowBiddersHistoryViewModel();
            var bids = db.Bids.Where(x => x.bid_fk_product == id).OrderByDescending(x => x.bid_id).Select(x => x.bid_fk_cus).ToList();
            model.customers = db.Customers.Where(x => bids.Contains(x.c_id)).OrderByDescending(x => bids.Contains(x.c_id)).ToList();
            model.bids = db.Bids.Where(x => x.bid_fk_product == id).OrderByDescending(x => x.bid_id).ToList();
            model.totalAmount = db.Bids.Where(x => x.bid_fk_product == id).ToList();
            if (model.customers.Count > 0)
            {
                return PartialView(model);
            }
            else
            {
                return PartialView();
            }
        }

        public ActionResult WinAuction(int id)
        {
            var customer_id = db.Bids.Where(x => x.bid_fk_product == id).OrderByDescending(x => x.bid_amount).Select(x => x.bid_fk_cus).FirstOrDefault();
            var customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();



            if (customer != null)
            {
                var check_cart = db.Carts.Where(x => x.cus_id == customer.c_id && x.p_id == id).FirstOrDefault();
                if (check_cart == null)
                {
                    Cart cartItem = new Cart();
                    cartItem.p_id = id;
                    cartItem.cus_id = customer.c_id;
                    db.Carts.Add(cartItem);
                    db.SaveChanges();
                }

                var product = db.Products.Where(x => x.p_id == id).FirstOrDefault();
                if (product != null)
                {
                    product.p_status = 0;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();

                    var bids = db.Bids.Where(x => x.bid_fk_product == id).ToList();
                    foreach (var bid in bids)
                    {
                        db.Entry(bid).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                }

                return Json(customer.c_userName, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var product = db.Products.Where(x => x.p_id == id).FirstOrDefault();
                if (product != null)
                {
                    product.p_status = 0;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

        public int CartSingleProduct(int id)
        {
            if (id > 0)
            {
                var customer_id = Convert.ToInt32(Session["customer_id"]);
                var check_cart = db.Carts.Where(x => x.p_id == id && x.cus_id == customer_id).FirstOrDefault();
                if (check_cart == null)
                {
                    Cart cartItem = new Cart();
                    cartItem.p_id = id;
                    cartItem.cus_id = customer_id;
                    db.Carts.Add(cartItem);
                    db.SaveChanges();
                }
                else
                {
                    return 2;
                }
                return 1;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region Register

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Customer cus, HttpPostedFileBase imageFile)
        {
            if (cus != null)
            {
                Customer c = new Customer();
                c.c_firstName = cus.c_firstName;
                c.c_lastName = cus.c_lastName;
                c.c_age = cus.c_age;
                c.c_gender = cus.c_gender;
                c.c_address = cus.c_address;
                c.c_phone = cus.c_phone;
                c.c_email = cus.c_email;
                c.c_userName = cus.c_userName;
                c.c_password = cus.c_password;
                c.register_date = DateTime.Now.ToLongDateString();

                string filename = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string extension = Path.GetExtension(imageFile.FileName);
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                {
                    filename = filename + extension;
                    c.c_image = "/Content/images/" + filename;
                    filename = Path.Combine(Server.MapPath("/Content/images/"), filename);
                    imageFile.SaveAs(filename);
                    db.Customers.Add(c);
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["msg"] = "<script>swal('Registered Successfully!', {icon: 'success'});</script>";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    ViewData["ExtensionError"] = "<script>alert('Image Format not supported')</script>";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public int CheckUsername(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var data = db.Customers.Where(x => x.c_userName.ToLower() == username.ToLower().Trim()).FirstOrDefault();
                if (data != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region Login

        public ActionResult Login(string ReturnUrl)
        {
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                ViewData["ReturnUrl"] = ReturnUrl.ToString();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(Customer cus, string ReturnUrl)
        {
            if (!string.IsNullOrEmpty(cus.c_userName) && !string.IsNullOrEmpty(cus.c_password))
            {
                if (IsValid(cus) == true)
                {
                    FormsAuthentication.SetAuthCookie(cus.c_userName, false);
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["Error"] = "Invalid username or password";
                    return RedirectToAction("Login", new { ReturnUrl = ReturnUrl });
                }
            }
            else
            {
                return View();
            }
        }

        public bool IsValid(Customer cus)
        {
            var data = db.Customers.Where(x => x.c_userName.ToLower() == cus.c_userName.ToLower().Trim() && x.c_password.ToLower() == cus.c_password.ToLower().Trim()).FirstOrDefault();
            if (data != null)
            {
                Session["customer_username"] = data.c_userName.ToString();
                Session["customer_id"] = data.c_id.ToString();
                Session["customer_image"] = data.c_image.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Logout

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["customer_id"] = null;
            Session["customer_username"] = null;
            Session["customer_image"] = null;
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Cart

        public ActionResult CartCount()
        {
            var customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                var cart = db.Carts.Where(x => x.cus_id == customer_id).Count();
                return Json(cart, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Cart()
        {
            var customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                CartViewModel model = new CartViewModel();
                var cart = db.Carts.Where(x => x.cus_id == customer_id).Select(x => x.p_id).ToList();
                if (cart != null && cart.Count > 0)
                {
                    model.products = db.Products.Where(x => cart.Contains(x.p_id)).OrderByDescending(x => x.p_id).ToList();
                    model.images = db.images.Where(x => cart.Contains(x.img_fk_product_id.Value) && x.img_selected == 1).ToList();
                    return PartialView(model);
                }
                else
                {
                    return PartialView();
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveCartItem(int id)
        {
            var customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                var cartItem = db.Carts.Where(x => x.cus_id == customer_id && x.p_id == id).FirstOrDefault();
                db.Entry(cartItem).State = EntityState.Deleted;
                db.SaveChanges();
            }
            return RedirectToAction("Cart");
        }

        #endregion

        #region Category Product

        public ActionResult CategoryProduct(int id)
        {
            CategoryProductViewModel model = new CategoryProductViewModel();

            model.categoryId = id;

            model.category = db.Categories.Where(x => x.cat_id == id && x.cat_status == 1).FirstOrDefault();

            model.featuredProducts = db.Products.Where(x => x.p_featured_product == 1 && x.p_fk_cat == id && x.p_status == 1).OrderByDescending(x => x.p_id).Take(3).ToList();

            model.image = db.images.Where(x => x.img_selected == 1).ToList();

            Session["loadMore"] = 6;

            model.products = db.Products.Where(x => x.p_fk_cat == id && x.p_status == 1 && x.p_featured_product == 0).OrderByDescending(x => x.p_id).Take(6).ToList();



            return View(model);
        }

        public ActionResult CategoryFilterProduct(int id, string search, int? sortBy, string loadMore)
        {
            CategoryProductViewModel model = new CategoryProductViewModel();

            model.search = search;

            model.categoryId = id;

            model.sortBy = sortBy;

            if (!string.IsNullOrEmpty(loadMore))
            {
                Session["loadMore"] = int.Parse(Session["loadMore"].ToString()) + 6;
            }

            model.products = CategoryFilterProductMethod(id, search, sortBy);

            model.image = db.images.Where(x => x.img_selected == 1).ToList();

            return PartialView(model);
        }

        public List<Product> CategoryFilterProductMethod(int id, string search, int? sortBy)
        {
            var product = db.Products.Where(x => x.p_fk_cat == id && x.p_status == 1 && x.p_featured_product == 0).OrderByDescending(x => x.p_id).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                product = product.Where(x => x.p_name.ToLower().Contains(search.Trim().ToLower())).ToList();
            }
            if (sortBy.HasValue)
            {
                switch (sortBy.Value)
                {
                    case 1:
                        product = product.OrderByDescending(x => x.p_id).ToList();
                        break;
                    case 2:
                        product = product.OrderByDescending(x => x.p_price).ToList();
                        break;
                    case 3:
                        product = product.OrderBy(x => x.p_price).ToList();
                        break;
                    case 4:
                        product = product.OrderBy(x => x.p_id).ToList();
                        break;
                    default:
                        product = product.ToList();
                        break;
                }
            }

            int countOfProducts = 6;

            if (Session["loadMore"] != null)
            {
                countOfProducts = int.Parse(Session["loadMore"].ToString());
                return product.Take(countOfProducts).ToList();
            }
            else
            {
                return product.Take(countOfProducts).ToList();
            }
        }

        #endregion

        #region Customer Profile

        public ActionResult CustomerDashboard()
        {
            int customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                ViewData["customer"] = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                ViewData["totalBids"] = db.Bids.Where(x => x.bid_fk_cus == customer_id).Count();
                ViewData["auctionWon"] = db.orders.Where(x => x.o_fk_cus_id == customer_id).Count();
                ViewData["wishlist"] = db.Wishlists.Where(x => x.w_fk_cus_id == customer_id).Count();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CustomerPersonalProfile()
        {
            int customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                var customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                return View(customer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CustomerMyBids()
        {
            int customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                CustomerMyBidsViewModel model = new CustomerMyBidsViewModel();
                var bids = db.Bids.Where(x => x.bid_fk_cus == customer_id).Select(x => x.bid_fk_product).ToList();
                model.products = db.Products.Where(x => bids.Contains(x.p_id)).OrderByDescending(x => x.p_id).ToList();
                model.customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                model.images = db.images.Where(x => x.img_selected == 1).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CustomerWinningBids()
        {
            int customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                CustomerMyBidsViewModel model = new CustomerMyBidsViewModel();
                var bids = db.Bids.Where(x => x.bid_fk_cus == customer_id).Select(x => x.bid_fk_product).ToList();
                var order = db.orders.Where(x => x.o_fk_cus_id == customer_id).Select(x => x.o_id).ToList();
                var order_details = db.order_details.Where(x => order.Contains(x.od_id)).Select(x => x.od_fk_product_id).ToList();
                var cart = db.Carts.Where(x => x.cus_id == customer_id).Select(x => x.p_id).ToList();
                model.products = db.Products.Where(x => cart.Contains(x.p_id) || order_details.Contains(x.p_id)).OrderByDescending(x => x.p_id).ToList();
                model.customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                model.images = db.images.Where(x => x.img_selected == 1).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CustomerWishlist()
        {
            int customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                CustomerMyBidsViewModel model = new CustomerMyBidsViewModel();
                var wishlist = db.Wishlists.Where(x => x.w_fk_cus_id == customer_id).OrderByDescending(x => x.w_id).Select(x => x.w_fk_product_id).ToList();
                model.products = db.Products.Where(x => wishlist.Contains(x.p_id)).OrderByDescending(x => x.p_id).ToList();
                model.customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                model.images = db.images.Where(x => x.img_selected == 1).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Wishlist Product

        public ActionResult AddWishlistItem(int id)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (id > 0)
            {
                int customer_id = Convert.ToInt32(Session["customer_id"]);
                if (customer_id > 0)
                {
                    var wishlistItem = db.Wishlists.Where(x => x.w_fk_cus_id == customer_id && x.w_fk_product_id == id).FirstOrDefault();
                    if (wishlistItem != null)
                    {
                        result.Data = new { Success = false, Message = "no" };
                    }
                    else
                    {
                        Wishlist wishlist = new Wishlist();
                        wishlist.w_fk_cus_id = customer_id;
                        wishlist.w_fk_product_id = id;
                        db.Wishlists.Add(wishlist);
                        db.SaveChanges();
                        result.Data = new { Success = true, Message = "yes" };
                    }
                }
                else
                {
                    result.Data = new { Success = false, Message = "error" };
                }
            }
            else
            {
                result.Data = new { Success = false, Message = "error" };
            }
            return result;
        }

        public ActionResult CountWishlistItem()
        {
            var customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                var wishlist = db.Wishlists.Where(x => x.w_fk_cus_id == customer_id).Count();
                return Json(wishlist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveWishlistItem(int id)
        {
            var customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                var wishlistItem = db.Wishlists.Where(x => x.w_fk_cus_id == customer_id && x.w_fk_product_id == id).FirstOrDefault();
                db.Entry(wishlistItem).State = EntityState.Deleted;
                db.SaveChanges();

                CustomerMyBidsViewModel model = new CustomerMyBidsViewModel();
                var wishlist = db.Wishlists.Where(x => x.w_fk_cus_id == customer_id).OrderByDescending(x => x.w_id).Select(x => x.w_fk_product_id).ToList();
                model.products = db.Products.Where(x => wishlist.Contains(x.p_id)).OrderByDescending(x => x.p_id).ToList();
                model.customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                model.images = db.images.Where(x => x.img_selected == 1).ToList();
                return PartialView(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Checkout

        public ActionResult Checkout()
        {
            var customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                CheckoutViewModel model = new CheckoutViewModel();
                var cart = db.Carts.Where(x => x.cus_id == customer_id).Select(x => x.p_id).ToList();
                model.products = db.Products.Where(x => cart.Contains(x.p_id)).OrderByDescending(x => x.p_id).ToList();
                model.customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                if (model.customer != null && model.products != null && model.products.Count > 0)
                {
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Checkout(order order)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            var customer_id = Convert.ToInt32(Session["customer_id"]);
            var cart = db.Carts.Where(x => x.cus_id == customer_id).Select(x => x.p_id).ToList();
            if (customer_id > 0)
            {
                if (cart != null && cart.Count > 0)
                {
                    order o = new order();
                    o.o_fk_cus_id = customer_id;
                    o.o_address = order.o_address;
                    o.o_phone = order.o_phone;
                    o.o_email = order.o_email;
                    o.o_zip = order.o_zip;
                    o.o_total_amout = order.o_total_amout;
                    o.o_status = "Pending";
                    db.orders.Add(o);
                    db.SaveChanges();

                    order_details order_details = new order_details();
                    var products = db.Products.Where(x => cart.Contains(x.p_id)).OrderByDescending(x => x.p_id).ToList();
                    var cartItem = db.Carts.Where(x => x.cus_id == customer_id).ToList();
                    foreach (var product in products)
                    {
                        order_details.od_fk_product_id = product.p_id;
                        order_details.od_fk_o = o.o_id;
                        db.order_details.Add(order_details);
                        db.SaveChanges();

                        product.p_status = 0;
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    foreach (var c in cartItem)
                    {
                        db.Entry(c).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    Session["Thank_You"] = o.o_id;
                    result.Data = new { Success = true, Message = o.o_id };
                }
                else
                {
                    result.Data = new { Success = false, Message = "no" };
                }
            }
            else
            {
                result.Data = new { Success = false };
            }
            return result;
        }

        #endregion

        #region Order

        public ActionResult Order()
        {
            var customer_id = Convert.ToInt32(Session["customer_id"]);
            if (customer_id > 0)
            {
                OrderViewModel model = new OrderViewModel();
                model.order = db.orders.Where(x => x.o_fk_cus_id == customer_id).ToList();
                var orders = db.orders.Where(x => x.o_fk_cus_id == customer_id).Select(x => x.o_id).ToList();
                model.order_details = db.order_details.Where(x => orders.Contains(x.od_fk_o.Value)).OrderByDescending(x => x.od_id).ToList();
                var order_detail = db.order_details.Where(x => orders.Contains(x.od_fk_o.Value)).Select(x => x.od_fk_product_id).ToList();
                model.products = db.Products.Where(x => order_detail.Contains(x.p_id)).OrderByDescending(x => x.p_id).ToList();
                model.images = db.images.Where(x => x.img_selected == 1).ToList();
                if (model.order != null && model.order_details != null && model.products != null && model.images != null)
                {
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Thank You

        public ActionResult Thank_You()
        {
            int order_id = Convert.ToInt32(Session["Thank_You"]);
            if (order_id > 0)
            {
                var check_order = db.orders.Where(x => x.o_id == order_id).FirstOrDefault();
                if (check_order != null)
                {
                    Session["Thank_You"] = 0;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Thank_You(Review r)
        {
            if (ModelState.IsValid)
            {
                if (r != null)
                {
                    Review review = new Review();
                    review.r_username = r.r_username;
                    review.r_email = r.r_email;
                    review.r_stars = r.r_stars;
                    review.r_message = r.r_message;
                    review.r_status = 0;
                    review.r_fk_c_id = Convert.ToInt32(Session["customer_id"]);
                    db.Reviews.Add(review);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        #endregion

    }
}