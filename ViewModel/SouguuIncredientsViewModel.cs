using BATTARI_api.Models.DTO;

namespace BATTARI_api.ViewModel;

public class SouguuIncredientsViewModel
{
    public Dictionary<int, SouguuWebsocketDto> SouguuWebsocketDtos ;
    public ISouguuService SouguuService;

    private Task _timer;
    public SouguuIncredientsViewModel(ISouguuService souguuService)
    {
        this.SouguuService = souguuService;
        SouguuWebsocketDtos = souguuService.GetLatestIncredient();
    }
}