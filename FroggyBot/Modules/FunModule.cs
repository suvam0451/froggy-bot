using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FroggyBot.Commands;
using FroggyBot.Database;
using FroggyBot.Database.Models;
using FroggyBot.Util;
using Nekos.Net;
using Nekos.Net.Endpoints;
using System;


namespace FroggyBot.Modules
{
    // Derive to produce Randomization
    interface IFunModule
    {
        // Use to generate embeds
        EmbedBuilder TaggedSelf();
        EmbedBuilder TaggedBot();
        EmbedBuilder TaggedPerson();
        // EmbedBuilder TaggedMulti(); // TODO: Add multi-ping
    }

    /**
    *   Holds information about the sender and the reciever(s)
    *   Also contains repeatedly used commands
    */
    public class FunModuleBase : ModuleBase<ShardedFroggyCommandContext>
    {
        // ID store
        protected ulong senderId;
        protected ulong recieverId;
        protected ulong botId;

        // Username store
        protected string senderNick = null;
        protected string recieverNick = null;

        // Embed string/mention store
        protected string senderText = null;
        protected string recieverText = null;

        // Internally Used classes
        protected string imageUrl = null;
        protected EmbedBuilder retval = null;

        // Shared embed code
        protected EmbedBuilder NekosEmbedBase()
            => new EmbedBuilder()
                .WithImageUrl(imageUrl)
                .WithFooter("Powered by Nekos.Life");

        // Default bot response when the bot is lewded
        protected static EmbedBuilder BotLewdedResponse()
            => new EmbedBuilder()
                .WithImageUrl("https://media1.tenor.com/images/5ddd0704271739e00b6985fbfdfeda21/tenor.gif?itemid=12962907")
                .WithDescription("Nowo, if you do that I can't get mawwied (⁄ ⁄•⁄ω⁄•⁄ ⁄)⁄");

        protected static EmbedBuilder BotLonelyResponse() => new EmbedBuilder().
        // Syntactic sugar to respon with an embed
        protected Task<IUserMessage> SendEmbed(Embed embed)
            => ReplyAsync(string.Empty, embed: embed);

        // Makes an embed from an array of description builders
        protected EmbedBuilder MakeASweetEmbed(string[] _ref)
            => NekosEmbedBase()
                .WithDescription(_ref[RandArrange(_ref)]);

        // Uses the message to update { sender, reciever, botId }
        protected void UpdateFromContext(ShardedFroggyCommandContext _context,
            IUser _reciever = null)
        {
            // Update sender details
            senderId = Context.Message.Author.Id;
            senderNick = Context.Message.Author.Username;
            senderText = MentionTextGenerator(senderId);
            botId = Context.Client.CurrentUser.Id;
            if (_reciever != null)
            {
                // Update recieve details
                recieverId = _reciever.Id;
                recieverNick = _reciever.Username;
                recieverText = MentionTextGenerator(recieverId);
            }
        }

        // Use this to turn the user pinging on/off => { x, $"@<x>" }
        protected string MentionTextGenerator(ulong x) => $"<@{x}>";

        // Returns a random index between [0, length - 1]
        protected int RandArrange(string[] _ref) => new Random().Next(0, _ref.Length);
    }

    // Later gator
    // public class FFmpegModule : FunModuleBase
    // {
    //     [Command("ffmpeg")]
    //     public async Task FFmpegAsync()
    //     {
    //         CommandRunner.RunCommand("ffmpeg", true);
    //         await ReplyAsync($@"Done");
    //     }
    // }

    // ----------------- HUG ------------------------
    public class HugModule : FunModuleBase, IFunModule
    {
        // IFunModule
        public EmbedBuilder TaggedBot() => BotLewdedResponse();

        public EmbedBuilder TaggedPerson()
            => MakeASweetEmbed(new String[]{
                $"{senderText} transfers 3 units energy to {recieverText}",
                $"{senderText} consomes 3 units stress from {recieverText}"});

        public EmbedBuilder TaggedSelf()
            => MakeASweetEmbed(new string[]{
                $"{senderText} curls up to seek warmth in a cold, winter night..." });

