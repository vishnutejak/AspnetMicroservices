using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _respository;

        public DiscountController(IDiscountRepository discountRepository)
        {
            _respository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
        }

        [HttpGet("{productName}",Name ="GetDiscount")]
        [ProducesResponseType(typeof(Coupon),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var coupon = await _respository.GetDiscount(productName);

            return Ok(coupon);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {
            var inserted = await _respository.CreateDiscount(coupon);

            return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
        {
            var inserted = await _respository.UpdateDiscount(coupon);

            return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscount(string productName)
        {
           return Ok( await _respository.DeleteDiscount(productName));
        }
    }
}
