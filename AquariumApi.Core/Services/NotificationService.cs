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
        public Task EmitAsync(DispatchedNotification notif)
        {
            return _aquariumDao.EmitNotification(notif);
        }

        public Task EmitAsync(DispatchedNotification notif, List<int> aquariumAccountIds)
        {
            return _aquariumDao.EmitNotification(notif, aquariumAccountIds);
        }

        public ICollection<DispatchedNotification> GetAllDispatchedNotifications()
        {
            return _aquariumDao.GetAllDispatchedNotifications();
        }

        
    }
}