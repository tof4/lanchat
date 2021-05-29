using System.ComponentModel.DataAnnotations;

namespace Lanchat.Core.FileTransfer.Models
{
    internal class FileReceiveRequest
    {
        [Required] public string FileName { get; init; }
        [Required] public long PartsCount { get; init; }
    }
}