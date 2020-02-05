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
using SendGrid;
using SendGrid.Helpers.Mail;
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
    public interface INotificationService
    {
        void DeleteDispatchedNotification(int notificationId);
        void DeleteNotification(int notificationId);
        void DismissDispatchedNotification(int notificationId);
        void DismissDispatchedNotifications(List<int> notificationIds);
        void DismissNotifications(List<int> notificationIds);
        Task EmitAsync(DispatchedNotification notif);
        Task EmitAsync(DispatchedNotification notif, List<int> aquariumAccountIds);
        ICollection<DispatchedNotification> GetAllDispatchedNotifications();
        ICollection<Notification> GetNotificationsByAccountId(int id);
        ICollection<Notification> GetNotificationsById(List<int> notificationIds);
        void DeleteDispatchedNotifications(List<int> notificationIds);
        Task EmitAsync(int senderId, string title, string subtitle, string body = "");
    }
    public class NotificationService : INotificationService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;

        public NotificationService(IConfiguration configuration,IAquariumDao aquariumDao)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
        }

        public void DeleteDispatchedNotification(int notificationId)
        {
            _aquariumDao.DeleteDispatchedNotification(notificationId);
        }
        public void DeleteDispatchedNotifications(List<int> notificationIds)
        {
            _aquariumDao.DeleteDispatchedNotifications(notificationIds);
        }
        public void DismissDispatchedNotification(int notificationId)
        {
            _aquariumDao.DismissDispatchedNotification(notificationId);
        }
        public void DismissDispatchedNotifications(List<int> notificationIds)
        {
            _aquariumDao.DismissDispatchedNotifications(notificationIds);
        }

        public void DeleteNotification(int notificationId)
        {
            _aquariumDao.DeleteNotification(notificationId);
        }
        public void DismissNotifications(List<int> notificationIds)
        {
            _aquariumDao.DismissNotifications(notificationIds);
        }
        public async Task EmitAsync(DispatchedNotification notif)
        {
            await _aquariumDao.EmitNotification(notif);
        }
        public async Task EmitAsync(int senderId,string title,string subtitle,string body = "")
        {
            await EmitAsync(new DispatchedNotification
            {
                Date = DateTime.Now.ToUniversalTime(),
                Type = NotificationTypes.LoginDeviceActivity,
                DispatcherId = senderId,
                Title = title,
                Subtitle = subtitle,
                Body = body
            }, new List<int>() { senderId });
        }

        public async Task EmitAsync(DispatchedNotification notif, List<int> aquariumAccountIds)
        {
            await _aquariumDao.EmitNotification(notif, aquariumAccountIds);
        }

        public ICollection<DispatchedNotification> GetAllDispatchedNotifications()
        {
            return _aquariumDao.GetAllDispatchedNotifications();
        }

        public ICollection<Notification> GetNotificationsById(List<int> notificationIds)
        {
            return _aquariumDao.GetNotificationsById(notificationIds);
        }
        public ICollection<Notification> GetNotificationsByAccountId(int id)
        {
            return _aquariumDao.GetNotificationsByAccountId(id);
        }



    }
}