using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2WebMVC.Models.JunctionModels;
using P2WebMVC.Types;

namespace P2WebMVC.Models.DomainModels
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        // User relationship
        public required Guid UserId { get; set; }  // FK
        [ForeignKey("UserId")]
        public User? Buyer { get; set; }  // Navigation property

        // Address relationship
        public required Guid AddressId { get; set; }  // FK
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }  // Navigation property

        // Products in this order
        public required ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        // Pricing & status
        public required decimal TotalPrice { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public PaymentMode PaymentMode { get; set; } = PaymentMode.None;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.pending;
        public DateTime? ShippingDate { get; set; } = DateTime.UtcNow.AddDays(7);
    }
}
