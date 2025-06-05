using UnityEngine;
using VContainer;

namespace Managers
{
    public interface IHapticManager
    {
        void PlayWarning();
        void PlayFailure();
        void PlaySuccess();
        void PlayLight();
        void PlayMedium();
        void PlayHeavy();
        void PlayDefault();
        void PlayVibrate();
        void PlaySelection();
        void ToggleHaptics();
        bool IsHapticEnabled { get; }
    }

    public class HapticManager : MonoBehaviour, IHapticManager
    {
        [Inject] private GameData.GameData _gameData;
    
        public bool IsHapticEnabled => _gameData.isHapticOn;
        
        public void PlayWarning()
        {
            if (!CanPlay()) return;
            Taptic.Warning();
        }

        public void PlayFailure()
        {
            if (!CanPlay()) return;
            Taptic.Failure();
        }

        public void PlaySuccess()
        {
            if (!CanPlay()) return;
            Taptic.Success();
        }

        public void PlayLight()
        {
            if (!CanPlay()) return;
            Taptic.Light();
        }

        public void PlayMedium()
        {
            if (!CanPlay()) return;
            Taptic.Medium();
        }

        public void PlayHeavy()
        {
            if (!CanPlay()) return;
            Taptic.Heavy();
        }

        public void PlayDefault()
        {
            if (!CanPlay()) return;
            Taptic.Default();
        }

        public void PlayVibrate()
        {
            if (!CanPlay()) return;
            Taptic.Vibrate();
        }

        public void PlaySelection()
        {
            if (!CanPlay()) return;
            Taptic.Selection();
        }
        public void ToggleHaptics()
        {
            _gameData.isHapticOn = !_gameData.isHapticOn;
        
            if (_gameData.isHapticOn)
            {
                Taptic.Selection();
            }
        }
        
        private bool CanPlay()
        {
            return _gameData.isHapticOn;
        }
    }
}