using System;

namespace SilverlightSnake
{
    public class MainPresenter : ViewModelBase
    {
        PresenterModes Mode = PresenterModes.Initial;

        public string MessageText
        {
            get { return _messageText; }
            set { 
                _messageText = value; 
                OnPropertyChanged("MessageText");
            }
        } private string _messageText;

        //} private bool _messageTextChangedNotificator;

        public MainPresenter()
        {
            MessageText = "Nix passiert.";
        }
    }

    public enum PresenterModes
    {
        Initial, Running, Pause, Restart, NextLevel, Retire
    }
}
