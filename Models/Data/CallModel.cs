using System.ComponentModel.DataAnnotations;
using BATTARI_api.Models.Enum;

namespace BATTARI_api.Models.Data;

public class CallModel
{
    [Key]
    public int CallId { get; set; }
    public CallStatusEnum Status { get; set; }
    public DateTime CallStartTime { get; set; }
    public required string SouguuReason { get; set; }
    public DateTime SouguuDateTime { get; set; }
    public SouguuReasonStatusEnum SouguuReasonStatus { get; set; }
    public int CallTime { get; set; }
    public int User1Id { get; set; }
    public int User2Id { get; set; }
}