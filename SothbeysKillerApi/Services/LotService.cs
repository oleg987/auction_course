using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public interface ILotService
{
    LotResponse LotInfoById(Guid lotId);
    List<LotResponse> GetLotsByAuctionId(Guid auctionId);
    Guid CreateLot(CreateLotRequest request);
    void ModifyLotById(ModifyLotRequest request);
    void DeleteLotById(Guid lotId);
}

public class LotService : ILotService
{
    public static List<Lot> _lotsStorage = [];
    
    public LotResponse LotInfoById(Guid lotId)
    {
        var lot = _lotsStorage
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
        if (AuctionService.auctionsStorage.Any(auc => auc.Id == auctionId))
        {
            throw new ArgumentException("No auction found");
        }
        
        var lots = _lotsStorage
            .Where(lot => lot.AuctionId == auctionId)
            .Select(lot => new LotResponse(lot.Id, lot.AuctionId, lot.Title, lot.Description, lot.StartPrice, lot.PriceStep))
            .OrderBy(lot => lot.Title)
            .ToList();

        if (lots.Count == 0)
        {
            throw new NullReferenceException("No lots found");
        }
        
        return lots;
    }

    public Guid CreateLot(CreateLotRequest request)
    {
        if (!AuctionService.auctionsStorage.All(auc => auc.Start > DateTime.Now && auc.Id == request.AuctionId))
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
        
        _lotsStorage.Add(lot);

        return lot.Id;
    }

    public void ModifyLotById(ModifyLotRequest request)
    {
        var selectedLot = _lotsStorage.FirstOrDefault(lot => lot.Id == request.Id);

        if (selectedLot is null)
        {
            throw new NullReferenceException("No lots found");
        }
        
        if (AuctionService.auctionsStorage.All(auction => auction.Id != selectedLot.AuctionId && auction.Start <= DateTime.Now))
        {
            throw new ArgumentException("No matching auction found.");
        }
        
        _lotsStorage.Remove(selectedLot);
        
        var lot = new Lot()
        {
            Id = selectedLot.Id,
            Title = request.Title,
            Description = request.Description,
            StartPrice = request.StartPrice,
            PriceStep = request.PriceStep,
            AuctionId = selectedLot.AuctionId
        };
        
        _lotsStorage.Add(lot);
    }

    public void DeleteLotById(Guid lotId)
    {
        var selectedLot = _lotsStorage.FirstOrDefault(lot => lot.Id == lotId);

        if (selectedLot is null)
        {
            throw new NullReferenceException("No lots found");
        }
        
        if (AuctionService.auctionsStorage.All(auction => auction.Id != selectedLot.AuctionId && auction.Start <= DateTime.Now))
        {
            throw new ArgumentException("No matching auction found.");
        }
        
        _lotsStorage.Remove(selectedLot);
    }
}