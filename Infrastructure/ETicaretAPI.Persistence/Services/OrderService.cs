using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class OrderService : IOrderService
    {
        readonly IOrderWriteRepository _orderWriteRepository;
        readonly IOrderReadRepository _orderReadRepository;
        readonly ICompletedOrderWriteRepository _completedOrderWriteRepository;
        readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository, ICompletedOrderWriteRepository completedOrderWriteRepository, ICompletedOrderReadRepository completedOrderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _completedOrderWriteRepository = completedOrderWriteRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
        }

        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
            string orderCode = new Random().NextDouble().ToString();
            await _orderWriteRepository.AddAsync(new()
            {               
                Address = createOrder.Address,
                Id = Guid.Parse(createOrder.BasketId),
                Description = createOrder.Description,
                OrderCode = (orderCode.Substring(orderCode.IndexOf(',')+2))
            });
            await _orderWriteRepository.SaveAsync();
        }

        public async Task<ListOrder> GetAllOrdersAsync(int page, int size)
        { var query = _orderReadRepository.Table.Include(o => o.Basket)
                .ThenInclude(b => b.User)
                .Include(o => o.Basket)
                    .ThenInclude(b => b.BasketItems)
                    .ThenInclude(bi => bi.Product); //order lar geliyor

            var data = query.Skip(size * page).Take(size); //sorgunun devamını burada getirdik
            //.Take((page*size)..size)//yeni özellik index özelliği       


            var data2 = from order in data // data nın içinde ilgili sayfaya ait olan daha az veri ile işlem yapıyoruz
                    join completedOrder in _completedOrderReadRepository.Table
                    on order.Id equals completedOrder.OrderId into co
                    from _co in co.DefaultIfEmpty()//left join order a karşılık boş olmayanların verilerini _co getirecek
                    select new
                    {
                        Id = order.Id,
                        CreateDate = order.CreateDate,
                        OrderCode = order.OrderCode,
                        Basket = order.Basket,
                        Completed = _co != null ? true : false //eğer CompletedOrde a karşılık bir değer var ise True dön dedik ilgili sayfada tamamlanan siparişleri göstermemek için
                    };

            return new()
            {
                TotalOrderCount = await query.CountAsync(),//tümünün sayısını ...
                Orders = await data2.Select(o => new
                {
                    Id = o.Id,
                    CreateDate=o.CreateDate,
                    OrderCode=o.OrderCode,
                    TotalPrice=o.Basket.BasketItems.Sum(bi=>bi.Product.Price * bi.Quantity),
                    UserName = o.Basket.User.UserName,
                    o.Completed
                }).ToListAsync()// ...istediğimizin datalarını aldık fenaa  Iqueryable ın sağladığı esneklik
            };
        }

        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            var data = _orderReadRepository.Table
                .Include(o => o.Basket)
                    .ThenInclude(b => b.BasketItems)
                        .ThenInclude(bi => bi.Product);

            var data2 = await (from order in data
                        join completedOrder in _completedOrderReadRepository.Table
                        on order.Id equals completedOrder.OrderId into co
                        from _co in co.DefaultIfEmpty()
                        select new
                        {
                            Id = order.Id,
                            CreateDate = order.CreateDate,
                            OrderCode = order.OrderCode,
                            Basket = order.Basket,
                            Completed = _co != null ? true : false,
                            Address = order.Address,
                            Description = order.Description
                        }).FirstOrDefaultAsync(o=>o.Id ==Guid.Parse(id));



            return new()
            {
                Id = data2.Id.ToString(),
                BasketItems = data2.Basket.BasketItems.Select(bi => new
                {
                    bi.Product.Name,
                    bi.Product.Price,
                    bi.Quantity
                }),
                Address = data2.Address,
                CreatedDate = data2.CreateDate,
                OrderCode = data2.OrderCode,
                Completed = data2.Completed
            };
        }

        public async Task<(bool,CompletedOrderDTO)> CompleteOrderAsync(string id)
        {
            Order? order = await _orderReadRepository.Table.Include(o => o.Basket)
                .ThenInclude(b=> b.User)
                .FirstOrDefaultAsync(o=>o.Id == Guid.Parse(id));
            if (order != null)
            {
                await _completedOrderWriteRepository.AddAsync(new() { OrderId =Guid.Parse(id)});
                return (await _completedOrderWriteRepository.SaveAsync() > 0,new CompletedOrderDTO { 
                    OrderCode = order.OrderCode,
                    OrderDate = order.CreateDate,
                    NameSurname = order.Basket.User.NameSurname,
                    Email = order.Basket.User.Email,
                    });
            }
            return (false,null);

        }
    }
}
