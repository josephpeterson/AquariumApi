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
        Post CreatePost(Post post);
        PostBoard CreatePostBoard(PostBoard board);
        PostCategory CreatePostCategory(PostCategory category);
        PostThread CreatePostThread(PostThread thread);
        void DeleteBoard(int boardId);
        void DeleteCategory(int categoryId);
        void DeletePost(int postId);
        void DeleteThread(int threadId);
        PostBoard GetBoardById(int boardId);
        Post GetPostById(int postId);
        List<PostCategory> GetPostCategories();
        PostThread GetThreadById(int threadId);
    }
    public class PostService : IPostService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;
        private IAccountService _accountService;

        public PostService(IConfiguration configuration, IAquariumDao aquariumDao, IAccountService accountService)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
            _accountService = accountService;
        }
        public List<PostCategory> GetPostCategories()
        {
            return _aquariumDao.GetPostCategories();
        }
        public PostBoard GetBoardById(int boardId)
        {
            return _aquariumDao.GetBoardById(boardId);
        }
        public PostThread GetThreadById(int threadId)
        {
            var thread = _aquariumDao.GetThreadById(threadId);
            return thread;
        }
        public Post GetPostById(int postId)
        {
            var post =  _aquariumDao.GetPostById(postId);
            return post;
        }
        public PostCategory CreatePostCategory(PostCategory category)
        {
            category.Name = category.Name?.Trim();
            if (category.Name == null)
                throw new Exception("Invalid category name");

            var nameInUse = GetPostCategories().Where(c => c.Name == category.Name).Any();
            if (nameInUse)
                throw new Exception("Category name already exists");
            

            return _aquariumDao.CreatePostCategory(category);
        }
        public PostBoard CreatePostBoard(PostBoard board)
        {
            board.AuthorId = _accountService.GetCurrentUserId();
            board.Timestamp = DateTime.Now;
            return _aquariumDao.CreatePostBoard(board);
        }
        public PostThread CreatePostThread(PostThread thread)
        {
            thread.AuthorId = _accountService.GetCurrentUserId();
            thread.Timestamp = DateTime.Now;

            //Make sure  there's a first post
            var firstPost = thread.Posts.First();
            firstPost.Content = firstPost.Content?.Trim();
            if (firstPost.Content == null)
                throw new Exception("Post must contain content");
            firstPost.AuthorId = thread.AuthorId;
            firstPost.Timestamp = DateTime.Now;

            return _aquariumDao.CreatePostThread(thread);
        }
        public Post CreatePost(Post post)
        {
            post.Content = post.Content?.Trim();
            if (post.Content == null)
                throw new Exception("Post must contain content");

            post.AuthorId = _accountService.GetCurrentUserId();
            post.Timestamp = DateTime.Now;
            return _aquariumDao.CreatePost(post);
        }
        public void DeleteCategory(int categoryId)
        {
            _aquariumDao.DeletePostCategory(categoryId);
        }
        public void DeleteBoard(int boardId)
        {
            _aquariumDao.DeletePostBoard(boardId);
        }
        public void DeleteThread(int threadId)
        {
            _aquariumDao.DeletePostThread(threadId);
        }
        public void DeletePost(int postId)
        {
            _aquariumDao.DeletePost(postId);
        }

    }
}
