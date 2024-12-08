using Microsoft.SemanticKernel;
using System.ComponentModel;

public class HikingMateWishlistService
{
    public event EventHandler? NeedUpdateUI;
    public List<HikingTrail> wishlistItems;

    public HikingMateWishlistService(List<HikingTrail> wishlistItems)
    {
        this.wishlistItems = wishlistItems;
    }

    [KernelFunction]
    [Description("Adds multiple hiking trails to the user's wishlist in a single request.")]
    public List<HikingTrail> AddMultipleHikingTrailsToWishlist(List<HikingTrail> items)
    {
        foreach (var item in items)
            item.Id = Guid.NewGuid().ToString();

        wishlistItems.AddRange(items);
      
        NeedUpdateUI?.Invoke(this, new EventArgs());

        return wishlistItems;
    }

    [KernelFunction]
    [Description("Adds new hiking trails to the user's wishlist.")]
    public HikingTrail AddHikingTrailsToWishlist(HikingTrail item)
    {
        item.Id = Guid.NewGuid().ToString();

        wishlistItems.Add(item);
        NeedUpdateUI?.Invoke(this, new EventArgs());

        return item;
    }

    [KernelFunction]
    [Description("Adds new hiking trails to the user's wishlist.")]
    public (HikingTrail? Result, string Message) UpdateHikingTrailInWishlist(HikingTrail hikingTrail)
    {
        var wishlist = wishlistItems.FirstOrDefault(x => x.Id == hikingTrail.Id);
        if (wishlist == null)
            return (null, "목록에 해당 Wishlist가 없습니다.");

        wishlistItems.Remove(wishlist);
        wishlistItems.Add(hikingTrail);

        NeedUpdateUI?.Invoke(this, new EventArgs());

        return (hikingTrail, "성공");
    }

    [KernelFunction]
    [Description("Removes a hiking trail from the user's wishlist.")]
    public (bool Success, string Message) RemoveHikingTrailFromWishlist(string id)
    {
        var wishlist = wishlistItems.FirstOrDefault(x => x.Id == id);
        if (wishlist == null)
            return (false, "목록에 해당 Wishlist가 없습니다.");

        wishlistItems.Remove(wishlist);

        NeedUpdateUI?.Invoke(this, new EventArgs());
        return (true, "성공");
    }
}