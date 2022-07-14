using Auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.ViewModels
{
    public class HomePageViewModel
    {
        public List<Category> categories { get; set; }
        public List<Product> products { get; set; }
        public List<image> images { get; set; }
        public List<Bid> currentBid { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Customer> customers { get; set; }
    }

    public class AboutUsViewModel
    {
        public List<Review> Reviews { get; set; }
        public List<Customer> customers { get; set; }
    }

    public class ShopViewModel
    {
        public List<Product> featuredProducts { get; set; }
        public List<Product> products { get; set; }
        public int? sortBy { get; set; }
        public string search { get; set; }
        public List<image> image { get; set; }
    }

    public class FilterShopViewModel
    {
        public List<Product> products { get; set; }
        public int? sortBy { get; set; }
        public string search { get; set; }
        public List<image> image { get; set; }
    }

    public class SingleProductViewModel
    {
        public Product product { get; set; }
        public Bid highestBid { get; set; }
        public List<image> images { get; set; }
        public List<Bid> bids { get; set; }
        public Customer customer {get; set; }
        public decimal ProductViews { get; set; }
        public int totalBidders { get; set; }
        public int totalBids { get; set; }
    }

    public class ShowBiddersHistoryViewModel
    {
        public List<Customer> customers { get; set; }
        public List<Bid> bids { get; set; }
        public List<Bid> totalAmount { get; set; }
    }    

    public class CartViewModel
    {
        public List<Product> products { get; set; }
        public List<image> images { get; set; }
    }

    public class CheckoutViewModel
    {
        public List<Product> products { get; set; }
        public Customer customer { get; set; }
    }

    public class OrderViewModel
    {
        public List<order> order { get; set; }
        public List<order_details> order_details { get; set; }
        public List<Product> products { get; set; }
        public List<image> images { get; set; }
    }

    public class CategoryProductViewModel
    {
        public List<Product> featuredProducts { get; set; }
        public List<Product> products { get; set; }
        public int? sortBy { get; set; }
        public string search { get; set; }
        public List<image> image { get; set; }
        public int categoryId { get; set; }
        public Category category { get; set; }
    }

    public class CustomerMyBidsViewModel
    {
        public Customer customer { get; set; }
        public List<Product> products { get; set; }
        public List<image> images { get; set; }
    }
    
}