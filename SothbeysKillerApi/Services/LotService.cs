using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public interface ILotService
{
    LotResponse LotInfoById(Guid lotId);
    List<LotResponse> GetLotsByAuctionId(Guid auctionId);
    Guid CreateLot(CreateLotRequest request);
    void ModifyLotById(Guid lotId, ModifyLotRequest request);
    void DeleteLotById(Guid lotId);
}

public class LotService : ILotService
{
    public static List<Lot> lotsStorage = [];
    
    public LotResponse LotInfoById(Guid lotId)
    {
        var lot = lotsStorage
            .Where(lot => lot.Id == lotId)
            .Select(lot => new LotResponse(lot.Id, lot.AuctionId, lot.Title, lot.Description, lot.StartPrice, lot.PriceStep))
            .FirstOrDefault();

        if (lot is null)
        {
            throw new NullReferenceException("No lots found");
        }

        return lot;
    }
    
    public List<LotResponse> GetLotsByAuctionId(Guid auctionId)
    {
        if (AuctionService.auctionsStorage.All(auc => auc.Id != auctionId))
        {
            throw new ArgumentException("No auction found");
        }
        
        var lots = lotsStorage
            .Where(lot => lot.AuctionId == auctionId)
            .Select(lot => new LotResponse(lot.Id, lot.AuctionId, lot.Title, lot.Description, lot.StartPrice, lot.PriceStep))
            .OrderBy(lot => lot.Title)
            .ToList();

        if (!lots.Any())
        {
            throw new NullReferenceException("No lots found");
        }
        
        return lots;
    }

    public Guid CreateLot(CreateLotRequest request)
    {
        if (AuctionService.auctionsStorage.All(auc => auc.Start <= DateTime.Now && auc.Id != request.AuctionId))
        {
            throw new ArgumentException("No auction found");
        }

        var lot = new Lot()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            StartPrice = request.StartPrice,
            PriceStep = request.PriceStep,
            AuctionId = request.AuctionId
        };
        
        lotsStorage.Add(lot);

        return lot.Id;
    }

    public void ModifyLotById(Guid lotId, ModifyLotRequest request)
    {
        if (request.StartPrice <= 0 || request.PriceStep <= 0)
        {
            throw new ArgumentException();
        }

        if (request.Title.Length < 3 || request.Title.Length > 255 || request.Description.Length < 3 ||
            request.Description.Length > 255)
        {
            throw new ArgumentException();
        }
        
        var selectedLot = lotsStorage.FirstOrDefault(lot => lot.Id == lotId);

        if (selectedLot is null)
        {
            throw new NullReferenceException("No lots found");
        }
        
        if (AuctionService.auctionsStorage.All(auction => auction.Id != selectedLot.AuctionId && auction.Start <= DateTime.Now))
        {
            throw new ArgumentException("No matching auction found.");
        }
        
        lotsStorage.Remove(selectedLot);
        
        var lot = new Lot()
        {
            Id = selectedLot.Id,
            Title = request.Title,
            Description = request.Description,
            StartPrice = request.StartPrice,
            PriceStep = request.PriceStep,
            AuctionId = selectedLot.AuctionId
        };
        
        lotsStorage.Add(lot);
    }

    public void DeleteLotById(Guid lotId)
    {
        var selectedLot = lotsStorage.FirstOrDefault(lot => lot.Id == lotId);

        if (selectedLot is null)
        {
            throw new NullReferenceException("No lots found");
        }
        
        if (AuctionService.auctionsStorage.All(auction => auction.Id != selectedLot.AuctionId && auction.Start <= DateTime.Now))
        {
            throw new ArgumentException("No matching auction found.");
        }
        
        lotsStorage.Remove(selectedLot);
    }
}