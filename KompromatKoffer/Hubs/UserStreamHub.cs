using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Exceptions;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace KompromatKoffer.Hubs
{

    [Authorize]
    public class UserStreamHub : Hub
    {
        private readonly ILogger _logger;

        public UserStreamHub(ILogger<UserStreamHub> logger)
        {
            _logger = logger;
        }

        public IEnumerable<IUser> AllListMembers;

        public IEnumerable<TweetModel> CurrentTweetModel;

        public ChannelReader<string> UserStream()
        {
            var channel = Channel.CreateUnbounded<string>();

            // We don't want to await WriteItems, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItems(channel.Writer);

            return channel.Reader;
        }

        public class TweetModel
        {

            public string TweetUser { get; set; }
            public string TweetUserProfilePicture { get; set; }
            public string TweetText { get; set; }
            public string TweetFullText { get; set; }
            public string TweetExtendedText { get; set; }
            public List<Tweetinvi.Models.Entities.IHashtagEntity> TweetHashtags { get; set; }
            public int TweetReTweetCount { get; set; }
            public int TweetFavoriteCount { get; set; }
            public int? TweetReplyCount { get; set; }
            public DateTime TweetCreatedAt { get; set; }

        }

        private async Task WriteItems(ChannelWriter<string> writer)
        {
            try
            {

                //Check for Rate Limits
                RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

                RateLimit.QueryAwaitingForRateLimit += (sender, args) =>
                {
                    _logger.LogInformation("Is awaiting for rate limits... " + args.Query);
                };

                //Get TwitterList
                var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);


                var AllMembers = list.GetMembers(list.MemberCount);
                AllListMembers = AllMembers;

                _logger.LogInformation("Members to Follow: " + AllMembers.Count());


                //Create Stream
                var stream = Tweetinvi.Stream.CreateFilteredStream();

                _logger.LogInformation("Start UserStreams");

                //Foreach Member in List addfollow stream
                foreach(var item in AllMembers.Select((value, index) => new { value, index }))
                {
                    stream.AddFollow(item.value.UserIdentifier);
                    _logger.LogInformation("{1} Added User {0} to stream...", item.value.UserIdentifier, item.index);
                    await Task.Delay(1);
                }
                
                //Only Match the addfollows
                stream.MatchOn = MatchOn.Follower;

                stream.MatchingTweetReceived += async (sender, args) =>
                {
                    if (args.Tweet.IsRetweet == true)
                    {
                        _logger.LogInformation("Skipped ReTweet...");
                    }
                    else
                    {
                        // let's use the embeded tweet from tweetinvi
                        var embedTweet = Tweet.GetOEmbedTweet(args.Tweet);

                        var tweet = Tweet.GetTweet(args.Tweet.Id);


                        var tweetModel = new TweetModel
                        {
                            //Tweet Details
                            TweetUser = tweet.CreatedBy.ScreenName,
                            TweetUserProfilePicture = tweet.CreatedBy.ProfileBackgroundImageUrlHttps,
                            TweetText = tweet.Text,
                            TweetFullText = tweet.FullText,
                            TweetExtendedText = tweet.ExtendedTweet.Text,
                            TweetHashtags = tweet.Hashtags,
                            TweetReTweetCount = tweet.RetweetCount,
                            TweetFavoriteCount = tweet.FavoriteCount,
                            TweetReplyCount = tweet.ReplyCount,
                            TweetCreatedAt = tweet.CreatedAt,
                        };
                        
                        //Write OEmbedTweet
                        await writer.WriteAsync(embedTweet.HTML);
                    }
                };

                stream.StartStreamMatchingAllConditions();
            }
            catch(TwitterException ex)
            {
                _logger.LogInformation("Twitter Exception", ex);
            }
            catch (ArgumentException ex)
            {
                _logger.LogInformation("Argument Exception", ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exceptions", ex);
            }

            await Task.Delay(5);
            writer.TryComplete();
        }

    }
}
    
