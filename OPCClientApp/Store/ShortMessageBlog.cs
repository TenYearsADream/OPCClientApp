using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCClientApp.Store
{
    internal class ShortMessageBlog
    {
        private const string RedisKeyPrefix = "QTY:";

        private IDatabase _redis = null;
        public ShortMessageBlog(IDatabase redis)
        {
            _redis = redis;
        }

        public void AddCounter(string address, int count, int time)
        {
            //var keyUserList = RedisKeyPrefix + "CounterList";
            //if (!_redis.SetContains(keyUserList, address))
            //{
            //    //throw new InvalidOperationException("Counter already exists");
            //    _redis.SetAdd(keyUserList, address);
            //}

            var keyAnton = RedisKeyPrefix + "{" + address + "}:CounterInfo";
            _redis.HashSet(keyAnton, new HashEntry[]
            {
                new HashEntry("qty", count),
                new HashEntry("time", time)
            });
        }

        public PinValue GetCounter(string address)
        {
            PinValue pinValue = new PinValue();

            var keyAnton = RedisKeyPrefix + "{" + address + "}:CounterInfo";
            var hashEntry = _redis.HashGetAll(keyAnton);
            if (hashEntry != null && hashEntry.Length > 0)
            {
                pinValue.Count = (int)hashEntry[0].Value;
                pinValue.Time = (int)hashEntry[1].Value;
            }

            return pinValue;

        }

        public void AddPost(string username, string post)
        {
            var keyUserPosts = RedisKeyPrefix + "{" + username + "}:Posts";

            var score = DateTime.UtcNow.Ticks;

            _redis.SortedSetAdd(keyUserPosts, new SortedSetEntry[]
            {
                new SortedSetEntry(post, score)
            });
        }

        public string[] GetHomePage()
        {
            var homePagePosts = _redis.SortedSetRangeByRank(RedisKeyPrefix + "homepage", 0, -1);

            if (homePagePosts.Length == 0)
            {
                var allKeys_UserPosts = GetKeysForAllUserPostSets();

                var txHomePage = _redis.CreateTransaction(); // MULTI
                txHomePage.AddCondition(Condition.KeyNotExists(RedisKeyPrefix + "homepage")); // WATCH
                txHomePage.SortedSetCombineAndStoreAsync(SetOperation.Union, RedisKeyPrefix + "homepage", allKeys_UserPosts);
                txHomePage.SortedSetRemoveRangeByRankAsync(RedisKeyPrefix + "homepage", 0, -3); // Top 2
                txHomePage.Execute(); // EXEC
            }

            homePagePosts = _redis.SortedSetRangeByRank(RedisKeyPrefix + "homepage", 0, -1);

            return homePagePosts.ToStringArray();
        }

        public string[] GetAllPosts()
        {
            var allKeys_UserPosts = GetKeysForAllUserPostSets();

            var txHomePage = _redis.CreateTransaction(); // MULTI
            txHomePage.SortedSetCombineAndStoreAsync(SetOperation.Union, RedisKeyPrefix + "allposts", allKeys_UserPosts);
            var allposts = txHomePage.SortedSetRangeByRankAsync(RedisKeyPrefix + "allposts");
            txHomePage.KeyDeleteAsync(RedisKeyPrefix + "allposts"); // created and deleted within MULTI, so never visible/conflict with another client
            txHomePage.Execute(); // EXEC

            allposts.Wait(); // notice: this wasn't the last command in the MULTI-EXEC/transaction - how cool is that! :)

            return allposts.Result.ToStringArray();
        }

        private RedisKey[] GetKeysForAllUserPostSets()
        {
            var keyUserList = RedisKeyPrefix + "UserList";
            var usernames = _redis.SetMembers(keyUserList);

            var keys = new List<RedisKey>();
            foreach (var u in usernames)
            {
                keys.Add(RedisKeyPrefix + "{" + u.ToString() + "}:Posts");
            }
            return keys.ToArray();
        }

        public void AddFollower(string username, string follower)
        {
            var keyFollowing = RedisKeyPrefix + "{" + follower + "}:following";
            _redis.SetAdd(keyFollowing, username);

            var keyFollower = RedisKeyPrefix + "{" + username + "}:followers";
            _redis.SetAdd(keyFollower, follower);
        }

        public string[] GetFeed(string user)
        {
            // Challenge:
            // follow the same pattern as for Get Home Page,
            // but get only the posts for users in the Following set of the specified user...

            throw new NotImplementedException();
        }
    }

    public struct PinValue
    {
        public int Count { get; set; }
        public int Time { get; set; }
    }
}

