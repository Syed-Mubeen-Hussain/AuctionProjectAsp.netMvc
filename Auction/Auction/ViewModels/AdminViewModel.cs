using Auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.ViewModels
{
    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            if (pageSize == 0) pageSize = 10;

            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
    }

    public class CategoryViewModel
    {
        public List<Category> categories { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class ProductViewModel
    {
        public List<Product> products { get; set; }
        public string SearchTerm { get; set; }
        public List<image> image { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class AdminReviewsViewModel
    {
        public List<Review> reviews { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class CustomerViewModel
    {
        public List<Customer> customers { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class AdminOrderViewModel
    {
        public List<order> orders { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class OrderDetailsViewModel
    {
        public order order { get; set; }
        public List<Product> products { get; set; }
    }

    public class ContactViewModel
    {
        public List<Contact> contacts { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class AdminLoginViewModel
    {
        public string ad_userName { get; set; }
        public string ad_password { get; set; }
        public bool RememberMe { get; set; }
    }
}