        // Command Handler
        [Command("hug")]
        public async Task TaskAsync(IUser user)
        {
            UpdateFromContext(Context, user);
            imageUrl = (await NekosClient.GetSfwAsync(SfwEndpoint.Hug)).FileUrl;

            if (senderId == recieverId) retval = TaggedSelf();
            else
                retval = (recieverId == botId) ? TaggedBot() : TaggedPerson();

            await SendEmbed(retval.Build());
        }
    }

    // -------------------- KISS ---------------------
    public class KissModule : FunModuleBase, IFunModule
    {
        // IFunModule
        public EmbedBuilder TaggedBot() => BotLewdedResponse();

        public EmbedBuilder TaggedPerson()
            => MakeASweetEmbed(new String[]{
                $"{senderText} performs lewd attacks on {recieverText}",
                $"{senderText} pulls a sneaky on {recieverText}'s cheeky"});

        public EmbedBuilder TaggedSelf()
            => MakeASweetEmbed(new string[]{
                $"{senderText} are uhhh... licking themselves apparently" });

        // Command Handler
        [Command("kiss")]
        public async Task TaskAsync(IUser user)
        {
            UpdateFromContext(Context, user);
            imageUrl = (await NekosClient.GetSfwAsync(SfwEndpoint.Kiss)).FileUrl;

            if (senderId == recieverId) retval = TaggedSelf();
            else
                retval = (recieverId == botId) ? TaggedBot() : TaggedPerson();

            await SendEmbed(retval.Build());
        }
    }

    // -------------------- FEED -------------------------
    public class FeedModule : FunModuleBase, IFunModule
    {
        // IFunModule
        public EmbedBuilder TaggedBot() => BotLewdedResponse();

        public EmbedBuilder TaggedPerson()
            => MakeASweetEmbed(new String[]{
                $"{senderText} is now a mommy bird. {recieverText}, say aaah <3"});

        public EmbedBuilder TaggedSelf()
            => MakeASweetEmbed(new string[]{
                $"{senderText} had a box of lunch... but {senderText} had somebody to share it with because {senderText} is a fucking weirdo" });


        [Command("feed")]
        public async Task TaskAsync(IUser user)
        {
            UpdateFromContext(Context, user);
            if (user != null)
            {
                imageUrl = (await NekosClient.GetSfwAsync(SfwEndpoint.Feed)).FileUrl;
                var baseEmbed = NekosEmbedBase();
                var embed = baseEmbed
                    .WithAuthor("Say aaaaahn~", url: imageUrl)
                    .Build();

                await ReplyAsync(string.Empty, embed: embed);
            }
        }
    }

    // ----------------- CUDDLE --------------------------
    public class CuddleModule : FunModuleBase, IFunModule
    {
        // IFunModule
        public EmbedBuilder TaggedBot() => BotLewdedResponse();

        public EmbedBuilder TaggedPerson()
            => MakeASweetEmbed(new String[]{
                $"{senderText} is trying to crawl up-to {recieverText}"});

        public EmbedBuilder TaggedSelf()
            => MakeASweetEmbed(new string[] { $"sexy alone time !!!" });


        [Command("cuddle")]
        public async Task TaskAsync(IUser user)
        {
            UpdateFromContext(Context, user);
            if (user != null)
            {
                imageUrl = (await NekosClient.GetSfwAsync(SfwEndpoint.Cuddle)).FileUrl;
                var baseEmbed = NekosEmbedBase();
                var senderId = Context.Message.Author.Id;
                var embed = baseEmbed
                    .WithAuthor($"A truly lecherous tackle by <@{senderId}> ")
                    .WithDescription($"A truly lecherous tackle by <@{senderId}> ")
                    .Build();

                await ReplyAsync(string.Empty, embed: embed);
            }
        }
    }

    public class SexProtestModule : FunModuleBase
    {
        [Command("sex")]
        public async Task TaskAsync()
        {
            var embed = new EmbedBuilder()
            .WithImageUrl("https://cdn.discordapp.com/attachments/845381765752291349/847543837353312286/tenor_2-3.gif")
            .Build();

            await ReplyAsync(string.Empty, embed: embed);
        }
    }
}