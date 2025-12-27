using System;
using System.ComponentModel.DataAnnotations;
using P2WebMVC.Models.JunctionModels;
using P2WebMVC.Types;

namespace P2WebMVC.Models.DomainModels;

public class Product
{

    [Key]
    public Guid ProductId { get; set; } = Guid.NewGuid();
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductImage { get; set; }
    public decimal ProductPrice { get; set; }
    public int ProductStock { get; set; }
    public ProductCategory Category { get; set; }

    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Weight { get; set; }


    public ICollection<CartProduct> ProductInCarts { get; set; } = [];   // navigation property //  collection of products in the cart
    public ICollection<OrderProduct> ProductInOrders { get; set; } = []; //  collection of products in the order

    public bool IsDeleted { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



}
