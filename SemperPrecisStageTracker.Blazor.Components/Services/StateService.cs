using System;
using System.Collections.Generic;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class StateService
    {
        public ShooterContract User { get; set; }
        public ShooterInformationResponse ShooterInfo { get; private set; } = new ();

        public UserPermissionContract Permissions { get; set; }

        public bool IsAuth => User != null;
        public string CurrentTheme { get; private set; } = string.Empty;
        /// <summary>
        /// The event that will be raised for state changed
        /// </summary>
        public event Action<StateService> OnStateChange;
 
        /// <summary>
        /// The state change event notification
        /// </summary>
        private void NotifyStateChanged() => OnStateChange?.Invoke(this);

        /// <summary>
        /// The method that will be accessed by the sender component 
        /// to update the state
        /// </summary>
        public void SetTheme(string theme)
        {
            CurrentTheme = theme;
            NotifyStateChanged();
        }
 
        /// <summary>
        /// The method that will be accessed by the sender component 
        /// to update the state
        /// </summary>
        public void SetShooterInfo(ShooterInformationResponse info)
        {
            ShooterInfo= info;
            NotifyStateChanged();
        }
    }
}