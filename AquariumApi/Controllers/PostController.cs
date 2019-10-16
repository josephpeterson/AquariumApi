using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AquariumApi.Controllers
{
    [Route("/v1/Post/")]
    public class PostController : Controller
    {
        public readonly IAquariumService _aquariumService;
        private readonly IPostService _postService;
        private readonly IPhotoManager _photoManager;
        public readonly ILogger<PostController> _logger;
        public PostController(IAquariumService aquariumService, IPostService postService, ILogger<PostController> logger)
        {
            _aquariumService = aquariumService;
            _postService = postService;
            _logger = logger;
        }

        [HttpGet]
        [Route("Categories")]
        public IActionResult GetAllPostCategories()
        {
            try
            {
                var data = _postService.GetPostCategories();
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Post/Categories endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpGet]
        [Route("Board/{boardId}")]
        public IActionResult GetPostBoard(int boardId)
        {
            try
            {
                var data = _postService.GetBoardById(boardId);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Post/Board/{boardId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpGet]
        [Route("Thread/{threadId}")]
        public IActionResult GetPostThread(int threadId)
        {
            try
            {
                var data = _postService.GetThreadById(threadId);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Post/Thread/{threadId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpPost]
        [Route("Category")]
        public IActionResult CreatePostCategory([FromBody]PostCategory postCategory)
        {
            try
            {
                var data = _postService.CreatePostCategory(postCategory);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Post/Category endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("Board")]
        public IActionResult CreatePostBoard([FromBody]PostBoard postBoard)
        {
            try
            {
                var data = _postService.CreatePostBoard(postBoard);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Post/Category endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpPost]
        [Route("Thread")]
        public IActionResult CreatePostThread([FromBody]PostThread postThread)
        {
            try
            {
                var data = _postService.CreatePostThread(postThread);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Post/Thread endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpPost]
        [Route("Post")]
        public IActionResult CreatePost([FromBody]Post post)
        {
            try
            {
                var data = _postService.CreatePost(post);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Post/Post endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete]
        [Route("Category/{categoryId}")]
        public IActionResult DeletePostCategory(int categoryId)
        {
            try
            {
                _postService.DeleteCategory(categoryId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE /v1/Post/Category/{categoryId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpDelete]
        [Route("Board/{boardId}")]
        public IActionResult DeletePostBoard(int boardId)
        {
            try
            {
                _postService.DeleteBoard(boardId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE /v1/Post/Board/{boardId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpDelete]
        [Route("Board/{threadId}")]
        public IActionResult DeletePostThread(int threadId)
        {
            try
            {
                _postService.DeleteThread(threadId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE /v1/Post/Thread/{threadId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpDelete]
        [Route("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            try
            {
                _postService.DeletePost(postId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE /v1/Post/Post/{postId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}