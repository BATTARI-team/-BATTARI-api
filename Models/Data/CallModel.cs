using System.ComponentModel.DataAnnotations;
using BATTARI_api.Models.Enum;

namespace BATTARI_api.Models.Data;

public class CallModel
{
    [Key]
    public int CallId { get; set; }
    public required CallStatusEnum Status { get; set; }
    public required DateTime CallStartTime { get; set; }
    public required string SouguuReason { get; set; }
    public required DateTime SouguuDateTime { get; set; }
    public SouguuReasonStatusEnum SouguuReasonStatus { get; set; }
    public required int CallTime { get; set; }
    public required int User1Id { get; set; }
    public required int User2Id { get; set; }
}