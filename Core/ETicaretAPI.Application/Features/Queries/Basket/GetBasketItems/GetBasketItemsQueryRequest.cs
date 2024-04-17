using MediatR;

namespace ETicaretAPI.Application.Features.Queryies.Basket.GetBasketItems
{
    public class GetBasketItemsQueryRequest:IRequest<List<GetBasketItemsQueryResponse>>
    {
    }
}