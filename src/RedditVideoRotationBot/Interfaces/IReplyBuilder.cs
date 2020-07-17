namespace RedditVideoRotationBot.Interfaces
{
    public interface IReplyBuilder
    {
        string BuildReply(RedditReplyBuilderParameters replyBuilderParameters);
    }
}
