using Markdig;

namespace HikingMate.Models
{
    public class ChatMessage
    {
        public string Id { get; set; }

        public string? Message { get; set; }

        public ChatMessageRole Role { get; set; }

        public ChatMessage(ChatMessageRole role, string? message = null)
        {
            Id = Guid.NewGuid().ToString();
            Role = role;
            Message = message;
        }

        public string Username => Role == ChatMessageRole.User ? "사용자" : "🏞️Hiking mate";

        public string CSS
        {
            get
            {
                switch (Role)
                {
                    case ChatMessageRole.Assistant:
                        return "received";
                    case ChatMessageRole.User:
                        return "sent";
                    case ChatMessageRole.System:
                    default:
                        return "system";
                }
            }
        }
    }
}

public enum ChatMessageRole { System, User, Assistant }

public class HikingRecord
{
    public string Title { get; set; }

    public string Date { get; set; }

    public string Content { get; set; }
}

public class HikingTrail
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }
}