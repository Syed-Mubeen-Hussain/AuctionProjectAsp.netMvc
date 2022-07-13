using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auction.Controllers
{
    public class SharedController : Controller
    {
        #region Image Upload
        public JsonResult UploadImage()
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                var file = Request.Files[0];
                var extension = Path.GetExtension(file.FileName);
                if (extension.ToLower() == ".png" || extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg")
                {
                    var fileName = file.FileName;

                    var path = Path.Combine(Server.MapPath("~/content/images/"), fileName);

                    file.SaveAs(path);

                    result.Data = new { Success = true, ImageURL = string.Format("/content/images/{0}", fileName) };
                }
                else
                {
                    result.Data = new { Success = false, Error = "Image Format not Supported" };
                }
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = ex.Message };
            }
            return result;
        }
        #endregion
    }
}