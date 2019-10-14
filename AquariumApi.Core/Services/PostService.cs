using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IPostService
    {
        PostReaction UpsertReaction(int reactionId);
        void DeleteReaction(PostReaction reaction);
        //PostComment GetReactionById(int reactionId);

        PostComment UpsertComment(PostComment comment);
        void DeleteComment(PostComment comment);
        //PostComment GetCommentById(int commentId);

        Post UpsertPost(Post post);
        void DeletePost(Post post);
        Post GetPostById(int postId);

        PostThread UpsertThread(Post post);
        void DeleteThread(PostThread thread);
        PostThread GetThreadById(int boardId);

        PostBoard UpsertBoard(PostBoard board);
        void DeleteBoard(PostBoard board);
        PostBoard GetBoardById(int boardId);

    }
    public class PostService : IPostService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;

        public PostService(IConfiguration configuration, IAquariumDao aquariumDao)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
        }

        public void DeleteBoard(PostBoard board)
        {
            throw new NotImplementedException();
        }

        public void DeleteComment(PostComment comment)
        {
            throw new NotImplementedException();
        }

        public void DeletePost(Post post)
        {
            throw new NotImplementedException();
        }

        public void DeleteReaction(PostReaction reaction)
        {
            throw new NotImplementedException();
        }

        public void DeleteThread(PostThread thread)
        {
            throw new NotImplementedException();
        }

        public PostBoard GetBoardById(int boardId)
        {
            throw new NotImplementedException();
        }

        public Post GetPostById(int postId)
        {
            throw new NotImplementedException();
        }

        public PostThread GetThreadById(int boardId)
        {
            throw new NotImplementedException();
        }

        public PostBoard UpsertBoard(PostBoard board)
        {
            throw new NotImplementedException();
        }

        public PostComment UpsertComment(PostComment comment)
        {
            throw new NotImplementedException();
        }

        public Post UpsertPost(Post post)
        {
            throw new NotImplementedException();
        }

        public PostReaction UpsertReaction(int reactionId)
        {
            throw new NotImplementedException();
        }

        public PostThread UpsertThread(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
