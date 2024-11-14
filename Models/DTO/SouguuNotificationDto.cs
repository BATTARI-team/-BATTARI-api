namespace BATTARI_api.Models.DTO;

public class SouguuNotificationDto
{
    public required DateTime CallStartTime { get; set; }
    public required int CallId { get; set; }
    public required DateTime CallEndTime { get; set; }
    public required string SouguuReason { get; set; }
    public required string Token { get; set; }
    public required DateTime SouguuDateTime { get; set; }
    public required int AiteUserId { get; set; }
}