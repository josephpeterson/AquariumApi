﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class Activity
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public ActivityTypes ActivityType { get; set; }
        public int AccountId { get; set; }
        public DateTime Timestamp { get; set; }
        public int Key1 { get; set; }

    }
    public class DeleteAquariumActivity : Activity
    {
        public int AquariumId
        {
            get
            {
                return Key1;
            }
            set
            {
                Key1 = value;
            }
        }
        [ForeignKey("Key1")]
        public Aquarium Aquarium { get; set; }

        public DeleteAquariumActivity()
        {
            this.ActivityType = ActivityTypes.DeleteAquariumActivity;
        }
    }
    public class CreateAquariumActivity : DeleteAquariumActivity
    {
        public CreateAquariumActivity()
        {
            this.ActivityType = ActivityTypes.CreateAquariumActivity;
        }
    }
    public class UpdateAquariumActivity : DeleteAquariumActivity
    {
        public UpdateAquariumActivity()
        {
            this.ActivityType = ActivityTypes.UpdateAquariumActivity;
        }
    }
    public class CreateAquariumTestResultsActivity : Activity
    {
        public int SnapshotId
        {
            get
            {
                return Key1;
            }
            set
            {
                Key1 = value;
            }
        }
        [ForeignKey("Key1")]
        public AquariumSnapshot Snapshot { get; set; }
        public CreateAquariumTestResultsActivity()
        {
            this.ActivityType = ActivityTypes.CreateAquariumTestResults;
        }
    }
    public class CreateAccountActivity : Activity
    {
        public CreateAccountActivity()
        {
            this.ActivityType = ActivityTypes.CreateAccountActivity;
        }
    }
    public class DeleteAccountActivity : Activity
    {
        public DeleteAccountActivity()
        {
            this.ActivityType = ActivityTypes.DeleteAccountActivity;
        }
    }
    public class LoginAccountActivity : Activity
    {
        public LoginAccountActivity()
        {
            this.ActivityType = ActivityTypes.LoginAccountActivity;
        }
    }
    public class DeviceLoginAccountActivity : Activity
    {
        public DeviceLoginAccountActivity()
        {
            this.ActivityType = ActivityTypes.DeviceLoginAccountActivity;
        }
    }
}